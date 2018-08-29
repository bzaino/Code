using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Security;
using System.Web;
using DirectoryServicesWrapper;
using System.Security.Principal;
using System.DirectoryServices;

namespace ASA.Web.WTF.Integration.DotNetMembership
{
    public class DotNetMembershipAdapter : ISecurityAdapter
    {
        private const string CLASSNAME = "ASA.Web.WTF.Integration.DotNetMembership.DotNetMembershipAdapter";
        static ASA.Log.ServiceLogger.IASALog _log = ASA.Log.ServiceLogger.ASALogManager.GetLogger(CLASSNAME);


        public DotNetMembershipAdapter() { }

        public ChangePasswordStatus ChangePassword(String oldPassword, String newPassword)
        {
            String logMethodName = ".ChangePassword(String oldPassword, String newPassword) - ";
            _log.Debug(logMethodName + " - Begin Method");

            MembershipUser user = GetUser();
            Boolean changeResponse = false;

            ChangePasswordStatus status = ChangePasswordStatus.Error;
            try
            {
                changeResponse = user.ChangePassword(oldPassword, newPassword);
            }
            catch (Exception ex)
            {
                throw new SecurityAdapterException("An error has occured in the .NET Membership provider while calling MembershipUser.ChangePassword(oldPassword, newPassword)", ex);
            }

            if (changeResponse)
            {
                status = ChangePasswordStatus.Success;
            }
            else
            {
                status = ChangePasswordStatus.Failure;
            }

            _log.Debug(logMethodName + " - End Method");

            return status;

        }

        public String ResetPassword()
        {
            String logMethodName = ".ResetPassword() - ";
            _log.Debug(logMethodName + " - Begin Method");

            String newPassword = null;
            try
            {
                newPassword = GetUser().ResetPassword();
            }
            catch (Exception ex)
            {
                throw new SecurityAdapterException("An error has occured in the .NET Membership provider while calling MembershipUser.ChangePassword(oldPassword, newPassword)", ex);
            }

            _log.Debug(logMethodName + " - End Method");
            return newPassword;
        }

        public String ResetPassword(string passwordAnswer)
        {
            String logMethodName = ".ResetPassword(string passwordAnswer) - ";
            _log.Debug(logMethodName + " - Begin Method");

            MembershipUser user = GetUser();

            String newPassword = null;
            try
            {
                newPassword = user.ResetPassword(passwordAnswer);
            }
            catch (Exception ex)
            {
                throw new SecurityAdapterException("An error has occured in the .NET Membership provider while calling MembershipUser.ResetPassword(passwordAnswer)", ex);
            }
            _log.Debug(logMethodName + " - End Method");
            return newPassword;
        }

        public Boolean UnlockUser(Object membershipId)
        {
            String logMethodName = ".UnlockUser(Object membershipId) - ";
            _log.Debug(logMethodName + " - Begin Method");

            Boolean unlockResult = false;
            MembershipUser user = GetUser(membershipId);

            try
            {
                unlockResult = user.UnlockUser();
            }
            catch (Exception ex)
            {
                throw new SecurityAdapterException("An error has occured in the .NET Membership provider while calling Membership.UnlockUser()", ex);
            }
            _log.Debug(logMethodName + " - End Method");

            return unlockResult;
        }

        public virtual IMemberAccountData GetMember()
        {
            String logMethodName = ".GetMember() - ";
            _log.Debug(logMethodName + " - Begin Method");

            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
               MembershipUser user = GetUser();
               if (user != null)
               {

                   MembershipUserWrapper wrappedMember = new MembershipUserWrapper(GetUser());
                   _log.Debug(logMethodName + " - End Method");
                   return wrappedMember;
               }
            }
            _log.Debug(logMethodName + " - End Method");

            return null;
        }

        public virtual IMemberAccountData GetMember(string username)
        {
            String logMethodName = ".GetMember(string username) - ";
            _log.Debug(logMethodName + " - Begin Method");


            MembershipUser user = null;
            IMemberAccountData returnData = null;
            try
            {
                user = GetUser(username);
            }
            catch (Exception ex)
            {
                throw new SecurityAdapterException("An error has occured in the .NET Membership provider while calling Membership.GetUser(username)", ex);
            }

            if (user != null)
            {
                returnData = user.ToMemberAccountData();
            }

            _log.Debug(logMethodName + " - End Method");

            return returnData;
        }

