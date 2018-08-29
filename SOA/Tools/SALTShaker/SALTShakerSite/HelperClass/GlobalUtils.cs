using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SALTShaker.HelperClass
{
	public static class GlobalUtils
	{
		#region set form properties.
        const int SLEEP_TIME = 1000; //one sec
		static public void setControlProperties(Control ctr)
		{
			if (ctr.GetType() == typeof(TextBox))
			{
				((TextBox)ctr).Visible = false;
			}
			if (ctr.GetType() == typeof(CheckBox))
			{
				((CheckBox)ctr).Enabled = false;
			}
			if (ctr.GetType() == typeof(Button))
			{
				((Button)ctr).Enabled = false;
				((Button)ctr).Visible = false;
			}
			if (ctr.GetType() == typeof(PlaceHolder))
			{
				((PlaceHolder)ctr).Visible = false;
			}
		}
		
		static public void SetAccessToControls(Control pageControl, string CurrentRole)
		{
			System.Web.UI.Page currentPage = (System.Web.UI.Page)pageControl;
			if ((currentPage.Title == "MemberDetail - SALTShaker" && CurrentRole == "MemberEditors") || (currentPage.Title == "OrganizationDetail - SALTShaker" && CurrentRole == "OrgEditors"))
			{
				return;
			}
			if (CurrentRole == "OrgEditors" || CurrentRole == "MemberEditors" || CurrentRole == "Auditors")
			{
				foreach (Control c in EnumerateControlsRecursive(pageControl))
				{
					setControlProperties(c);
				}
			}
		}
		static public IEnumerable<Control> EnumerateControlsRecursive(Control parent)
		{
			foreach (Control child in parent.Controls)
			{
				yield return child;
				foreach (Control descendant in EnumerateControlsRecursive(child))
					yield return descendant;
			}
		}

        static public bool WaiTTime()
        {
            System.Diagnostics.Stopwatch stopwatch = System.Diagnostics.Stopwatch.StartNew();
            while (true)
            {
                //some other processing to do possible
                if (stopwatch.ElapsedMilliseconds >= SLEEP_TIME)
                {
                    stopwatch.Stop();
                    break;
                }
            }
            return true;
        }

		#endregion
	}
}
		
	

	
