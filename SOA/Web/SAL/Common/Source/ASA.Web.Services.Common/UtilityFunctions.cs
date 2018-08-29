using System;
using System.Net.Mail;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Logging;
using System.Web.Security;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Net;

namespace ASA.Web.Services.Common
{
    public class UtilityFunctions
    {
        private const string CLASSNAME = "ASA.Web.Services.Common.UtilityFunctions";
        private static ASA.Log.ServiceLogger.IASALog _log = ASA.Log.ServiceLogger.ASALogManager.GetLogger(CLASSNAME);

        public static string CreateFormsAuthenticationTicket(string partnerId, string saltMemberId)
        {
            const string logMethodName = ". GetEncryptedToken()";
            _log.Debug(logMethodName + "Begin Method");

            string toReturn = "";
            if (!string.IsNullOrEmpty(partnerId) && !string.IsNullOrEmpty(saltMemberId))
            {
                DateTime currentDate = DateTime.Now;

                // Create a forms authentication ticket what will expire in 1 hour
                FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(
                    1,
                    saltMemberId,
                    currentDate,
                    currentDate.AddHours(1),
                    true,
                    partnerId);

                // Encrypt the authentication ticket
                string encryptedToken = FormsAuthentication.Encrypt(ticket);
                if (!string.IsNullOrEmpty(encryptedToken))
                {
                    toReturn = encryptedToken;
                }
                else
                {
                    _log.Debug("Something wrong during the encryption process!");
                }
            }
            else
            {
                _log.Debug("One of the parameters is empty!");
            }
            _log.Debug(logMethodName + "End Method");
            return toReturn;
        }

        public static string encryptString(string stringToEncrypt)
        {
            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1,
            stringToEncrypt,
            DateTime.Now,
            DateTime.Now.AddYears(2),
            false,
            FormsAuthentication.FormsCookiePath);

            // Encrypt the ticket.
            string encTicket = FormsAuthentication.Encrypt(ticket);

            return encTicket;
        }

        public static string GetEnumDescription(Enum enumValue)
        {
            string enumValueAsString = enumValue.ToString();

            var type = enumValue.GetType();
            FieldInfo fieldInfo = type.GetField(enumValueAsString);
            object[] attributes = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes.Length > 0)
            {
                var attribute = (DescriptionAttribute)attributes[0];
                return attribute.Description;
            }

            return enumValueAsString;
        }

        public static bool SendEmail (MailMessage message)
        {
            bool emailSent = false;

            String logMethodName = ".SendEmail(AskMeRequestModel requestModel) - ";
            _log.Debug(logMethodName + string.Format("Sending email to {0}, SMTPServer ={1}:{2}, subject={3}", message.To, Config.SMTPServer, Config.SMTPServerPort, message.Subject));
            
            try
            {
                using (System.Net.Mail.SmtpClient client = new SmtpClient(Config.SMTPServer, Config.SMTPServerPort))
                {
                    _log.Info("About to send Email to " + message.To);
                    client.Send(message);
                    emailSent = true;
                    _log.Info("Email sent to " + message.To);
                }
            }
            catch (Exception ex)
            {
                _log.Error(String.Format("Error sending email: {0}\n\n{1}", ex.InnerException));
            }

            return emailSent;
        }

    }

    public class CrossSiteScriptValidator
    {
        private const string Classname = "ASA.Web.Services.SearchService.Validation.CrossSiteScriptValidator";
        private static readonly Log.ServiceLogger.IASALog Log = ASA.Log.ServiceLogger.ASALogManager.GetLogger(Classname);

        public static bool IsValidObject(object value)
        {
            Log.Debug("- IsValidJson(object value) - " + "Begin Method");
            bool retValue = false;
            var xscriptPattern = new StringBuilder(@"<(.|\n)*?>");

            //Checks any js events i.e. onKeyUp(), onBlur(), alerts and custom js functions etc.  
            xscriptPattern.Append(@"((alert|on\w+|function\s+\w+)\s*\(\s*(['+\d\w](,?\s*['+\d\w]*)*)*\s*\))");
            //Checks any html tags i.e. <script, <embed, <object etc.
            xscriptPattern.Append(@"|(<(script|iframe|embed|frame|frameset|object|img|applet|body|html|style|layer|link|ilayer|meta|bgsound))");
            try
            {
                if (value != null && !String.IsNullOrEmpty(value.ToString()))
                {
                    //No cross-site script found ok to continue  
                    // ?? "" handles warning Possible 'null' assignment to an entity marked with 'Value cannot be null' attribute
                    if (!Regex.IsMatch(System.Web.HttpUtility.UrlDecode(value.ToString()) ?? "", xscriptPattern.ToString(), RegexOptions.IgnoreCase | RegexOptions.Compiled))
                    {
                        retValue = true;
                    }
                    else
                    {
                        //cross-site script found. Signal as invalid, operation shold be aborted. 
                        retValue = false;
                    }
                }
                else
                    //No cross-site script found ok to continue
                    retValue = true;
            }
            catch (Exception ex)
            {
                Log.Error("Exception executing CrossSiteScriptValidator.IsValidObject(object): ", ex);
                //No cross-site script but assume bad file and shold be aborted
                retValue = false;
            }
            Log.Debug("- IsValidObject(object value) - End Method");
            return retValue;
        }
    }
}
