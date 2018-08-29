using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Security.Principal;
using SALTShaker.HelperClass;
using System.Net;
using System.IO;

namespace SALTShaker
{
    public partial class Site : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //Assume session is still good.
            SaltShakerSession.PulseRate = "lifeIsGood blink";
            Response.AddHeader("Refresh", Convert.ToString((Session.Timeout * 50) + 5));
            //removing for now, not used anywhere
            //destination URL
            //string url = Request.Url.Scheme + "://" + Request.Url.Host + ":" + Request.Url.Port;
            //prevent session timeout
            //string StatusCode = SessionLifeSign.HeartBeat.Resuscitate(url);
            //if it fails redirect to login prompt
            if (Session.IsNewSession)
            {
                SaltShakerSession.PulseRate = "goodByeCruelWorld";
                if (Page.Request.Url.AbsoluteUri.ToLower().IndexOf("index.aspx") < 1)
                {
                    SaltShakerSession.LastVisited = Page.Request.Url.AbsoluteUri;
                }
                Response.Redirect("Index.aspx");
            }
        }

        //to do
        protected void ScriptManagerMemberData_AsyncPostBackError(object sender, AsyncPostBackErrorEventArgs e)
        {

            //string errorMsg = "ScriptManager AsyncPostBackError. This may have been caused by a service call failure or the service may be down for maintenance. Please try again later. If you believe this is valid error please contact support. ";
            //ScriptManagerMemberData.AsyncPostBackErrorMessage = String.Format(GlobalMessages.sMSG_WARNING, e.Exception.Message + errorMsg);
            //    foreach (Control uc_Warning in GlobalUtils.EnumerateControlsRecursive(Page.Controls[0]))
            //    {
            //        if (uc_Warning.ID == "warningMessageControl")
            //        {
            //            usercontrol.uc_WarningMessage ms= ((usercontrol.uc_WarningMessage)uc_Warning);
            //            ms.ShowMessage("Test");
            //        }
            //        else if (uc_Warning.ID == "uc_MemberData1")
            //        {
            //            ((usercontrol.uc_MemberData)uc_Warning).FindControl("warningMessageControl").Visible = true;
            //        }
            //    }
        }
    }
}