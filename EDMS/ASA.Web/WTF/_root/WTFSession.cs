using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ASA.Web.WTF
{
    public class WTFSession : ASA.Web.Utility.BaseSession
    {

        protected new const string APP_PREFIX = "WTF_";

        public static string DisplayName
        {

            get
            {
                return GetSessionValue<string>("DisplayName");

            }
            set
            {
                SetSessionValue<string>("DisplayName", value);
            }
        }

        public static object AccountId
        {
            get
            {
                return GetSessionValue<object>("AccountId");
            }

            set
            {
                SetSessionValue<object>("AccountId", value);
            }
        }

        public static object ProfileId
        {
            get
            {
                return GetSessionValue<object>("ProfileId");
            }

            set
            {
                SetSessionValue<object>("ProfileId", value);
            }
        }
        public static string ActiveEnrollmentStatus
        {

            get
            {
                return GetSessionValue<string>("ActiveEnrollmentStatus");
            }
            set
            {
                SetSessionValue<string>("ActiveEnrollmentStatus", value);
            }
        }
        public static string ActiveGradeLevel
        {

            get
            {
                return GetSessionValue<string>("ActiveGradeLevel");
            }
            set
            {
                SetSessionValue<string>("ActiveGradeLevel", value);
            }
        }
    }
}
