using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.DirectoryServices;
using System.Configuration;
using System.Diagnostics;
using System.Security.Principal;
using System.Collections;

namespace ASA.Web.Utility.ActiveDirectory
{
    public class ADConnector
    {
        private const string CLASSNAME = " ASA.Web.Utility.ActiveDirectory.ActiveDirectory";
        static ASA.Log.ServiceLogger.IASALog _log = ASA.Log.ServiceLogger.ASALogManager.GetLogger(CLASSNAME);

        static Hashtable adConnection = (Hashtable)ConfigurationManager.GetSection("adConnection");

        static string domain = adConnection["domain"].ToString();
        static string userId = adConnection["userID"].ToString();
        static string password = adConnection["password"].ToString();
        static string userContainer = adConnection["userContainer"].ToString();
        static string groupContainer = adConnection["groupContainer"].ToString();
        static string lockOutGroup = adConnection["lockOutGroup"].ToString();
        static int sleepTime = Convert.ToInt32(adConnection["sleepTime"]);
        static int retries = Convert.ToInt32(adConnection["retries"]);

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

            try
            {

                SearchResult result = null;

                for (int i = 0; i < retries; i++)
                {
                    _log.Debug(logMethodName + "Attempting directory entry search for " + userName + " Attempt#: " + i);


                    string path = string.Format("LDAP://{0}/{1}", domain, userContainer);
                    using (DirectoryEntry directoryEntry = new DirectoryEntry(path, userId, password, AuthenticationTypes.Secure))
                    {
                        using (DirectorySearcher search = new DirectorySearcher(directoryEntry))
                        {
                            search.Filter = String.Format("(cn={0})", userName);
                            search.SearchScope = SearchScope.Subtree;
                            result = search.FindOne();
                            _log.Debug(logMethodName + "Finished searching active directory");


                            if (result != null)
                            {
                                _log.Debug(logMethodName + "User found...getting user ObjectId");

                                objectGuid = new Guid(result.GetDirectoryEntry().NativeGuid).ToString();
                                break;
                            }
                            else
                            {
                                _log.Debug(logMethodName + "No user found in active directory");

                            }
                        }


                    }




                    _log.Debug(logMethodName + "There was a problem accessing active directory, retrying...");

                    System.Threading.Thread.Sleep(sleepTime);
                }




            }
            catch (Exception ex)
            {
                throw new Exception("Error attempting to get the AD directory entry for the user", ex);
            }

            String guidString = objectGuid != null ? objectGuid.ToString() : "NULL";

            _log.Debug(logMethodName + "User objectid has been retrieved the id is: " + guidString);

            return objectGuid;
        }

        /* -FUTURE 
        public static SecurityIdentifier GetObjectByNativeGuid(Guid nativeGuid)
        {
            SecurityIdentifier identifier = null;
            try
            {
                DirectoryEntry entry = new DirectoryEntry("LADP://<GUID=" + nativeGuid.ToString() + ">");

                Byte[] sidBytes = (Byte[])entry.Properties["objectSid"][0];
                IntPtr sidPtr = System.Runtime.InteropServices.Marshal.AllocHGlobal(sidBytes.Length);
                System.Runtime.InteropServices.Marshal.Copy(sidBytes, 0, sidPtr, sidBytes.Length);

                String sid = 
                

            }
            catch (Exception ex)
            {
                throw new Exception("Error attempting to get the AD directory entry for the user", ex);
            }
            return objectGuid;
        }*/

        public static void AddUserToGroupMember(string userName)
        {
            //string groupOu = "LDAP://app.extranet.local/OU=Groups, OU=EDMSNP,DC=app,DC=extranet,DC=local";
            string groupOu = string.Format("LDAP://{0}/{1}", domain, groupContainer);
            DirectoryEntry dom = new DirectoryEntry(groupOu, userId, password, AuthenticationTypes.Secure);

            DirectoryEntry group = dom.Children.Find(lockOutGroup);
            group.Properties["member"].Add("CN=" + userName + "," + userContainer);
            group.CommitChanges();
        }

        public static void RemoveUserFromGroupMember(string userName)
        {
            string groupOu = string.Format("LDAP://{0}/{1}, {2}", domain, lockOutGroup, groupContainer);
            DirectoryEntry dirEntry = new DirectoryEntry(groupOu, userId, password, AuthenticationTypes.Secure);
            dirEntry.Properties["member"].Remove("CN=" + userName + "," + userContainer);
            dirEntry.CommitChanges();
            dirEntry.Close();
        }
        
        public static void SetPassword(string emailAddress, string newPassword)
        {
            try
            {
                string path = string.Format("LDAP://{0}/{1}", domain, userContainer);
                using (DirectoryEntry directoryEntry = new DirectoryEntry(path, userId, password, AuthenticationTypes.Secure))
                {
                    using (DirectorySearcher search = new DirectorySearcher(directoryEntry))
                    {
                        search.Filter = String.Format("(cn={0})", emailAddress);

                        search.SearchScope = SearchScope.Subtree;
                        SearchResult result = search.FindOne();

                        if (result != null)
                        {
                            try
                            {
                                DirectoryEntry adUser = result.GetDirectoryEntry();
                                adUser.Invoke("setPassword", new object[] { newPassword });
                                adUser.CommitChanges();
                                adUser.Close();
                            }
                            catch (Exception ex)
                            {
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

        public static void UpdateUsernameAndEmailAddress(string emailAddress, string newEmailAddress)
        {
            try
            {
                string path = string.Format("LDAP://{0}/{1}", domain, userContainer);
                using (DirectoryEntry directoryEntry = new DirectoryEntry(path, userId, password, AuthenticationTypes.Secure))
                {
                    using (DirectorySearcher search = new DirectorySearcher(directoryEntry))
                    {
                        search.Filter = String.Format("(cn={0})", emailAddress);

                        search.SearchScope = SearchScope.Subtree;
                        SearchResult result = search.FindOne();

                        if (result != null)
                        {
                            DirectoryEntry adUser = result.GetDirectoryEntry();



                            adUser.Rename("cn=" + newEmailAddress);
                            adUser.Properties["userPrincipalName"].Value = newEmailAddress;
                            adUser.Properties["mail"].Value = newEmailAddress;
                            adUser.CommitChanges();
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
    public class DuplicateUserException : Exception
    {
        public DuplicateUserException()
        {

        }

        public DuplicateUserException(string message): base(message)
        {

        }

    }


}
