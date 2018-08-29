using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ASBIdentitySource.Plugin;
using System.Web.Mvc;
using System.IO;
using System.Net;
using System.ServiceModel.Web;
using System.ServiceModel;
using System.Configuration;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web.Script.Serialization;
using System.Collections;
using System.Text.RegularExpressions;
using System.Web.Helpers;
using SALTCoursesWSClient;
using ASA.Log.ServiceLogger;

namespace ASAIDP.SSO.Plugins
{
    public class SiteMemberPlugin : IDVerifier
    {
        static readonly IASALog _log = ASALogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public IdentityResult RetrieveIdentity(Dictionary<string, object> context)
        {
            const string logMethodName = ".RetrieveIdentity(Dictionary<string, object> context) - ";
            _log.Debug(logMethodName + "Begin Method");

            // Based on partner name, Populate sso custom attributes from ASAMember Model retrieved by call to SAL
            String partnerName = (String)context["partnerName"];
            String optionalParam = (String)context["optionalParam"];
            IdentityResult result = new IdentityResult();
            
            try
            {
                string memberPath = UtilityMethods.ReadConfigValue("pathGetMember");
                string memberResponse = WebServiceRequester.MakeServiceCall(memberPath);
                SiteMemberModel memberModel = UtilityMethods.DeserializeResponse<SiteMemberModel>(memberResponse);

                bool getsAdditionalValues = true;

                //Connection with Interships.com
                if (partnerName == "SaltIDP/Internships/PSP_OAuthDevConnection_To_Internships" || partnerName == "SaltIDP/Internships/PSP_OAuthProdConnection_To_Internships")
                {
                    getsAdditionalValues = false;
                    result = AddInternshipsAttributes(result, context, optionalParam, memberModel.PrimaryEmailKey);
                }

                //Connection with community Jive Prod
                if (partnerName.Contains("SaltIDP/Jive"))
                {
                    result = AddJiveAttributes(result, memberModel, optionalParam);
                }
                //Connection with remote Learner
                else if (partnerName == "SaltIDP/RemoteLearner/PSP_Dev_ConnectionTo_MoodlePortal" || partnerName == "SaltIDP/RemoteLearner/PSP_Test_ConnectionTo_MoodlePortal" || partnerName == "SaltIDP/RemoteLearner/PSP_Stage_ConnectionTo_MoodlePortal" || partnerName == "SaltIDP/RemoteLearner/PSP_Prod_ConnectionTo_MoodlePortal")
                {
                    result = AddRemoteLearnerAttributes(result, memberModel);

                    //Setup (create/update) user in Courses
                    MoodleUser mu = new MoodleUser(memberModel);
                    mu.SetupUser();
                }

                result = AddSSOCoreAttributes(result, memberModel, partnerName, getsAdditionalValues);

            }
            catch (Exception ex)
            {
                _log.Error(logMethodName + ex);
                throw ex;
            }
            _log.Debug(logMethodName + "End Method");

            return result;

        }

        private IdentityResult AddSSOCoreAttributes(IdentityResult result, SiteMemberModel memberModel, string partnerName, bool getsAdditionalValues)
        {
            //Add core attributes
            result.Identity = memberModel.PrimaryEmailKey;
            result.AddAttribute("FirstName", memberModel.FirstName);
            result.AddAttribute("LastName", memberModel.LastName);
            result.AddAttribute("Email", memberModel.PrimaryEmailKey);
            result.AddAttribute("PartnerName", partnerName);

            if (getsAdditionalValues)
            {
                string oeCode = "000000", branchCode = "00", oeAndBranch, organizationName = "", organizationLogoName = "", isMemberBenefit = "false";
                //determine where a single org info is required to be provided
                MemberOrganizationModel determinedOrg = new MemberOrganizationModel();
                //Connection with remote Learner
                if (partnerName == "SaltIDP/RemoteLearner/PSP_Dev_ConnectionTo_MoodlePortal" || partnerName == "SaltIDP/RemoteLearner/PSP_Test_ConnectionTo_MoodlePortal" || partnerName == "SaltIDP/RemoteLearner/PSP_Stage_ConnectionTo_MoodlePortal" || partnerName == "SaltIDP/RemoteLearner/PSP_Prod_ConnectionTo_MoodlePortal")
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
                result.AddAttribute("OECode", oeCode);
                result.AddAttribute("OEBranch", oeAndBranch);
                result.AddAttribute("SchoolName", organizationName);
                result.AddAttribute("SchoolLogoName", organizationLogoName);
                result.AddAttribute("IsMemberBenefit", isMemberBenefit); //as far as business this is not used/set in courses, may need to confer with oleg though
                result.AddAttribute("MembershipId", memberModel.MembershipId);
            }

            return result;
        }

