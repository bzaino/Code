using SALTShaker.BLL;
using SALTShaker.DAL.DataContracts;
using SALTShaker.HelperClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

using log4net;

using Color = System.Drawing.Color;
using GlobalLists = SALTShaker.GlobalLists;
using GlobalMessages = SALTShaker.GlobalMessages;
using GlobalUtils = SALTShaker.HelperClass.GlobalUtils;

public partial class usercontrol_uc_MemberDetail : System.Web.UI.UserControl
{
    private static readonly ILog logger = LogManager.GetLogger(typeof(usercontrol_uc_MemberDetail));
    //ref table names
    const string CALLRESULT = "refCallResult";
    const string INVALID_SEARCH_PARAM = "invalid";
    private static string _sortDirection;
    static System.Web.UI.WebControls.Image sortImage = new System.Web.UI.WebControls.Image();
    static string ascImagePath = "/Images/icon_asc.gif";
    static string descImagePath = "/Images/icon_desc.gif";
    //static string sortExpression;
    static private bool isDirty = false;

    private SaltSiteADmanager SM = new SaltSiteADmanager();
    MemberBL Members = new MemberBL();

    protected void Page_Load(object sender, EventArgs e)
    {

        if (!IsPostBack)
        {
            //clear old values
            SaltShakerSession.selectedMemberID = string.Empty;
            SaltShakerSession.selectedUserEmail = string.Empty;
            
            this.Page.Title = "MemberDetail - SALTShaker";
            GlobalUtils.SetAccessToControls(this.Page, SaltShakerSession.CurrentRole);
            InitMemberDataModelBySearchResultsQueryString();
            logger.Info("User: " + SaltShakerSession.UserId + " viewed member: " + SaltShakerSession.selectedMemberID + " , " + SaltShakerSession.selectedUserEmail);
        }
    }

    #region protected functions


    private void InitMemberDataModelBySearchResultsQueryString()
    {
        try
        {
            if (SaltShakerSession.UserId != null)
            {
                //Salt members unique numeric id taken
                //from Search Results QueryString
                if (Request["memberID"] != null)
                {
                    SaltShakerSession.selectedMemberID = Request["memberID"];
                }

                //UserName here refers email address
                //from Search Results QueryString
                if (Request["userName"] != null)
                {
                    SaltShakerSession.selectedUserEmail = Request["userName"];
                }

                //try seaching SALT DB first and display Member info
                //if MemberID param is not null
                if (!String.IsNullOrEmpty(SaltShakerSession.selectedMemberID))
                {
                    logger.Info("Salt MemberID: " + SaltShakerSession.selectedMemberID + " found calling FetchSaltMemberData() from InitMemberDataModelBySearchResultsQueryString. Network id:" + SaltShakerSession.UserId);
                    FetchSaltMemberDataAndDisplayToScreen();
                }
                else
                {
                        ShowMessage(String.Format(GlobalMessages.sMSG_MEMBERID_ISNULL, "UserID OR Email"));
                } 
            }
            else
            {
                Response.Redirect("Index.aspx");
            }
        }
        catch (Exception ex)
        {
            logger.Error("Exception in uc_MemberDetail.InitMemberDataMobelBySeachResultsQueryString:" + ex.Message);
            ExceptionMessageException Oops = new ExceptionMessageException(ex.Message);
            ShowMessage(String.Format(GlobalMessages.sMSG_WARNING, Oops.Message));
        }

    }

