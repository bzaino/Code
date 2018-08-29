using System;
using System.Configuration;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Text;
using System.IO;
using System.Collections;
using System.Runtime.Serialization.Json;

using ASA.Log.ServiceLogger;
using HttpsRequestWrapper;

namespace SALTCoursesWSClient
{
    public class MoodleUser
    {
        private UserModel muModel; //Moodle User (aka SALT Courses) model
        private SiteMemberModel memberModel; //SALT site model
        static string coursesServiceUrl = ConfigurationManager.AppSettings["CoursesServiceUrl"].ToString();
        static string coursesServiceToken = ConfigurationManager.AppSettings["CoursesServiceToken"].ToString();
        static string coursesList = ConfigurationManager.AppSettings["CoursesList"].ToString();
        static readonly IASALog _log = ASALogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //Constructor
        public MoodleUser(string memberId)
        {
            this.muModel = new UserModel() { username = memberId };
            this.memberModel = new SiteMemberModel(){ MembershipId = memberId };
        }

        //Constructor
        public MoodleUser(SiteMemberModel memberModel)
        {
            this.muModel = new UserModel();
            this.memberModel = memberModel;

            string password, authenticationMethod;
            password = "notcached"; //or "notpresent"; based on Remote Learner's recommendation
            authenticationMethod = "saml"; //set authentication method to saml SSO plugin

            muModel.password = password;
            muModel.auth = authenticationMethod;
            muModel.username = memberModel.MembershipId; //moodle username is set to salt member id
            muModel.idnumber = memberModel.MembershipId; //the idnumber is set the same as the username, this is required for enrollment
            muModel.firstname = memberModel.FirstName;
            muModel.lastname = memberModel.LastName;
            muModel.email = memberModel.PrimaryEmailKey;
            muModel.customfields = new List<CustomfieldModel>();
            AddCustomFields();
        }
        
        public void SetupUser()
        {
            const string logMethodName = ".SetupUser() - ";
            _log.Debug(logMethodName + "Begin Method");

            //check if user exists in moodle
            var existingUser = GetUser();
            
            bool userUpdated = false;
            int newUserId = -1;
            if (existingUser.id > 0)
            {
                userUpdated = UpdateUser();
            }
            else
            {
                //create moodle account
                newUserId = CreateUser();
            }
            
            //if user exists in Moodle, create enrollment in Elis
            if (userUpdated || newUserId > 0)
            {
                EnrollUserIntoUsersets();
            }

            _log.Debug(logMethodName + "End Method");
        }

        private void EnrollUserIntoUsersets()
        {
            const string logMethodName = ".EnrollUserIntoUsersets() - ";
            _log.Debug(logMethodName + "Begin Method");

            foreach (var org in this.memberModel.Organizations)
            {
                if (org.IsContracted)
                {
                    EnrollUser(org.OrganizationName);
                }
            }
            _log.Debug(logMethodName + "End Method");
        }

        private void AddCustomFields()
        {
            string oeCode = "000000", oeAndBranch = "00000000", organizationName = "", organizationLogoName = "", isMemberBenefit = "false";

            //use the determined org where a single org info is required to be provided
            if (this.memberModel.OrganizationIdForCourses != null)
            {
                //pick determined org based on determined org id
                var determinedOrg = this.memberModel.Organizations.Find(o => o.OrganizationId.ToString() == this.memberModel.OrganizationIdForCourses);
                if (determinedOrg != null)
                {
                    oeCode = determinedOrg.OECode;
                    oeAndBranch = determinedOrg.OECode + determinedOrg.BranchCode;
                    organizationName = determinedOrg.OrganizationName;
                    organizationLogoName = determinedOrg.OrganizationLogoName;
                    isMemberBenefit = (!string.IsNullOrWhiteSpace(determinedOrg.OrganizationLogoName) && !determinedOrg.OrganizationLogoName.Equals("nologo", StringComparison.OrdinalIgnoreCase)).ToString();
                }
            }

            AddCustomField("oecode", oeCode);
            AddCustomField("oeranch", oeAndBranch); //Note: missing 'b' this has been already setup that way (with a typo) in moodle
            AddCustomField("schoolname", organizationName);
            AddCustomField("schoollogoname", organizationLogoName);
            AddCustomField("IsMemberBenefit", isMemberBenefit);

            //ReportingID
            int reportingIdCount = memberModel.Organizations.Count(org => !string.IsNullOrEmpty(org.ReportingId));
            if (reportingIdCount == 1)
            {
                var reportingId = memberModel.Organizations.First(org => !string.IsNullOrEmpty(org.ReportingId)).ReportingId;
                AddCustomField("ReportingID", reportingId);
            }
            else if (reportingIdCount > 1)
            {
                AddCustomField("ReportingID", "******"); //passing placeholder value to moodle to mean user has multiple reporting ids
            }
        }

