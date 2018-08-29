
using System.Web.Security;
//using System.Diagnostics;
using System.DirectoryServices;
using System.Security.Permissions;

using System;
using System.Security;
using System.Collections;
using System.Configuration;
using System.Runtime.InteropServices;

namespace DirectoryServicesWrapper
{

    [SecuritySafeCritical]
    [DirectoryServicesPermission(SecurityAction.Assert, Unrestricted = true)]
    public class SaltADMembershipProvider : ActiveDirectoryMembershipProvider
    {
        public SaltADMembershipProvider()
            : base()
        {
            //Below is used for debugging purposes

            //Assembly a = Assembly.GetExecutingAssembly();
            //System.Security.SecurityRuleSet srs = a.SecurityRuleSet;
            //bool ift = a.IsFullyTrusted;

            //Type t = a.GetType("DirectoryServicesWrapper.SaltADMembershipProvider");

            //bool isc = t.IsSecurityCritical;
            //bool issc = t.IsSecuritySafeCritical;
            //bool ist = t.IsSecurityTransparent;

        }
        private const string Classname = "ASA.Web.DirectoryServicesWrapper.SaltADMembershipProvider";
        //using log4net here as opposed to ASA.Log.ServiceLogger.IASALog because IASALog is not strongly 
        //named and can't be used in the GAC
        private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(Classname);

        [SecuritySafeCritical]
        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {
            base.Initialize(name, config);
        }

        // Demand the zone requirement for the calling application.
        [SecuritySafeCritical]
        public override bool ValidateUser(string username, string password)
        {
            // Reading the name of the selected file from the OpenFileDialog box
            // and reading the file requires FileIOPermission.  The user control should 
            // have this permission granted through its code group; the Web page that calls the 
            // control should not have this permission.  The Assert command prevents a stack walk 
            // that would fail because the caller does not have the required FileIOPermission.  
            // The use of Assert can open up security vulnerabilities if used incorrectly or 
            // inappropriately. Therefore, it should be used with great caution.
            // The Assert command should be followed by a RevertAssert as soon as the file operation 
            // is completed.
            //new FileIOPermission(PermissionState.Unrestricted).Assert();
            new DirectoryServicesPermission(PermissionState.Unrestricted).Assert();

            return base.ValidateUser(username, password);
        }

        [SecuritySafeCritical]
        public override MembershipUser GetUser(string userId, bool userOnline)
        {
            return base.GetUser(userId, userOnline);
        }

        [SecuritySafeCritical]
        public MembershipUser GetUser()
        {

            return Membership.GetUser();
        }

        [SecuritySafeCritical]
        public MembershipUser GetUser(object obj)
        {
            return Membership.GetUser(obj);
        }

        [SecuritySafeCritical]
        public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
        {
            return base.CreateUser(username, password, email, passwordQuestion, passwordAnswer, isApproved, providerUserKey, out status);
        }

        [SecuritySafeCritical]
        [DirectoryServicesPermission(SecurityAction.Assert, Unrestricted = true)]
        public class ADConnector
        {
            private const string CLASSNAME = " ASA.Web.Utility.ActiveDirectory.ActiveDirectory";

            static Hashtable adConnection = (Hashtable)ConfigurationManager.GetSection("adConnection");

            static string domain = adConnection["domain"].ToString();
            static string userId = adConnection["userID"].ToString();
            static string password = adConnection["password"].ToString();
            static string userContainer = adConnection["userContainer"].ToString();
            static string groupContainer = adConnection["groupContainer"].ToString();
            static string lockOutGroup = adConnection["lockOutGroup"].ToString();
            static int sleepTime = Convert.ToInt32(adConnection["sleepTime"]);
            static int retries = Convert.ToInt32(adConnection["retries"]);

