using System;
using System.Net.Mail;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Web.Security;
using System.Web.Script.Serialization;

using ASA.Web.Services.ASAMemberService.DataContracts;
using ASA.Web.Services.ASAMemberService.ServiceContracts;
using ASA.Web.Services.ASAMemberService.Validation;
using ASA.Web.Services.Common;
using ASA.Web.WTF;
using ASA.Web.WTF.Integration;

namespace ASA.Web.Services.ASAMemberService
{
    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class ASAMember
    {
        /// <summary>
        /// The class name
        /// </summary>
        private const string Classname = "ASA.Web.Services.ASAMemberService.ASAMember";

        /// <summary>
        /// The logger
        /// </summary>
        static readonly Log.ServiceLogger.IASALog Log = ASA.Log.ServiceLogger.ASALogManager.GetLogger(Classname);
        /// <summary>
        /// The member adapter
        /// </summary>
        private readonly IAsaMemberAdapter _asaMemberAdapter = null;
        /// <summary>
        /// Initializes a new instance of the <see cref="ASAMember"/> class.
        /// </summary>
        /// 
        private static readonly JavaScriptSerializer JsonConvert = new JavaScriptSerializer();
        public ASAMember()
        {
            Log.Info("ASA.Web.Services.ASAMemberService.ASAMember() object being created ...");
            _asaMemberAdapter = new AsaMemberAdapter();
        }

        #region REST-style

        #region POST (a.k.a. Account Maintenance)

        [OperationContract]
        [WebInvoke(UriTemplate = "Member/", Method = "POST")]
        public ASAMemberModel AccountMaintenance(ASAMemberModel memberIn)
        {
            String logMethodName = ".AccountMaintenance(ASAMemberModel memberIn) - ";
            Log.Debug(logMethodName + "Begin Method");


            Log.Debug(logMethodName + "End Method");
            return memberIn;
        }

        #endregion POST (a.k.a. Account Maintenance)

        #region PUT

        /// <summary>
        /// Registers the member.
        /// </summary>
        /// <param name="memberIn">member registration information</param>
        /// <param name="logon">Create session cookies</param>
        /// <returns>ASAMemberModel</returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "Member/?Session={logon}", Method = "PUT")]
        public ASAMemberModel RegisterMember(ASAMemberModel memberIn, bool logon = false)
        {
            const string logMethodName = ".RegisterMember(ASAMemberModel memberIn) - ";
            Log.Debug(logMethodName + "Begin Method");

            bool validModel = ASAMemberValidation.ValidateASAMember(memberIn);

            if (validModel)
            {
                //If no organization has been supplied check to see if a school has been supplied; this is for backwards compatibility,
                //if no organization or school, allocate one and set to No School Selected
                if (memberIn.Organizations == null || memberIn.Organizations.Count == 0)
                {
                    if (memberIn.Schools == null || memberIn.Schools.Count == 0)
                    {
                        MemberOrganizationModel mom = new MemberOrganizationModel() { OECode = "000000", BranchCode = "00" };
                        memberIn.Organizations = new List<MemberOrganizationModel>() { mom };
                        Log.Info(logMethodName + String.Format("Organization manually added for member: {0}. Set to No School Selected", memberIn.Emails[0].EmailAddress));
                    }
                    else
                    {
                        MemberOrganizationModel mom = new MemberOrganizationModel() { OECode = memberIn.Schools[0].OECode, BranchCode = memberIn.Schools[0].BranchCode, ExpectedGraduationYear = memberIn.ExpectedGraduationYear };
                        memberIn.Organizations = new List<MemberOrganizationModel>() { mom };
                        Log.Info(logMethodName + String.Format("Organization copied from school model for member: {0}.", memberIn.Emails[0].EmailAddress));
                    }
                }

                //only 2 items should be entered per registration and they should not be the same
                if (memberIn.Organizations.Count == 2)
                {
                    //will count the unique organization ids, should be 2, if 1 then they are the same, remove one
                    if (1 == memberIn.Organizations.GroupBy(o => o.OrganizationId).Count())
                    {
                        memberIn.Organizations.RemoveAt(1);
                        Log.Info(logMethodName + String.Format("Duplicate Organization removed for member: {0} and continuing to process", memberIn.Emails[0].EmailAddress));
                    }
                }

                if (System.Web.HttpContext.Current != null && System.Web.HttpContext.Current.Session != null)
                {
                    Log.Debug(logMethodName + "ClearSession being called at the beginning of Regsitration process");
                    Utility.BaseSession.ClearSession();
                }

                MemberCreationStatus createStatus;

                var member = new ASAMemberModel();

                if (_asaMemberAdapter.GetMemberByEmail(memberIn.Emails[0].EmailAddress) != null)
                {
                    var error = new ErrorModel("Error. It looks like you already have a SALT account. Visit saltmoney.org to login or to recover your password.", "WEB ASAMember Service");
                    error.Code = "DuplicateUserName";
                    member.ErrorList.Add(error);
                    Log.Debug(logMethodName + "End Method");
                    return member;
                }

                var authInfo = new MemberAuthInfo
                    {
                        Username = memberIn.Emails[0].EmailAddress,
                        Email = memberIn.Emails[0].EmailAddress,
                        Password = memberIn.Password,
                    };

                var profile = new MemberProfileData
                    {
                        ContactFrequency = memberIn.ContactFrequency,
                        FirstName = memberIn.FirstName,
                        LastName = memberIn.LastName,
                        EmailAddress = memberIn.Emails[0].EmailAddress,
                        Source = memberIn.Source,
                        InvitationToken = memberIn.InvitationToken,
                        YearOfBirth = memberIn.YearOfBirth,
                    };

                if (memberIn.Organizations.Any())
                {
                    profile.Organizations = new MemberOrganizationList<MemberOrganizationData>();
                    foreach (MemberOrganizationModel organization in memberIn.Organizations)
                    {
                        MemberOrganizationData item = new MemberOrganizationData();
                        item.OrganizationId = organization.OrganizationId;
                        item.OECode = organization.OECode;
                        item.BranchCode = organization.BranchCode;
                        item.ExpectedGraduationYear = organization.ExpectedGraduationYear;
                        item.ReportingId = organization.ReportingId;
                        item.IsOrganizationDeleted = organization.IsOrganizationDeleted;
                        profile.Organizations.Add(item);
                    }
                }

                var newMember = IntegrationLoader.LoadDependency<ISiteMembership>("siteMembership").CreateMember(authInfo, profile, out createStatus);

                if (createStatus != MemberCreationStatus.Success)
                {
                    var error = new ErrorModel("Error. Unable to complete your registration. Please check your information and try again.", "ASAMember Service");
                    error.Code = "GenericError";
                    member.ErrorList.Add(error);
                    Log.Debug(logMethodName + "End Method");
                    return member;
                }

                if (logon)
                {
                    memberIn = _asaMemberAdapter.Logon(logon, memberIn.Emails[0].EmailAddress);
                    _asaMemberAdapter.SetMemberIdCookie(memberIn.MembershipId);
                }
            }

            return memberIn;
        }

        /// <summary>
        /// Delete a member
        /// </summary>
        /// <returns>bool</returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "Member/{memberId}", Method = "DELETE", ResponseFormat = WebMessageFormat.Json)]
        public bool DeleteMember(string memberId)
        {
            const string logMethodName = ".DeleteMember(ASAMemberModel memberIn) - ";
            Log.Debug(logMethodName + "Begin Method");

            bool result = false;

            //Check the incoming memberId against what's in the incoming model            
            ASAMemberModel currentUser = GetMemberFromContext();
            if (_asaMemberAdapter.IsCurrentUser(memberId))
            {
                try
                {
                    result = _asaMemberAdapter.DeleteMember(currentUser);

                }
                catch (Exception e)
                {
                    Log.Error("Exception deleteing member", e);
                    throw;
                }
            }
            else
            {
                throw new WebFaultException<string>("Not Authorized", System.Net.HttpStatusCode.Unauthorized);
            }

            Log.Debug(logMethodName + "End Method");
            return result;
        }

        [OperationContract]
        [WebInvoke(UriTemplate = "Member/{memberId}/OrganizationsAffiliation", ResponseFormat = WebMessageFormat.Json, Method = "PUT")]
        public bool UpdateMemberOrgAffliiation(string memberId, IList<MemberOrganizationModel> memberOrgAffiliations)
        {
            if (_asaMemberAdapter.IsCurrentUser(memberId))
            {
                if (MemberOrganizationValidation.validateMemberOrganizationModel(memberOrgAffiliations))
                {
                    return _asaMemberAdapter.UpdateMemberOrgAffliiation(memberId, memberOrgAffiliations);
                }
                else
                {
                    throw new System.ComponentModel.DataAnnotations.ValidationException();
                }
            }
            else
            {
                throw new WebFaultException<string>("Not Authorized", System.Net.HttpStatusCode.Unauthorized);
            }
        }

        [OperationContract]
        [WebInvoke(UriTemplate = "Member/{memberId}/ProfileResponses", ResponseFormat = WebMessageFormat.Json, Method = "PUT")]
        public bool UpdateMemberProfileResponses(string memberId, IList<MemberProfileQAModel> profileResponse)
        {
            if (_asaMemberAdapter.IsCurrentUser(memberId))
            {
                if (MemberProfileResponseValidation.validateMemberProfileResponse(profileResponse))
                {
                    return _asaMemberAdapter.UpdateMemberProfileResponses(memberId, profileResponse);
                }
                else
                {
                    throw new System.ComponentModel.DataAnnotations.ValidationException();
                }
            }
            else
            {
                throw new WebFaultException<string>("Not Authorized", System.Net.HttpStatusCode.Unauthorized);
            }
        }

        
        [OperationContract]
        [WebInvoke(UriTemplate = "QuestionAnswerResponse", ResponseFormat = WebMessageFormat.Json, Method = "PUT")]
        public bool UpsertQuestionAnswer(QuestionAnswerReponseModel choicesResponse)
        {
            const string logMethodName = "- QuestionAnswerResponse(QuestionAnswerReponseModel choicesResponse) - ";
            Log.Debug(logMethodName + "Begin Method");

            bool result = false;
            try
            {
                ASAMemberModel currentUser = GetMemberFromContext();
                if (_asaMemberAdapter.IsCurrentUser(choicesResponse.memberId))
                {
                if (MemberProfileResponseValidation.validateMemberQAResponse(choicesResponse))
                    {
                        result = _asaMemberAdapter.UpsertQuestionAnswer(choicesResponse.memberId, choicesResponse.choicesList);
                    }
                    else
                    {
                        throw new System.ComponentModel.DataAnnotations.ValidationException();
                    }
                }
                else
                {
                    throw new WebFaultException<string>("Not Authorized", System.Net.HttpStatusCode.Unauthorized);
                }
            }
            catch (ValidationException validatEx)
            {
                string errMsg = validatEx.Message;
                if (choicesResponse.choicesList.Count == 0)
                {
                    errMsg = string.Format("{0}: choicesResponse failed validation {1}", "Exception executing your request in UpsertQuestionAnswer", "object choicesResponse.choicesList must have length greater than zero " + errMsg + ". Call exited without change.");
                }
                Log.Error(errMsg, validatEx.StackTrace);
            }
            catch (Exception ex)
            {
                Log.Error("Exception executing your request in UpsertQuestionAnswer:", ex);
            }
            Log.Debug(logMethodName + "End Method");
            return result;
        }                                                                          
        #endregion POST

        #region GET

        /// <summary>
        /// Gets the member by the Active Directory key.
        /// </summary>
        /// <param name="adKey">The Active Directory Key.</param>
        /// <returns></returns>
        [OperationContract]
        [WebGet(UriTemplate = "ForgotPassword?token={systemId}", ResponseFormat = WebMessageFormat.Json)]
        [AspNetCacheProfile("DoNotCache")]
        public ASAMemberModel GetMemberByActiveDirectoryKey(string systemId)
        {
            Log.Debug("START GetMemberByActiveDirectoryKey");

            FormsAuthenticationTicket ticket = null;
            bool bTickedExpired = false;

            try
            {
                ticket = FormsAuthentication.Decrypt(systemId);

                //COV-10391
                bTickedExpired = IsTicketExpired(ticket);
            }
            catch (Exception ex)
            {
                Log.Error("Exception decrypt ticket for ticket " + systemId, ex);
            }

            ASAMemberModel member = null;

            if (!bTickedExpired)
            {
                try
                {
                    //COV-10331
                    if (ticket != null)
                    {
                        Guid activeDirectoryGuid = new Guid(ticket.Name);
                        member = _asaMemberAdapter.GetMember(activeDirectoryGuid);

                        if (member != null)
                        {
                            //save primary email
                            MemberEmailModel email = member.Emails.SingleOrDefault(p => p.IsPrimary);

                            //only want to give back email. The rest would be to much information.
                            member = new ASAMemberModel();
                            member.Emails = new System.Collections.Generic.List<MemberEmailModel>() { email };
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("Exception retrieving member ", ex.ToString());
                }
            }

            if (member == null)
            {
                member = new ASAMemberModel();

                if (bTickedExpired)
                    member.ErrorList.Add(new ErrorModel("Ticket Expired"));
                else
                    member.ErrorList.Add(new ErrorModel("User not found"));
            }

            Log.Debug("END GetMemberByActiveDirectoryKey");
            return member;
        }

        //COV-10391
        public static bool IsTicketExpired(FormsAuthenticationTicket ticket)
        {

            bool bTicketExpired = false;

            if (ticket == null)
            {
                bTicketExpired = true;
            }
            else if (ticket.Expired)
            {
                bTicketExpired = true;
                Log.Error("Ticket has expired for " + ticket.Name);
            }
            return bTicketExpired;
        }

        #endregion GET

        #endregion REST-style

        #region old-style

        /// <summary>
        /// Gets the member from the context.
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebGet(UriTemplate = "GetMember/Individual", ResponseFormat = WebMessageFormat.Json)]
        [AspNetCacheProfile("DoNotCache")]
        public ASAMemberModel GetMemberFromContext()
        {
            Log.Debug("START GetMemberFromContext");
            //TODO: perform any input validation here.
            ASAMemberModel member = null;

            try
            {
                var activeDirectoryKey = _asaMemberAdapter.GetActiveDirectoryKeyFromContext();

                if (!string.IsNullOrWhiteSpace(activeDirectoryKey))
                    member = _asaMemberAdapter.GetMember(new Guid(activeDirectoryKey));
            }
            catch
            {
            }

            if (member == null)
            {
                member = new ASAMemberModel();
                member.ErrorList.Add(new ErrorModel("User not found"));
            }
            else
            {
                member.ProfileQAs = _asaMemberAdapter.GetAllProfileQAs(int.Parse(member.MembershipId));
            }

            Log.Debug("END GetMemberFromContext");
            return member;
        }

        /// <summary>
        /// Gets the basic individual info.
        /// </summary>
        /// <returns></returns>
        /// <remarks>No reason to get just the Basic info if we are always doing a full lookup to Avectra regardless. 
        /// use GET /Member/{memberID}, instead.</remarks>
        [OperationContract]
        [WebGet(UriTemplate = "GetBasicIndividualInfo", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [AspNetCacheProfile("DoNotCache")]
        public BasicIndividualInfoModel GetBasicIndividualInfo()
        {
            Log.Debug("START GetBasicIndividualInfo");
            var retModel = new BasicIndividualInfoModel();

            try
            {
                var member = IntegrationLoader.LoadDependency<ISiteMembership>("siteMembership").GetMember();
                retModel.DisplayName = member.Profile.DisplayName;
            }
            catch (Exception e)
            {
                Log.Error("GetBasicIndividualInfo: Exception => " + e.ToString());
                retModel.ErrorList.Add(new ErrorModel { BusinessMessage = "Unable to Get Basic Individual Info" });
            }

            Log.Debug("END GetBasicIndividualInfo");
            return retModel;
        }


        /// <summary>
        /// Gets the organization by oe branch.
        /// </summary>
        /// <param name="oeCode">The oe code.</param>
        /// <param name="branch">The branch.</param>
        /// <param name="extOrgId">The external org ID.</param> 
        /// <returns></returns>
        [OperationContract]
        [WebGet(UriTemplate = "PreRegistration?oe={oeCode}&br={branch}&extOrgId={extOrgId}", ResponseFormat = WebMessageFormat.Json)]
        [AspNetCacheProfile("DoNotCache")]
        public BasicOrgInfoModel GetOrganization(string oeCode, string branch, string extOrgId)
        {
            const string logMethodName = ".GetOrganization(string oeCode, string branch, string extOrgId) - ";
            Log.Debug(logMethodName + "Begin Method");

            var retModel = new BasicOrgInfoModel();

            try
            {
                if (!string.IsNullOrEmpty(extOrgId))
                {
                    retModel = _asaMemberAdapter.GetOrganizationByExternalOrgID(extOrgId);
                }
                else
                {
                    retModel = _asaMemberAdapter.GetOrganizationByOeBranch(oeCode, branch);
                }
            }
            catch (Exception e)
            {
                Log.Error("GetOrganization: Exception => " + e.ToString());
                retModel.ErrorList.Add(new ErrorModel { BusinessMessage = "Unable to Get Basic Org Info" });
            }

            Log.Debug(logMethodName + "End Method");

            return retModel;
        }

        /// <summary>
        /// Gets the basic org info.
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebGet(UriTemplate = "GetBasicOrgInfo", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [AspNetCacheProfile("DoNotCache")]
        public BasicOrgInfoModel GetBasicOrgInfo()
        {
            Log.Debug("START GetBasicOrgInfo");
            var retModel = new BasicOrgInfoModel();

            try
            {
                //int memberId = _asaMemberAdapter.GetMemberIdFromContext();
                var member = GetMemberFromContext();
                var memberOrg = _asaMemberAdapter.GetMemberOrganizations(int.Parse(member.MembershipId)).First();

                retModel.OrgLogo = memberOrg.OrganizationLogoName;
                retModel.OrgName = memberOrg.OrganizationName;
                retModel.IsBranded = !string.IsNullOrWhiteSpace(memberOrg.OrganizationLogoName) && !memberOrg.OrganizationLogoName.Equals("nologo", StringComparison.OrdinalIgnoreCase);

            }
            catch (Exception e)
            {
                Log.Error("GetBasicOrgInfo: Exception => " + e.ToString());
                retModel.ErrorList.Add(new ErrorModel { BusinessMessage = "Unable to Get Basic Org Info" });
            }

            Log.Debug("END GetBasicOrgInfo");
            return retModel;
        }

        #endregion old-style

        [WebGet(UriTemplate = "Profile", ResponseFormat = WebMessageFormat.Json)]
        public VLCMemberProfileModel GetVlcMemberProfile()
        {
            int currentUserMemberId = _asaMemberAdapter.GetMemberIdFromContext();
            var returnedProfile = _asaMemberAdapter.GetVlcProfile(currentUserMemberId);
            return returnedProfile;
        }

        [WebInvoke(UriTemplate = "Profile", ResponseFormat = WebMessageFormat.Json, Method = "POST")]
        public bool UpdateVlcMemberProfile(VLCMemberProfileModel profile)
        {
            if (VLCMemberProfileValidation.validateMemberProfileModel(profile))
            {
                int currentUserMemberId = _asaMemberAdapter.GetMemberIdFromContext();
                profile.MemberID = currentUserMemberId;
                return _asaMemberAdapter.UpdateVlcProfile(profile);
            }
            return false;
        }

        [OperationContract]
        [WebGet(UriTemplate = "Member/Individual/Courses?source={fromMoodle}", ResponseFormat = WebMessageFormat.Json)]
        public IList<MemberCourseModel> GetMemberCourses(bool fromMoodle = false)
        {
            int currentUserMemberId = _asaMemberAdapter.GetMemberIdFromContext();
            var myCourses = _asaMemberAdapter.GetMemberCourses(currentUserMemberId, fromMoodle);
            return myCourses;
        }

        [OperationContract]
        [WebInvoke(UriTemplate = "Member/Individual/SyncCourses", Method = "POST")]
        public bool SyncCoursesCompletion()
        {
            int currentUserMemberId = _asaMemberAdapter.GetMemberIdFromContext();
            return _asaMemberAdapter.SyncCoursesCompletion(currentUserMemberId);

        }

        [OperationContract]
        [WebGet(UriTemplate = "GetMember/Individual/Products", ResponseFormat = WebMessageFormat.Json)]
        public IList<MemberProductModel> GetMemberProducts()
        {
            int currentUserMemberId = _asaMemberAdapter.GetMemberIdFromContext();
            var myProducts = _asaMemberAdapter.GetMemberProducts(currentUserMemberId);
            return myProducts;
        }

        [OperationContract]
        [WebInvoke(UriTemplate = "SubscribeToProduct/", Method = "PUT")]
        public bool AddMemberProduct(MemberProductModel memberProduct)
        {
            if (MemberProductValidation.validateMemberProductModel(memberProduct))
            {
                int currentUserMemberId = _asaMemberAdapter.GetMemberIdFromContext();
                memberProduct.MemberID = currentUserMemberId;
                return _asaMemberAdapter.AddMemberProduct(memberProduct);
            }
            return false;
        }

        [OperationContract]
        [WebInvoke(UriTemplate = "UpdateProductSubscription/", ResponseFormat = WebMessageFormat.Json, Method = "POST")]
        public bool UpdateMemberProduct(MemberProductModel memberProduct)
        {
            if (MemberProductValidation.validateMemberProductModel(memberProduct))
            {
                int currentUserMemberId = _asaMemberAdapter.GetMemberIdFromContext();
                memberProduct.MemberID = currentUserMemberId;
                return _asaMemberAdapter.UpdateMemberProduct(memberProduct);
            }
            return false;
        }

        /// <summary>
        /// Return the encrypted token.
        /// </summary>
        /// <param name="partnerId">this indicates the third party partner ID</param>
        /// <param name="saltMemberId">this is salt membership ID</param>
        /// <returns>string</returns>
        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "RequestToken/{partnerId}/{saltMemberId}")]
        public string CreateEncryptedToken(string partnerId, string saltMemberId)
        {
            const string logMethodName = ". GetEncryptedToken()";
            Log.Debug(logMethodName + "Begin Method");

            try
            {
                int memberId = 0;
                if (!string.IsNullOrEmpty(saltMemberId) && !string.IsNullOrEmpty(partnerId) && int.TryParse(saltMemberId, out memberId))
                {
                    if (memberId == _asaMemberAdapter.GetMemberIdFromContext())
                    {
                        return UtilityFunctions.CreateFormsAuthenticationTicket(partnerId, saltMemberId);
                    }
                    else
                    {
                        Log.Error(logMethodName + ": Either the userId doesn't match the passed in memberId or the user has not logged in.");
                        return "";
                    }
                }
                else
                {
                    Log.Error(logMethodName + ": One of the parameters is either null or empty.");
                    return "";
                }

            }
            catch (Exception ex)
            {
                Log.Error(logMethodName + ": Exception => " + ex.ToString());
                return "";
            }
        }

        [OperationContract]
        [WebGet(UriTemplate = "GetMemberQuestionAnswer/{memberId}/{sourceID}", ResponseFormat = WebMessageFormat.Json)]
        public IList<vMemberQuestionAnswerModel> GetMemberQuestionAnswer(string memberId, string sourceID)
        {
            const string logMethodName = " -getMemberQuestionAnswer(string memberId)";
            List<vMemberQuestionAnswerModel> listMemberQuestionAnswer = new List<vMemberQuestionAnswerModel>();
            try
            {
                ASAMemberModel currentUser = GetMemberFromContext();
                if (_asaMemberAdapter.IsCurrentUser(memberId))
                {
                    int MemberID = int.Parse(memberId);
                    int SourceID = int.Parse(sourceID);
                    listMemberQuestionAnswer = _asaMemberAdapter.GetMemberQuestionAnswer(MemberID, SourceID).ToList();
                }
                else
                {
                    throw new WebFaultException<string>("Not Authorized", System.Net.HttpStatusCode.Unauthorized);
                }

            }
            catch (Exception ex)
            {
                Log.Error(logMethodName + ": Raised an exception executing your request +> ", ex);
            }
            Log.Debug(logMethodName + "End Method");
            return listMemberQuestionAnswer.ToList();
        }       

        [OperationContract]
        [WebGet(UriTemplate = "Todos", ResponseFormat = WebMessageFormat.Json)]
        public IList<MemberToDoModel> GetMemberToDos()
        {
            int currentMemberID = _asaMemberAdapter.GetMemberIdFromContext();
            IList<MemberToDoModel> todos = new List<MemberToDoModel>();
            todos = _asaMemberAdapter.GetMemberToDos(currentMemberID);
            return todos;
        }

        [OperationContract]
        [WebInvoke(UriTemplate = "Todos", Method = "POST")]
        public bool UpsertMemberToDo(MemberToDoModel todo)
        {
            const string logMethodName = " -UpsertMemberToDo(string memberId)";
            Log.Debug(logMethodName + "Begin Method");
            if (ModelValidation<MemberToDoModel>.validateModel(todo))
            {
                if (_asaMemberAdapter.IsCurrentUser(todo.MemberID.ToString()))
                {
                    try
                    {
                        bool status = _asaMemberAdapter.UpsertMemberToDo(todo);
                        Log.Debug(logMethodName + "End Method");
                        return status;
                    }
                    catch (Exception ex)
                    {
                        Log.Error(logMethodName + ": Raised an exception executing your request +> ", ex);
                        throw new Exception("Exception from UpsertMemberToDo: ", ex);
                    }
                }
                else
                {
                    throw new WebFaultException<string>("Not Authorized", System.Net.HttpStatusCode.Unauthorized);
                }
            }
            return false;
        }

        /// <summary>
        /// Call to update the aspxauth Expiration to prevent timeout when in non rememberMe state
        /// </summary>
        /// <returns>true</returns>
        [OperationContract]
        [WebGet(UriTemplate = "Beat", ResponseFormat = WebMessageFormat.Json)]
        public bool KeepAspxauthSessionAlive()
        {
            String logMethodName = "KeepAspxauthSessionAlive() - ";
            Log.Debug(logMethodName + "Begin Method");

            System.Web.HttpCookie aspxauthCookie = System.Web.HttpContext.Current.Request.Cookies.Get(".ASPXAUTH");
            if (aspxauthCookie != null)
            {
                int sessionTimeOut = Convert.ToInt16(System.Configuration.ConfigurationManager.AppSettings["SessionTimeOut"]);

                //if no value supplied then default to 15 minutes otherwise use value from the config file
                sessionTimeOut = sessionTimeOut == 0 ? 15 : sessionTimeOut;

                var ticket = FormsAuthentication.Decrypt(aspxauthCookie.Value);

                if (ticket != null)
                {
                    var newticket = new FormsAuthenticationTicket(ticket.Version,
                                                                  ticket.Name,
                                                                  ticket.IssueDate,
                                                                  DateTime.Now.AddMinutes(sessionTimeOut),
                                                                  false, //isPersistent
                                                                  "",
                                                                  ticket.CookiePath);

                    aspxauthCookie.Value = FormsAuthentication.Encrypt(newticket);
                    aspxauthCookie.Domain = "saltmoney.org";
                    System.Web.HttpContext.Current.Response.Cookies.Set(aspxauthCookie);
                }
            }

            Log.Debug(logMethodName + "End Method");
            return true;
        }

        [OperationContract]
        [WebInvoke(UriTemplate = "SendChatEmail", Method = "POST")]
        public bool SendEmailToMember(string chatTranscript)
        {
            bool emailSuccess = false;

            ASAMemberModel currentUser = GetMemberFromContext();

            MailMessage message = new MailMessage("Hope@asa.org", currentUser.Emails[0].EmailAddress);

            //validate chat transcript
            if (!CrossSiteScriptValidator.IsValidObject(chatTranscript))
            {
                Log.Error("Failed CrossSiteScriptValidator: Possible cross site attack intercepted and save for inspection [ ", chatTranscript + " ]");
                message.Dispose();
                return emailSuccess;
            }

            string messageBody = chatTranscript;            

            message.Body = messageBody;
            message.IsBodyHtml = true;
            message.Subject = "Your Chat Transcript With Hope";

            emailSuccess = UtilityFunctions.SendEmail(message);

            return emailSuccess;
        }

    }
}