        private void AddCustomField(string shortname, string value)
        {
            CustomfieldModel profileAttribute = new CustomfieldModel();
            profileAttribute.shortname = shortname;
            profileAttribute.value = value;
            this.muModel.customfields.Add(profileAttribute);
        }

        private bool HasAccount(MoodleGetUsersResponseModel responseModel)
        {
            List<UserModel> returnedUsers = responseModel.users.ToList<UserModel>();
            if (returnedUsers.Count > 0)
            {
                return (muModel.username == returnedUsers[0].username);
            }
            return false;
        }

        private UserModel GetUser()
        {
            const string logMethodName = ".GetUser() - ";
            _log.Debug(logMethodName + "Begin Method");
            UserModel moodleUser = new UserModel();

            string functionName = "core_user_get_users";
            String postData = String.Format("{0}={1}", "criteria[0][key]=username&criteria[0][value]", this.muModel.username); //username in moodle is the SALT MemberID
            string callResult = MoodleServiceCall(functionName, postData);

            if (callResult.Contains("exception"))
            {
                // Error (e.g. invalidtoken)
                MoodleExceptionModel moodleError = UtilityMethods.DeserializeResponse<MoodleExceptionModel>(callResult);
                _log.Error("Error in: " + logMethodName + " - " + moodleError.errorcode + " - " + moodleError.message);
                _log.Debug("Debug info in: " + logMethodName + " - " + moodleError.debuginfo);
                throw new Exception(moodleError.message);
            }
            else
            {
                MoodleGetUsersResponseModel responseModel = UtilityMethods.DeserializeResponse<MoodleGetUsersResponseModel>(callResult);
                if (HasAccount(responseModel))
                {
                    if (responseModel.users.Count > 0)
                    {
                        //update Model with the returned id, as update moodle account requires an id returned by get
                        muModel.id = responseModel.users[0].id;
                        moodleUser = responseModel.users[0];
                    }
                }
            }
            _log.Debug(logMethodName + "End Method");

            return moodleUser;
        }

        private int CreateUser()
        {
            const string logMethodName = ".CreateUser() - ";
            _log.Debug(logMethodName + "Begin Method");

            string functionName = "core_user_create_users";
            string postData = String.Format("users[0][username]={0}&users[0][password]={1}&users[0][auth]={2}",
                                            muModel.username, muModel.password, muModel.auth);
            //update postData with more parameters
            postData = ExtendPostData(postData);
            string creationResult = MoodleServiceCall(functionName, postData);
            //handle result
            if (creationResult.Contains("exception"))
            {
                // Error
                MoodleExceptionModel moodleError = UtilityMethods.DeserializeResponse<MoodleExceptionModel>(creationResult);
                _log.Error("Error in: " + logMethodName + " - " + moodleError.errorcode + " - " + moodleError.message + " for member with emailaddress " + memberModel.PrimaryEmailKey);
                _log.Debug("Debug info in: " + logMethodName + " - " + moodleError.debuginfo);
                return -1;
            }
            else
            {
                // Successful creation
                List<MoodleCreateUserResponseModel> createdUsers = UtilityMethods.DeserializeResponse<List<MoodleCreateUserResponseModel>>(creationResult);
                if (createdUsers.Count > 0)
                {
                    //update Model with the returned id, required for any subsequent update calls
                    muModel.id = createdUsers[0].id;
                }
            }
            _log.Debug(logMethodName + "End Method");

            return muModel.id;
        }

