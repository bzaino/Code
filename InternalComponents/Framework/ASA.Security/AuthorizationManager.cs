using System;
using System.Collections.Generic;
using System.Text;
using ASA.Common;
using System.Web.Security;

namespace ASA.Security
{
    public class AuthorizationManager
    {
        public static bool CheckAccess(Ticket ticket, string action)
        {
            //load Parameters with ASA settings
            Parameters.Instance.SectionName = "Security";

            string role = Parameters.Instance.GetStrValue(action,"");
            if (role == String.Empty)
            {
                throw new ApplicationException("Not Authorized.  No Roles are Configured for this action");
            }

            string[] rolesAllowed = role.Split(';');

            bool bAllowed = false;

            if (ticket.ExpirationDate.CompareTo(DateTime.Now) >= 0)
                bAllowed = true;

            if (bAllowed)
            {
                string[] roles = ticket.Roles;
                foreach (string s in roles)
                {
                    foreach (string sAllowed in rolesAllowed)
                    {
                        if (s.Equals(sAllowed))
                        {
                            bAllowed = true;
                            break;
                        }
                        else
                        {
                            bAllowed = false;
                        }
                    }
                    if (bAllowed)
                        break;
                }
            }

            return bAllowed;
        }

        public static void CheckAccess(string username, string action)
        {
            bool bAllowed = false;
            if (action != String.Empty && action != null && username != String.Empty)
            {
                //load Parameters with ASA settings
                Parameters.Instance.SectionName = "Security";

                string role = Parameters.Instance.GetStrValue(action, "");

                if (role == String.Empty)
                {
                    throw new ApplicationException("Not Authorized.  No Roles are Configured for this action");
                }

                string[] rolesAllowed = role.Split(';');

                foreach (string s in rolesAllowed)
                {
                    if (Roles.IsUserInRole(username, s))
                    {
                        bAllowed = true;
                        break;
                    }
                }
            }
            if (!bAllowed)
                throw new ApplicationException("Not Authorized");

        }
    }
}
