using System;
using System.Collections.Generic;
using System.Configuration;
using System.DirectoryServices;
using System.Web.Configuration;
using System.Web.Security;

using log4net;

namespace SALTShaker.BLL
{
    public class SaltSiteADmanager : IDisposable
    {
        Configuration rootWebConfig1 = WebConfigurationManager.OpenWebConfiguration(null);
        KeyValueConfigurationElement AD_LDAP_ConnectSetting;
        KeyValueConfigurationElement AD_LDAP_UserID;
        KeyValueConfigurationElement AD_LDAP_Password;
        private static readonly ILog logger = LogManager.GetLogger(typeof(SaltSiteADmanager));
        private object RetutnObj;

        public SaltSiteADmanager()
        {
            //constructor
        }
        // GET: /AD/Validate
        public string ValidateUser(string emailAdd)
        {
            const string logMethodName = "-  ValidateUser(string emailAdd) - ";
            logger.Debug(logMethodName + "Begin Method");
            string User = Membership.GetUserNameByEmail("testUser001@asa.org");

            if (string.IsNullOrEmpty(User))
            {
                return String.Format(GlobalMessages.sMSG_EMAILNOTFOUND, User, ".");
            }

            logger.Debug("- End Method -");
            return "Welcome " + User;
        }
        public ADUserDetails GetUserOU(string sEmail)
        {
            const string logMethodName = "-  GetUserOU(string sEmail) - ";
            logger.Debug(logMethodName + "Begin Method");

            //get web config settings
            GetADWebConfigSettings();
            string emailAdd = sEmail.Trim();
            int iActiveFlag;
            bool bisNotActive;
            ADUserDetails userDetails = new ADUserDetails();
            userDetails.EmailAddress = emailAdd;
            try
            {
                using (DirectoryEntry myLdapConnection = new DirectoryEntry(AD_LDAP_ConnectSetting.Value, AD_LDAP_UserID.Value, AD_LDAP_Password.Value, AuthenticationTypes.Secure))
                {

                    using (DirectorySearcher search = new DirectorySearcher(myLdapConnection))
                    {

                        //Searching on CN cause CN can be duplicate
                        search.Filter = ("(&(objectclass=user)(objectcategory=person)(CN=" + emailAdd + "))");
                        SearchResultCollection collectedResult = search.FindAll();

                        if (collectedResult.Count.Equals(0))
                        {
                            search.Filter = ("(&(objectclass=user)(objectcategory=person)(mail=" + emailAdd + "))");
                            collectedResult = search.FindAll();
                        }

                        foreach (SearchResult temp in collectedResult)
                        {

                            UserDetail userDetail = new UserDetail();
                            userDetail.DateOfCreation = DateTime.Parse(temp.Properties["WhenCreated"][0].ToString()).ToLocalTime().ToString();
                            //May not always be here so check first
                            if (!String.IsNullOrEmpty(temp.Properties["distinguishedname"][0].ToString()))
                                userDetail.EnvironmentName = GetEnvironmentName(temp.Properties["distinguishedname"][0].ToString());

                            userDetail.UserName = temp.Properties["name"][0].ToString();
                            userDetail.UserPrincipalName = temp.Properties["userPrincipalName"][0].ToString();
                            userDetail.Mail = temp.Properties["mail"][0].ToString();
                            userDetail.CN = temp.Properties["CN"][0].ToString();

                            //check for active account
                            iActiveFlag = (int)temp.Properties["userAccountControl"][0];
                            bisNotActive = Convert.ToBoolean(iActiveFlag & 0x0002);
                            if (bisNotActive)
                            {
                                userDetail.bisActice = false;
                            }
                            else
                            {
                                userDetail.bisActice = true;
                            }
                            userDetails.UserDetails.Add(userDetail);
                            //COV-10550 - variable not used removing.
                            //DirectoryEntry ou = temp.GetDirectoryEntry();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("SaltSiteADmanager.ADUserDetails:" + ex.Message);
                ExceptionMessageException Oops = new ExceptionMessageException(ex.Message);
                userDetails.ErrorMsg = ex.Message;
            }

            logger.Debug("- End Method -");
            return userDetails;
        }

        private string GetEnvironmentName(string p)
        {
            const string logMethodName = "- GetEnvironmentName(string p)- ";
            logger.Debug(logMethodName + "Begin Method");
            try
            {
                if (p.Contains("EDMSNP"))
                {
                    return "Non-Production";
                }
                else if (p.Contains("EDMS"))
                {
                    return "Production";
                }
                else if (p.Contains("AMSAUsers"))
                {
                    return "AMSA\\Users";
                }
                else { return p; }
            }
            catch (Exception ex)
            {
                logger.Error("SaltSiteADmanager.GetEnvironmentName: " + ex.Message);
                return p;
            }
            finally
            {
                logger.Debug(logMethodName + "End Method");
            }
        }

        public bool IsMemberActive(String email)
        {
            return FindADAccount(email) != null;
        }

        private SearchResult FindADAccount(String sEmail)
        {
            const string logMethodName = "-  FindADAccount(string sEmail) - ";
            logger.Debug(logMethodName + "Begin Method");

            string emailAdd = sEmail.Trim();
            string UserName = emailAdd;
            SearchResult result = null;
            //testUser001@asa.or
            try
            {
                GetADWebConfigSettings();
                using (DirectoryEntry myLdapConnection = new DirectoryEntry(AD_LDAP_ConnectSetting.Value, AD_LDAP_UserID.Value, AD_LDAP_Password.Value, AuthenticationTypes.Secure))
                {
                    //DirectoryEntry myLdapConnection = new DirectoryEntry("LDAP://app.extranet.local/DC=app,DC=extranet,DC=local", "sv_NPedmsacctcre", "XeJpOWC1", AuthenticationTypes.Secure);
                    using (DirectorySearcher search = new DirectorySearcher(myLdapConnection))
                    {
                        //Searching on CN cause CN can be duplicate
                        search.Filter = ("(&(objectclass=user)(objectcategory=person)(CN=" + emailAdd + "))");
                        SearchResultCollection collectedResult = search.FindAll();
                        if (collectedResult.Count.Equals(0))
                        {
                            search.Filter = ("(&(objectclass=user)(objectcategory=person)(mail=" + emailAdd + "))");
                            collectedResult = search.FindAll();
                        }

                        result = search.FindOne();
                    }
                }

            }
            catch (System.DirectoryServices.DirectoryServicesCOMException ex)
            {
                //DoSomethingWith --> E.Message.ToString();
                logger.Error("SaltSiteADmanager.SearchResult: DirectoryServicesCOMException " + ex.Message);
                ExceptionMessageException Oops = new ExceptionMessageException(ex.Message);

            }
            catch (Exception ex)
            {
                logger.Error("SaltSiteADmanager.SearchResult:" + ex.Message);
                ExceptionMessageException Oops = new ExceptionMessageException(ex.Message);
            }

            logger.Debug("- End Method -");
            return result;
        }

        public string DeleteADAccount(string sEmail)
        {
            const string logMethodName = "- DeleteADAccount(string sEmail) - ";
            logger.Debug(logMethodName + "Begin Method");

            string emailAdd = sEmail.Trim();
            string UserName = emailAdd;
            object MemberObject = new object[] { };
            SearchResult result = FindADAccount(emailAdd);
            string sRetString = string.Empty;

            using (DirectoryEntry entryToUpdate = new DirectoryEntry())
            {
                try
                {
                    string ADUserName = Membership.GetUserNameByEmail(emailAdd);

                    if (!string.IsNullOrEmpty(ADUserName))
                    {
                        bool delected = Membership.DeleteUser(ADUserName);
                        if (delected)
                        {
                            sRetString = String.Format(GlobalMessages.sMSG_USER_ACTACTIVATE, UserName, "deleted").ToString();
                            return sRetString;
                        }
                        else
                        {
                            sRetString = String.Format(GlobalMessages.sMSG_EMAILNOTFOUND, UserName, ".");
                            return sRetString;
                        }
                    }
                    //try this if other method fails
                    if (result != null)
                    {
                        MemberObject = new object[] { result.Path };
                        RetutnObj = entryToUpdate.Invoke("Remove", MemberObject);
                        entryToUpdate.CommitChanges();
                        entryToUpdate.Close();
                        entryToUpdate.Dispose();
                        sRetString = string.Format(GlobalMessages.sMSG_USER_ACTACTIVATE, UserName, "deleted");
                        return sRetString;
                    }
                    else
                    {
                        sRetString = string.Format(GlobalMessages.sMSG_EMAILNOTFOUND, UserName, ".");
                        return sRetString;
                    }
                }

                catch (System.DirectoryServices.DirectoryServicesCOMException ex)
                {
                    ExceptionMessageException Oops = new ExceptionMessageException(ex.Message);
                    string msg = UserName + " was not found to be deleted because of error: " + ex.Message;
                    logger.Error("SaltSiteADmanager.DeleteADAccount:DirectoryServicesCOMException " + msg);
                    sRetString = string.Format(GlobalMessages.sMSG_EMAILNOTFOUND, msg, GlobalMessages.sCUSTOM_ERRORCODE);
                    return sRetString;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    ExceptionMessageException Oops = new ExceptionMessageException(ex.Message);
                    logger.Error("SaltSiteADmanager.DeleteADAccount: " + ex.Message);
                    sRetString = string.Format(GlobalMessages.sMSG_EMAILNOTFOUND, UserName + " was not found to be deleted because of error: " + ex.Message, GlobalMessages.sCUSTOM_ERRORCODE);
                    return sRetString;
                }
                finally 
                {
                    entryToUpdate.Dispose();
                    logger.Debug("- End Method -");
                }
            }
        }

        public string Deactivate(string sEmail)
        {
            const string logMethodName = "- Deactivate(string sEmail) - ";
            logger.Debug(logMethodName + "Begin Method");

            string emailAdd = sEmail.Trim();
            string UserName = emailAdd;
            try
            {
                SearchResult result = FindADAccount(emailAdd); //search.FindOne();

                if (result != null)
                {
                    DirectoryEntry entryToUpdate = result.GetDirectoryEntry();
                    DateTime expires;
                    // get convert long system_object type from AD to date ;
                    long AccountExpire = GetInt64(entryToUpdate, "accountExpires");
                    if (AccountExpire == long.MaxValue || AccountExpire <= 0 || DateTime.MaxValue.ToFileTime() <= AccountExpire)
                    {
                        expires = DateTime.MaxValue;
                    }
                    else
                    {
                        expires = DateTime.FromFileTimeUtc(AccountExpire);
                    }

                    int iActiveFlag = (int)entryToUpdate.Properties["userAccountControl"].Value;
                    bool bisNotActive = Convert.ToBoolean(iActiveFlag & 0x0002);
                    if (bisNotActive)
                    {
                        //reactivate account
                        entryToUpdate.Properties["userAccountControl"].Value = iActiveFlag & ~0x2; //ADS_UF_NORMAL_ACCOUNT;
                        entryToUpdate.Properties["LockOutTime"].Value = 0; //unlock account
                    }
                    else
                    {
                        //Disable a User Account
                        entryToUpdate.Properties["userAccountControl"].Value = iActiveFlag | 0x2; //ADS_UF_ACCOUNTDISABLE;
                    }

                    entryToUpdate.CommitChanges();
                    entryToUpdate.Close();
                    Console.WriteLine("\n\n...User accout was disable");
                    return String.Format(GlobalMessages.sMSG_USER_ACTACTIVATE, UserName, (bisNotActive) ? "reactivated" : "deactivated");
                }
                else
                {
                    return String.Format(GlobalMessages.sMSG_EMAILNOTFOUND, UserName, ".");
                }
            }
            catch (System.DirectoryServices.DirectoryServicesCOMException ex)
            {
                //DoSomethingWith --> E.Message.ToString();
                logger.Error("SaltSiteADmanager.Deactivate: DirectoryServicesCOMException " + ex.Message);
                ExceptionMessageException Oops = new ExceptionMessageException(ex.Message);
                return String.Format(GlobalMessages.sMSG_EMAILNOTFOUND, UserName + " was not found to be deleted because of error: " + ex.Message, GlobalMessages.sCUSTOM_ERRORCODE);
            }
            catch (Exception ex)
            {
                logger.Error("SaltSiteADmanager.Deactivate: " + ex.Message);
                ExceptionMessageException Oops = new ExceptionMessageException(ex.Message);
                return String.Format(GlobalMessages.sMSG_EMAILNOTFOUND, UserName + " was not found to be deleted because of error: " + ex.Message, GlobalMessages.sCUSTOM_ERRORCODE);
            }
            finally
            {
                logger.Debug("- End Method -");
            }
        }

        public string UpdateADUser(string emailAdd, string oldEmail)
        {
            const string logMethodName = "- UpdateADUser(string emailAdd, string oldEmail) - ";
            logger.Debug(logMethodName + "Begin Method");

            if (!string.IsNullOrEmpty(emailAdd))
            {
                SearchResult result;
                try
                {
                    GetADWebConfigSettings();
                    //DirectoryEntry myLdapConnection = new DirectoryEntry("LDAP://app.extranet.local/OU=Users,OU=EDMSNP,DC=app,DC=extranet,DC=local", "sv_NPedmsacctcre", "XeJpOWC1", AuthenticationTypes.Secure); ;
                    using (DirectoryEntry myLdapConnection = new DirectoryEntry(AD_LDAP_ConnectSetting.Value, AD_LDAP_UserID.Value, AD_LDAP_Password.Value, AuthenticationTypes.Secure))
                    {
                        MembershipUser currentUser = Membership.GetUser(emailAdd.Trim(), true /* userIsOnline */);
                        using (DirectorySearcher search = new DirectorySearcher(myLdapConnection))
                        {
                            search.Filter = ("(&(objectclass=user)(objectcategory=person)(CN=" + oldEmail.Trim() + "))");
                            SearchResultCollection collectedResult = search.FindAll();
                            if (collectedResult.Count.Equals(0))
                            {
                                search.Filter = ("(&(objectclass=user)(objectcategory=person)(mail=" + oldEmail.Trim() + "))");
                                collectedResult = search.FindAll();
                            }

                            result = search.FindOne();
                        }
                        if (result != null)
                        {
                            // create new object from search result
                            using (DirectoryEntry entryToUpdate = result.GetDirectoryEntry())
                            {
                                // get new title and write to AD
                                entryToUpdate.Rename("CN=" + emailAdd.Trim()); //changes name property too
                                entryToUpdate.Rename("userPrincipalName=" + emailAdd.Trim());
                                entryToUpdate.Rename("mail=" + emailAdd.Trim());
                                entryToUpdate.Dispose();
                            }
                            logger.Debug(String.Format(GlobalMessages.sMSG_USER_IMFO_CHANGED_SUCCESS, oldEmail, emailAdd));
                        }
                        else
                        {
                            string msg = String.Format(GlobalMessages.sMSG_USER_IMFO_CHANGED_FAILDED, oldEmail, GlobalMessages.sCUSTOM_ERRORCODE);
                            logger.Debug(msg);
                            return msg;
                        }
                    }

                }
                catch (Exception ex)
                {
                    logger.Error("SaltSiteADmanager.UpdateADUser: " + ex.Message);
                    ExceptionMessageException Oops = new ExceptionMessageException(ex.Message);
                    return String.Format(GlobalMessages.sMSG_WARNING, "Member with email address " + oldEmail + " was not updated because of AD error: " + ex.Message + " " + GlobalMessages.sCUSTOM_ERRORCODE).ToString();
                }
                finally 
                {
                    logger.Debug("- End Method -");
                }
            }
            else
            {
                return String.Format(GlobalMessages.sMSG_VALUE_ISNULL, "Email address", GlobalMessages.sCUSTOM_ERRORCODE);
            }
          
            return String.Format(GlobalMessages.sMSG_USER_IMFO_CHANGED_SUCCESS, oldEmail, emailAdd);
        }

        private string GetADWebConfigSettings()
        {
            const string logMethodName = "- GetADWebConfigSettings() - ";
            logger.Debug(logMethodName + "Begin Method");

            try
            {
                if (rootWebConfig1.AppSettings.Settings.Count == 0)
                    //try diffrent path
                    rootWebConfig1 = WebConfigurationManager.OpenWebConfiguration("/");

                if (rootWebConfig1.AppSettings.Settings.Count > 0)
                {
                    AD_LDAP_ConnectSetting = rootWebConfig1.AppSettings.Settings["ADServiceLDAPConnection"];
                    AD_LDAP_UserID = rootWebConfig1.AppSettings.Settings["userID"];
                    AD_LDAP_Password = rootWebConfig1.AppSettings.Settings["password"];
                    if (AD_LDAP_ConnectSetting == null)
                        return String.Format(GlobalMessages.sMSG_WARNING, "Config Error: ADServiceLDAPConnection must be set in webconfig " + GlobalMessages.sCUSTOM_ERRORCODE);
                    if (AD_LDAP_UserID == null)
                        return String.Format(GlobalMessages.sMSG_WARNING, "Config Error: userID must be set in webconfig " + GlobalMessages.sCUSTOM_ERRORCODE);
                    if (AD_LDAP_Password == null)
                        return String.Format(GlobalMessages.sMSG_WARNING, "Config Error: password must be set in webconfig " + GlobalMessages.sCUSTOM_ERRORCODE);
                }
                return "Success";
            }
            catch (Exception ex)
            {
                logger.Error("SaltSiteADmanager.GetADWebConfigSettings: " + ex.Message);
                ExceptionMessageException Oops = new ExceptionMessageException(ex.Message);
                return String.Format(GlobalMessages.sMSG_WARNING, ex.Message + "  " + GlobalMessages.sCUSTOM_ERRORCODE);
            }
            finally
            {
                logger.Debug("- End Method -");
            }
        }

        private Int64 GetInt64(DirectoryEntry entry, string attr)
        {
            const string logMethodName = "- GetInt64(DirectoryEntry entry, string attr) - ";
            logger.Debug(logMethodName + "Begin Method");
            try
            {
                //we will use the marshaling behavior of the searcher
                using (DirectorySearcher ds = new DirectorySearcher(entry, String.Format("({0}=*)", attr), new string[] { attr }, SearchScope.Base))
                {
                    SearchResult sr = ds.FindOne();
                    if (sr != null)
                    {
                        if (sr.Properties.Contains(attr))
                        {
                            return (Int64)sr.Properties[attr][0];
                        }
                    }
                    return -1;
                }
            }
            catch (Exception ex)
            {
                logger.Error("SaltSiteADmanager.GetInt64: " + ex.Message);
                return -1;
            }
            finally
            {
                logger.Debug("- End Method -");
            }
        }

        public List<UserDetail> GetADList(string sDomainName, string sSearchParam = "")
        {
            const string logMethodName = "- GetADList(string sDomainName, string sSearchParam = '')- ";
            logger.Debug(logMethodName + "Begin Method");
            ADUserDetails ADUserList = new ADUserDetails();
            try
            {
                using (DirectoryEntry entry = new DirectoryEntry("LDAP://" + sDomainName + "/OU=AMSAUsers,OU=Users,OU=Boston,DC=amsa,DC=com"))
                {
                    using (DirectorySearcher search = new DirectorySearcher(entry))
                    {

                        if (!String.IsNullOrEmpty(sSearchParam))
                        {
                            // this searches for objects that sSearchParam
                            search.Filter = (String.Format("({0}=*)", sSearchParam));
                        }

                        SearchResultCollection collectedResult = search.FindAll();

                        foreach (SearchResult temp in collectedResult)
                        {
                            UserDetail ADuser = new UserDetail();
                            if (temp.Properties["mail"].Count > 0)
                            {
                                if (!String.IsNullOrEmpty(temp.Properties["mail"][0].ToString()))
                                {
                                    ADuser.DateOfCreation = DateTime.Parse(temp.Properties["WhenCreated"][0].ToString()).ToLocalTime().ToString();
                                    //May not always be here so check first
                                    if (temp.Properties["distinguishedname"].Count > 0)
                                        ADuser.EnvironmentName = GetEnvironmentName(temp.Properties["distinguishedname"][0].ToString());
                                    if (temp.Properties["userPrincipalName"].Count > 0)
                                        ADuser.UserPrincipalName = temp.Properties["userPrincipalName"][0].ToString();
                                    if (temp.Properties["CN"].Count > 0)
                                        ADuser.CN = temp.Properties["CN"][0].ToString();
                                    if (temp.Properties["telephonenumber"].Count > 0)
                                        ADuser.Phone = temp.Properties["telephonenumber"][0].ToString();
                                    if (temp.Properties["samaccountname"].Count > 0)
                                        ADuser.DomainID = temp.Properties["samaccountname"][0].ToString();
                                    if (temp.Properties["title"].Count > 0)
                                        ADuser.title = temp.Properties["title"][0].ToString();
                                    if (temp.Properties["department"].Count > 0)
                                        ADuser.Department = temp.Properties["department"][0].ToString();
                                    if (temp.Properties["thumbnailphoto"].Count > 0)
                                        ADuser.TN_Photo = temp.Properties["thumbnailphoto"][0].ToString();

                                    ADuser.Mail = temp.Properties["mail"][0].ToString();
                                    ADuser.UserName = temp.Properties["name"][0].ToString();
                                    ADUserList.UserDetails.Add(ADuser);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("SaltSiteADmanager.GetADList: " + ex.Message);
                ExceptionMessageException Oops = new ExceptionMessageException(ex.Message);
            }

            logger.Debug("- End Method -");
            return ADUserList.UserDetails;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this); 
        }
        protected virtual void Dispose(bool disposing)
        {
            //dispose of this object
            Dispose(disposing);
        }

    }
}
