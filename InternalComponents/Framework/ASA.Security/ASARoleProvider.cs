using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Security;
using System.Collections;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.Caching;
using System.Web;
using ASA.Common;
using ASA.BusinessEntities.MRM;
using ASA.DataAccess.MRM.Interfaces;


namespace ASA.Security
{
    public class ASARoleProvider : RoleProvider
    {
        private string _AppName;

        static IMRMDao mrmProxy = (IMRMDao)ContextHelper.GetContextObject("MRMDAO");

        public override void Initialize(string name, NameValueCollection config)
        {
            if (config == null)
                throw new ArgumentNullException("config");
            if (String.IsNullOrEmpty(name))
                name = "MembershipConnectionString";
            //if (string.IsNullOrEmpty(config["description"]))
            //{
            //    config.Remove("description");
            //    config.Add("description", "ASA Custom Role Provider");
            //}
            base.Initialize(name, config);
        }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            //not implemented
        }

        public override string ApplicationName
        {
            get { return _AppName; }
            set
            {
                if (_AppName != value)
                {
                    _AppName = value;
                }
            }
        }

        public override void CreateRole(string roleName)
        {
            //not implemented            
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            //not implemented
            return false;
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            return GetUsersInRole(roleName);           
        }

        public override string[] GetAllRoles()
        {
            //not implemented            
            return new string[0];            
        }

        public override string[] GetRolesForUser(string username)
        {
            String[] strReturn = null;

             if ( HttpRuntime.Cache != null  &&  HttpRuntime.Cache.Count > 0)
                    strReturn = (string[]) HttpRuntime.Cache[username];

            if (strReturn == null)
            {
                try
                {

                    VwCustomerMembershipFlag customerInfo = mrmProxy.GetCustomerInfo(username);

                    if (customerInfo != null)
                    {
                        strReturn = new String[(int)customerInfo.Count];
                        if (customerInfo.PaidMember.Equals("Yes"))
                        {
                            strReturn[0] = "Paid Member";
                        }
                        else if (customerInfo.RegisteredUser.Equals("Yes"))
                        {
                            strReturn[0] = "Registered User";
                        }
                    }
                    else
                    {
                        strReturn = new String[0];
                    }

                    HttpRuntime.Cache.Add(username, strReturn, null, DateTime.Now.AddSeconds(600), Cache.NoSlidingExpiration, CacheItemPriority.High, null);

                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            return strReturn;
        }

        public override string[] GetUsersInRole(string roleName)
        {
            //not implemented
            return null;
            
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            bool bReturn = false;
            try
            {
                //string [] users= GetUsersInRole(roleName);
                string [] roles = GetRolesForUser(username);
                foreach(string role in roles)
                {
                    if(role.Equals(roleName))
                    {
                        bReturn = true;
                        break;
                    }
                }                  
                
            }
            catch
            {
            }
            return bReturn;           
            
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            // not implemented
        }

        public override bool RoleExists(string roleName)
        {
            //not implemented
            return false;
        }

        
    }
}
