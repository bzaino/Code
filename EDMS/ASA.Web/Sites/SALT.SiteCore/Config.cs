using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections.Specialized;
using System.Configuration;
using System.Collections;

namespace ASA.Web.Sites.SALT
{
    public class Config
    {
        private static NameValueCollection appSettings = ConfigurationManager.AppSettings;

        private static Hashtable smtpConnection = (Hashtable)ConfigurationManager.GetSection("smtpConnection");

        public static String ConnectionString { get { return String.Empty; } }//ConfigurationManager.ConnectionStrings[Connection].ConnectionString; } }

        public static Boolean Testing
        {
            get { return bool.Parse(appSettings["testing"]); }
        }

        public static string Connection
        {
            get { return appSettings["sqlConnectionStringName"]; }
        }

        public static string SMTPServer
        {
            get { return smtpConnection["SMTPServer"].ToString(); }
        }
        public static int SMTPServerPort
        {
            get {
                int port = -1;
                if (smtpConnection["SMTPServerPort"] != null)
                {
                    int.TryParse(smtpConnection["SMTPServerPort"].ToString(), out port);    
                }
                return port; 
            
                }
        }

        public static string SALTEmailSender
        {
            get { return smtpConnection["SALTEmailSender"].ToString(); }
        }

        public static string EmailaddressChangedEmail
        {
            get { return appSettings["EmailaddressChangedEmail"]; }
        }
        public static string ForgotPasswordEmail
        {
            get { return appSettings["ForgotPasswordEmail"]; }
        }
        public static string NewPasswordEmail
        {
            get { return appSettings["NewPasswordEmail"]; }
        }

        public static int ForgotPasswordTokenExpire
        {
            get
            {
                int tokenexpireinminutes = 1440; // Default is 1 day
                if (appSettings["TokenExpireInMinutes"] != null)
                {
                    int.TryParse(appSettings["TokenExpireInMinutes"].ToString(), out tokenexpireinminutes);
                }
                return tokenexpireinminutes;

            }
        }
    }
}