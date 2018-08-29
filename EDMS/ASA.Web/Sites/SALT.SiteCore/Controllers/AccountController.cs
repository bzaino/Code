//The general rule with namespace order is grouped by main library stacked in alphabetical order. 
//When using composed API's Like ours its best to put the namespaces closest to your class
//at the bottm of the list

//System is always at the top
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using System.Web.Security;
using System.Text;

//3rd - we have Data Tier Classes 
//Often used directly by your code
//(TODO: These should be factored out at this layer)
using ASA.Web.Services.ASAMemberService;
using ASA.Web.Services.ASAMemberService.DataContracts;
using ASA.Web.Services.ASAMemberService.ServiceContracts;
using ASA.Web.Services.Common;

//4th - Middle tier/"smart" helper classes (same as data tier)
using ASA.Web.WTF;
using ASA.Web.WTF.Integration.MVC3;

//Last - Local Namespaces - Areas of cross dependency in the project. 
using ASA.Web.Sites.SALT.Models;
using DirectoryServicesWrapper;
using ASA.Web.WTF.Integration;
using CommonConfig = ASA.Web.Services.Common.Config;

namespace ASA.Web.Sites.SALT.Controllers
{
    public class AccountController : AsaController
    {
        private const string CLASSNAME = "ASA.Web.Sites.SALT.Controllers.AccountController";
        static ASA.Log.ServiceLogger.IASALog _log = ASA.Log.ServiceLogger.ASALogManager.GetLogger(CLASSNAME);

        const string dateFormat = "MM/dd/yyyy";
        private IAsaMemberAdapter _memberAdapter = null;

        #region Error Messages

        private const string dbFailure = "There was an error retrieving your information from our database.";
        private const string pwdChangeSuccess = "Password changed successfully.";
        private const string pwdChangeErrorUnknown = "An error occurred changing the password.";
        private const string pwdChangeErrorFailure = "The password provided is invalid. Please enter a valid password value.";
        private const string pwdChangeErrorRulesFailure = "";
        private const string profileUpdateFailure = "Profile questions member response update failed.";
        private const string profileUpdateSuccess = "Profile questions member response updated successfully.";
        private const string memberUpdateFailure = "Member update failed.";

        #endregion

        #region Construnction and Init
        public AccountController()
        {
            String logMethodName = ".ctor() - ";
            _log.Debug(logMethodName + "Begin Method");

            _memberAdapter = new AsaMemberAdapter();

            _log.Debug(logMethodName + "End Method");
        }

        /// <summary>
        /// Test Constructor
        /// </summary>
        /// <param name="memberAdapter"></param>
        public AccountController(IAsaMemberAdapter memberAdapter)
        {
            String logMethodName = ".ctor(IAsaMemberAdapter memberAdapter) - ";
            _log.Debug(logMethodName + "Begin Method - OBSOLETE");

            _memberAdapter = memberAdapter;

            _log.Debug(logMethodName + "End Method");
        }
        #endregion

        #region LogOn/LogOff
        /// <summary>
        /// Login Page
        /// </summary>
        /// <returns></returns>
        