    /// <summary>
    /// Kicks off the Global init and calls helper function
    /// renderDataToTable which displasy info to Member Details screen.
    /// </summary>
    private void FetchSaltMemberDataAndDisplayToScreen()
    {
        try
        {
            //data is already in memory from search screen results
            //don't call service again.
            if (!FindCashedDataModelForMatchingMember(SaltShakerSession.selectedMemberID))
            {
                GlobalLists.ListMemberResults = Members.GetMembersBySearchParms(Int32.Parse(SaltShakerSession.selectedMemberID.Trim()), "", "", "", "").ToList();
                GlobalLists.ListActivityHistory = Members.GetMemberActivityHistory(Int32.Parse(SaltShakerSession.selectedMemberID.Trim())).ToList();
            }
            //render data to screen with global objects
            if (GlobalLists.ListMemberResults.Count > 0 && GlobalLists.ListActivityHistory.Count > 0)
            {
                vMemberAcademicInfoModel MemberItem = GlobalLists.ListMemberResults[0];
                MemberActivityHistoryModel memberActivityHistory = GlobalLists.ListActivityHistory.Last();
                //Get the latest DeActivation Date
                Nullable<DateTime> recentDeActivationDate = null;

                if (GlobalLists.ListActivityHistory.Count > 0)
                {
                    var deactivationRecord = GlobalLists.ListActivityHistory.AsEnumerable()
                                                            .Where(ma => ma.RefActivityTypeName == "DeActivation");
                    if (deactivationRecord.Count() > 0)
                    {
                        recentDeActivationDate = deactivationRecord.OrderByDescending(a => a.ActivityDate).First().ActivityDate;
                    }
                }
                
                //render grid
                this.renderDataToTable(MemberItem.MemberID.ToString(), MemberItem.FirstName,
                    MemberItem.LastName, MemberItem.EmailAddress,
                    MemberItem.IsContactAllowed, MemberItem.IsMemberActive, MemberItem.EnrollmentStatusDescription,
                    MemberItem.GradeLevelDescription,
                    MemberItem.MemberFirstActivationDate.ToShortDateString(),
                    recentDeActivationDate.ToString(), memberActivityHistory.ModifiedBy,
                    MemberItem.WelcomeEmailSent == true ? "Sent" : "Not Sent");
                //COV-10540 use SM object allocated as global above instead of allocating a new object.
                if (SM.IsMemberActive(MemberItem.EmailAddress) == false)
                {
                    CheckBoxActive.Text = "Deactivated";
                    CheckBoxActive.BackColor = Color.Red;
                    CheckBoxActive.Checked = true;
                    CheckBoxActive.Enabled = false;
                }
            }
        }
        catch (Exception ex)
        {
            logger.Error("Exception in uc_MemberDetail.FetchSaltMemberDataAndDisplayToScreen:" + ex.Message);
            ExceptionMessageException Oops = new ExceptionMessageException(ex.Message);
            ShowMessage(String.Format(GlobalMessages.sMSG_WARNING, ex.Message));
        }
    }

    // To DO: move to beter place
    /// <summary>
    /// Check if data found in search results is still valid
    /// if true set GlobalLists.ListSaltMemberModel and GlobalLists.ListActivityHistory
    /// Which are used to render the Member details page.
    /// returns True if data found.
    /// </summary>
    /// <param name="MemberID"></param>
    /// <returns>bool</returns>
    private bool FindCashedDataModelForMatchingMember(string MemberID)
    {
        if (MemberID != null)
        {
            var member = GlobalLists.ListSaltMemberModel.Where(entity => entity.MemberID == Int32.Parse(MemberID.Trim())).FirstOrDefault();
            if (member != null)
            {
                GlobalLists.ListActivityHistory = Members.GetMemberActivityHistory(Int32.Parse(MemberID.Trim())).ToList();
                GlobalLists.ListMemberResults = new List<vMemberAcademicInfoModel>();
                GlobalLists.ListMemberResults.Add(new vMemberAcademicInfoModel()
                {
                    ActivationDate = member.ActivationDate,
                    ActiveDirectoryKey = member.ActiveDirectoryKey,
                    BranchCode = member.BranchCode,
                    DeactivationDate = member.DeactivationDate,
                    DisplayName = member.DisplayName,
                    EmailAddress = member.EmailAddress,
                    EnrollmentStatusCode = member.EnrollmentStatusCode,
                    EnrollmentStatusDescription = member.EnrollmentStatusDescription,
                    FirstName = member.FirstName,
                    GradeLevelCode = member.GradeLevelCode,
                    GradeLevelDescription = member.GradeLevelDescription,
                    InvitationToken = member.InvitationToken,
                    IsContactAllowed = member.IsContactAllowed,
                    IsContracted = member.IsContracted,
                    IsMemberActive = member.IsMemberActive,
                    LastName = member.LastName,
                    MemberActivationHistoryID = member.MemberActivationHistoryID,
                    MemberFirstActivationDate = member.MemberFirstActivationDate,
                    MemberID = member.MemberID == null ? (int)0 : (int)member.MemberID,
                    OPECode = member.OPECode,
                    RefEnrollmentStatusID = member.RefEnrollmentStatusID,
                    RefGradeLevelID = member.RefGradeLevelID,
                    RefRegistrationSourceID = member.RefRegistrationSourceID,
                    RefOrganizationID = member.RefOrganizationID,
                    OrganizationDescription = member.OrganizationDescription,
                    OrganizationExternalID = member.OrganizationExternalID,
                    OrganizationLogoName = member.OrganizationLogoName,
                    OrganizationName = member.OrganizationName,
                    WelcomeEmailSent = member.WelcomeEmailSent
                });
                return true;
            }
        }
        return false;
    }

