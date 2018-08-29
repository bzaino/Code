using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.Web.Configuration;
using System.Web.Security;

using ComponentSpace.SAML2;
using ComponentSpace.SAML2.Utility;

using ASAIDP.SSO.Plugins;
using SALTCoursesWSClient;
using ASA.Log.ServiceLogger;
using ASA.Web.Common.Extensions;

namespace ASAIDP.Controllers
{
    public static class AppSettings
    {
        public const string Attribute = "Attribute";
        public const string SubjectName = "SubjectName";
        public const string TargetUrl = "TargetUrl";
    }
    public class SAML2Controller : Controller
    {
        private static readonly IASALog _log = ASALogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ActionResult SSOService()
        {
            // Either an authn request has been received or login has just completed in response to a previous authn request.

            _log.Debug("SSO Service Begin");
            string partnerSP = null;
            string myCurrentSP = SAMLIdentityProvider.GetPartnerPendingResponse();
            Dictionary<string, object> paramDictionary = new Dictionary<string, object> { 
                { "optionalParam", Request.Params["optionalParam"]}
            };
            if (Request.Form.AllKeys.Contains("SAMLRequest") || (Request.QueryString.AllKeys.Contains("SAMLRequest") && (Request.QueryString.AllKeys.Contains("RelayState") || Request.QueryString.AllKeys.Contains("Signature"))))
            {
                // Receive the authn request from the service provider (SP-initiated SSO).
                _log.Debug("Calling ReceiveSSO");
                SAMLIdentityProvider.ReceiveSSO(Request, out partnerSP);
                myCurrentSP = SAMLIdentityProvider.GetPartnerPendingResponse();
                _log.Debug("Received SSO from " + partnerSP);
               
            }

            // If the user isn't logged in at the identity provider, force the user to login.
            if (!User.Identity.IsAuthenticated)
            {
                _log.Debug("Redirecting to login");
                FormsAuthentication.RedirectToLoginPage();
                return new EmptyResult();
            }


            // The user is logged in at the identity provider.
            // Respond to the authn request by sending a SAML response containing a SAML assertion to the SP.
            // Use the configured or logged in user name as the user name to send to the service provider (SP).
            // Include some user attributes.
            string userName = WebConfigurationManager.AppSettings[AppSettings.SubjectName];
            IDictionary<string, string> attributes = new Dictionary<string, string>();

            if (string.IsNullOrEmpty(userName))
            {
                try
                {

                    string memberPath = UtilityMethods.ReadConfigValue("pathGetMember");
                    _log.Debug("Calling " + memberPath);
                    string memberResponse = WebServiceRequester.MakeServiceCall(memberPath);
                    SiteMemberModel memberModel = UtilityMethods.DeserializeResponse<SiteMemberModel>(memberResponse);
                    userName = memberModel.MembershipId.ToString();
                    bool getsAdditionalValues = true;
                    
                    //determine which SP, and populate the respective member attributes
                    myCurrentSP = SAMLIdentityProvider.GetPartnerPendingResponse();
                    //Connection with remote Learner
                    if (myCurrentSP.Contains("oldmoney.remote-learner.net") || myCurrentSP.Contains("saltcourses.saltmoney.org"))
                    {
                        attributes = AddRemoteLearnerAttributes(attributes, memberModel);

                        //Setup (create/update) user in Courses
                        MoodleUser mu = new MoodleUser(memberModel);
                        mu.SetupUser();
                    }

                    if (myCurrentSP.Contains("sso.online.tableau.com"))
                    {
                        attributes = AddTableauAttributes(attributes, memberModel);
                    }

                    if (myCurrentSP.Contains("community.saltmoney.org"))
                    {
                        String optionalParam = (String)paramDictionary["optionalParam"];
                        attributes = AddJiveAttributes(attributes, memberModel, optionalParam);
                    }

                    _log.Debug("Calling AddSSOCoreAttributes");
                    attributes = AddSSOCoreAttributes(attributes, memberModel, myCurrentSP, getsAdditionalValues);
                    _log.Debug("Returned from  AddSSOCoreAttributes with " + attributes.Count() + " Attributes");
                }
                catch (Exception ex)
                {

                    _log.Error(ex);
                    throw ex;
                }

            }
            try { 
                _log.Debug("Calling SendSSO for " + userName);
                SAMLIdentityProvider.SendSSO(Response, userName, attributes);
            }
            catch (Exception ex)
            {
                _log.Error(ex);
                throw ex;
            }
            return new EmptyResult();
        }

