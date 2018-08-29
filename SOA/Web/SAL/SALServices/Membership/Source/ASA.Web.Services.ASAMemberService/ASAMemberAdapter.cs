using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Mail;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;

using Config = ASA.Web.Services.Common.Config;

using Asa.Salt.Web.Common.Types.Enums;
using ASA.Log.ServiceLogger;
using ASA.Web.Services.ASAMemberService.DataContracts;
using ASA.Web.Services.ASAMemberService.ServiceContracts;
using ASA.Web.Services.Common;
using ASA.Web.Services.Proxies;
using ASA.Web.WTF;
using ASA.Web.WTF.Integration;

namespace ASA.Web.Services.ASAMemberService
{
    public class AsaMemberAdapter : IAsaMemberAdapter
    {
        /// <summary>
        /// The class name
        /// </summary>
        private const string Classname = "ASA.Web.Services.ASAMemberService.AsaMemberAdapter";

        /// <summary>
        /// The log
        /// </summary>
        private static readonly IASALog Log = ASALogManager.GetLogger(Classname);

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentAdapter"/> class.
        /// </summary>
        public AsaMemberAdapter()
        {
        }

        /// <summary>
        /// Retrieves user based upon the NatvieGuid in Active directory.
        /// This is the most preffered method for accessing a member from XWeb do not use the others unless you have no other option.
        /// </summary>
        /// <param name="systemId">NativeID from AD.</param>
        /// <returns>
        /// Populated member model.
        /// </returns>
        public ASAMemberModel GetMember(Guid systemId)
        {
            const string logMethodName = ".GetMember(Guid systemId) - ";
            Log.Debug(logMethodName + "Begin Method");

            var member = IntegrationLoader.LoadDependency<ISaltServiceAgent>("saltServiceAgent").GetUserByActiveDirectoryKey(systemId.ToString()).ToDomainObject();

            Log.Debug(logMethodName + "End Method");
            return member;
        }

        /// <summary>
        /// Retrieves user based upon the pre-registration token provided in registration email hyperlink
        /// </summary>
        /// <param name="email">Email address of member to lookup</param>
        public ASAMemberModel GetMemberByEmail(string email)
        {
            const string logMethodName = "GetMemberByEmail(string email) - ";
            Log.Debug(logMethodName + "Begin Method");

            var member = IntegrationLoader.LoadDependency<ISaltServiceAgent>("saltServiceAgent").GetUserByUsername(email).ToDomainObject();
            if (member != null)
            {
                member.ProfileQAs = GetAllProfileQAs(int.Parse(member.MembershipId));
            }
            Log.Debug(logMethodName + "End Method");
            return member;
        }

        /// <summary>
        /// Will get all the the products the member's organizations subscribe to
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns>a list of type OrganizationProductModel contain </returns>
        public List<OrganizationProductModel> GetMemberOrganizationProducts(int memberId)
        {
            List<OrganizationProductModel> orgProducts = IntegrationLoader.LoadDependency<ISaltServiceAgent>("saltServiceAgent").GetOrganizationProductsByMemberId(memberId).ToArray().ToDomainObject();
            return orgProducts;
        }

        /// <summary>
        /// Gets the active directory key from context.
        /// </summary>
        /// <returns></returns>
        public string GetActiveDirectoryKeyFromContext()
        {
            const string logMethodName = "GetActiveDirectoryKeyFromContext() - ";
            Log.Debug(logMethodName + "Begin Method");

            var key = string.Empty;
            try
            {
                if (HttpContext.Current.Request.Cookies["IndividualId"] != null)
                {
                    key = HttpContext.Current.Request.Cookies["IndividualId"].Value;
                }
                else
                {
                    var user = Membership.GetUser();
                    if (user != null && user.UserName != null)
                    {

                        key = GetMemberByEmail(user.UserName).ActiveDirectoryKey;
                    }
                }

            }
            catch (Exception ex)
            {
                Log.Error("error with: " + logMethodName, ex);
                key = string.Empty;
            }

            Log.Debug(logMethodName + "End Method");

            return key;
        }

