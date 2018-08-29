using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace ASA.Web.Common.Extensions
{
    public static class HttpContextExtension
    {
        public static void CleanupCookies(this HttpContextBase httpContext)
        {
            Dictionary<string, bool> dictRootDomainCookies = new Dictionary<string, bool>();

            // The Http Request does not include the domain of the cookies, so we need
            // to explicitly list which ones we know are at the root domain, ".saltmoney.org"

            dictRootDomainCookies.Add(".ASPXAUTH", true);
            dictRootDomainCookies.Add("IndividualId", true);
            dictRootDomainCookies.Add("MemberId", true);
            dictRootDomainCookies.Add("WT_FPC", true);
            dictRootDomainCookies.Add("_ga", true);
            dictRootDomainCookies.Add("_gat", true);
            dictRootDomainCookies.Add("referUrl", true);

            foreach (String cookieName in httpContext.Request.Cookies.AllKeys)
            {
                RemoveCookie(httpContext, cookieName, dictRootDomainCookies.ContainsKey(cookieName));

            }
        
        }

        private static void RemoveCookie(HttpContextBase httpContext, string cookieName, bool hasDomain = false)
        {
            if (httpContext.Request.Cookies.AllKeys.Contains(cookieName))
            {
                HttpCookie cookie = httpContext.Request.Cookies[cookieName];
                if (hasDomain)
                {
                    cookie.Domain = "saltmoney.org";
                }
                cookie.Expires = DateTime.Now.AddDays(-1);
                httpContext.Response.Cookies.Add(cookie);
            }
        }
    }
}