        //still used by Lessons
        // GET: /Account/LogOn
        [OutputCache(CacheProfile = "DoNotCache")]
        public ActionResult LogOn()
        {
            String logMethodName = "HttpGet.LogOn() - ";
            _log.Debug(logMethodName + "Begin Method");
            if (!IsValidReferrer())
            {
                return Redirect("/Home/Index");
            }

            ViewResult viewResult = null;
            viewResult = View("Overlay/LogOn");

            _log.Debug(logMethodName + "End Method");
            return viewResult;
        }        
        // POST: /Account/LogOn
        [HttpPost]
        [OutputCache(CacheProfile = "DoNotCache")]
        public ActionResult LogOn(LogOnModel model, string returnUrl, string PartnerName = "")
        {
            String logMethodName = "HttpPost.LogOn(LogOnModel model, string returnUrl)) - ";
            _log.Debug(logMethodName + "Begin Method");

            JsonResult jsonResult = null;

            string cookieStateVal = model.checkRememberMe;
            string loginFailureMessage = "Your login was unsuccessful. Please check your email address and/or password and try again.";
            string loginLockOutMessage = "Your account has been locked due to 5 failed login attempts. Please try again in 30 minutes.";


            jsonResult = Json(new { Success = false, Message = loginFailureMessage });

            if (ModelState.IsValid)
            {
                _log.Debug(logMethodName + "AccountController.LogOn ModelState.IsValid");

                try
                {

                    ISiteMembership membership = IntegrationLoader.LoadDependency<ISiteMembership>("siteMembership");

                    if (membership.ValidateCredentials(model.UserName, model.Password))
                    {
                        _log.Info(logMethodName + string.Format("{0} logged on.", model.UserName.ToUpper()));
                        membership.SignIn(model.UserName);

                        ASAMemberModel activeModel = GetMemberModel(model);
                        string memberId = !string.IsNullOrEmpty(activeModel.MembershipId) ? activeModel.MembershipId : "";

                        //Delete any existing .ASPXAUTH cookie, otherwise we will send back duplicate cookies
                        //One of the ASPXAUTH cookies has default values, meaning expiration date is in the past
                        //Sometimes the browser chooses this cookie, and ignores it
                        //This causes the website to look like the user isnt logged in and gets into a state where cache needs to be cleared
                        if (System.Web.HttpContext.Current.Response.Cookies.Get(".ASPXAUTH") != null)
                        {
                            System.Web.HttpContext.Current.Response.Cookies.Remove(".ASPXAUTH");
                        }
                        try
                        {
                            _memberAdapter.SetMemberIdCookie(memberId);

                            if (cookieStateVal == "true")
                            {
                                HttpCookie cookie = FormsAuthentication.GetAuthCookie(model.UserName, true);
                                cookie.Domain = "saltmoney.org";
                                cookie.Secure = true;
                                cookie.Expires = DateTime.Now.AddYears(2);
                                System.Web.HttpContext.Current.Response.Cookies.Add(cookie);
                            }
                            else
                            {
                                HttpCookie cookie = FormsAuthentication.GetAuthCookie(model.UserName, false);
                                cookie.Domain = "saltmoney.org";
                                cookie.Secure = true;
                                System.Web.HttpContext.Current.Response.Cookies.Add(cookie);
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new SecurityAdapterException("An error has occured creating AuthCookie", ex);
                        }

                        if (!string.IsNullOrWhiteSpace(memberId))
                        {
                            int userId = Convert.ToInt32(memberId);
                            AsaMemberAdapter adapter = new AsaMemberAdapter();
                            adapter.UpdateLastLoginTimestamp(userId);
                            adapter.SyncCoursesCompletion(userId);
                        }

                        if (System.Web.HttpContext.Current.Request.Cookies.AllKeys.Contains("IndividualId"))
                        {
                            HttpCookie cookie = System.Web.HttpContext.Current.Request.Cookies["IndividualId"];
                            cookie.Domain = "saltmoney.org";
                            cookie.Expires = DateTime.Now.AddDays(-1);
                            System.Web.HttpContext.Current.Response.Cookies.Add(cookie);
                        }

                        if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/") && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                        {
                            jsonResult = Json(new { Success = true, ReturnUrl = returnUrl, MemberId = memberId, Member = activeModel });
                        }
                        //qc 7454
                        else if (isValidURI(returnUrl))
                        {
                            var uri = new Uri(returnUrl);
                            string serverPath = uri.GetLeftPart(UriPartial.Path);
                            string LoginToken = HttpUtility.ParseQueryString(uri.Query).Get("LoginToken").Replace(" ", "+");
                            if (String.IsNullOrEmpty(PartnerName))
                            {
                                PartnerName = HttpUtility.ParseQueryString(uri.Query).Get("PartnerName");
                            }
                            returnUrl = serverPath + "?LoginToken=" + Server.UrlEncode(LoginToken);
                            jsonResult = Json(new { Success = true, ReturnUrl = returnUrl + "&partnerName=" + PartnerName, MemberId = memberId, Member = activeModel });
                        }
                        //SWD-8886 added Remember me;cookieStateVal
                        else if (!String.IsNullOrEmpty(returnUrl) && (returnUrl.Contains(System.Configuration.ConfigurationManager.AppSettings["myMoney101UrlNew"].ToString()) || cookieStateVal == "true"))
                        {
                            jsonResult = Json(new { Success = true, ReturnUrl = returnUrl, MemberId = memberId, Member = activeModel });
                        }
                        else
                        {
                            // cmak. redirect user to homepage on login instead of loans page
                            HttpRequest request = System.Web.HttpContext.Current.Request;
                            string appUrl = HttpRuntime.AppDomainAppVirtualPath;
                            string portUrl = Request.Url.Port != 80 ? ":" + Request.Url.Port.ToString() : "";
                            string url = string.Format("{0}://{1}{2}", request.Url.Scheme, request.Url.Authority, appUrl);

                            jsonResult = Json(new { Success = true, ReturnUrl = url, MemberId = memberId, Member = activeModel });
                        }

                    }
                    else
                    {
                        string name = model.UserName.ToString();
                        MembershipUser mu = Membership.GetUser(name);

                        if (mu != null &&  true == mu.IsLockedOut)
                        {
                            _log.Info(logMethodName + string.Format("User id locked out --> {0}", model.UserName.ToUpper()));
                            jsonResult = Json(new { Success = false, Message = loginLockOutMessage });
                        }
                        else
                        {
                            _log.Info(logMethodName + string.Format("Invalid log on --> {0}", model.UserName.ToUpper()));
                            jsonResult = Json(new { Success = false, Message = loginFailureMessage });
                        }
                    }
                }
                catch (Exception e)
                {
                    _log.Error(logMethodName + "There was a problem encountered during login", e);
                    jsonResult = Json(new { Success = false, Message = "There was a problem encountered during login." });
                }
            }

            _log.Debug(logMethodName + "End Method");
            return jsonResult;
        }

        //qc 7454
        private bool isValidURI(string returnUrl)
        {
            String logMethodName = "bMatchedURI - ";
            _log.Debug(logMethodName + "Begin Method");

            Uri validURI;
            bool isValid = false;
            if (Uri.TryCreate(returnUrl, UriKind.Absolute, out validURI))
            {
                if (validURI.Host == new Uri(ConfigurationManager.AppSettings.Get("IdentityProvider")).Host)
                {
                    isValid = true;
                }

            }

            _log.Debug(logMethodName + "End Method");
            _log.Debug(logMethodName + "End bMatchedURI");

            return isValid;
        }

        // GET: /Account/LogOff
        [DisableContentLoad]
        public ActionResult LogOff()
        {
            String logMethodName = "HttpGet.LogOff() - ";
            _log.Debug(logMethodName + "Begin Method");

            _log.Debug(logMethodName + "Logging Out User : " + SiteMember.MemberId);
            EmptyCache();
            ISiteMembership membership = IntegrationLoader.LoadDependency<ISiteMembership>("siteMembership");
            membership.SignOut();

            _log.Debug(logMethodName + "End Method");
            return Redirect("/Home/Index");
        }