            [SecuritySafeCritical]
            public static string GetObjectGUID(string userName)
            {
                String logMethodName = ".GetObjectGUID(string userName)";

                _log.Debug(logMethodName + "Method Begin");
                _log.Debug(logMethodName + "Looking up ActiveDirectory Object ID using ADSI : " + userName);


                string objectGuid = string.Empty;

                if (string.IsNullOrEmpty(userName))
                {
                    _log.Warn(logMethodName + "Error: Username is empty");

                    return objectGuid;
                }

                _log.Debug(logMethodName + "Provided username is valid, looking user up in ActiveDirectory");
                for (int i = 0; i < retries; i++)
                {
                    SearchResult result = null;
                    DirectoryEntry resultDE = null;
                    try
                    {
                        _log.Debug(logMethodName + "Attempting directory entry search for " + userName + " Attempt#: " + i);
                        string path = string.Format("LDAP://{0}/{1}", domain, userContainer);

                        using (DirectoryEntry directoryEntry = new DirectoryEntry(path, userId, password, AuthenticationTypes.Secure))
                        {

                            //Refresh Cache to attempt to get past the AD error "The server is not operational"
                            directoryEntry.RefreshCache();

                            //COV-10460 - removed test for if (directoryEntry != null) not needed directoryEntry.RefreshCache() returns object
                            using (DirectorySearcher search = new DirectorySearcher(directoryEntry))
                            {
                                search.Filter = String.Format("(cn={0})", userName);
                                search.SearchScope = SearchScope.Subtree;
                                result = search.FindOne();
                                _log.Debug(logMethodName + "Finished searching active directory");
                                if (result != null)
                                {
                                    _log.Debug(logMethodName + "User found...getting user ObjectId");

                                    resultDE = result.GetDirectoryEntry();
                                    objectGuid = new Guid(resultDE.NativeGuid).ToString();
                                    resultDE.Close();
                                    break;
                                }
                                else
                                {
                                    _log.Debug(logMethodName + "No user found in active directory: search for " + userName + " Attempt#: " + i);
                                    // SWD-5616 - Sleep before you try again, even if there wasn't an exception.
                                    System.Threading.Thread.Sleep(sleepTime);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _log.Debug(logMethodName + "Exception: ", ex);

                        //COV-10549 - fix resource leak on exception
                        if (resultDE != null)
                        {
                            resultDE.Close();
                        }
                        //if exhausted the number of tries and still no ad user found throw ex 
                        if (i >= retries)
                        {
                            throw new Exception("Exception attempting to get the AD directory entry for the user. No more retries. User Name: " + userName, ex);
                        }
                        //log and go to sleep
                        else
                        {
                            _log.Debug(logMethodName + "GetObjectGUID active directory set theard to sleep: " + sleepTime + " search for " + userName + " Attempt#: " + i);
                            System.Threading.Thread.Sleep(sleepTime);
                        }
                    }
                }

                //SWD-5161 - Make absolutely sure we are not returning an empty string.
                if (objectGuid == null || objectGuid == "")
                {
                    throw new Exception("Null or empty object GUID for user. No more retries. User Name: " + userName);
                }

                String guidString = objectGuid != null ? objectGuid.ToString() : "NULL";

                _log.Debug(logMethodName + "User objectid has been retrieved the id is: " + guidString);

                return objectGuid;
            }

            //SWD-7461 - can not use/access System.Diagnostics when running under trust level="High" or below - need full control
            //[SecuritySafeCritical]
            //public static void LogSystemDiagnosticsProcessGetCurrentProcess()
            //{
            //    String logMethodName = ".LogSystemDiagnosticsProcessGetCurrentProcess()";

            //    var currentProcess = System.Diagnostics.Process.GetCurrentProcess();
            //    _log.Error(logMethodName + "Process information");
            //    _log.Error(logMethodName + "-------------------");
            //    _log.Error(logMethodName + "CPU time");
            //    _log.ErrorFormat(logMethodName + "Total       {0}", currentProcess.TotalProcessorTime);
            //    _log.ErrorFormat(logMethodName + "User        {0}", currentProcess.UserProcessorTime);
            //    _log.ErrorFormat(logMethodName + "Privileged  {0}", currentProcess.PrivilegedProcessorTime);
            //    _log.Error(logMethodName + "Memory usage");
            //    _log.ErrorFormat(logMethodName + "Current     {0:N0} B", currentProcess.WorkingSet64);
            //    _log.ErrorFormat(logMethodName + "Peak        {0:N0} B", currentProcess.PeakWorkingSet64);
            //    _log.ErrorFormat(logMethodName + "Active threads  {0:N0}", currentProcess.Threads.Count);

            //    System.Diagnostics.Process[] processlist = System.Diagnostics.Process.GetProcesses();
            //    foreach (System.Diagnostics.Process theprocess in processlist)
            //    {
            //        _log.ErrorFormat(logMethodName + "Process: {0} ID: {1}", theprocess.ProcessName, theprocess.Id);
            //    }
            //}

            [SecuritySafeCritical]
            public static void LogActiveDirectoryMembershipProviderProperties()
            {
                String logMethodName = ".LogActiveDirectoryMembershipProviderProperties()";

                ActiveDirectoryMembershipProvider adProvider = (ActiveDirectoryMembershipProvider)(Membership.Provider);

                _log.Error(logMethodName + "Method Begin");
                _log.ErrorFormat(logMethodName + "ApplicationName : {0}", adProvider.ApplicationName);
                _log.ErrorFormat(logMethodName + "CurrentConnectionProtection : {0}", adProvider.CurrentConnectionProtection);
                _log.ErrorFormat(logMethodName + "Description : {0}", adProvider.Description);
                _log.ErrorFormat(logMethodName + "EnablePasswordReset : {0}", adProvider.EnablePasswordReset);
                _log.ErrorFormat(logMethodName + "EnablePasswordRetrieval : {0}", adProvider.EnablePasswordRetrieval);
                _log.ErrorFormat(logMethodName + "EnableSearchMethods : {0}", adProvider.EnableSearchMethods);
                _log.ErrorFormat(logMethodName + "MinRequiredNonAlphanumericCharacters : {0}", adProvider.MaxInvalidPasswordAttempts);
                _log.ErrorFormat(logMethodName + "ApplicationName : {0}", adProvider.MinRequiredNonAlphanumericCharacters);
                _log.ErrorFormat(logMethodName + "MinRequiredPasswordLength : {0}", adProvider.MinRequiredPasswordLength);
                _log.ErrorFormat(logMethodName + "Name : {0}", adProvider.Name);
                _log.ErrorFormat(logMethodName + "PasswordAnswerAttemptLockoutDuration : {0}", adProvider.PasswordAnswerAttemptLockoutDuration);
                _log.ErrorFormat(logMethodName + "PasswordAttemptWindow : {0}", adProvider.PasswordAttemptWindow);
                _log.ErrorFormat(logMethodName + "PasswordFormat : {0}", adProvider.PasswordFormat);
                _log.ErrorFormat(logMethodName + "PasswordStrengthRegularExpression : {0}", adProvider.PasswordStrengthRegularExpression);
                _log.ErrorFormat(logMethodName + "RequiresQuestionAndAnswer : {0}", adProvider.RequiresQuestionAndAnswer);
                _log.ErrorFormat(logMethodName + "RequiresUniqueEmail : {0}", adProvider.RequiresUniqueEmail);
                _log.Error(logMethodName + "Method End");
            }

            [SecuritySafeCritical]
            public static void AddUserToGroupMember(string userName)
            {
                string groupOu = string.Format("LDAP://{0}/{1}", domain, groupContainer);
                //COV 10341
                using (DirectoryEntry dom = new DirectoryEntry(groupOu, userId, password, AuthenticationTypes.Secure))
                {
                    //COV 10340
                    using (DirectoryEntry group = dom.Children.Find(lockOutGroup))
                    {
                        group.Properties["member"].Add("CN=" + userName + "," + userContainer);
                        group.CommitChanges();
                    }
                }
            }

            [SecuritySafeCritical]
            public static void RemoveUserFromGroupMember(string userName)
            {
                string groupOu = string.Format("LDAP://{0}/{1}, {2}", domain, lockOutGroup, groupContainer);
                DirectoryEntry dirEntry = new DirectoryEntry(groupOu, userId, password, AuthenticationTypes.Secure);
                dirEntry.Properties["member"].Remove("CN=" + userName + "," + userContainer);
                dirEntry.CommitChanges();
                dirEntry.Close();
            }

            [SecuritySafeCritical]
            public static bool DeleteUserFromAD(string userName)
            {
                //this is just a wrapper to the existing call that was used in Full trust
                return Membership.DeleteUser(userName, true);
            }

            [SecuritySafeCritical]
            public static void SetPassword(string emailAddress, string newPassword)
            {
                try
                {
                    string path = string.Format("LDAP://{0}/{1}", domain, userContainer);
                    using (DirectoryEntry directoryEntry = new DirectoryEntry(path, userId, password, AuthenticationTypes.Secure))
                    {

                        //Refresh Cache to attempt to get past the AD error "The server is not operational"
                        directoryEntry.RefreshCache();

                        using (DirectorySearcher search = new DirectorySearcher(directoryEntry))
                        {
                            search.Filter = String.Format("(cn={0})", emailAddress);

                            search.SearchScope = SearchScope.Subtree;
                            SearchResult result = search.FindOne();

                            if (result != null)
                            {
                                DirectoryEntry adUser = result.GetDirectoryEntry();
                                try
                                {
                                    
                                    //COV-10456 - get object returned so we can close it properly if one is returned
                                    DirectoryEntry retObject = (DirectoryEntry)adUser.Invoke("setPassword", new object[] { newPassword });
                                    adUser.CommitChanges();
                                    adUser.Close();
                                    if (retObject != null)
                                        retObject.Close();
                                }
                                catch (Exception ex)
                                {
                                    //COV-10548 - fix resource leak on exception
                                    if (adUser != null)
                                    {
                                        adUser.Close();
                                    }
                                    throw new Exception("User " + emailAddress + " AD setPassword() call failed.   Cannot reset password.  Exception=" + ex.Message);
                                }
                            }
                            else
                            {
                                throw new Exception("User " + emailAddress + " not found in Active Directory.   Cannot reset password.");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error searching Active Directory for user " + emailAddress + " during SetPassword().   Exception=" + ex.Message);
                }
            }

            [SecuritySafeCritical]
            public static void ChangePassword(string emailAddress, string oldPassword, string newPassword)
            {
                try
                {
                    string path = string.Format("LDAP://{0}/{1}", domain, userContainer);
                    using (DirectoryEntry directoryEntry = new DirectoryEntry(path, userId, password, AuthenticationTypes.Secure))
                    {
                        //Refresh Cache to attempt to get past the AD error "The server is not operational"
                        directoryEntry.RefreshCache();

                        using (DirectorySearcher search = new DirectorySearcher(directoryEntry))
                        {
                            search.Filter = String.Format("(cn={0})", emailAddress);

                            search.SearchScope = SearchScope.Subtree;
                            SearchResult result = search.FindOne();

                            if (result != null)
                            {
                                DirectoryEntry adUser = result.GetDirectoryEntry();
                                try
                                {                              
                                    //COV-10457 - get object returned so we can close it properly if one is returned
                                    DirectoryEntry retObject = (DirectoryEntry)adUser.Invoke("changePassword", new object[] { oldPassword, newPassword });
                                    adUser.CommitChanges();
                                    adUser.Close();
                                    if (retObject != null)
                                        retObject.Close();
                                }
                                catch (Exception ex)
                                {
                                    //COV-10546 - fix resource leak on exception
                                    if (adUser != null)
                                    {
                                        adUser.Close();
                                    }
                                    Exception baseException = ex.GetBaseException();
                                    if (baseException is COMException)
                                    {
                                        COMException comException = baseException as COMException;
                                        switch (comException.ErrorCode)
                                        {
                                            case -2147024810:
                                                throw new Exception("The current password does not match our records. Please try again.");
                                            case -2147022651:
                                                throw new Exception("The new password does not meet security requirements of the domain.");
                                            case -2147023570:
                                                throw new Exception("invalidate user or password.");
                                            case -2147016657:
                                                throw new Exception("invalidate user or password. Please try again. ");
                                            default:
                                                throw ex;
                                        }
                                    }
                                    else
                                    {
                                        throw new Exception("Error updating password for user " + emailAddress + ". Cannot reset password.");
                                    }
                                }
                            }
                            else
                            {
                                throw new Exception("User " + emailAddress + " not found in Active Directory.   Cannot reset password.");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error searching Active Directory for user " + emailAddress + " during SetPassword().   Exception=" + ex.Message);
                }
            }

            [SecuritySafeCritical]
            public static void UpdateUsernameAndEmailAddress(string emailAddress, string newEmailAddress)
            {
                try
                {
                    string path = string.Format("LDAP://{0}/{1}", domain, userContainer);
                    using (DirectoryEntry directoryEntry = new DirectoryEntry(path, userId, password, AuthenticationTypes.Secure))
                    {
                        //Refresh Cache to attempt to get past the AD error "The server is not operational"
                        directoryEntry.RefreshCache();

                        using (DirectorySearcher search = new DirectorySearcher(directoryEntry))
                        {
                            search.Filter = String.Format("(cn={0})", emailAddress);

                            search.SearchScope = SearchScope.Subtree;
                            SearchResult result = search.FindOne();

                            if (result != null)
                            {
                                DirectoryEntry adUser = result.GetDirectoryEntry();

                                //COV-10547 - fix resource leak on exception
                                try
                                {
                                    adUser.Rename("cn=" + newEmailAddress);
                                    adUser.Properties["userPrincipalName"].Value = newEmailAddress;
                                    adUser.Properties["mail"].Value = newEmailAddress;
                                    adUser.CommitChanges();
                                    adUser.Close();
                                }
                                catch (Exception ex)
                                {
                                    if (adUser != null)
                                    {
                                        adUser.Close();
                                    }
                                    throw new Exception(String.Format("Could not change User email address --> {0}, Exception = {1}", emailAddress, ex.Message));
                                }
                            }
                            else
                            {
                                throw new Exception("User not found --> " + emailAddress);
                            }
                        }
                    }
                }
                catch (DirectoryServicesCOMException ex)
                {
                    if (ex.Message == "The object already exists.\r\n")
                    {
                        throw new DuplicateUserException(ex.Message);
                    }
                    else
                    {
                        throw ex;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error updating username and email address.", ex);
                }
            }

            private static DirectoryEntry GetDirectoryEntry()
            {
                string path = string.Format("LDAP://{0}/{1}", domain, userContainer);
                DirectoryEntry directoryEntry = new DirectoryEntry(path, userId, password, AuthenticationTypes.Secure);
                return directoryEntry;
            }
        }

        [SecuritySafeCritical]
        public class DuplicateUserException : Exception
        {
            public DuplicateUserException()
            {

            }

            public DuplicateUserException(string message)
                : base(message)
            {

            }

        }

    }
}