        private IDictionary<string, string> AddJiveAttributes(IDictionary<string, string> attributes, SiteMemberModel memberModel, string optionalParam)
        {
            const string logMethodName = ".AddJiveAttributes(IDictionary<string, string> attributes, SiteMemberModel memberModel, string optionalParam) - ";
            _log.Debug(logMethodName + "Begin Method");

            //populating isAmbassodor flag
            string isSchoolAmbassador = "False", enrollmentStatus = "", loanStatus = "";
            bool isASAEmployee = false;
            foreach (var item in memberModel.Roles)
            {
                if (item != null && item.IsMemberRoleActive)
                {
                    if (item.RoleName == "School Ambassador")
                    {
                        isSchoolAmbassador = "True";
                    }
                    else if (item.RoleName == "ASA Employee")
                    {
                        isASAEmployee = true;
                    }
                }
            }

            if (isASAEmployee)
            {
                attributes.Add("UserGroup", "SALT Employee");
            }
            else
            {
                attributes.Add("UserGroup", "SALT Member");
            }

            if (!string.IsNullOrEmpty(memberModel.EnrollmentStatus))
            {
                Dictionary<string, string> enrollmentDict = new Dictionary<string, string>() { 
                            {"F", "I'm enrolled full time"},
                            {"H", "I'm enrolled half time"},
                            {"L", "I'm enrolled less than half time"},
                            {"G", "I'm already graduated"},
                            {"W", "I left before graduating"},
                            {"X", "I haven't gone to college"}
                        };
                enrollmentStatus = enrollmentDict[memberModel.EnrollmentStatus];
            }
            if (memberModel.ProfileQAndAs.Count > 0)
            {
                foreach (var qa in memberModel.ProfileQAndAs)
                {
                    if (qa.QuestionName == "Student Loan Repayment Status")
                    {
                        loanStatus = qa.AnsName;
                        break;
                    }
                }
            }
            if (!String.IsNullOrEmpty(optionalParam))
            {
                attributes.Add("CampaignID", optionalParam);
            }

            if (string.IsNullOrEmpty(memberModel.CommunityDisplayName))
            {
                memberModel.CommunityDisplayName = ConfigurationManager.AppSettings["CommunityDefaultUserName"].ToString();
            }

            attributes.Add("IsSchoolAmbassador", isSchoolAmbassador);
            attributes.Add("CommunityDisplayName", memberModel.CommunityDisplayName);
            attributes.Add("IsCommunityActive", memberModel.IsCommunityActive.ToString());
            attributes.Add("YearOfBirth", memberModel.YearOfBirth.ToString());
            attributes.Add("GraduationYear", memberModel.Organizations[0].ExpectedGraduationYear.ToString()); //this needs an answer from business on the rules.
            attributes.Add("EnrollmentStatus", enrollmentStatus);
            attributes.Add("LoanStatus", loanStatus);

            _log.Debug(logMethodName + "End Method");

            return attributes;
        }

        private IDictionary<string, string> AddTableauAttributes(IDictionary<string, string> attributes, SiteMemberModel memberModel)
        {
            return attributes;
        }

        private IDictionary<string, string> AddSSOCoreAttributes(IDictionary<string, string> attributes, SiteMemberModel memberModel, string partnerName, bool getsAdditionalValues)
        {
            //Add core attributes
            //attributes.Identity = memberModel.PrimaryEmailKey;
            attributes.Add("FirstName", memberModel.FirstName);
            attributes.Add("LastName", memberModel.LastName);
            attributes.Add("Email", memberModel.PrimaryEmailKey);
            attributes.Add("PartnerName", partnerName);

            if (getsAdditionalValues)
            {
                string oeCode = "000000", branchCode = "00", oeAndBranch, organizationName = "", organizationLogoName = "", isMemberBenefit = "false";
                //determine where a single org info is required to be provided
                MemberOrganizationModel determinedOrg = new MemberOrganizationModel();
                //Connection with remote Learner
                if (partnerName.Contains("oldmoney.remote-learner.net") || partnerName.Contains("Saltcourses.saltmoney.org"))
                {
                    //pick determined org based on determined org id
                    if (memberModel.OrganizationIdForCourses != null)
                    {
                        determinedOrg = memberModel.Organizations.Find(o => o.OrganizationId.ToString() == memberModel.OrganizationIdForCourses);
                    }
                    else if (memberModel.Organizations.Count() == 1) //no org determined and there's a single org
                    {
                        determinedOrg = memberModel.Organizations[0];
                    }
                }
                else
                {
                    //pick the first org for Jive. Internships won't get in here since it sets getsAdditionalValues = false;
                    determinedOrg = memberModel.Organizations[0];
                }

                if (determinedOrg.OECode != null) { oeCode = determinedOrg.OECode; }
                if (determinedOrg.BranchCode != null) { branchCode = determinedOrg.BranchCode; }
                oeAndBranch = oeCode + branchCode;
                organizationName = determinedOrg.OrganizationName;
                organizationLogoName = determinedOrg.OrganizationLogoName;
                isMemberBenefit = (!string.IsNullOrWhiteSpace(determinedOrg.OrganizationLogoName) && !determinedOrg.OrganizationLogoName.Equals("nologo", StringComparison.OrdinalIgnoreCase)).ToString();
                attributes.Add("OECode", oeCode);
                attributes.Add("OEBranch", oeAndBranch);
                attributes.Add("SchoolName", organizationName);
                attributes.Add("SchoolLogoName", organizationLogoName);
                attributes.Add("IsMemberBenefit", isMemberBenefit); //as far as business this is not used/set in courses, may need to confer with oleg though
                attributes.Add("MembershipId", memberModel.MembershipId);
            }

            return attributes;
        }