        #endregion

        #region ManageAccount

        [HttpPost]
        [OutputCache(CacheProfile = "DoNotCache")]
        public ActionResult ManageAccount(ASAMemberModel memModelIn)
        {

            bool bContinue = true;
            JsonResult jsonResult = null;

            ManageAccountModel mmaModel = GetManageAccountModel(memModelIn);
            ASAMemberModel memModelOut = new ASAMemberModel();


            String logMethodName = "HttpPost.ManageAccount(ManageAccountModel model) - ";
            _log.Debug(logMethodName + "Begin Method");
            if (mmaModel.IsValid())
            {
                if (SiteMember == null || SiteMember.Account == null || SiteMember.Profile == null)
                {
                    _log.Error(logMethodName + "Error in SiteMember, Unable to continue");
                    memModelOut.ErrorList.Add(new ErrorModel("Unable to continue"));
                    jsonResult = Json(memModelOut);
                    return jsonResult;
                }

                string currentEmail = SiteMember.Profile.Email.Address;

                #region Update email/username                    
                if (!(mmaModel.EmailAddress.Equals(currentEmail, StringComparison.InvariantCultureIgnoreCase)))
                {
                    _log.Info(logMethodName + "Processing User name change for user : " + SiteMember.MemberId);
                    _log.Debug(logMethodName + "Changing User Name...");

                    bool IsAuthorized = IntegrationLoader.CurrentSecurityAdapter.ValidateCredentials(currentEmail, mmaModel.Password);
                    bool profileUpdated = false;

                    if (IsAuthorized)
                    {
                        try
                        {
                            SiteMember.Account.ChangeUsername(mmaModel.EmailAddress);

                            _log.Debug(logMethodName + "Updating Member Profile Data");
                            MemberProfileData siteMemberUpdate = ConvertManageAccountModelToMemberProfile(mmaModel);
                            siteMemberUpdate.ActiveDirectoryKey = new Guid(SiteMember.Profile.Id.ToString());

                            //SWD-8760 - put organizations onto Member object before send to update
                            if (memModelIn.Organizations.Any())
                            {
                                siteMemberUpdate.Organizations = new MemberOrganizationList<MemberOrganizationData>();
                                foreach (MemberOrganizationModel organization in memModelIn.Organizations)
                                {
                                    MemberOrganizationData item = new MemberOrganizationData();
                                    item.OrganizationId = organization.OrganizationId;
                                    item.OECode = organization.OECode;
                                    item.BranchCode = organization.BranchCode;
                                    item.ExpectedGraduationYear = organization.ExpectedGraduationYear;
                                    item.ReportingId = organization.ReportingId;
                                    item.IsOrganizationDeleted = organization.IsOrganizationDeleted;
                                    siteMemberUpdate.Organizations.Add(item);
                                }
                            }

                            SiteMember.Profile.SetValues(siteMemberUpdate);                            

                            try
                            {
                                profileUpdated = SiteMember.Profile.Save();
                            }
                            catch (Exception ex)                            
                            {
                                _log.Error(logMethodName + "Error occured while attempting to update profile", ex);
                                throw new WtfException("Error occured while attempting update profile", ex);
                            }

                            if (profileUpdated)
                            {
                                _log.Debug(logMethodName + "Username change successful...logging user out of system.");

                                try
                                {
                                    string emailContent = GetEmailTemplate(Config.EmailaddressChangedEmail);

                                    if (!String.IsNullOrEmpty(emailContent))
                                    {
                                        string emailOldAddressEmail = string.Empty;
                                        string emailNewAddressEmail = string.Empty;
                                        if (memModelIn.Organizations != null && memModelIn.Organizations.Count() > 0)
                                        {
                                            emailOldAddressEmail = emailContent
                                                    .Replace("|%~Profile.DisplayName%|", mmaModel.FirstName)
                                                    .Replace("|%~Profile.EmailAddress%|", mmaModel.EmailAddress.Trim())
                                                    .Replace("|%~School.logo%|", memModelIn.Organizations[0].Brand)
                                                    .Replace("|%~School.logo%|", memModelIn.Organizations[0].Brand)
                                                    .Replace("|%~School.logo%|", memModelIn.Organizations[0].Brand)
                                                    .Replace("|%~School.name%|", memModelIn.Organizations[0].OrganizationName);

                                            emailNewAddressEmail = emailContent
                                                    .Replace("|%~Profile.DisplayName%|", mmaModel.FirstName)
                                                    .Replace("|%~Profile.EmailAddress%|", mmaModel.EmailAddress.Trim())
                                                    .Replace("|%~School.logo%|", memModelIn.Organizations[0].Brand)
                                                    .Replace("|%~School.logo%|", memModelIn.Organizations[0].Brand)
                                                    .Replace("|%~School.logo%|", memModelIn.Organizations[0].Brand)
                                                    .Replace("|%~School.name%|", memModelIn.Organizations[0].OrganizationName);
                                        }
                                        try
                                        {
                                            _log.Debug(logMethodName + "Sending EmailAddress changed email to old emailAddress");
                                            SendConfirmation("Your Email Address for saltmoney.org Has Changed", currentEmail, emailOldAddressEmail);
                                        }
                                        catch
                                        {
                                            _log.Error(logMethodName + "EmailAddress changed email failed to be sent to old EmailAddress");
                                        }

                                        try
                                        {
                                            _log.Debug(logMethodName + "Sending EmailAddress changed email to new emailAddress");
                                            SendConfirmation("Your Email Address for saltmoney.org Has Changed", mmaModel.EmailAddress.Trim(), emailNewAddressEmail);
                                        }
                                        catch
                                        {
                                            _log.Error(logMethodName + "EmailAddress changed email failed to be sent to new EmailAddress");
                                        }
                                    }
                                    else
                                    {
                                        _log.Error(logMethodName + "EmailAddress changed email failed template was empty");
                                    }
                                }
                                catch
                                {
                                    _log.Error(logMethodName + "EmailAddress changed email failed to be sent");
                                }

                                EmptyCache();
                                LogOff();

                                mmaModel.RedirectURL = "/index.html";
                                bContinue = false;
                            }                            
                            
                        }
                        catch (SaltADMembershipProvider.DuplicateUserException ex)
                        {
                            _log.Error(logMethodName + string.Format("Email address change failed. Current Email: '{0}', New Email: '{1}'", currentEmail, mmaModel.EmailAddress), ex);
                            mmaModel.ErrorList.Add(new ErrorModel("", "Email address already exists."));
                            _log.Debug(logMethodName + "End Method");
                            bContinue = false;
                            //return View(mmaModel);
                        }
                        catch (Exception ex)
                        {
                            _log.Error(logMethodName + string.Format("Email address change failed. Current Email: '{0}', New Email: '{1}'", currentEmail, mmaModel.EmailAddress), ex);
                            mmaModel.ErrorList.Add(new ErrorModel("", "Email address change failed"));
                            bContinue = false;
                            //return View(mmaModel);

                        }
                        if (!profileUpdated)
                        {
                            //rollback EmailAddress change in AD
                            _log.Error(logMethodName + "Error occured while attempting to update profile, Now Rolling back Email address change");
                            _log.Debug(logMethodName + "Error occured while attempting to update Email address " + currentEmail);
                            mmaModel.ErrorList.Add(new ErrorModel("", "Profile change Failed."));
                            mmaModel.ErrorList.Add(new ErrorModel("", "Rolling back Email address change"));
                            SiteMember.Account.Username = mmaModel.EmailAddress;
                            SiteMember.Account.ChangeUsername(currentEmail);
                            SiteMember.Account.Username = currentEmail;
                            mmaModel.EmailAddress = currentEmail;
                            mmaModel.UserName = currentEmail;
                            bContinue = false;
                        }

                    }
                    else
                    {
                        mmaModel.EmailAddress = currentEmail; //revert back email change in the model to current email
                        mmaModel.UserName = currentEmail;

                        _log.Error(logMethodName + string.Format("An error occurred changing Email address, '{0}' to '{1}'", currentEmail, mmaModel.EmailAddress));
                        mmaModel.ErrorList.Add(new ErrorModel("", "Your email could not be changed. Please make sure your password is correct and try again."));
                        bContinue = false;
                    }
                }
                #endregion

                #region Update Password
                if(bContinue)
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(mmaModel.ConfirmPassword) && !string.IsNullOrEmpty(mmaModel.Password))
                        {
                            _log.Info(logMethodName + "Processing change password request for user : " + SiteMember.MemberId);

                            SaltADMembershipProvider.ADConnector.ChangePassword(mmaModel.EmailAddress.Trim(), mmaModel.Password, mmaModel.NewPassword);
                            _log.Debug(logMethodName + "User Password Changed Successfully");

                            try
                            {
                                string emailContent = GetEmailTemplate(Config.NewPasswordEmail);

                                if (!String.IsNullOrEmpty(emailContent))
                                {
                                    string email = emailContent
                                            .Replace("|%~Profile.DisplayName%|", mmaModel.FirstName);

                                    _log.Debug(logMethodName + "Sending password change confirmaton message");

                                    SendConfirmation("Your Salt Password has changed", mmaModel.EmailAddress.Trim(), email);
                                }
                                else
                                {
                                    _log.Error(logMethodName + "Password Changed email failed template was empty");
                                }
                            }
                            catch
                            {
                                _log.Error(logMethodName + "Password Changed email failed to be sent");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _log.Error(logMethodName
                            + string.Format("An error occurred updating student '{0} {1}'.  MembershipAccountID: '{2}'",
                            mmaModel.FirstName, mmaModel.LastName, mmaModel.MembershipAccoundId != null ? mmaModel.MembershipAccoundId : "not found"), ex);

                        mmaModel.ErrorList.Add(new ErrorModel("Membership-Database-Failure", dbFailure));
                        bContinue = false;
                    }
                }
                #endregion

                #region Update Profile
                if (bContinue)
                {                    
                    try
                    {
                        _log.Debug(logMethodName + "Updating Member Profile Data");
                        MemberProfileData siteMemberUpdate = ConvertManageAccountModelToMemberProfile(mmaModel);

                        SiteMember.Profile.SetValues(siteMemberUpdate);

                        bool memberUpdated = UpdateMember(memModelIn);
                        if (memberUpdated)
                        {
                            _log.Debug(logMethodName + string.Format("Member profile updated for student '{0} {1}', MembershipAccountID: '{2}'", mmaModel.FirstName, mmaModel.LastName, mmaModel.MembershipAccoundId));
                        }
                        else
                        {
                            _log.Error(logMethodName + string.Format("Update Failed for member profile student '{0} {1}', MembershipAccountID: '{2}'", mmaModel.FirstName, mmaModel.LastName, mmaModel.MembershipAccoundId));
                            mmaModel.ErrorList.Add(new ErrorModel("Profile-Update-Failure", memberUpdateFailure));
                        }
                        
                        bool responseUpdated = UpdateMemberProfileQResponses(memModelIn);
                        if (responseUpdated)
                        {
                            _log.Debug(logMethodName + string.Format("Member profile question responses updated for student '{0} {1}', MembershipAccountID: '{2}'", mmaModel.FirstName, mmaModel.LastName, mmaModel.MembershipAccoundId));
                        }
                        else
                        {
                            _log.Error(logMethodName + string.Format("Update Failed for member profile questions responses for student '{0} {1}', MembershipAccountID: '{2}'", mmaModel.FirstName, mmaModel.LastName, mmaModel.MembershipAccoundId));
                            mmaModel.ErrorList.Add(new ErrorModel("Profile-Update-Failure", profileUpdateFailure));
                        }
                    }
                    catch (Exception ex)
                    {
                        _log.Error(logMethodName + string.Format("An error occurred updating student '{0} {1}' does not exist in Membership Database. MembershipAccountID: '{2}'", mmaModel.FirstName, mmaModel.LastName, mmaModel.MembershipAccoundId != null ? mmaModel.MembershipAccoundId : "not found"), ex);
                        mmaModel.ErrorList.Add(new ErrorModel("Membership-Database-Failure", dbFailure));
                        bContinue = false;
                    }
                }
                #endregion

                EmptyCache();

                memModelOut = GetMemberModel(mmaModel);
                jsonResult = Json(memModelOut);
            }
            else
            {
                _log.Debug(logMethodName + "Invalid model");

                //give back the incoming model/data with errorlist included.
                memModelIn.ErrorList = mmaModel.ErrorList;
                jsonResult = Json(memModelIn);
            }

