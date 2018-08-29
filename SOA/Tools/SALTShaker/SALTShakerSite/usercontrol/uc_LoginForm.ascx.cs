using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using SALTShaker.HelperClass;

public partial class usercontrol_uc_LoginForm : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        validateSession();
    }
    private void validateSession() 
    {
        SaltShakerSession.PulseRate = "goodByeCruelWorld";
        //redirect to welcome page if the session is still valid
        if (!Session.IsNewSession)
        {
            if (!String.IsNullOrEmpty(SaltShakerSession.LastVisited))
            {
                Response.Redirect(SaltShakerSession.LastVisited);
            }
            else
            {
                Response.Redirect("Default.aspx");
            }
        }
    }
}