using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace SALTShaker.HelperClass
{
    public class SaltShakerSession
    {
        protected static HttpSessionState MySession
        {
            get { return HttpContext.Current.Session;  }
        }

        protected static string ToStringOrDefault(object strIn, string strDefault=null)
        {
            return (strIn==null) ? strDefault : strIn.ToString();
        }

        public static string OrganizationSortExpression
        {
            get
            {
                return ToStringOrDefault(MySession["OrganizationSortExpression"]);
            }

            set
            {
                MySession["OrganizationSortExpression"] = value;
            }
        }


        public static string CurrentRole
        {
            get
            {
                return ToStringOrDefault(MySession["CurrentRole"]);
            }

            set
            {
                MySession["CurrentRole"] = value;
            }
        }
        public static string UserId
        {
            get
            {
                return ToStringOrDefault(MySession["UserId"]);
            }

            set
            {
                MySession["UserId"] = value;
            }
        }
        public static bool? IsAdmin
        {
            get
            {
                return (bool?)MySession["IsAdmin"];
            }

            set
            {
                MySession["IsAdmin"] = value;
            }
        }

        public static string  PulseRate
        {
            get
            {
                return ToStringOrDefault(MySession["PulseRate"]);
            }

            set
            {
                MySession["PulseRate"] = value;
            }
        }

        
        public static string LastVisited
        {
            get
            {
                return ToStringOrDefault(MySession["LastVisited"]);
            }

            set
            {
                MySession["LastVisited"] = value;
            }
        }

        public static string MemberSortExpression
        {
            get
            {
                return ToStringOrDefault(MySession["MemberSortExpression"]);
            }

            set
            {
                MySession["MemberSortExpression"] = value;
            }
        }

        public static string selectedMemberID
        {
            get
            {
                return ToStringOrDefault(MySession["selectedMemberID"]);
            }

            set
            {
                MySession["selectedMemberID"] = value;
            }
        }

        public static string selectedUserEmail
        {
            get
            {
                return ToStringOrDefault(MySession["selectedUserEmail"]);
            }

            set
            {
                MySession["selectedUserEmail"] = value;
            }
        }

        public static string selectedRefOrganizationID
        {
            get
            {
                return ToStringOrDefault(MySession["selectedRefOrganizationID"]);
            }

            set
            {
                MySession["selectedRefOrganizationID"] = value;
            }
        }

        public static string UserEnvironment
        {
            get
            {
                return ToStringOrDefault(MySession["UserEnvironment"]);
            }

            set
            {
                MySession["UserEnvironment"] = value;
            }
        }
    }
}