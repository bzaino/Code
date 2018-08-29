using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Collections;

namespace ASA.Web.Services.Common
{
    public static class Config
    {

        private const string CLASSNAME = " ASA.Web.Services.Common";
        private static Hashtable smtpConnection = (Hashtable)ConfigurationManager.GetSection("smtpConnection");
        static ASA.Log.ServiceLogger.IASALog _log = ASA.Log.ServiceLogger.ASALogManager.GetLogger(CLASSNAME);

        private static bool _isTesting = false;
        public static bool Testing 
        {
            get
            {
                try
                {
                    _isTesting = Boolean.Parse(ConfigurationManager.AppSettings["testing"].ToString());
                    return _isTesting;
                }
                catch
                {
                    return false;
                }
            }
        }

        private static string _mockObjectDirectory = "";
        public static string MockObjectDirectory
        {
            get
            {
                try
                {
                    _mockObjectDirectory = ConfigurationManager.AppSettings["mockObjectDirectory"].ToString();
                    return _mockObjectDirectory;
                }
                catch
                {
                    return "";
                }
            }
        }
        private static string _SMTPServer = "";
        public static string SMTPServer
        {
            get
            {
                try
                {
                    _SMTPServer = smtpConnection["SMTPServer"].ToString();
                    return _SMTPServer;
                }
                catch
                {
                    return _SMTPServer;
                }
            }
        }

        private static int _SMTPServerPort = 25;
        public static int SMTPServerPort
        {
            get
            {
                try
                {
                    _SMTPServerPort = Int32.Parse(smtpConnection["SMTPServerPort"].ToString());
                    return _SMTPServerPort;
                }
                catch
                {
                    return _SMTPServerPort;
                }
            }
        }

        private static string _SALTEmailSender = "";
        public static string SALTEmailSender
        {
            get
            {
                try
                {
                    _SALTEmailSender = smtpConnection["SALTEmailSender"].ToString();
                    return _SALTEmailSender;
                }
                catch
                {
                    return _SALTEmailSender;
                }
            }
        }

        //Qualtrics config settings
        private static string _QTA_Process = "";
        public static string QTA_Process
        {
            get
            {
                try
                {
                    _QTA_Process = ConfigurationManager.AppSettings["QTAProcess"].ToString();
                    return _QTA_Process;
                }
                catch
                {
                    return _QTA_Process;
                }
            }
        }
        private static string _QTA_URL = "";
        public static string QTA_URL
        {
            get
            {
                try
                {
                    _QTA_URL = ConfigurationManager.AppSettings["QTAURL"].ToString();
                    return _QTA_URL;
                }
                catch
                {
                    return _QTA_URL;
                }
            }
        }
        private static string _QTA_User = "";
        public static string QTA_User
        {
            get
            {
                try
                {
                    _QTA_User = ConfigurationManager.AppSettings["QTAUser"].ToString();
                    return _QTA_User;
                }
                catch
                {
                    return _QTA_User;
                }
            }
        }
        private static string _QTA_Token = "";
        public static string QTA_Token
        {
            get
            {
                try
                {
                    _QTA_Token = ConfigurationManager.AppSettings["QTAToken"].ToString();
                    return _QTA_Token;
                }
                catch
                {
                    return _QTA_Token;
                }
            }
        }
        private static string _QTA_API_Version = "";
        public static string QTA_API_Version
        {
            get
            {
                try
                {
                    _QTA_API_Version = ConfigurationManager.AppSettings["QTAAPIVersion"].ToString();
                    return _QTA_API_Version;
                }
                catch
                {
                    return _QTA_API_Version;
                }
            }
        }
        private static string _QTA_Library_ID = "";
        public static string QTA_Library_ID
        {
            get
            {
                try
                {
                    _QTA_Library_ID = ConfigurationManager.AppSettings["QTALibraryID"].ToString();
                    return _QTA_Library_ID;
                }
                catch
                {
                    return _QTA_Library_ID;
                }
            }
        }
        private static string _QTA_List_ID = "";
        public static string QTA_List_ID
        {
            get
            {
                try
                {
                    _QTA_List_ID = ConfigurationManager.AppSettings["QTAListID"].ToString();
                    return _QTA_List_ID;
                }
                catch
                {
                    return _QTA_List_ID;
                }
            }
        }
    }
}