            _log.Debug(logMethodName + "End Method");
            return jsonResult;
        }

        private bool UpdateMemberProfileQResponses(ASAMemberModel memModelIn)
        {
            AsaMemberAdapter _adapter = new AsaMemberAdapter();
            IList<MemberProfileQAModel> responses = new List<MemberProfileQAModel>();
            foreach (var response in memModelIn.ProfileQAndAs)
            {
                MemberProfileQAModel respModel = new MemberProfileQAModel();
                respModel.AnsExternalId = response.AnsExternalId;
                respModel.QuestionExternalId = response.QuestionExternalId;
                respModel.CustomValue = response.CustomValue;
                responses.Add(respModel);
            }

            bool result = _adapter.UpdateMemberProfileResponses(memModelIn.MembershipId, responses);
            return result;

        }

        private bool UpdateMember(ASAMemberModel memModelIn)
        {
            const string logMethodName = ".UpdateMember(ASAMemberModel memModelIn) - ";
            _log.Debug(logMethodName + "Begin Method");

            AsaMemberAdapter adapter = new AsaMemberAdapter();
            Asa.Salt.Web.Common.Types.Enums.MemberUpdateStatus status;

            try
            {
                status = adapter.Update(memModelIn);
            }
            catch (Exception ex)
            {
                throw new DataProviderException("Unable to update member profile", ex);
            }

            _log.Debug(logMethodName + "End Method");

            if (status == Asa.Salt.Web.Common.Types.Enums.MemberUpdateStatus.Success)
                return true;
            else
                return false;
        }