        private string ExtendPostData(string postData)
        {
            //add profile attributes
            postData += String.Format("&users[0][firstname]={0}&users[0][lastname]={1}&users[0][email]={2}",
                                            muModel.firstname, muModel.lastname, muModel.email);
            //add custom fields; schoolname, schoollogoname, oecode, oeranch, ReportingID
            for(int i = 0; i < this.muModel.customfields.Count; i++)
            {
                postData += "&users[0][customfields][" + i + "][type]=" + this.muModel.customfields[i].shortname +
                            "&users[0][customfields][" + i + "][value]=" + this.muModel.customfields[i].value;
            }
            return postData;
        }

        private bool UpdateUser()
        {
            const string logMethodName = ".UpdateUser() - ";
            _log.Debug(logMethodName + "Begin Method");
            bool bUpdate = false;
            string functionName = "core_user_update_users";
            string postData = String.Format("users[0][id]={0}", muModel.id); //id required for moodle update
            //update postData with more parameters
            postData = ExtendPostData(postData);
            string updateResult = MoodleServiceCall(functionName, postData);

            //null returned means successful update
            if (updateResult == "null")
            {
                bUpdate = true;
            }
            else if (updateResult.Contains("exception"))
            {
                // Error
                MoodleExceptionModel moodleError = UtilityMethods.DeserializeResponse<MoodleExceptionModel>(updateResult);
                _log.Error("Error in: " + logMethodName + " - " + moodleError.errorcode + " - " + moodleError.message + " for member with emailaddress " + memberModel.PrimaryEmailKey);
                _log.Debug("Debug info in: " + logMethodName + " - " + moodleError.debuginfo);
                return bUpdate;
            }
            _log.Debug(logMethodName + "End Method");

            return bUpdate;
        }

        private void EnrollUser(string usersetName)
        {
            const string logMethodName = ".EnrollUser(string usersetName) - ";
            _log.Debug(logMethodName + "Begin Method");

            string functionName = "local_datahub_elis_userset_enrolment_create";
            String postData = String.Format("data[user_username]={0}&data[user_idnumber]={1}&data[user_email]={2}",
                                           muModel.username, muModel.idnumber, muModel.email);
            postData += "&data[userset_name]=" + usersetName;

            string enrollmentResult = MoodleServiceCall(functionName, postData);      
            //handle result
            if (enrollmentResult.Contains("exception"))
            {
                // Error
                MoodleExceptionModel moodleError = UtilityMethods.DeserializeResponse<MoodleExceptionModel>(enrollmentResult);
                _log.Error("Exception in: " + logMethodName + moodleError.errorcode + " - " + moodleError.message + " for member with emailaddress " + memberModel.PrimaryEmailKey);
                _log.Debug("Debug info in: " + logMethodName + " - " + moodleError.debuginfo);
            }
            else
            {
                // Successful enrollment
                ElisUsersetEnrollmentCreateResponseModel enrolledUser = UtilityMethods.DeserializeResponse<ElisUsersetEnrollmentCreateResponseModel>(enrollmentResult);
                if (enrolledUser.messagecode == "userset_enrolment_created")
                {
                    //capture result into model if to be required for future features, otherwise successful and do nothing.
                }
            }
            _log.Debug(logMethodName + "End Method");
            //return enrollmentResult;
        }

        private string UnEnrollUser()
        {
            //string functionName = "local_datahub_elis_userset_enrolment_delete";

            throw new NotImplementedException();
        }

