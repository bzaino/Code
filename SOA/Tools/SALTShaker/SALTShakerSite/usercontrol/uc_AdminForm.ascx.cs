using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Security.Principal;
using System.Web.Security;
using SALTShaker.BLL;
using System.DirectoryServices.AccountManagement;
using SALTShaker.HelperClass;

public partial class usercontrol_uc_AdminForm : System.Web.UI.UserControl
{
    IPrincipal CurrentUser;
    string sOrinalRole = String.Empty;
    string sUpdatedRole = String.Empty;
    bool isAdmin;
    SaltSiteADmanager SM = new SaltSiteADmanager();
    //*******************************************************
    //REQUIRES matching Parameters for use String.Format
    //*******************************************************
    //CmdRemoveRole_Click
    const string sMSG_REMOVE_FAILED = "User {0} can not be removed  from role {1}{2} because <b>{0} is not assigned to role {1}</b>.{2} If would like to add {0} from role {1},{2}  Click the Add user to Role button.";
    const string sMSG_REMOVE_SUCCESS = "User {0} has been successfully removed from the {1} role";

    //CmdChangeRole_Click 
    const string sMSG_ADD_FAILED = "User {0} was not added to role {1}{2} because <b>{0} is already assigned to role {1}</b>.{2} If would like to remove {0} from role {1},{2} Click the (Remove from Role) button.";
    const string sMSG_ADD_SUCCESS = "User {0} has been successfully added to the {1} role";

    //Catch Exception errors
    const string sMSG_WARNING = "Oops that was unexpected: {0}";
  
    //ButtonSearch_Click
    const string sMSG_SEARCH_FAILED = "Can not find the user account, Please enter either user name or full name.";
   
    //Promp user msgs.
    const string sMSG_SEARCHTEXT_Validation = "Please enter a valid name in the search box first";
    const string sMSG_USERFOUND = "User: {0} found, the current default role for {1} is {2}. If this is corrent click [Add USER TO ROLE] button. Otherwise select another roles from the drop down box above for {1} to use.";
    
    //No Admin Previlege.
    const string sMSG_Amins_Previlege = "You don't have admins previlege";

    //Multiple or no roles error.
    const string sMSG_MultiNoRoles_Error = "Either multiple roles or no roles associated with the user account";

    //Can not switch roles for yourself.
    const string sMSG_Switch_Restrict = "Switch roles for yourself can not be performed!";

    protected void Page_Load(object sender, EventArgs e)
    {
        CurrentUser = ((IPrincipal)HttpContext.Current.User);
        isAdmin = SaltShakerSession.IsAdmin != null ? (bool)SaltShakerSession.IsAdmin : false;
        if (!IsPostBack)
        {
            if (CurrentUser != null && !String.IsNullOrEmpty(CurrentUser.Identity.Name))
            {
                initialize();
            }
            else {
                ShowMessage(String.Format(sMSG_WARNING, "Oops"));
            }
        }
    }

    #region Private functions

    private void initialize()
    {
        try
        {
            //clear error message etc.
            HideMessage();
            //check for user Active directory info for valid user
            if (CurrentUser.Identity.IsAuthenticated)
            {
                LabelFirstName.Text = CurrentUser.Identity.Name;
                LabelRole.Text = SaltShakerSession.CurrentRole;
            }
        }
        catch (Exception ex)
        {
            ExceptionMessageException Oops = new ExceptionMessageException(String.Format(sMSG_WARNING, ex.Message));
            ShowMessage(Oops.Message);
        }
    }

    private void ShowMessage(string sMessage)
    {
        this.uc_UserPrompMessage.ShowMessage(sMessage);
        //LabelRole.Text = sUpdatedRole;  Show message should be just showing the message, why updating the role text EX: what if update role is failed, we don't want to change the text then, correct?
    }

    private void HideMessage()
    {
        uc_UserPrompMessage.HideMessage();
    }

    #endregion

}