        private ASAMemberModel GetMemberModel(LogOnModel model)
        {
            ASAMemberModel member = _memberAdapter.GetMemberByEmail(model.UserName);
            return member;
        }

        private ASAMemberModel GetMemberModel(ManageAccountModel mmaModel)
        {
            ASAMemberModel mmaModelOut = _memberAdapter.GetMemberByEmail(mmaModel.UserName);
            //COV-10479 - ensure model is not null before using.
            if (mmaModelOut != null)
            {
                mmaModelOut.ErrorList = mmaModel.ErrorList;
                mmaModelOut.RedirectURL = mmaModel.RedirectURL;
            }
            return mmaModelOut;
        }

        private ManageAccountModel GetManageAccountModel(ASAMemberModel memModel)
        {

            ManageAccountModel outModel = new ManageAccountModel();

            outModel.AddressValidated = false;

            //Only one school should be coming in we'll figure out if it's and add or update later on...
            if (memModel.Organizations.Count > 0)
            {
                outModel.OrganizationId = memModel.Organizations[0].OrganizationId;
                outModel.BranchCode = memModel.Organizations[0].BranchCode;
                outModel.OECode = memModel.Organizations[0].OECode;
                outModel.OrganizationName = memModel.Organizations[0].OrganizationName;
                outModel.ExpectedGraduationYear = memModel.Organizations[0].ExpectedGraduationYear;
            }

            outModel.EnrollmentStatus = memModel.EnrollmentStatus;
            outModel.GradeLevel = memModel.GradeLevel;
            outModel.ContactFrequency = memModel.ContactFrequency;            
            outModel.FirstName = memModel.FirstName;
            outModel.LastName = memModel.LastName;
            outModel.DisplayName = memModel.DisplayName;
            outModel.Source = memModel.Source;
            outModel.USPostalCode = (String.IsNullOrEmpty(memModel.USPostalCode) ? null : memModel.USPostalCode);
            outModel.SALTSchoolTypeID = (memModel.SALTSchoolTypeID != null ? memModel.SALTSchoolTypeID : null);
            outModel.IsCommunityActive = memModel.IsCommunityActive;
            outModel.WelcomeEmailSent = memModel.WelcomeEmailSent;

            if (memModel.YearOfBirth.HasValue)
                outModel.YOB = memModel.YearOfBirth; 

            string emailAddress = string.Empty;

            try
            {
                emailAddress = memModel.Emails.SingleOrDefault(e => e.IsPrimary).EmailAddress;
            }
            catch (Exception ex)
            {
                _log.Error(string.Format(ex.Message));
            }

            outModel.UserName = emailAddress;
            outModel.EmailAddress = emailAddress;

            outModel.MembershipAccoundId = memModel.MembershipId;
            outModel.MembershipStartDate = memModel.MembershipStartDate.GetValueOrDefault().ToShortDateString();

            if (memModel.Phones.Count > 0)
            {
                outModel.PhoneNumber = memModel.Phones[0].PhoneNumber;
                outModel.PhoneNumberType = memModel.Phones[0].Type;
            }
            else
            {
                outModel.PhoneNumber = string.Empty;
                outModel.PhoneNumberType = string.Empty;
            }
            outModel.NewPassword = memModel.NewPassword;
            outModel.ConfirmPassword = memModel.NewPassword; //TODO: is this right?
            outModel.Password = memModel.Password;

            return outModel;
        }
        #endregion

