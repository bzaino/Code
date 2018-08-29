using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;

namespace ASAIDP.SSO.Plugins
{
    public class WebServiceRequester
    {

        public static string MakeWebPageCall(string path)
        {
            return MakeHttpsCall(path, "text/html; encoding='utf-8'");
        }

        public static string MakeServiceCall(string path)
        {
            return MakeHttpsCall(path, "application/json; encoding='utf-8'");
        }

        public static string MakeHttpsCall(string path, string contentType)
        {
            //string querystring = WebOperationContext.Current.IncomingRequest.UriTemplateMatch.RequestUri.Query;

            //Read cookie from request
            HttpCookie saltAuthCookie = null;

            //COV 10564
            if (HttpContext.Current != null)
            {
                saltAuthCookie = HttpContext.Current.Request.Cookies[".ASPXAUTH"];
            }

            if (saltAuthCookie == null)
            {
                return null;
            }

            Cookie cookieHeader = HttpCookieToCookie(saltAuthCookie); //.ToString();
            cookieHeader.Domain = "saltmoney.org";
            cookieHeader.Secure = false;

            HttpWebRequest SALRequest = (HttpWebRequest)WebRequest.Create(path);
            SALRequest.Method = "GET";
            SALRequest.ContentType = contentType;

            var cookieContainer = new CookieContainer();
            SALRequest.CookieContainer = cookieContainer;
            SALRequest.CookieContainer.Add(cookieHeader);

            SALRequest.Proxy = null; //bypass proxy for local (IDP to SAL) requests

            BypassCertificateError();

            string response;
            using (HttpWebResponse SALResult = (HttpWebResponse)SALRequest.GetResponse())
            {
                // Get the stream containing content returned by the server.
                Stream dataStream = SALResult.GetResponseStream();
                Encoding encode = System.Text.Encoding.GetEncoding("utf-8");

                using (StreamReader readStream = new StreamReader(dataStream, encode))
                {
                    response = readStream.ReadToEnd();
                    // Releases the resources of the Stream.
                    readStream.Close();
                }

                // Releases the resources of the response.
                SALResult.Close();
            }
            return response;
        }



        static bool MyCertHandler(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors error)
        {
            // Ignore errors
            return true;
        }

        /// <summary>
        /// solution for exception
        /// System.Net.WebException: 
        /// The underlying connection was closed: Could not establish trust relationship for the SSL/TLS secure channel. ---> System.Security.Authentication.AuthenticationException: The remote certificate is invalid according to the validation procedure.
        /// </summary>
        protected static void BypassCertificateError()
        {
            ServicePointManager.ServerCertificateValidationCallback +=

                delegate(
                    Object sender1,
                    X509Certificate certificate,
                    X509Chain chain,
                    SslPolicyErrors sslPolicyErrors)
                {
                    return true;
                };
        }

        public static HttpCookie CookieToHttpCookie(Cookie cookie)
        {
            HttpCookie httpCookie = new HttpCookie(cookie.Name);

            /*Copy keys and values*/
            foreach (string value in cookie.Value.Split('&'))
            {
                string[] val = value.Split('=');
                httpCookie.Values.Add(val[0], val[1]); /* or httpCookie[val[0]] = val[1];  */
            }

            /*Copy Porperties*/
            httpCookie.Domain = cookie.Domain;
            httpCookie.Expires = cookie.Expires;
            httpCookie.HttpOnly = cookie.HttpOnly;
            httpCookie.Path = cookie.Path;
            httpCookie.Secure = cookie.Secure;

            return httpCookie;

        }

        protected static Cookie HttpCookieToCookie(HttpCookie httpCookie)
        {
            Cookie cookie = new Cookie(httpCookie.Name, httpCookie.Value);

            /*Copy keys and values*/
            foreach (string value in httpCookie.Value.Split('&'))
            {
                string[] val = value.Split('=');
                cookie.Value = val[0];
                //cookie.Values.Add(val[0], val[1]); /* or cookie[val[0]] = val[1];  */
            }

            /*Copy Porperties*/
            cookie.Domain = httpCookie.Domain;
            cookie.Expires = httpCookie.Expires;
            cookie.HttpOnly = httpCookie.HttpOnly;
            cookie.Path = httpCookie.Path;
            cookie.Secure = httpCookie.Secure;

            return cookie;

        }
    }
}