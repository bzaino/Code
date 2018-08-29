using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;

namespace SALTCoursesWSClient
{
    public class UtilityMethods
    {
        public static string ReadConfigValue(string attributeName)
        {
            string attributePath;
            //COV-10377 - added try/catch in case AppSetting not defined. Should not happen
            try
            {
                attributePath = ConfigurationManager.AppSettings[attributeName].ToString();
                if (attributePath == null)
                    attributePath = String.Empty;
            }
            catch
            {
                attributePath = String.Empty;
            }

            return attributePath;
        }


        public static T DeserializeResponse<T>(string jsonResponse)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            T responseJSONDict = serializer.Deserialize<T>(jsonResponse);
            return responseJSONDict;
        }


    }
}