        #region ForgotPassword
        public ActionResult ForgotPassword()
        {
            String logMethodName = "HttpGet.ForgotPassword() - ";
            _log.Debug(logMethodName + "Begin Method");
            if (!IsValidReferrer())
            {
                return Redirect("/Home/Index");
            }

            ViewResult viewResult = null;
            viewResult = View("Overlay/ForgotPassword");

            _log.Debug(logMethodName + "End Method");
            return viewResult;
        }

        [HttpPost]
        //[ValidateAntiForgeryToken(Salt = "FORGOT_PASSWORD")]
        [OutputCache(CacheProfile = "DoNotCache")]
        public ActionResult ForgotPassword(ForgotPasswordModel model)
        {
            String logMethodName = "HttpPost.ForgotPassword(ForgotPasswordModel model) - ";
            _log.Debug(logMethodName + "Begin Method");
            JsonResult jsonResult = null;

            JsonResult jsonErrorResult = Json(new { Success = false, Message = "There was an error sending confirmation email. Please try again." });
            if (ModelState.IsValid)
            {
                try
                {
                    string emailAddr = model.Email;

                    ASAMemberModel member = _memberAdapter.GetMemberByEmail(emailAddr);

                    if (member != null && member.IndividualId != null)
                    {
                        _log.Info(logMethodName + "Processing Forgot password Request for user : " + member.ActiveDirectoryKey);

                        string port = "";
                        if (Request.Url.Port != 80)
                        {
                            port = ":" + Request.Url.Port.ToString();
                        }

                        model.ResendPasswordUrl = Request.Url.Scheme + "://" + Request.Url.Host + port + Request.ApplicationPath.Replace("//", "/") + "Home/NewPassword?";

                        string encTicket = "";
                        if (member.IndividualId != null)
                        {

                            try
                            {
                                DateTime today = DateTime.Now;

                                // Create a forms authentication ticket what will expire (1 day is the default but this could change in web.config)
                                FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(
                                    1,
                                    member.IndividualId,   // Active Directory Key
                                    today,
                                    today.AddMinutes(Config.ForgotPasswordTokenExpire),
                                    true,
                                    "");

                                // Encrypt the authentication ticket
                                encTicket = FormsAuthentication.Encrypt(ticket);
                            }

                            catch (Exception ex)
                            {
                                _log.Warn(logMethodName + "There was an error creating ticket to send confirmation email. Please try again.", ex);
                                return jsonErrorResult;
                            }
                        }

                        if (!string.IsNullOrEmpty(encTicket))
                            model.ResendPasswordUrl += "token=" + encTicket;

                        try
                        {
                            string emailContent = GetEmailTemplate(Config.ForgotPasswordEmail);

                            if (!String.IsNullOrEmpty(emailContent))
                            {
                                string email = emailContent
                                        .Replace("|%~Profile.DisplayName%|", member.FirstName)
                                        .Replace("|%ResetPasswordTokenUrl%|", model.ResendPasswordUrl);

                                _log.Debug(logMethodName + "Sending Password email");
                                SendConfirmation("Reset Your Salt Password", emailAddr, email);
                            }
                            else
                            {
                                _log.Error(logMethodName + "There was an error sending confirmation email template was empty.");
                                return jsonErrorResult;
                            }
                        }
                        catch (Exception ex)
                        {
                            _log.Error(logMethodName + "There was an error sending confirmation email. Please try again.", ex);
                            return jsonErrorResult;
                        }
                    }
                    else
                    {
                        _log.Debug(logMethodName + "User not found for email : " + model.Email);
                    }
                }
                catch (Exception ex)
                {
                    _log.Error(logMethodName + "There was a problem sending the Forgot Password email.", ex);
                    return jsonErrorResult;
                }
            }

            jsonResult = Json(new { Success = true });

            _log.Debug(logMethodName + "End Method");
            return jsonResult;
        }
        #endregion

