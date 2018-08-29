using System;
using System.Collections;
using System.Net;
using System.Web;

using log4net;

namespace SALTShaker.HelperClass
{
    public class SessionLifeSign
    {
        //set up logging
        private static readonly ILog logger = LogManager.GetLogger(typeof(SessionLifeSign));
        //create private stub 
        private SessionLifeSign() { }
        
        //create static class constructor with instant to avoid
        //static classes and static instance fields from being shared 
        //between all requests to the application in asp.net web
        public static SessionLifeSign HeartBeat
        {
            get
            {
                IDictionary items = HttpContext.Current.Items;
                string sKey = "LifeLine-" + HttpContext.Current.Session.SessionID;
                if (!items.Contains(sKey))
                {
                    items[sKey] = new SessionLifeSign();
                }
                return items[sKey] as SessionLifeSign;
            }
        }

        public string Resuscitate(string url)
        {
            return WebRequestSessionRevive(url);
        }
        //call Generic Handler .ashx file to refress session
        private string WebRequestSessionRevive(string url)
        {
            const string logMethodName = "- WebRequestSessionRevive(string url)- ";
            logger.Debug(logMethodName + "Begin Method");
            string ret = string.Empty;
            try
            {
                WebRequest webRequest = WebRequest.Create(url + "/SessionKeepAlive.ashx");
                if (webRequest != null)
                {
                    //set current authentication
                    webRequest.UseDefaultCredentials = true;
                    webRequest.PreAuthenticate = true;
                    CredentialCache credCache = new CredentialCache();
                    credCache.Add(new Uri(url), "Ntlm", CredentialCache.DefaultNetworkCredentials);
                    webRequest.Credentials = credCache;

                    //setup and post request
                    webRequest.Method = "POST";
                    webRequest.ContentType = "text/xml; encoding='utf-8'";
                    webRequest.ContentLength = 0;
                    webRequest.Timeout = 20000;
                    //I use a method to ignore bad certs caused by misc errors
                    IgnoreBadCertificates();

                    //COV-10551 - add using to resolve resource leaks
                    using (HttpWebResponse webresponse = (HttpWebResponse)webRequest.GetResponse())
                    {
                        ret = webresponse.StatusDescription;
                        webresponse.Close();
                    }
                }
            }
            catch (Exception LifeLineEx)
            {
                logger.Error("SessionLifeSign.WebRequestSessionRevive: " + LifeLineEx.Message, LifeLineEx);
            }
            logger.Debug("- End Method -");
            return ret;
        }

        /// <summary>
        /// Together with the AcceptAllCertifications method right
        /// below this causes to bypass errors caused by SLL-Errors.
        /// </summary>
        private static void IgnoreBadCertificates()
        {
            System.Net.ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(AcceptAllCertifications);
        }

        /// <summary>
        /// In Short: the Method solves the Problem of broken Certificates.
        /// Sometime when requesting Data and the sending Webserverconnection
        /// is based on a SSL Connection, an Error is caused by Servers whoes
        /// Certificate(s) have Errors. Like when the Cert is out of date
        /// and much more... So at this point when calling the method,
        /// this behaviour is prevented
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="certification"></param>
        /// <param name="chain"></param>
        /// <param name="sslPolicyErrors"></param>
        /// <returns>true</returns>
        private static bool AcceptAllCertifications(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certification, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            return true;
        } 
    }

}