        public virtual IMemberAccountData GetMember(object membershipId)
        {
            String logMethodName = ".GetMember(object membershipId) - ";
            _log.Debug(logMethodName + " - Begin Method");

            MemberAccountData accountData = null;
            try
            {
                accountData = GetUser(membershipId).ToMemberAccountData();
            }
            catch (Exception ex)
            {
                throw new SecurityAdapterException("An error has occured in the .NET Membership provider while calling Membership.GetUser(membershipId)", ex);
            }
            _log.Debug(logMethodName + " - End Method");

            return accountData;
        }

        public virtual IMemberAccountData CreateMember(MemberAuthInfo authInfo, out MemberCreationStatus status)
        {
            String logMethodName = ".CreateMember(MemberAuthInfo authInfo, out MemberCreationStatus status) - ";
            _log.Debug(logMethodName + " - Begin Method");
            

            IMemberAccountData returnData;

            returnData = CreateMember(
                 authInfo,
                 new MemberProfileData()
                 {
                     EmailAddress = authInfo.Email


                 },
                out status);

            _log.Debug(logMethodName + " - End Method");
            return returnData;
        }

        public virtual IMemberAccountData CreateMember(MemberAuthInfo authInfo, IMemberProfileData profile, out MemberCreationStatus status)
        {
            String logMethodName = ".CreateMember(MemberAuthInfo authInfo, IMemberProfileData profile, out MemberCreationStatus status) - ";
            _log.Debug(logMethodName + " - Begin Method");

            status = MemberCreationStatus.Error;
            
            IMemberAccountData data = null;

            MembershipCreateStatus createStatus = MembershipCreateStatus.Success;

            try
            {
                // Membership.CreateUser Method - throws exception MembershipCreateUserException if user is 
                // not created: 
                //     http://msdn.microsoft.com/en-us/library/t8yy6w3h.aspx
                data = new MembershipUserWrapper(Membership.CreateUser(
                    authInfo.Username,
                    authInfo.Password,
                    authInfo.Email));
            }

            catch (MembershipCreateUserException e)
            {
                createStatus = e.StatusCode;
                _log.Debug(logMethodName + " - User " + authInfo.Username + " was not created - status=" + createStatus);
            }

            catch (Exception ex)
            {
                SaltADMembershipProvider.ADConnector.LogActiveDirectoryMembershipProviderProperties();

                throw new SecurityAdapterException("An error has occured in the .NET Membership provider while calling Membership.CreateUser(username, password, username) for username " + authInfo.Username, ex);
            }

            switch (createStatus)
            {
                case MembershipCreateStatus.Success:
                    status = MemberCreationStatus.Success;
                    //FormsAuthentication.SetAuthCookie(authInfo.Username, true);
                    //FormsAuthentication.Authenticate(authInfo.Username, authInfo.Password);
                    break;
                case MembershipCreateStatus.DuplicateEmail:
                    status = MemberCreationStatus.DuplicateEmail;
                    break;
                case MembershipCreateStatus.DuplicateProviderUserKey:
                    status = MemberCreationStatus.DuplicateUserName;
                    break;
                case MembershipCreateStatus.DuplicateUserName:
                    status = MemberCreationStatus.DuplicateUserName;
                    break;
                case MembershipCreateStatus.InvalidAnswer:
                    status = MemberCreationStatus.InvalidAnswer;
                    break;

                case MembershipCreateStatus.InvalidEmail:
                    status = MemberCreationStatus.InvalidEmail;
                    break;
                case MembershipCreateStatus.InvalidPassword:
                    status = MemberCreationStatus.InvalidPassword;
                    break;
                case MembershipCreateStatus.InvalidProviderUserKey:
                    status = MemberCreationStatus.InvalidUserName;
                    break;
                case MembershipCreateStatus.InvalidQuestion:
                    status = MemberCreationStatus.InvalidQuestion;
                    break;
                case MembershipCreateStatus.InvalidUserName:
                    status = MemberCreationStatus.InvalidUserName;
                    break;
                case MembershipCreateStatus.ProviderError:
                    status = MemberCreationStatus.AdapterError;
                    break;
                case MembershipCreateStatus.UserRejected:
                    status = MemberCreationStatus.UserRejected;
                    break;
                default:
                    status = MemberCreationStatus.Error;

                    break;
            }

            _log.Debug(logMethodName + " - End Method");

            return data;
        }

        public Boolean ValidateCredentials(String username, String password)
        {
            String logMethodName = ".ValidateCredentials(String username, String password) - ";
            _log.Debug(logMethodName + " - Begin Method");

            Boolean returnData = false;

            try
            {
                returnData = Membership.ValidateUser(username, password);
            }
            catch (Exception ex)
            {
                throw new SecurityAdapterException("An error has occured in the .NET Membership provider while calling Membership.ValidateUser(username, password)", ex);
            }

            _log.Debug(logMethodName + " - End Method");

            return returnData;
        }