        #region ResendPassword (aka "NewPassword" / PasswordReset)
        [OutputCache(CacheProfile = "DoNotCache")]
        public ActionResult NewPassword(string token)
        {
            String logMethodName = "HttpGet.NewPassword(string token) - ";
            _log.Debug(logMethodName + "Begin Method");
            _log.Debug(logMethodName + "Loading new Password for token : " + token);

            ViewResult viewResult = null;

            if (!IsValidReferrer())
            {
                return Redirect("/Home/Index");
            }

            NewPasswordModel model = new NewPasswordModel();
            viewResult = View("NewPassword", model);
            model.Token = token;

            MemberEmailModel email;
            try
            {
                AsaMemberAdapter memberAdapter = new AsaMemberAdapter();
                ASAMemberModel member = memberAdapter.GetMember(new Guid(token)); //TODO: Update WTF API to support this correctly
				
                email = member.Emails.SingleOrDefault(e => e.IsPrimary);
                if (email == null || string.IsNullOrEmpty(email.EmailAddress))
                {
                    throw new SALTException("An email address does not exist for this user.");
                }

            }
            catch (Exception ex)
            {
                _log.Error(logMethodName + string.Format("An error occurred retrieving member by token for token: {0}", token), ex);
                ModelState.AddModelError("Membership-Database-Failure", dbFailure);
                return viewResult;
            }

            model.Email = email.EmailAddress;

            _log.Debug(logMethodName + "End Method");
            return viewResult;
        }

        [HttpPost]
        [OutputCache(CacheProfile = "DoNotCache")]
        [DisableContentLoad]
        public ActionResult NewPassword(NewPasswordModel model)
        {
            String logMethodName = "HttpPost.NewPassword(NewPasswordModel model) - ";
            _log.Debug(logMethodName + "Begin Method");

            JsonResult jsonResult = null;
            //This is safe to do. When creating a result object all you are doing is setting the properites. The constructors should do 
            //nothing. Result objects execute when the MVC framework calls the Execute method using a controllercontext
            JsonResult jsonError = Json(new { Success = false, Message = "Your password must be at least 8 characters and contain 3 of the following: uppercase letter, lowercase letter, number, special character (like !, @, #, $, &, *)." });

            if (ModelState.IsValid)
            {
                try
                {
                    FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(model.Token);

                    if (ticket != null && !ticket.Expired)
                    {
                        ASAMemberModel asaMemberModel;

                        try
                        {
                            //COV-10480 - check to ensure asaMemberModel if null to return error.
                            asaMemberModel = _memberAdapter.GetMemberByEmail(model.Email.Trim());
                            if (asaMemberModel == null || asaMemberModel.ActiveDirectoryKey != ticket.Name)
                            {
                                if (asaMemberModel == null)
                                {
                                    _log.Warn(logMethodName + "Member could not be found by e-mail address");
                                }
                                else
                                {
                                    _log.Warn(logMethodName + "Forgot Password Email Token doesn't match e-mail address");
                                }
                                return jsonError;
                            }
                        }
                        catch (Exception ex)
                        {
                            _log.Error(logMethodName + "An error occurred retrieving member from AD email: " + model.Email, ex);
                            return jsonError;
                        }

                        _log.Debug(logMethodName + "Processing change password request for user : " + model.Email);
                        MembershipUser member;
                        try
                        {

                            member = Membership.GetUser(model.Email.Trim(), false);
                            SaltADMembershipProvider.ADConnector.SetPassword(model.Email.Trim(), model.NewPassword);
                        }
                        catch (Exception ex)
                        {
                            _log.Error(logMethodName + "An error occurred changing password", ex);

                            return jsonError;
                        }

                        ChangePasswordModel cpm = new ChangePasswordModel();

                        try
                        {
                            string emailContent = GetEmailTemplate(Config.NewPasswordEmail);

                            if (!String.IsNullOrEmpty(emailContent))
                            {
                                string email = emailContent
                                        .Replace("|%~Profile.DisplayName%|", asaMemberModel.FirstName);

                                _log.Debug(logMethodName + "Sending password change confirmaton message");

                                SendConfirmation("Your Salt Password has changed", model.Email, email);
                            }
                            else
                            {
                                _log.Error(logMethodName + "Failed to send email for:" + model.Email,  " template was empty");
                            }
                        }
                        catch (Exception ex)
                        {
                            _log.Error(logMethodName + "Failed to send email for:" + model.Email, ex);
                        }
                    }
                    else
                    {
                        _log.Warn(logMethodName + "Forgot Password Token invalid or expired for email " + model.Email);
                        return jsonError;
                    }
                }
                catch(Exception ex)
                {
                    _log.Warn(logMethodName + "Unable to decrypt Forgot Password Token for email " + model.Email, ex);
                    return jsonError;
                }
            }
            
            else
            {
                _log.Warn(logMethodName + "Invalid Model");
                return jsonError;
            }

            jsonResult = Json(new { Success = true });

            _log.Debug(logMethodName + "End Method");
            return jsonResult;
        }

        #endregion

        #region DeActivateAccount
        /// <summary>
        /// Not in Spec
        /// </summary>
        /// <returns></returns>
        /// 
        //[DisableContentLoad]
        public ActionResult DeActivateAccount()
        {
            String logMethodName = "HttpGet.DeActivateAccount() - ";
            _log.Debug(logMethodName + "Begin Method");

            if (!IsValidReferrer())
            {
                return Redirect("/Home/Index");
            }

            ViewResult viewResult = null;
            viewResult = View("Overlay/DeActivateAccount");

            _log.Debug(logMethodName + "End Method");
            return viewResult;


        }