        /// <summary>
        /// Will build a list of CourseModels from the web.config entries for Salt Courses from moodle
        /// </summary>
        /// <returns>a list of type CourseModel</returns>
        public List<CourseModel> BuildCoursesFromConfig()
        {
            const string logMethodName = ".BuildCoursesFromConfig() - ";
            _log.Debug(logMethodName + "Begin Method");
            //Build course list from config which has a sequence of id, idnumber, shortname values
            List<CourseModel> courses = new List<CourseModel>();
            var coursesParamsFromConfig = coursesList.Split('|');
            for (int i = 0; i < coursesParamsFromConfig.Length; i+=4)
            {
                if (i % 4 == 0)
                {
                    CourseModel course = new CourseModel
                    {
                        id = int.Parse(coursesParamsFromConfig[i]),
                        idnumber = coursesParamsFromConfig[i + 1],
                        shortname = coursesParamsFromConfig[i + 2],
                        contentid = coursesParamsFromConfig[i + 3]
                    };
                    courses.Add(course);
                }
            }
            _log.Debug(logMethodName + "End Method");

            return courses;
        }

        public List<CourseModel> GetUserCourses(bool fromMoodle = false)
        {
            const string logMethodName = ".GetUserCourses(int saltMemberId) - ";
            _log.Debug(logMethodName + "Begin Method");

            List<CourseModel> courses = BuildCoursesFromConfig();
            //if fromMoodle is false, return the courses representation built form config, i.e. without user's actual completion data
            if (fromMoodle)
            {
                //check if user exists in moodle
                this.muModel = this.GetUser();

                if (!string.IsNullOrEmpty(this.muModel.username))
                {
                    string classidnumber = "";
                    int username = int.Parse(this.muModel.username);
                    string functionName = "local_datahub_elis_class_enrolment_update";

                    foreach (var course in courses)
                    {
                        //populate each course with grade and completestatusid
                        classidnumber = course.idnumber;

                        String postData = String.Format("data[class_idnumber]={0}&data[user_username]={1}", classidnumber, username);
                        string callResult = MoodleServiceCall(functionName, postData);
                        if (callResult.Contains("exception"))
                        {
                            // Error
                            MoodleExceptionModel moodleError = UtilityMethods.DeserializeResponse<MoodleExceptionModel>(callResult);
                            _log.Error("Exception in: " + logMethodName + moodleError.errorcode + " - " + moodleError.message + " for member with MemberID " + this.muModel.username);
                            _log.Debug("Debug info in: " + logMethodName + " - " + moodleError.debuginfo);
                        }
                        else
                        {
                            var courseUpdateResponse = UtilityMethods.DeserializeResponse<ElisClassEnrolmentUpdateResponseModel>(callResult);
                            course.grade = courseUpdateResponse.record.grade;
                            course.completestatusid = courseUpdateResponse.record.completestatusid;
                        }
                    }
                }
            }            
            _log.Debug(logMethodName + "End Method");

            return courses;
        }

        private static string MoodleServiceCall(string functionName, String postData)
        {
            const string logMethodName = ".MoodleServiceCall(string functionName, String postData) - ";
            _log.Debug(logMethodName + "Begin Method");

            string restFormat = "json";
            string coreParams = String.Format("?wstoken={0}&wsfunction={1}&moodlewsrestformat={2}", coursesServiceToken, functionName, restFormat);
            string requestUrl = string.Format(coursesServiceUrl + coreParams);
            string requestMethod = "POST"; //"GET";
            string contentType = "application/x-www-form-urlencoded"; //"application/json";
            // Encode the parameters as form data:
            byte[] formDataAsBytes = UTF8Encoding.UTF8.GetBytes(postData);
            bool bKeepAlive = false;
            string responseEncodingFormat = "UTF-8";

            HttpsRequestProvider httpsProvider = new HttpsRequestProvider();
            string responseString = httpsProvider.DoHttpsWebRequest(requestUrl, requestMethod, contentType, formDataAsBytes, bKeepAlive, responseEncodingFormat);
            _log.Debug(logMethodName + "End Method");

            return responseString;
        }
        
    }
}