        /// <summary>
        /// Gets the Member ID key from context.
        /// </summary>
        /// <returns></returns>
        public int GetMemberIdFromContext()
        {
            const string logMethodName = "GetMemberIdFromContext() - ";
            Log.Debug(logMethodName + "Begin Method");

            int key = 0;
            try
            {
                if (HttpContext.Current.Request.Cookies["MemberId"] != null)
                {
                    key = Int32.Parse(FormsAuthentication.Decrypt(HttpContext.Current.Request.Cookies["MemberId"].Value).Name);
                }

            }
            catch (Exception ex)
            {
                Log.Error("error with: " + logMethodName, ex);
                key = 0;
            }

            Log.Debug(logMethodName + "End Method");

            return key;
        }


        /// <summary>
        /// Gets the organization by oe branch.
        /// </summary>
        /// <param name="oeCode">The oe code.</param>
        /// <param name="branch">The branch.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public BasicOrgInfoModel GetOrganizationByOeBranch(string oeCode, string branch)
        {
            var toReturn = new BasicOrgInfoModel();

            var organization = IntegrationLoader.LoadDependency<ISaltServiceAgent>("saltServiceAgent").GetOrganization(oeCode, branch);

            if (organization != null)
            {
                toReturn = organization.ToDomainObject();
            }

            return toReturn;
        }

        /// <summary>
        /// Gets the organization by external org id.
        /// </summary>
        /// <param name="externalOrgId">The external org ID.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public BasicOrgInfoModel GetOrganizationByExternalOrgID(string externalOrgId)
        {
            var toReturn = new BasicOrgInfoModel();

            var organization = IntegrationLoader.LoadDependency<ISaltServiceAgent>("saltServiceAgent").GetOrganizationByExternalOrgID(externalOrgId);

            if (organization != null)
            {
                toReturn = organization.ToDomainObject();
            }

            return toReturn;
        }

        /// <summary>
        /// Des the activate account.
        /// </summary>
        /// <param name="activeDirectoryKey">The active directory key.</param>
        /// <returns></returns>
        public ResultCodeModel DeActivateAccount(string activeDirectoryKey)
        {
            const string logMethodName = "DeActivateAccount(string activeDirectoryKey) - ";
            Log.Debug(logMethodName + "Begin Method");
            var toReturn = new ResultCodeModel(1);
            var member = GetMember(new Guid(activeDirectoryKey));

            if (member != null)
            {
                IntegrationLoader.LoadDependency<ISaltServiceAgent>("saltServiceAgent").DeactivateUser(GetMemberIdFromContext(), member.PrimaryEmailKey.ToString());
            }

            Log.Debug(logMethodName + "End Method");

            return toReturn;
        }

        private bool isSubscribedToProduct(List<OrganizationProductModel> products, int productID)
        {
            bool isSubbed = false;
            foreach (var item in products)
            {
                if (item.ProductID == productID && item.IsOrgProductActive == true)
                {
                    isSubbed = true;
                }
            }
            return isSubbed;
        }

        /// <summary>
        /// Gets the member.
        /// </summary>
        /// <param name="memberId">The member id.</param>
        /// <returns></returns>
        public ASAMemberModel GetMember(int memberId)
        {
            const string logMethodName = ".GetMember(int memberId) - ";
            Log.Debug(logMethodName + "Begin Method");

            var member = IntegrationLoader.LoadDependency<ISaltServiceAgent>("saltServiceAgent").GetUserByUserId(memberId).ToDomainObject();

            Log.Debug(logMethodName + "End Method");
            return member;
        }