        /// <summary>
        /// Not in Spec
        /// </summary>
        /// <param name="confirm"></param>
        /// <returns></returns>
        [HttpPost]
        [DisableContentLoad]
        public ActionResult DeActivateAccount(bool? confirm)
        {
            String logMethodName = "HttpPost.DeActivateAccount(bool? confirm) - ";
            _log.Debug(logMethodName + "Begin Method");

            //It is preferable to use the built in MVC helpers for generatng URL's you need to pass around the applicaton.
            //This ensures that as routing is changed overtime your code will still work properly. 

            //String url = Url.RouteUrl("Home",
            //     new RouteValueDictionary(new { controller = "Home", action = "Index" }),
            //     Request.Url.Scheme, Request.Url.Host);

            string url = Request.Url.Scheme + "://" + Request.Url.Host + (Request.Url.Port != 80 ? ":" + Request.Url.Port.ToString() : "") + Request.ApplicationPath.Replace("//", "/") + "index.html";

            JsonResult jsonResult = null;
            jsonResult = Json(new { Success = true, NavigateTo = url });

            try
            {
                _log.Debug(logMethodName + "Deactivating Member Account Member : " + SiteMember.MemberId);

                //TODO: Add Deactivate to WTF Api, create 1 call at this layer to replace logic in controller with logic in
                //SiteMember and its supporting providers/adapters.
                _memberAdapter.DeActivateAccount(SiteMember.MemberId.ToString());
            }
            catch (Exception ex)
            {
                _log.Error(logMethodName + "Error deleting member from Avectra", ex);
                jsonResult = Json(new { Success = false, Message = "An error occurred deactivating your account" });
                _log.Debug(logMethodName + "End Method");
                return jsonResult;
            }

            ISiteMembership membership = IntegrationLoader.LoadDependency<ISiteMembership>("siteMembership");

            try
            {
                SaltADMembershipProvider.ADConnector.DeleteUserFromAD(User.Identity.Name);
                HttpContext.Cache.Remove(User.Identity.Name);
                membership.SignOut();
            }
            catch (Exception ex)
            {
                _log.Error(logMethodName + "Error deleting member from AD", ex);
                jsonResult = Json(new { Success = false, Message = "An error occurred deactivating your account" });
                _log.Debug(logMethodName + "End Method");
                return jsonResult;
            }
            _log.Debug(logMethodName + "End Method");
            return jsonResult;

        }
        #endregion


        #region Helper Methods
        private MemberProfileData ConvertManageAccountModelToMemberProfile(ManageAccountModel model)
        {
            String logMethodName = ".ConvertManageAccountModelToMemberProfile(ManageAccountModel model) - ";
            _log.Debug(logMethodName + "Begin Method");

            MemberProfileData siteMemberUpdate = new MemberProfileData();

            siteMemberUpdate.FirstName = model.FirstName;
            siteMemberUpdate.LastName = model.LastName;
            siteMemberUpdate.DisplayName = model.DisplayName;
            siteMemberUpdate.EnrollmentStatus = model.EnrollmentStatus;
            siteMemberUpdate.ContactFrequency = model.ContactFrequency;
            siteMemberUpdate.Source = model.Source;
            siteMemberUpdate.GradeLevel = model.GradeLevel;
            siteMemberUpdate.USPostalCode = model.USPostalCode;
            siteMemberUpdate.SALTSchoolTypeID = model.SALTSchoolTypeID;
            siteMemberUpdate.IsCommunityActive = model.IsCommunityActive;
            siteMemberUpdate.WelcomeEmailSent = model.WelcomeEmailSent;

            if (model.YOB == null)
            {
                siteMemberUpdate.YearOfBirth = SiteMember.Profile.YearOfBirth;
            }
            else
            {
                siteMemberUpdate.YearOfBirth = model.YOB;
            }

            // Grabbing the primary allows us to not have to store the object key on the client side. (In a multiton scenario this or some surrgoate would need to be used to update properly)

            siteMemberUpdate.EmailAddress = !string.IsNullOrWhiteSpace(model.EmailAddress)
                                                ? model.EmailAddress
                                                : SiteMember.Profile.Email.Address;

            _log.Debug(logMethodName + "End Method");
            return siteMemberUpdate;
        }

        private string GetEmailTemplate(string emailTemplateFileName)
        {
            String logMethodName = ".GetEmailTemplate(string emailTemplateFileName) - ";
            _log.Debug(logMethodName + "Begin Method");

            string emailContent;

            try
            {
                string filename = HttpContext.Request.PhysicalApplicationPath.ToString() + emailTemplateFileName;

                using(StreamReader sr = new StreamReader(filename))
                { 
                    emailContent = sr.ReadToEnd();
                }
            }
            catch
            {
                _log.Warn(logMethodName + "Email Template " + emailTemplateFileName + " failed to load");
                emailContent = String.Empty;
            }

            _log.Debug(logMethodName + "End Method");
            return emailContent;
        }

        private void SendConfirmation(string emailSubject, string email, string emailbody)
        {
            String logMethodName = ".SendConfirmation(string emailSubject, string email, string emailbody) - ";
            _log.Debug(logMethodName + "Begin Method");

            var to = email;
            var from = Config.SALTEmailSender;
            var subject = emailSubject;
            var body = emailbody;
            var message = new MailMessage(from, to, subject, body) { SubjectEncoding = Encoding.Default, IsBodyHtml = true, BodyEncoding = Encoding.Default };
            SmtpClient mailClient = null;

            _log.Debug(logMethodName + string.Format("Sending email to {0}, SMTPServer ={1}:{2}, subject={3}", email, Config.SMTPServer, Config.SMTPServerPort, emailSubject));

            //COV 10466
            try
            {
                mailClient = new SmtpClient(Config.SMTPServer, Config.SMTPServerPort);
                mailClient.Send(message);
                mailClient.Dispose();
            }
            catch (Exception ex)
            {
                _log.ErrorFormat("Error sending email: {0}\n\n{1}", ex.InnerException.StackTrace);
                if (mailClient != null)
                {
                    mailClient.Dispose();
                }
            }
            finally
            {
                message.Dispose();
            }

            _log.Debug(logMethodName + "End Method");
        }

        #endregion
    }
}