    protected void HideEditForAuditors()
    {
        try
        {
            if (SaltShakerSession.CurrentRole == "Auditors")
            {
                FindControl("UpdateBtn").Visible = false;
            }
        }
        catch (Exception Ex)
        {

            logger.Error("Exception in uc_MemberDetail.HideEditForAuditors:" + Ex.Message);
            ShowMessage(String.Format(GlobalMessages.sMSG_WARNING, Ex.Message));
        }
    }

    protected void OrganizationsGridView_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        OrganizationsGridView.DataSource = GlobalLists.MemberOrganizations;
        OrganizationsGridView.PageIndex = e.NewPageIndex;
        OrganizationsGridView.DataBind();
    }

    protected void OrganizationsGridView_Sorting(object sender, GridViewSortEventArgs e)
    {
        SetSortDirection(SortDireaction);
        if (GlobalLists.MemberOrganizations.Count > 1)
        {
            //Sort the data.
            var prop = typeof(MemberOrganizationModel).GetProperty(e.SortExpression);
            List<MemberOrganizationModel> sortedOrganizations;
            if (_sortDirection == "ASC")
            {
                sortedOrganizations = GlobalLists.MemberOrganizations.OrderBy(ll => prop.GetValue(ll, null)).ToList();
            }
            else
            {
                sortedOrganizations = GlobalLists.MemberOrganizations.OrderByDescending(ll => prop.GetValue(ll, null)).ToList();
            }
            OrganizationsGridView.DataSource = sortedOrganizations;
            OrganizationsGridView.DataBind();
            SortDireaction = _sortDirection;
            int columnIndex = 0;
            foreach (DataControlFieldHeaderCell headerCell in OrganizationsGridView.HeaderRow.Cells)
            {
                if (headerCell.ContainingField.SortExpression == e.SortExpression)
                {
                    columnIndex = OrganizationsGridView.HeaderRow.Cells.GetCellIndex(headerCell);
                }
            }
            OrganizationsGridView.HeaderRow.Cells[columnIndex].Controls.Add(sortImage);
        }
    }

    public string SortDireaction
    {
        get
        {
            if (ViewState["SortDireaction"] == null)
                return string.Empty;
            else
                return ViewState["SortDireaction"].ToString();
        }
        set
        {
            ViewState["SortDireaction"] = value;
        }
    }

    protected void RolesGridView_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        RolesGridView.DataSource = GlobalLists.MemberRoles;
        RolesGridView.PageIndex = e.NewPageIndex;
        RolesGridView.DataBind();
        GlobalUtils.SetAccessToControls(this.Page, SaltShakerSession.CurrentRole);
    }

    protected void RolesGridView_Sorting(object sender, GridViewSortEventArgs e)
    {
        SetSortDirection(SortDireaction);
        if (GlobalLists.MemberRoles.Count > 1)
        {
            //Sort the data.
            var prop = typeof(vMemberRoleModel).GetProperty(e.SortExpression);
            List<vMemberRoleModel> sortedRefAndMemberRoles;
            if (_sortDirection == "ASC")
            {
                sortedRefAndMemberRoles = GlobalLists.MemberRoles.OrderBy(ll => prop.GetValue(ll, null)).ToList();
            }
            else
            {
                sortedRefAndMemberRoles = GlobalLists.MemberRoles.OrderByDescending(ll => prop.GetValue(ll, null)).ToList();
            }
            RolesGridView.DataSource = sortedRefAndMemberRoles;
            RolesGridView.DataBind();
            SortDireaction = _sortDirection;
            int columnIndex = 0;
            foreach (DataControlFieldHeaderCell headerCell in RolesGridView.HeaderRow.Cells)
            {
                if (headerCell.ContainingField.SortExpression == e.SortExpression)
                {
                    columnIndex = RolesGridView.HeaderRow.Cells.GetCellIndex(headerCell);
                }
            }
            RolesGridView.HeaderRow.Cells[columnIndex].Controls.Add(sortImage);
        }
    }

    protected void chkview_CheckedChanged(object sender, EventArgs e)
    {
        SetDirtyFlagTrue(sender, e);
    }

    protected void SetDirtyFlagTrue(object sender, EventArgs e)
    {
        isDirty = true;
        //set html textbox val to prevent leaving when change has happend
        isDirtyHidden.Value = "true";
    }

    protected void SetSortDirection(string sortDirection)
    {
        if (sortDirection == "ASC")
        {
            _sortDirection = "DESC";
            sortImage.ImageUrl = ascImagePath;
        }
        else
        {
            _sortDirection = "ASC";
            sortImage.ImageUrl = descImagePath;
        }
    }

    protected void tabMenu1_MenuItem_Click(object sender, MenuEventArgs e)
    {
        showtabItem(Int32.Parse(e.Item.Value));
    }

    protected void cmdYes_Click(object sender, EventArgs e)
    {
        try
        {
            if (!String.IsNullOrEmpty(LabelEA.Text))
            {
                string sMessage = SM.DeleteADAccount(LabelEA.Text.Trim());
                if (sMessage.Contains("deleted"))
                {
                    string saltShakerUserName = string.Format("AMSA\\{0}", SaltShakerSession.UserId);
                    Members.SetMemberTableActiveFlag(false, LabelEA.Text, saltShakerUserName);
                }
                ShowMessage(sMessage);
                PHAlertbox.Visible = false;
                CheckBoxActive.Text = "Deactivated";
                CheckBoxActive.BackColor = Color.Red;
                CheckBoxActive.Checked = true;
                CheckBoxActive.Enabled = false;
                //deactivation also sets WelcomeEmail, but member is no refresh yet
            }
        }
        catch (Exception ex)
        {
            ExceptionMessageException Oops = new ExceptionMessageException(String.Format(GlobalMessages.sMSG_WARNING, ex.Message));
            ShowMessage(Oops.Message);
            PHAlertbox.Visible = false;
        }
    }

    protected void cmdNo_Click(object sender, EventArgs e)
    {
        PHAlertbox.Visible = false;
        CheckBoxActive.Checked = true;
        CheckBoxActive.Enabled = true;

    }

    protected void CheckBoxActive_CheckedChanged(object sender, EventArgs e)
    {
        CheckBoxActive.Enabled = false;
        PHAlertbox.Visible = true;
    }

    #endregion

    #region private functions

    private void renderDataToTable(string id, string fN, string lN, string email,
        bool isContact, bool isActive, string eS, string gL, string activationDate,
        string deactivationDate, string modifiedBy, string welcomeEmailSentText)
    {
        LabelMemberID.Text = String.IsNullOrEmpty(id) ? "Prospect" : id;
        LabelFN.Text = fN;
        LabelLN.Text = lN;
        LabelEA.Text = email;
        CheckBoxActive.Checked = isActive;
        LabelES.Text = eS;
        LabelGL.Text = gL;
        LabelActivationDate.Text = activationDate;
        LabelDeactivationDate.Text = deactivationDate;
        LabelModifiedBy.Text = modifiedBy;

    }
   
    private void LoadAndMergeRefAndMemberRoles(int memberId)
    {
        if (GlobalLists.RefUserRoles.Count == 0)
        {
            GlobalLists.RefUserRoles = Members.GetAllRefUserRoles().ToList();
        }
        List<vMemberRoleModel> memberRolesList = new List<vMemberRoleModel>();
        if (SaltShakerSession.selectedMemberID != null)
        {
            GlobalLists.MemberRoles = Members.GetMemberRoles(memberId).ToList();

            foreach (var refRole in GlobalLists.RefUserRoles)
            {
                vMemberRoleModel memRole = new vMemberRoleModel();
                memRole.RefMemberRoleID = refRole.RefMemberRoleID;
                memRole.RoleName = refRole.RoleName;
                memRole.RoleDescription = refRole.RoleDescription;
                foreach (var memberRole in GlobalLists.MemberRoles)
                {
                    if (memberRole.RefMemberRoleID == refRole.RefMemberRoleID)
                    {
                        memRole.IsMemberRoleActive = memberRole.IsMemberRoleActive;
                        memRole.CreatedBy = memberRole.CreatedBy;
                        memRole.CreatedDate = memberRole.CreatedDate;
                        memRole.ModifiedBy = memberRole.ModifiedBy;
                        memRole.ModifiedDate = memberRole.ModifiedDate;
                        break;
                    }
                }
                memberRolesList.Add(memRole);
            }
            GlobalLists.MemberRoles = memberRolesList;
        }
        else
        {
            ShowMessage(String.Format(GlobalMessages.sMSG_EMPTYPARAM, "Member id"));
        }
    }

    private void renderRolesGridView(string memberId)
    {
        HideMessage();
        try
        {
            if (!String.IsNullOrEmpty(memberId))
            {
                LoadAndMergeRefAndMemberRoles(int.Parse(memberId.Trim()));

                if (GlobalLists.MemberRoles.Count == 0)
                {
                    //hide grids
                    ValidateGridDisplayParameters(-1);
                    ShowMessage(GlobalMessages.sMSG_NO_USERROLES_WARNING);
                }
                else
                {
                    RolesGridView.DataSource = GlobalLists.MemberRoles;
                    RolesGridView.DataBind();
                }
            }
            else
            {
                ShowMessage(String.Format(GlobalMessages.sMSG_EMPTYPARAM, "Member id"));
            }

        }
        catch (Exception ex)
        {
            logger.Error("Exception in uc_MemberDetail.renderOrganizationsGridView:" + ex.Message);
            ShowMessage(String.Format(GlobalMessages.sMSG_WARNING, ex.Message));
        }
    }

    private void renderOrganizationsGridView(string memberId)
    {
        HideMessage();
        try
        {
            if (!String.IsNullOrEmpty(memberId))
            {
                GlobalLists.MemberOrganizations = Members.GetMemberOrganizations(int.Parse(memberId.Trim())).ToList();


                if (GlobalLists.MemberOrganizations.Count == 0)
                {
                    //hide grids
                    ValidateGridDisplayParameters(-1);
                    ShowMessage(GlobalMessages.sMSG_NO_ORGANIZATION_WARNING);
                }
                else
                {
                    OrganizationsGridView.DataSource = GlobalLists.MemberOrganizations;
                    OrganizationsGridView.DataBind();
                }
            }
            else
            {
                ShowMessage(String.Format(GlobalMessages.sMSG_EMPTYPARAM, "Member id"));
            }

        }
        catch (Exception ex)
        {
            logger.Error("Exception in uc_MemberDetail.renderOrganizationsGridView:" + ex.Message);
            ShowMessage(String.Format(GlobalMessages.sMSG_WARNING, ex.Message));
        }
    }

    private void ShowMessage(string sMessage)
    {
        this.warningMessageControl.ShowMessage(sMessage);
    }

    private void HideMessage()
    {
        this.warningMessageControl.HideMessage();
    }


    //Display grid based on tab index
    private void showtabItem(int index)
    {
        try
        {
            object sender = new object();
            EventArgs e = new EventArgs();
            MultiView c = (MultiView)this.FindControl("MultiViewRelatedContent");
            //check for valid parameters before hiding taking any others actions
            if (ValidateGridDisplayParameters(index))
            {
                switch (index)
                {
                    case 0:
                        OrganizationsGridViewPanel.Visible = true;
                        this.renderOrganizationsGridView(SaltShakerSession.selectedMemberID);
                        break;
                    case 1:
                        RolesGridViewPanel.Visible = true;
                        this.renderRolesGridView(SaltShakerSession.selectedMemberID);
                        break;
                }
            }
            c.ActiveViewIndex = index;
            GlobalUtils.SetAccessToControls(this.Page, SaltShakerSession.CurrentRole);
        }
        catch (Exception ex)
        {
            logger.Error("Exception in uc_MemberDetail.showtabItem:" + ex.Message);
            ExceptionMessageException Oops = new ExceptionMessageException(ex.Message);
            ShowMessage(String.Format(GlobalMessages.sMSG_WARNING, Oops.Message));
        }
    }

    //check for valid parameters before hiding hiding grids
    //and display correct error message if no parameter is valid
    private bool ValidateGridDisplayParameters(int index)
    {
        string sMemberID = SaltShakerSession.selectedMemberID;
        string errMsg = string.Format(GlobalMessages.sMSG_MEMBERID_ISNULL, "Salt UserID" );
        //error messages and warnings
        HideMessage();

        if (!String.IsNullOrEmpty(sMemberID))
        {
            RolesGridViewPanel.Visible = false;
            return true;
        }
        else
        {
            ShowMessage(errMsg);
        }
        return false;
    }

    protected void CmdSaveChanges_Click(object sender, EventArgs e)
    {
        bool _isDirtyRoles = isDirty;
        bool bSuccess = true;

        if (isDirty && !String.IsNullOrEmpty(SaltShakerSession.UserId))
        {
            string saltShakerUserName = string.Format("AMSA\\{0}", SaltShakerSession.UserId);

            if (_isDirtyRoles)
            {
                bSuccess = saveMemberRoleUpdates();
            }
            else
            {
                ShowMessage(string.Format(GlobalMessages.sMSG_WARNING, "your update appears to have failed. Please contact support for assistance [ContactPrefSave]"));
            }            
        }

    }

    private bool saveMemberRoleUpdates()
    {
        bool bSuccess = true;
        try
        {
            CheckBox cbxMemberRoleActive;
            TextBox txtbRefUserRoleID;
            List<vMemberRoleModel> memberRoles = new List<vMemberRoleModel>();

            for (int i = 0; i < RolesGridView.Rows.Count; i++)
            {
                cbxMemberRoleActive = (CheckBox)RolesGridView.Rows[i].FindControl("IsMemberRoleActive");
                txtbRefUserRoleID = (TextBox)RolesGridView.Rows[i].FindControl("RefMemberRoleID");
                if (!string.IsNullOrEmpty(txtbRefUserRoleID.Text))
                {
                    var memberRoleMatch = GlobalLists.MemberRoles.Where(entity => entity.RefMemberRoleID == Int32.Parse(txtbRefUserRoleID.Text.Trim())).FirstOrDefault();
                    if (memberRoleMatch != null)
                    {
                        //check if the value changed and only add if it does
                        if (memberRoleMatch.IsMemberRoleActive != cbxMemberRoleActive.Checked)
                        {
                            vMemberRoleModel memRole = new vMemberRoleModel();
                            memRole.RefMemberRoleID = Int32.Parse(txtbRefUserRoleID.Text.Trim());
                            memRole.IsMemberRoleActive = cbxMemberRoleActive.Checked;
                            memberRoles.Add(memRole);
                        }
                    }
                    //RefRole being activated for the the first time for this member, insert into MemberRole (associate with this member)
                    else if (cbxMemberRoleActive.Checked)
                    {
                        vMemberRoleModel memRole = new vMemberRoleModel();
                        memRole.RefMemberRoleID = Int32.Parse(txtbRefUserRoleID.Text.Trim());
                        memRole.IsMemberRoleActive = cbxMemberRoleActive.Checked;
                        memberRoles.Add(memRole);
                    }
                }
            }

            if(memberRoles.Count > 0)
            {
                string saltShakerUserName = string.Format("AMSA\\{0}", SaltShakerSession.UserId);
                bSuccess = Members.UpdateMemberRoles(int.Parse(SaltShakerSession.selectedMemberID), memberRoles, saltShakerUserName);
                if (bSuccess)
                {
                    //give it a second to update
                    GlobalUtils.WaiTTime();
                    renderRolesGridView(SaltShakerSession.selectedMemberID);
                    ClearAllDirtyFlags();
                    ShowMessage("MemberRoles " +GlobalMessages.sMSG_UPDATE_SUCCESS);
                }
                else
                {
                    ShowMessage(string.Format(GlobalMessages.sMSG_WARNING, "MemberRoles update appears to have failed. Please contact support for assistance [MemberRoleSave]"));
                }
            }
        }
        catch (Exception saveEx)
        {
            ShowMessage(string.Format(GlobalMessages.sMSG_WARNING, "your MemberRoles update appears to have failed. Please contact support for assistance " + saveEx));
        }

        return bSuccess;
    }

    private void ClearAllDirtyFlags()
    {
        isDirtyHidden.Value = "false";
    }
    #endregion

}