        private IDictionary<string, string> AddRemoteLearnerAttributes(IDictionary<string, string> attributes, SiteMemberModel memberModel)
        {
            //ReportingID
            int reportingIdCount = memberModel.Organizations.Count(org => !string.IsNullOrEmpty(org.ReportingId));
            if (reportingIdCount == 1)
            {
                var reportingId = memberModel.Organizations.First(org => !string.IsNullOrEmpty(org.ReportingId)).ReportingId;
                attributes.Add("ReportingID", reportingId);
            }
            else if (reportingIdCount > 1)
            {
                attributes.Add("ReportingID", "******"); //passing placeholder value to moodle to mean user has multiple reporting ids
            }

            //Courses
            var courses = memberModel.OrganizationProducts.Where(p => p.ProductTypeID == 1).GroupBy(n => n.ProductName).Select(y => y.FirstOrDefault());
            if (courses.Count() > 0)
            {
                foreach (var course in courses)
                {
                    //E.g. ("Budgeting", "1");
                    attributes.Add(course.ProductName, course.IsOrgProductActive ? "1" : "0");
                }
            }

            return attributes;
        }
        
        public ActionResult SLOService()
        {
            // Receive the single logout request or response.
            // If a request is received then single logout is being initiated by the service provider.
            // If a response is received then this is in response to single logout having been initiated by the identity provider.
            bool isRequest = false;
            bool hasCompleted = false;
            string logoutReason = null;
            string partnerSP = null;
            string relayState = null;

            SAMLIdentityProvider.ReceiveSLO(Request, Response, out isRequest, out hasCompleted, out logoutReason, out partnerSP, out relayState);

            if (isRequest)
            {
                // Logout locally.
                FormsAuthentication.SignOut();

                string logoutPath = UtilityMethods.ReadConfigValue("pathLogout");
                _log.Debug("Calling " + logoutPath);
                string logoutResponse = WebServiceRequester.MakeWebPageCall(logoutPath);
                this.HttpContext.CleanupCookies();
                

                // Respond to the SP-initiated SLO request indicating successful logout.
                SAMLIdentityProvider.SendSLO(Response, null);
            }
            else
            {
                if (hasCompleted)
                {
                    // IdP-initiated SLO has completed.
                    Response.Redirect("~/");
                }
            }

            return new EmptyResult();
        }

        public ActionResult ECP()
        {
            // Receive an authn request from an enhanced client or proxy (ECP).
            string partnerSP = null;

            SAMLIdentityProvider.ReceiveSSO(Request, out partnerSP);

            // In this example, the user's credentials are assumed to be included in the HTTP authorization header.
            // The application should authenticate the user against some user registry.
            // In this example, the credentials are assumed to be valid and no check is made.
            string userName = null;
            string password = null;

            HttpBasicAuthentication.GetAuthorizationHeader(Request, out userName, out password);

            // Respond to the authn request by sending a SAML response containing a SAML assertion to the SP.
            // Use the configured or logged in user name as the user name to send to the service provider (SP).
            // Include some user attributes.
            if (!string.IsNullOrEmpty(WebConfigurationManager.AppSettings[AppSettings.SubjectName]))
            {
                userName = WebConfigurationManager.AppSettings[AppSettings.SubjectName];
            }

            IDictionary<string, string> attributes = new Dictionary<string, string>();

            foreach (string key in WebConfigurationManager.AppSettings.Keys)
            {
                if (key.StartsWith(AppSettings.Attribute))
                {
                    attributes[key.Substring(AppSettings.Attribute.Length + 1)] = WebConfigurationManager.AppSettings[key];
                }
            }

            SAMLIdentityProvider.SendSSO(Response, userName, attributes);

            return new EmptyResult();
        }

    }
}
