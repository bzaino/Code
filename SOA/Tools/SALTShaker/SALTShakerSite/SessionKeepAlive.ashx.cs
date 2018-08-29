using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
namespace SALTShaker
{
    /// <summary>
    /// Summary description for SessionRevive
    /// used to prevent session timeout
    /// called from sitemaster
    /// </summary>
    public class SessionRevive : IHttpHandler, IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            //default session to now to prevent expire
            context.Session[0] = DateTime.Now;
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}