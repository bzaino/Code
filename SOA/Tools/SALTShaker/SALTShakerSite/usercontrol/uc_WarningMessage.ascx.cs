using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SALTShaker.usercontrol
{
    public partial class uc_WarningMessage : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
           ///To modify as needed.
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sMessage"></param>
        public void ShowMessage(string sMessage)
        {
            LabelWarning.Text = sMessage;
            LabelWarning.Visible = true;
        }

        /// <summary>
        /// 
        /// </summary>
        public void HideMessage()
        {
            LabelWarning.Text = String.Empty;
            LabelWarning.Visible = false;
        }
    }
}