        private IdentityResult AddRemoteLearnerAttributes(IdentityResult result, SiteMemberModel memberModel)
        {
            //ReportingID
            int reportingIdCount = memberModel.Organizations.Count(org => !string.IsNullOrEmpty(org.ReportingId));
            if (reportingIdCount == 1)
            {
                var reportingId = memberModel.Organizations.First(org => !string.IsNullOrEmpty(org.ReportingId)).ReportingId;
                result.AddAttribute("ReportingID", reportingId);
            }
            else if (reportingIdCount > 1)
            {
                result.AddAttribute("ReportingID", "******"); //passing placeholder value to moodle to mean user has multiple reporting ids
            }

            //Courses
            var courses = memberModel.OrganizationProducts.Where(p => p.ProductTypeID == 1).ToList();
            if (courses.Count > 0)
            {
                foreach (var course in courses)
                {
                    //E.g. ("Budgeting", "1");
                    result.AddAttribute(course.ProductName, course.IsOrgProductActive ? "1" : "0");
                }
            }

            return result;
        }

        private IdentityResult AddJiveAttributes(IdentityResult result, SiteMemberModel memberModel, string optionalParam)
        {
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
                result.AddAttribute("UserGroup", "SALT Employee");
            }
            else
            {
                result.AddAttribute("UserGroup", "SALT Member");
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
                foreach(var qa in memberModel.ProfileQAndAs)
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
                result.AddAttribute("CampaignID", optionalParam);
            }

            if (string.IsNullOrEmpty(memberModel.CommunityDisplayName))
            {
                memberModel.CommunityDisplayName = ConfigurationManager.AppSettings["CommunityDefaultUserName"].ToString();
            }

            result.AddAttribute("IsSchoolAmbassador", isSchoolAmbassador);
            result.AddAttribute("CommunityDisplayName", memberModel.CommunityDisplayName);
            result.AddAttribute("IsCommunityActive", memberModel.IsCommunityActive.ToString());
            result.AddAttribute("YearOfBirth", memberModel.YearOfBirth.ToString());
            result.AddAttribute("GraduationYear", memberModel.Organizations[0].ExpectedGraduationYear.ToString()); //this needs an answer from business on the rules.
            result.AddAttribute("EnrollmentStatus", enrollmentStatus);
            result.AddAttribute("LoanStatus", loanStatus);

            return result;
        }

        private IdentityResult AddInternshipsAttributes(IdentityResult result, Dictionary<string, object> context, string optionalParam, string email)
        {
            result.AddAttribute("UserID", email);
            if (optionalParam == "mobileInternships")
            {
                result.AddAttribute("Mobile", "true");
            }

            var utmParams = HttpUtility.ParseQueryString(string.Empty);
            foreach (var param in context)
            {
                if (param.Key.StartsWith("utm_"))
                {
                    utmParams.Add(param.Key, param.Value.ToString());

                }
            }

            if (context.ContainsKey("UrlSuffix"))
            {
                string redirectUrl = (String)context["UrlSuffix"];
                if (utmParams.Count > 0)
                {
                    redirectUrl += "?" + utmParams.ToString();
                }
                result.AddAttribute("RedirectUrl", redirectUrl);
            }
            return result;
        }


 


    }
}