        public Int32 MinPasswordLength
        {
            get 
            {
                String logMethodName = "GET.MinPasswordLength - ";
                _log.Debug(logMethodName + " - Begin Method");

                int returnData = -1;
                try
                {
                    returnData = Membership.MinRequiredPasswordLength;
                }
                catch (Exception ex)
                {
                    throw new SecurityAdapterException("An error has occured in the .NET Membership provider while calling Membership.MinRequiredPasswordLength", ex);
                }

                _log.Debug(logMethodName + " - End Method");

                return returnData;
            }
        }

        public void SignIn(string username)
        {
            String logMethodName = ".SignIn(string username) - ";
            _log.Debug(logMethodName + " - Begin Method");
            try
            {
                HttpCookie cookie = FormsAuthentication.GetAuthCookie(username, false);
                cookie.Domain = "saltmoney.org";
                cookie.Secure = true;
                HttpContext.Current.Response.Cookies.Add(cookie);
            }
            catch (Exception ex)
            {
                throw new SecurityAdapterException("An error has occured creating AuthCookie", ex);
            }

            _log.Debug(logMethodName + " - End Method");

        }

        public void SignOut()
        {
            String logMethodName = ".SignOut() - ";
            _log.Debug(logMethodName + " - Begin Method");
            try
            {
                FormsAuthentication.SignOut();
                //Roles.DeleteCookie();
                HttpContext.Current.Session.Clear();
            }
            catch (Exception ex)
            {
                throw new SecurityAdapterException("An error has occured in the .NET Membership provider while calling Membership.SignOut()", ex);
            }
            _log.Debug(logMethodName + " - End Method");

        }

        /// <summary>
        /// Deletes a user from AD
        /// </summary>
        /// <param name="membershipId">Membership id is the username</param>
        /// <returns>bool</returns>
        public bool DeleteMember(object membershipId)
        {
            String logMethodName = ".DeleteMember(object membershipId) - ";
            _log.Debug(logMethodName + " - Begin Method");
            bool success = false;


            string username = membershipId.ToString();
            

            try
            {
                success = SaltADMembershipProvider.ADConnector.DeleteUserFromAD(username);
            }
            catch (Exception ex)
            {
                throw new SecurityAdapterException("An error has occured in the .NET Membership provider while calling SaltADMembershipProvider.ADConnector.DeleteUserFromAD(userName)", ex);
            }
            _log.Debug(logMethodName + " - End Method");

            return success;
        }

        public bool DeactivateMember(object membershipId)
        {
            String logMethodName = ".DeactivateMember(object membershipId) - ";
            _log.Debug(logMethodName + " - Begin Method");
            _log.Debug(logMethodName + " - End Method");

            throw new NotImplementedException();
        }

        private MembershipUser GetUser(Object membershipId = null)
        {
            String logMethodName = ".GetUser(Object membershipId = null) - ";
            _log.Debug(logMethodName + " - Begin Method");
            MembershipUser user = null;

            try
            {
                if (membershipId == null)
                {
                    user = Membership.GetUser();
                }
                else
                {
                    if (membershipId.ToString().IndexOf('@') == -1)
                    {
                        user = Membership.GetUser(membershipId);
                    }
                    else
                    {
                        user = Membership.GetUser(membershipId.ToString());
                    }
                }

            }
            catch (Exception ex)
            {
                throw new SecurityAdapterException("An error has occured in the .NET Membership provider while calling Membership.GetUser()", ex);
            }
            _log.Debug(logMethodName + " - End Method");

            return user;
        }

        public virtual Boolean ChangeUsername(Object membershipId, String currentUsername, String newUsername)
        {
            String logMethodName = ".ChangeUsername(Object membershipId, String currentUsername, String newUsername) - ";
            _log.Debug(logMethodName + " - Begin Method");
            bool result = false;
            try
            {
                SaltADMembershipProvider.ADConnector.UpdateUsernameAndEmailAddress(currentUsername, newUsername);
                result = true;
            }
            catch (SaltADMembershipProvider.DuplicateUserException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {

                throw new SecurityAdapterException("An error has occured in the .NET Membership provider while calling ChangeUsername(Object membershipId, String currentUsername, String newUsername)", ex);
            }
            

            _log.Debug(logMethodName + " - End Method");

            return result;

        }
        
    }
}