        /// <summary>
        /// Creates the specified member.
        /// </summary>
        /// <param name="member">The member.</param>
        /// <returns></returns>
        public RegistrationResultModel Create(ASAMemberModel member)
        {
            const string logMethodName = ".Create(ASAMemberModel member) - ";
            Log.Debug(logMethodName + "Begin Method");

            RegistrationResultModel toReturn = null;
            try
            {
                var email = member.Emails.First().EmailAddress;
                var updateResult = SaltServiceAgent.RegisterUser(member.ToUserRegistrationDataContract());

                toReturn = new RegistrationResultModel()
                {
                    Member = updateResult.Member.ToDomainObject(),
                    CreateStatus = (MemberUpdateStatus)updateResult.CreateStatus
                };

                //the site membership code will delete the account in active directory 
                //if an exception is raised.
                if (toReturn.CreateStatus == MemberUpdateStatus.InvalidOrganization)
                {
                    throw new Exception("The organization entered was invalid");
                }
                else if (toReturn.CreateStatus == MemberUpdateStatus.IncompleteProfile)
                {
                    throw new Exception("The user profile is incomplete");
                }
                else if (toReturn.CreateStatus == MemberUpdateStatus.Failure)
                {
                    throw new Exception("An exception has occured creating the user");
                }
                else if (toReturn.CreateStatus == MemberUpdateStatus.Duplicate)
                {
                    throw new Exception("The user already exists in the system.");
                }
            }
            catch (Exception x)
            {
                Log.Error(string.Format(logMethodName + "Error inserting new Individual record for member: MemberAccountId={0}, Email={1}, StackTrace={2}"
                    , member.MembershipId != null ? member.MembershipId.ToString() : "null"
                    , member.Emails != null && member.Emails.Count > 0 && !string.IsNullOrEmpty(member.Emails[0].EmailAddress) ? member.Emails[0].EmailAddress : "null"
                    , x));
                throw x;
            }

            //Call async Qualtric's Target Audience API
            if (Common.Config.QTA_Process.ToLower() == "on")
            {
                QualtricsTA QTA = new QualtricsTA();
                QTA.CreateUser(toReturn.Member);
            }

            Log.Debug(logMethodName + "End Method");

            return toReturn;
        }

        /// <summary>
        /// Updates the specified member.
        /// </summary>
        /// <param name="member">The member.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception">There was an error updating Individual record</exception>
        public MemberUpdateStatus Update(ASAMemberModel member)
        {
            const string logMethodName = ".Update(ASAMemberModel member) - ";
            Log.Debug(logMethodName + "Begin Method");
            MemberUpdateStatus memberUpdateStatus = MemberUpdateStatus.Failure;

            try
            {
                memberUpdateStatus = IntegrationLoader.LoadDependency<ISaltServiceAgent>("saltServiceAgent").UpdateUser(member.ToDataContract());
            }
            catch (Exception ex)
            {
                String msg = String.Format("There was an error updating Individual record - {0}", ex.Message);
                throw new Exception(msg);
            }
            //Call async Qualtric's Target Audience API
            if (Common.Config.QTA_Process.ToLower() == "on" && memberUpdateStatus == MemberUpdateStatus.Success)
            {
                var updatedMember = GetMember(Convert.ToInt32(member.MembershipId));
                QualtricsTA QTA = new QualtricsTA();
                QTA.UpdateUser(updatedMember);
            }

            Log.Debug(logMethodName + "End Method");
            return memberUpdateStatus;
        }

        /// <summary>
        /// Delete the specified member from SALT db and ActiveDirectory.
        /// </summary>
        /// <param name="member">The member.</param>
        /// <returns>bool</returns>
        /// <exception cref="System.Exception">There was an error deleteing Member record</exception>
        public bool DeleteMember(ASAMemberModel member)
        {
            const string logMethodName = ".DeleteMember(ASAMemberModel member) - ";
            Log.Debug(logMethodName + "Begin Method");
            bool result = false;

            //delete SALT db information.
            result = SaltServiceAgent.DeleteUser(Convert.ToInt32(member.MembershipId));
            if (0 == false.CompareTo(result))
            {
                Log.Error(logMethodName + "Error deleting member from SALT DB");
                //when debugging, for some reason it step into the if and highlights this line, but it does not throw the error.
                //if you comment out the the construction of the error then the debugger will not step in here.
                throw new WebFaultException<string>("Failed to delete member", System.Net.HttpStatusCode.BadRequest);
            }

            //delete Active Directory Account.
            try
            {
                result = IntegrationLoader.LoadDependency<ISecurityAdapter>("securityAdapter").DeleteMember(member.Emails[0].EmailAddress);
            }
            catch (Exception ex)
            {
                Log.Error(logMethodName + "Error deleting member from AD", ex);
                throw new WebFaultException<string>("Failed to delete member", System.Net.HttpStatusCode.BadRequest);
            }

            Log.Debug(logMethodName + "End Method");
            return result;
        }

