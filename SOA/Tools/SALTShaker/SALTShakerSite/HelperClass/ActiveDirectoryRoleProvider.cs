using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Collections.Specialized;
using System.DirectoryServices;
using System.Web.Configuration;
using System.Globalization;
using System.Collections;
using System.Configuration.Provider;
using System.DirectoryServices.AccountManagement;
using System.Configuration;
using log4net;
using log4net.Config;

namespace SALTShaker.HelperClass
{
    public class ActiveDirectoryRoleProvider : RoleProvider, IDisposable
    {
        private string ConnectionStringName { get; set; }
        public override String ApplicationName { get; set; }
        Configuration rootWebConfig = WebConfigurationManager.OpenWebConfiguration(null);
        KeyValueConfigurationElement roleTypeInConfig;
        private static readonly ILog logger = LogManager.GetLogger(typeof(ActiveDirectoryRoleProvider));
        public override void Initialize(string name, NameValueCollection config)
        {
            ConnectionStringName = config["connectionStringName"];
            base.Initialize(name, config);
        }
        //this function is to get the active directory group string from web config file. And this value is tokenized in udepoly.
        public string getADRoleString(string roleType)
        {
            if (rootWebConfig.AppSettings.Settings.Count == 0)
            {
                //try diffrent path
                rootWebConfig = WebConfigurationManager.OpenWebConfiguration("/");
            }
            roleTypeInConfig = rootWebConfig.AppSettings.Settings[roleType];
            return roleTypeInConfig.Value.ToString();
        }

