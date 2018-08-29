///////////////////////////////////////////////////////////////////////////////
//
//  Revision: 	$Revision: $
//  Date:		$Date:  $
//  Author:		$Author:  $
//  Archive Name:	$Archive:  $
//  WorkFile Name:	$Workfile:  $
//
//  Description:
//  This class contains static methods used to authenticate domain credentials against an Active Directory server
//  using LDAP.
//
//  ASA Proprietary Information
//
///////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Text;
using System.DirectoryServices;
using ASA.Log.ServiceLogger;

namespace ASA.Security
{
    public class ActiveDirectoryAuthentication
    {
        static IASALog _Log = ASALogManager.GetLogger(typeof(ActiveDirectoryAuthentication));

        public static bool AuthenticateUser(string userName, string password)
        {
            //Check A/D server to see if the username is valid
            string Domain = Utility.GetDomain(ref userName);

            return (AuthenticateUser(Domain, userName, password));
        }

        public static bool AuthenticateUser(string Domain,string userName, string password)
        {
            //Check A/D server to see if the username is valid
            return(IsAuthenticated(Domain, userName, password));
        }

        /// <summary>
        /// See if a user is authenticated in Active Directory.
        /// </summary>
        /// <param name="domain">ex.) amsa</param>
        /// <param name="username"></param>
        /// <param name="pwd">The password.</param>
        /// <returns>True if the username and password exist in Active Directory; false otherwise.</returns>
        private static bool IsAuthenticated(string domain, string username, string pwd)
        {
            string domainAndUsername = domain + @"\" + username;
            DirectoryEntry entry = new DirectoryEntry("LDAP://" + domain, domainAndUsername, pwd);
            try
            {
                //Bind to the native AdsObject to force authentication.
                object obj = entry.NativeObject;
                return (true);
            }
            catch (Exception ex)
            {
                _Log.InfoFormat("Error authorizing user {0}\\{1} ({2})", domain, username, ex.Message);
                return false;
            }
        }
    }
}
///////////////////////////////////////////////////////////////////////////////
//
//	$Log: $
///////////////////////////////////////////////////////////////////////////////