        /// <summary>
        /// Updates the login timestamp for the user after a successful login
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns></returns>
        public bool UpdateLastLoginTimestamp(int userId)
        {
            Task.Factory.StartNew(() =>
            {
                var user = SaltServiceAgent.Async.GetUserByUserId(userId, ar => { });

                if (user == null) return;

                user.LastLoginDate = DateTime.Now;
                SaltServiceAgent.Async.UpdateUser(user, uar => { });
            });

            return true;
        }

        /// <summary>
        /// Gets the member VLC profile.
        /// </summary>
        /// <param name="userId">The member id.</param>
        /// <returns></returns>
        public VLCMemberProfileModel GetVlcProfile(int userId)
        {
            //TODO change this to returning the Model Retrieved
            return SaltServiceAgent.GetVlcProfile(userId).ToDomainObject();
        }

        /// <summary>
        /// Updates or creates the member VLC profile.
        /// </summary>
        /// <param name="profile">The vlc profile to update.</param>
        /// <returns></returns>
        public bool UpdateVlcProfile(VLCMemberProfileModel profile)
        {
            return SaltServiceAgent.UpdateVlcProfile(profile.ToDataContract());
        }

        /// <summary>
        /// Updates member profile responses.
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="profileResponses"></param>
        /// <returns></returns>
        public bool UpdateMemberProfileResponses(string memberId, IList<MemberProfileQAModel> profileResponses)
        {
            return SaltServiceAgent.UpdateMemberProfileResponses(Int32.Parse(memberId), profileResponses.ToDataContract());
        }

        /// <summary>
        /// Updates Member Organization/s affiliation
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="memberOrgAffiliations"></param>
        /// <returns></returns>
        public bool UpdateMemberOrgAffliiation(string memberId, IList<MemberOrganizationModel> memberOrgAffiliations)
        {
            return SaltServiceAgent.UpdateMemberOrgAffiliation(Int32.Parse(memberId), memberOrgAffiliations.ToDataContract());
        }

        public IList<MemberOrganizationModel> GetMemberOrganizations(int userId)
        {
            var returnList = new List<MemberOrganizationModel>();
            var organizationList = SaltServiceAgent.GetMemberOrganizations(userId);
            foreach (var org in organizationList)
            {
                returnList.Add(org.ToMemberOrganizationModel());
            }

            return returnList;
        }

        /// <summary>
        /// Get a List of Salt Courses offered by Moodle from the IDP web.config
        /// </summary>
        /// <returns>List of MemberCourseModel</returns>
        public List<MemberCourseModel> GetSaltCoursesFromWebConfig()
        {
            SALTCoursesWSClient.MoodleUser mu = new SALTCoursesWSClient.MoodleUser("dummyId");
            List<SALTCoursesWSClient.CourseModel> saltCourses = mu.BuildCoursesFromConfig();

            List<MemberCourseModel> memberCourseModel = new List<MemberCourseModel>();
            foreach (var course in saltCourses)
            {
                memberCourseModel.Add(course.ToMemberCourseModel());
            }

            return memberCourseModel;
        }