        public string FindUserRole(string Userid, string sLDAPLocation)
        {
            //string adminResult = SearchInAdminGroup(Userid, sDomainName);
            string adminResult = SearchInGroup(Userid, sLDAPLocation, "Admin");

            if (adminResult == "Not Found")
            {
                string orgEditorResult = SearchInGroup(Userid, sLDAPLocation, "OrgEditor");
                if (orgEditorResult == "Not Found")
                {
                    string memberEditorResult = SearchInGroup(Userid, sLDAPLocation, "MemberEditor");
                    if (memberEditorResult == "Not Found")
                    {
                        string auditorResult = SearchInGroup(Userid, sLDAPLocation, "Auditor");
                        if (auditorResult == "Not Found")
                        {
                            return "INVALID USER";
                        }
                        else
                        {
                            return auditorResult;
                        }
                    }
                    else
                    {
                        return memberEditorResult;
                    }
                }
                else
                {
                    return orgEditorResult;
                }
            }
            else
            {
                return adminResult;
            }
        }
        public string SearchInGroup(string Userid, string sLDAPLocation, string sGroup)
        {
            string sRetValue = "Not Found";
            DirectoryEntry SaltGroups = new DirectoryEntry(sLDAPLocation);
            try
            {
                DirectoryEntry SaltGroup = SaltGroups.Children.Find(this.getADRoleString(sGroup));
                using (SaltGroup)
                {
                    object members = SaltGroup.Invoke("members", null);
                    foreach (var groupMember in (IEnumerable)members)
                    {
                        using (DirectoryEntry member = new DirectoryEntry(groupMember))
                        {
                            // we are looking at a group within the group, not an individual yet
                            if (member.SchemaClassName == "group")
                            {
                                //search if Userid is in the group, if so they are in SRetValue group
                                DirectorySearcher srch = new DirectorySearcher(member.Name);
                                using (srch)
                                {
                                    SearchResultCollection srchColl = srch.FindAll();
                                    using (srchColl)
                                    {
                                        foreach (SearchResult rs in srchColl)
                                        {
                                            ResultPropertyCollection resultPropColl = rs.Properties;
                                            foreach (Object memberColl in resultPropColl["member"])
                                            {
                                                if (memberColl.ToString().ToUpper() == Userid.ToUpper())
                                                {
                                                    sRetValue = sGroup + "s";
                                                    return sRetValue;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            //we are looking at individuals within the top level group
                            if (Userid.Contains(member.Name) && (sRetValue == "Not Found"))
                            {
                                sRetValue = sGroup + "s";
                                return sRetValue;
                            }

                        }
                    }
                }

            }
            catch (Exception ex)
            {
                logger.Error("ActiveDirectoryRoleProvider.SearchInGroup:" + ex.Message);
            }
            finally
            {
                SaltGroups.Dispose();
            }
            return sRetValue;
        }

        public override string[] GetRolesForUser(string username)
        {
            return helperGetUserRoles(username);
        }

        private string[] helperGetUserRoles(string username)
        {
            var allRoles = new List<string>();
            string sADuserName = HttpContext.Current.User.Identity.Name;
            string currentRole = string.Empty;
            //TODO: get this from LDAP web config string AD_LDAP_ConnectSetting     
            string sLDAPLocation = "LDAP://" + ConnectionStringName + "/ OU = Groups, OU = Boston, DC = amsa, DC = com";
            try
            {
                using (PrincipalContext ctx = new PrincipalContext(ContextType.Domain))
                {
                    UserPrincipal user = UserPrincipal.FindByIdentity(ctx, sADuserName);
                    if (user != null)
                    {
                        currentRole = FindUserRole(user.DistinguishedName, sLDAPLocation);
                        if (!string.IsNullOrEmpty(currentRole))
                        {
                            allRoles.Add(currentRole);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("ActiveDirectoryRoleProvider.helperGetUserRoles:" + ex.Message);
                return allRoles.ToArray();
            }

            return allRoles.ToArray();
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            string[] roles = GetRolesForUser(username);

            foreach (string role in roles)
            {
                if (role.Equals(roleName, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }
        /// <summary>
        /// Retrieve listing of all roles.
        /// </summary>
        /// <returns>String array of roles</returns>
        public override string[] GetAllRoles()
        {
            return new string[4] { "Admins", "OrgEditors", "MemberEditors", "Auditors" };
        }

        /// <summary>
        /// Determine if given role exists
        /// </summary>
        /// <param name="rolename">Role to check</param>
        /// <returns>Boolean indicating existence of role</returns>
        public override bool RoleExists(string rolename)
        {
            return GetAllRoles().Any(role => role == rolename);
        }

        /// <summary>
        /// Return sorted list of usernames like usernameToMatch in rolename
        /// </summary>
        /// <param name="rolename">Role to check</param>
        /// <param name="usernameToMatch">Partial username to check</param>
        /// <returns></returns>
        public override string[] FindUsersInRole(string rolename, string usernameToMatch)
        {
            if (!RoleExists(rolename))
                throw new ProviderException(String.Format("The role '{0}' was not found.", rolename));

            return (
                from user in GetUsersInRole(rolename)
                where user.ToLower().Contains(usernameToMatch.ToLower())
                select user

            ).ToArray();
        }
        /// <summary>
        /// Retrieve listing of all users in a specified role.
        /// </summary>
        /// <param name="rolename">String array of users</param>
        /// <returns></returns>
        public override string[] GetUsersInRole(String rolename)
        {
            return helperGetUsersInRole(rolename);
        }

        private string[] helperGetUsersInRole(String rolename)
        {
            logger.Debug("- helperGetUsersInRole(String rolename) -  Begin Method");
            if (!RoleExists(rolename))
                throw new ProviderException(String.Format("The role '{0}' was not found.", rolename));

            using (PrincipalContext ctx = new PrincipalContext(ContextType.Domain))
            {
                try
                {
                    using (GroupPrincipal p = GroupPrincipal.FindByIdentity(ctx, IdentityType.SamAccountName, rolename))
                    {
                        var retArr = new[] { "" };
                        if (p != null)
                        {
                            retArr = (
                                from user in p.GetMembers(true)
                                select user.SamAccountName
                        ).ToArray();
                        }
                        return retArr;
                    }
                }
                catch (Exception ex)
                {
                    // TODO: LogError( "Unable to query Active Directory.", ex );
                    logger.Error("ActiveDirectoryRoleProvider.helperGetUsersInRole:" + ex.Message);
                    return new[] { "" };
                }
                finally
                {
                    logger.Debug("- end method --");
                }
            }
  
        }

        #region Non Supported Base Class Functions

        /// <summary>
        /// AddUsersToRoles not supported.  For security and management purposes, ADRoleProvider only supports read operations against Active Direcory. 
        /// </summary>
        public override void AddUsersToRoles(string[] usernames, string[] rolenames)
        {
            throw new NotSupportedException("Unable to add users to roles.  For security and management purposes, ADRoleProvider only supports read operations against Active Direcory.");
        }

        /// <summary>
        /// CreateRole not supported.  For security and management purposes, ADRoleProvider only supports read operations against Active Direcory. 
        /// </summary>
        public override void CreateRole(string rolename)
        {
            throw new NotSupportedException("Unable to create new role.  For security and management purposes, ADRoleProvider only supports read operations against Active Direcory.");
        }

        /// <summary>
        /// DeleteRole not supported.  For security and management purposes, ADRoleProvider only supports read operations against Active Direcory. 
        /// </summary>
        public override bool DeleteRole(string rolename, bool throwOnPopulatedRole)
        {
            throw new NotSupportedException("Unable to delete role.  For security and management purposes, ADRoleProvider only supports read operations against Active Direcory.");
        }

        /// <summary>
        /// RemoveUsersFromRoles not supported.  For security and management purposes, ADRoleProvider only supports read operations against Active Direcory. 
        /// </summary>
        public override void RemoveUsersFromRoles(string[] usernames, string[] rolenames)
        {
            throw new NotSupportedException("Unable to remove users from roles.  For security and management purposes, ADRoleProvider only supports read operations against Active Direcory.");
        }
        #endregion

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
