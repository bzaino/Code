using SALTShaker.HelperClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
//using System.Security.Principal;
//Adding logging
//using log4net;
//using log4net.Config;

namespace SALTShaker
{
    public partial class Welcome : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(SaltShakerSession.CurrentRole) || SaltShakerSession.CurrentRole == "INVALID USER")
            {
                EnterButton.Visible = false;
            }
            else 
            {
                LabelMessage.Visible = false;
            }
        }
    }
}