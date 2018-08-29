using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SALTShaker.BLL;
using SALTShaker.HelperClass;

namespace SALTShaker.usercontrol
{
    public partial class uc_SALTADMember : System.Web.UI.UserControl
    {
        //Catch Exception errors
        const string sMSG_WARNING = "Oops that was unexpected: {0}";
        const string sMSG_EmailValidation = "Please enter a valid email";

        //regex
        string emailPattern = @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$";

        SaltSiteADmanager SM = new SaltSiteADmanager();
        MemberBL BL = new MemberBL();
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        private void ShowMessage(string sMessage)
        {
            this.uc_UserPrompMessage.ShowMessage(sMessage);
        }

        private void HideMessage()
        {
            uc_UserPrompMessage.HideMessage();
        }

        protected void cmdGetUserinfo_Click(object sender, EventArgs e)
        {
            string EmailAddress = TextUserEmailAddress.Text.Trim();
            try
            {
                if (!String.IsNullOrEmpty(EmailAddress) && System.Text.RegularExpressions.Regex.IsMatch(EmailAddress, emailPattern))
                {
                    string sActiveStatus = "UNKNOWN, this may indicate an error in the AD account. ";
                    ADUserDetails ADUsers = SM.GetUserOU(EmailAddress);
                    foreach (UserDetail User in ADUsers.UserDetails)
                    {
                        hrefLink.HRef = User.EnvironmentName;
                        SaltUserEmail.Text = User.Mail;
                        SaltUserName.Text = User.UserName;
                        DateOfCreation.Text = User.DateOfCreation;
                        UserPrincipalName.Text = User.UserPrincipalName;
                        UserEnvironmentName.Text = User.EnvironmentName;
                        SaltShakerSession.UserEnvironment = UserEnvironmentName.Text;
                        CN.Text = User.CN;
                        sActiveStatus = (User.bisActice == true) ? "ACTIVE" : "[[ NOT ]] ACTIVE";

                        //if (User.bisActice)
                        //    cmdToggleActivate.Text = "Deactivate user";
                        //else
                        //    cmdToggleActivate.Text = "Re-Activate user";

                        if (!User.EnvironmentName.Equals("Production"))
                            UserEnvironmentName.ForeColor = System.Drawing.ColorTranslator.FromHtml("green");

                        PHListUserInfo.Visible = true;
                        ShowMessage("User " + EmailAddress + " is currently " + sActiveStatus);
                    }
                    if (ADUsers.UserDetails.Count == 0)
                    {
                        ShowMessage("No users with email address: " + EmailAddress + " were found.");
                        PHListUserInfo.Visible = false;
                    }
                }
                else
                {
                    ShowMessage(sMSG_EmailValidation);
                    PHListUserInfo.Visible = false;
                }
            }
            catch (Exception ex)
            {
                //needs loging
                ExceptionMessageException Oops = new ExceptionMessageException(String.Format(sMSG_WARNING, ex.Message));
                ShowMessage(Oops.Message);
            }
        }

        //protected void cmdToggleActivate_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (!String.IsNullOrEmpty(SaltUserEmail.Text))
        //        {
        //            string sMessage = SM.Deactivate(SaltUserEmail.Text);
        //            if (sMessage.IndexOf("deactivated") < 0)
        //            {
        //                cmdToggleActivate.Text = "Deactivate user";
        //                BL.SetMemberTableActiveFlag(true, SaltUserEmail.Text);
        //            }
        //            else
        //            {
        //                cmdToggleActivate.Text = "Re-Activate user";
        //                BL.SetMemberTableActiveFlag(false, SaltUserEmail.Text);
        //            }

        //            ShowMessage(sMessage);

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        //needs loging
        //        ExceptionMessageException Oops = new ExceptionMessageException(String.Format(sMSG_WARNING, ex.Message));
        //        ShowMessage(Oops.Message);
        //    }
        //}

        protected void cmdYes_Click(object sender, EventArgs e)
        {
            try
            {
                if (!String.IsNullOrEmpty(SaltUserEmail.Text))
                {
                    string sMessage = SM.DeleteADAccount(SaltUserEmail.Text.Trim());
                    if (sMessage.Contains("deleted"))
                    {
                        string userName = string.Format("AMSA\\{0}", SaltShakerSession.UserId);
                        BL.SetMemberTableActiveFlag(false, SaltUserEmail.Text, userName);
                        PHListUserInfo.Visible = false;
                    }
                    SaltUserEmail.Text = "";
                    TextUserEmailAddress.Text = "";
                    ShowMessage(sMessage);
                    PHAlertbox.Visible = false;
                    

                }
            }
            catch (Exception ex)
            {
                //needs loging
                ExceptionMessageException Oops = new ExceptionMessageException(String.Format(sMSG_WARNING, ex.Message));
                ShowMessage(Oops.Message);
                PHListUserInfo.Visible = true;
                PHAlertbox.Visible = false;
            }
        }

        protected void cmdNo_Click(object sender, EventArgs e)
        {
            PHAlertbox.Visible = false;
            PHListUserInfo.Visible = true;
        }
        protected void cmdDeleteADUser_Click(object sender, EventArgs e)
        {
            try
            {
                if (!String.IsNullOrEmpty(SaltUserEmail.Text))
                {
                    PHListUserInfo.Visible = false;
                    PHAlertbox.Visible = true;
                    //string sMessage = SM.DeleteADAccount(SaltUserEmail.Text);
                    //ShowMessage(sMessage);

                }
            }
            catch (Exception ex)
            {
                //needs loging
                ExceptionMessageException Oops = new ExceptionMessageException(String.Format(sMSG_WARNING, ex.Message));
                ShowMessage(Oops.Message);
                PHListUserInfo.Visible = true;
                PHAlertbox.Visible = false;
            }
        }
    }
}
