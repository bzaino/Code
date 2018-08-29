using ComponentSpace.SAML2.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace ASAIDP.Session
{
    public class CookieSSOSession : AbstractSSOSessionStore, ISSOSessionStore
    {
        private static string StateCookieName = "S2S";

        public override object Load(Type type)
        {

            HttpCookie cookie = HttpContext.Current.Request.Cookies[StateCookieName];
            if (cookie != null)
            {
                byte[] bytes = Convert.FromBase64String(HttpUtility.UrlDecode(cookie.Value));
                return Deserialize(bytes);
            }
            else
            {
                return null;
            }

        }

        public override void Save(object ssoSession)
        {
            object obj = ssoSession;
            byte[] bytes = Serialize(ssoSession);
            string value = HttpUtility.UrlEncode(Convert.ToBase64String(bytes));
            HttpContext.Current.Response.SetCookie(new HttpCookie(StateCookieName) { Domain = ".saltmoney.org", Value = value, HttpOnly = true, Secure = true  });

        }


    }
}