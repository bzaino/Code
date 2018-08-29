using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Collections;

namespace ASA.Web.Services.AlertService
{
    public static class Config
    {
        private static Hashtable smtpConnection = (Hashtable)ConfigurationManager.GetSection("smtpConnection");
  
        //put all config entries here.. "Fincher" style.  ;-)
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

        private static int _SMTPServerPort = 20000;
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
        private static string _AskMeEmailRecipient = "";
        public static string AskMeEmailRecipient
        {
            get
            {
                try
                {
                    _AskMeEmailRecipient = smtpConnection["AskMeEmailRecipient"].ToString();
                    return _AskMeEmailRecipient;
                }
                catch
                {
                    return _AskMeEmailRecipient;
                }
            }
        }
        private static string _SaltLiveEmailRecipient = "";
        public static string SaltLiveEmailRecipient
        {
            get
            {
                try
                {
                    _SaltLiveEmailRecipient = smtpConnection["SaltLiveEmailRecipient"].ToString();
                    return _SaltLiveEmailRecipient;
                }
                catch
                {
                    return _SaltLiveEmailRecipient;
                }
            }
        }
        private static string _ContentFeedbackEmail = "";
        public static string ContentFeedbackEmail
        {
            get
            {
                try
                {
                    _ContentFeedbackEmail = smtpConnection["ContentFeedbackEmail"].ToString();
                    return _ContentFeedbackEmail;
                }
                catch
                {
                    return _ContentFeedbackEmail;
                }
            }
        }
    }
}