        public IList<MemberCourseModel> GetMemberCourses(int memberId, bool fromMoodle = false)
        {
            String logMethodName = "- GetMemberCourses(int memberId, bool fromMoodle = false) :";
            Log.Debug(logMethodName + "Begin Method");
            var returnList = new List<MemberCourseModel>();
            try
            {
                SALTCoursesWSClient.MoodleUser mu = new SALTCoursesWSClient.MoodleUser(memberId.ToString());
                List<SALTCoursesWSClient.CourseModel> saltCourses;
                ASAMemberModel member = GetMember(memberId);
                List<OrganizationProductModel> courseProducts = member.OrganizationProducts.Where(p => p.ProductTypeID == 1).ToList();
                if (fromMoodle)
                {
                    saltCourses = mu.GetUserCourses(true);
                    foreach (var course in saltCourses)
                    {
                        //add to return list only courses that the organization participates in
                        foreach (var cproduct in courseProducts)
                        {
                            if (cproduct.ProductName == course.idnumber)
                            {
                                returnList.Add(course.ToMemberCourseModel());
                                break;
                            }
                        }
                    }
                }
                else
                {
                    saltCourses = mu.GetUserCourses(false);
                    var coursesContentIds = new List<string>();
                    foreach (var course in saltCourses)
                    {
                        coursesContentIds.Add(course.contentid);
                    }
                    var memberTodos = GetMemberToDos(memberId).ToList();
                    //filter member todos taking only courses
                    var memberCoursesTodos = memberTodos.Where(item => coursesContentIds.Contains(item.ContentID)).ToList();

                    foreach (var course in saltCourses)
                    {
                        //decorate the couse completion status from membertodo
                        foreach (var todo in memberCoursesTodos)
                        {
                            if (todo.ContentID == course.contentid && todo.RefToDoStatusID == 2)
                            {
                                course.completestatusid = todo.RefToDoStatusID;
                                break;
                            }
                        }
                        //add to return list only courses that the organization participates in
                        foreach (var cproduct in courseProducts)
                        {
                            if (cproduct.ProductName == course.idnumber)
                            {
                                returnList.Add(course.ToMemberCourseModel());
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(logMethodName + "Exception =>", ex);
            }
            Log.Debug(logMethodName + "End Method");
            return returnList;
        }

        /// <summary>
        /// Syncs Courses ToDo completion status from Moodle asynchronously
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool SyncCoursesCompletion(int userId)
        {
            String logMethodName = "- SyncCoursesCompletion(int userId) :";
            Log.Debug(logMethodName + "Begin Method");
            //Sync courses completion asynchronously
            Task.Factory.StartNew(() =>
            {
                Log.Debug(logMethodName + "Execute Method Asynchronous - Begin Method");
                bool syncHappened = false;

                List<OrganizationProductModel> orgProducts = GetMemberOrganizationProducts(userId);

                //if not subscibed to courses product no need to sync, just return false
                if (!isSubscribedToProduct(orgProducts, 2))
                {
                    return syncHappened;
                }

                //get courses status from moodle
                var memberCourses = GetMemberCourses(userId, true);
                var completedCourses = memberCourses.Where(course => course.CompleteStatus).ToList();
                var coursesContentIds = new List<string>();
                foreach (var course in memberCourses)
                {
                    coursesContentIds.Add(course.ContentID);
                }

                //get membertodos from salt db
                var memberTodos = GetMemberToDos(userId).ToList();
                //filter member todos taking only courses
                var memberCoursesTodos = memberTodos.Where(item => coursesContentIds.Contains(item.ContentID)).ToList();
                bool courseAlreadySynced = false;
                //compare course todo list against moodle completion
                foreach (var course in completedCourses)
                {
                    foreach (var courseTodo in memberCoursesTodos)
                    {
                        if (courseTodo.ContentID == course.ContentID && courseTodo.RefToDoStatusID == 2)
                        {
                            courseAlreadySynced = true;
                            break;
                        }
                    }
                    if (!courseAlreadySynced)
                    {
                        //Sync todo status from mooodle
                        MemberToDoModel courseTodo = new MemberToDoModel()
                        {
                            MemberID = userId,
                            ContentID = course.ContentID,
                            RefToDoTypeID = 1,
                            RefToDoStatusID = 2 //Complete
                        };

                        UpsertMemberToDo(courseTodo);
                        syncHappened = true; //there was at least one course synced during this run
                    }
                    courseAlreadySynced = false; //reset sync flag for next course check
                }
                Log.Debug(logMethodName + "Execute Method Asynchronous - End Method");
                return syncHappened;
            });
            Log.Debug(logMethodName + "End Method");
            return true;
        }

        public IList<MemberProductModel> GetMemberProducts(int memberId)
        {
            var returnList = new List<MemberProductModel>();

            var productList = SaltServiceAgent.GetMemberProducts(memberId);
            foreach (var product in productList)
            {
                returnList.Add(product.ToMemberProductModel());
            }

            return returnList;
        }

        public bool AddMemberProduct(MemberProductModel memberProduct)
        {

            bool result = SaltServiceAgent.AddMemberProduct(memberProduct.ToMemberProductContract());
            return result;
        }

        public bool UpdateMemberProduct(MemberProductModel memberProduct)
        {
            bool result = SaltServiceAgent.UpdateMemberProduct(memberProduct.ToMemberProductContract());
            return result;
        }

        public ASAMemberModel Logon(bool logon, string emailAddress)
        {
            const string logMethodName = ".Logon(bool logon, string emailAddress - ";
            Log.Debug(logMethodName + "Begin Method");

            ASAMemberModel member = null;

            if (logon)
            {
                member = GetMemberByEmail(emailAddress);

                if (member != null)
                {
                    Log.Debug(logMethodName + "Member found, creating sesion cookies");
                    //creates WTFSession.NewSession() & authentication cookie
                    IntegrationLoader.LoadDependency<ISiteMembership>("siteMembership").SignIn(emailAddress);

                    if (System.Web.HttpContext.Current.Request.Cookies.AllKeys.Contains("IndividualId"))
                    {
                        HttpCookie cookie = System.Web.HttpContext.Current.Request.Cookies["IndividualId"];
                        cookie.Domain = "saltmoney.org";
                        cookie.Expires = DateTime.Now.AddDays(-1);
                        System.Web.HttpContext.Current.Response.Cookies.Add(cookie);
                    }
                    if (System.Web.HttpContext.Current.Request.Cookies.AllKeys.Contains("MemberId"))
                    {
                        HttpCookie cookie = System.Web.HttpContext.Current.Request.Cookies["MemberId"];
                        cookie.Domain = "saltmoney.org";
                        cookie.Expires = DateTime.Now.AddDays(-1);
                        System.Web.HttpContext.Current.Response.Cookies.Add(cookie);
                    }

                    UpdateLastLoginTimestamp(Convert.ToInt32(member.MembershipId));
                }
            }
            else
            {
                member = new ASAMemberModel();
            }

            Log.Debug(logMethodName + "End Method");
            return member;
        }

        /// <summary>
        /// Sets response cookies
        /// </summary>
        public void SetMemberIdCookie(string memberId)
        {
            try
            {
                HttpCookie memberIdCookie = new HttpCookie("MemberId", UtilityFunctions.encryptString(memberId));
                memberIdCookie.Domain = "saltmoney.org";
                memberIdCookie.Secure = true;
                memberIdCookie.Expires = DateTime.Now.AddYears(2);
                System.Web.HttpContext.Current.Response.Cookies.Add(memberIdCookie);
            }
            catch (Exception ex)
            {
                throw new SecurityAdapterException("An error has occured creating AuthCookie", ex);
            }
        }

        /// <summary>
        /// Checks if the memberId passed in matches the one in context
        /// </summary>
        /// <returns>bool</returns>
        public bool IsCurrentUser(string memberId)
        {
            int memberIdFormContext = GetMemberIdFromContext();
            return (memberIdFormContext != 0 && memberIdFormContext.ToString() == memberId);
        }

        /// <summary>
        /// Inserts or Updates user Scholarship question/s responses 
        /// </summary>
        /// <param name="MemberID">ID</param>
        /// <param name="responses">IList<MemberQA></param>
        /// <returns>bool</returns>
        public bool UpsertQuestionAnswer(string sMemberID, IList<MemberQAModel> choicesList)
        {
            String logMethodName = ".UpsertQuestionAnswer(string sMemberID, IList<MemberQAModel> Responses) ";
            Log.Debug(logMethodName + "Begin Method");
            bool retval = false;
            try
            {
                int MemberID = int.Parse(sMemberID);
                retval = SaltServiceAgent.UpsertQuestionAnswer(MemberID, choicesList.ToDataContract());
            }
            catch (Exception ex)
            {
                Log.Error("Exception UpsertQuestionAnswer: ", ex);
            }
            Log.Debug(logMethodName + "End Method");
            return retval;
        }

        public IList<vMemberQuestionAnswerModel> GetMemberQuestionAnswer(Nullable<Int32> MemberID, string EmailAddress, int SourceID)
        {
            String logMethodName = "- GetMemberQuestionAnswer(Nullable<Int32> MemberID, string EmailAddress) :";
            Log.Debug(logMethodName + "Begin Method");
            List<vMemberQuestionAnswerModel> returnList = new List<vMemberQuestionAnswerModel>();
            try
            {
                var QuestionAnswerList = SaltServiceAgent.GetMemberQuestionAnswer(MemberID, EmailAddress, SourceID);
                foreach (var QuestionAnswer in QuestionAnswerList)
                {
                    returnList.Add(QuestionAnswer.ToDomainObject());
                }
            }
            catch (Exception ex)
            {
                Log.Error(logMethodName + "Exception =>", ex);
            }
            Log.Debug(logMethodName + "End Method");
            return returnList.ToList();
        }

        public IList<vMemberQuestionAnswerModel> GetMemberQuestionAnswer(int memberId, int SourceID)
        {
            Nullable<Int32> MemberID = memberId;
            string EmailAddress = string.Empty;
            var QuestionAnswerList = GetMemberQuestionAnswer(MemberID, EmailAddress, SourceID).ToList();
            return QuestionAnswerList;
        }

        public IList<MemberToDoModel> GetMemberToDos(int memberID)
        {
            List<MemberToDoModel> returnList = new List<MemberToDoModel>();
            try
            {
                var memberTodoList = SaltServiceAgent.GetMemberToDos(memberID);
                foreach (var todoContract in memberTodoList)
                {
                    returnList.Add(todoContract.ToDomainObject());
                }
            }
            catch (Exception ex)
            {
                Log.Error("GetMemberToDos Exception =>", ex);
            }
            return returnList;
        }

        public bool UpsertMemberToDo(MemberToDoModel todo)
        {
            bool result;
            try
            {
                result = SaltServiceAgent.UpsertMemberToDo(todo.ToDataContract());
            }
            catch (Exception ex)
            {
                Log.Error("UpsertMemberToDo Exception =>", ex);
                throw;
            }
            return result;
        }

        public Dictionary<string, MemberToDoModel> GetToDoDictionary(int memberId)
        {
            Dictionary<string, MemberToDoModel> todos = new Dictionary<string, MemberToDoModel>();

            IList<MemberToDoModel> todoList = GetMemberToDos(memberId);

            foreach (var todo in todoList)
            {
                todos.Add(todo.ContentID, todo);
            }

            return todos;
        }

        #region GetProfileQuestionsAndAnswers

        private IList<ProfileQuestionModel> GetAllProfileQuestions()
        {
            return IntegrationLoader.LoadDependency<ISaltServiceAgent>("saltServiceAgent").GetAllProfileQuestions().ToDomainObject().ToList();
        }

        private IList<ProfileAnswerModel> GetAllProfileAnswers()
        {
            return IntegrationLoader.LoadDependency<ISaltServiceAgent>("saltServiceAgent").GetAllProfileAnswers().ToDomainObject().ToList();
        }

        public List<ProfileResponseModel> GetProfileResponses(int memberId)
        {
            return IntegrationLoader.LoadDependency<ISaltServiceAgent>("saltServiceAgent").GetProfileResponses(memberId).ToDomainObject().ToList();
        }

        public IList<ProfileQAModel> GetAllProfileQAs(int memberId)
        {
            var profQAs = new List<ProfileQAModel>();
            var questions = GetAllProfileQuestions();
            var answers = GetAllProfileAnswers();

            profQAs = MatchQuesAndAns(questions, answers);
            ProfileQAModel res = new ProfileQAModel();
            res.Responses = GetProfileResponses(memberId);
            profQAs.Add(res);

            return profQAs;
        }

        private List<ProfileQAModel> MatchQuesAndAns(IList<ProfileQuestionModel> questions, IList<ProfileAnswerModel> answers)
        {
            var result = new List<ProfileQAModel>();
            foreach (var ques in questions)
            {
                ProfileQAModel qa = new ProfileQAModel();
                qa.QuestionID = ques.QuestionID;
                qa.QuestionName = ques.QuestionName;
                qa.QuestionDescription = ques.QuestionDescription;
                qa.ProfileQuestionTypeName = ques.ProfileQuestionTypeName;
                qa.ProfileQuestionPriority = ques.ProfileQuestionPriority;
                qa.IsProfileQuestionActive = ques.IsProfileQuestionActive;
                qa.IsInLineProfileQuestion = ques.IsInLineProfileQuestion;

                var filteredAnswers = answers.Where(a => a.QuestionID == ques.QuestionID).OrderBy(a => a.ProfileAnswerDisplayOrder).ToList();
                qa.Answers = new List<PAnswerModel>();
                foreach (var ans in filteredAnswers)
                {
                    var answer = new PAnswerModel();
                    answer.AnsID = ans.AnsID;
                    answer.AnsName = ans.AnsName;
                    answer.AnsDescription = ans.AnsDescription;
                    answer.IsProfileAnswerActive = ans.IsProfileAnswerActive;
                    answer.ProfileAnswerDisplayOrder = ans.ProfileAnswerDisplayOrder;
                    qa.Answers.Add(answer);
                }

                result.Add(qa);
            }

            return result;
        }

        #endregion

        public string BuildOutEndecaBoost(int memberId)
        {
            //first remove any objects that do not have an external question or Answer Id
            //These have no tie in to endeca content to boost a record.

            List<ProfileResponseModel> profileResponses = GetProfileResponses(memberId);

            List<MemberProfileQAModel> workingList = new List<MemberProfileQAModel>();
            foreach (var mpqa in profileResponses)
            {
                //if either is null or empty, skip
                if (mpqa.AnsExternalId == 0 || mpqa.QuestionExternalId == 0)
                {
                    continue;
                }
                else
                {
                    //only need question and answer so just filling those 2 values in.
                    MemberProfileQAModel mpqam = new MemberProfileQAModel();
                    mpqam.AnsExternalId = mpqa.AnsExternalId;
                    mpqam.QuestionExternalId = mpqa.QuestionExternalId;
                    workingList.Add(mpqam);
                }
            }

            int maxNumberOfQuestionsToBoost = 10;

            String beginningPiece = "Endeca.stratify(collection()/record[";
            String endingPiece = "],*)";
            String orPiece = " or ";
            String dimensionName = "Persona";

            String queryBooster = "{0}=collection(\"dimensions\")/dval[name=\"{0}\"]/dval[name=\"{1}\"]/dval[name=\"{2}\"]//id";

            String queryString = String.Empty;

            int i = 0;
            int profileResponsesCount = workingList.Count;
            foreach (MemberProfileQAModel mpqa in workingList)
            {
                i++;

                //put on the beginning piece of the string on last response
                if (i == 1)
                    queryString = beginningPiece;

                if (i <= profileResponsesCount)
                {
                    queryString += String.Format(queryBooster, dimensionName, mpqa.QuestionExternalId, mpqa.AnsExternalId);
                    //more than one record && it's not the last record or the maxNumberOfQuestionsToBoost
                    if (profileResponsesCount > 1 && (profileResponsesCount > i && maxNumberOfQuestionsToBoost != i))
                        queryString += orPiece;
                }

                //put on the ending piece of the string on last response
                if (i == profileResponsesCount || i == maxNumberOfQuestionsToBoost)
                {
                    queryString += endingPiece;
                    break;
                }
            }

            return queryString;
        }

    }
}
