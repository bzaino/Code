using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;

using SALTShaker.BLL;
using SALTShaker.DAL.DataContracts;
//Adding logging
using SALTShaker.HelperClass;

namespace SALTShaker.usercontrol
{
    public partial class uc_MemberData : System.Web.UI.UserControl
    {
        static MemberBL Members = new MemberBL();
        int iRecordCount = 0;
        string selectedMemberID;
        string selectedUserName;
        int TabSeachIndx = 1;
        int TabGridIndx = 0;
        int startIndex = 0;
        int pageNumber = 1;
        int totalNoRows = 0;
        public static int pageSize = 10; //the no of records per page is currently set to 10
        public static int NumofMemberRecordsToRetrieve = Int32.Parse(ConfigurationManager.AppSettings["NumofMemberRecordsToRetrieve"]);
        public static int NumofPages = 1;

        //Regex
        static string memberIDPattern = @"^\d{0,9}$";
        static string namePattern = @"^$|^[A-Za-z-_.' ]*$";
        static string communityPattern = @"^$|^[A-Za-z-_.' 0-9]*$";
        //sorting params
        static private string _sortDirection;
        System.Web.UI.WebControls.Image sortImage = new System.Web.UI.WebControls.Image();
        static string ascImagePath = "/Images/icon_asc.gif";
        static string descImagePath = "/Images/icon_desc.gif";
        string sortExpression;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (String.IsNullOrEmpty(SaltShakerSession.UserId))
                {
                    Response.Redirect("Index.aspx");
                }
            }
        }

        #region GRID Related functionality

        private void UniqueSearch(string id, string email)
        {
            try
            {
                HideMessage();
                //Search by MemberID should route to Detail page, if found
                if (!String.IsNullOrEmpty(id))
                {
                    selectedMemberID = Members.GetMember(Int32.Parse(id)).MemberID.ToString();
                    RedirectToDetails(selectedMemberID, email, GlobalMessages.sMSG_NORECORDFOUND_MEMBERID);
                }

                //Search by Email Address should route to Detail page, if found
                if (!String.IsNullOrEmpty(email))
                {
                    GlobalLists.SaltMemberModel = Members.GetMember(email);
                    if (GlobalLists.SaltMemberModel != null)
                    {
                        selectedMemberID = GlobalLists.SaltMemberModel.MemberID.ToString();
                        selectedUserName = GlobalLists.SaltMemberModel.EmailAddress;
                        RedirectToDetails(selectedMemberID, selectedUserName, GlobalMessages.sMSG_NORECORDFOUND_USERNAME);
                    }
                    else
                    {
                        this.SearchPartialEmailAndRenderGridView(email);
                    }
                }
            }
            catch (FormatException)
            {
                ShowMessage(GlobalMessages.sMSG_INVALITD_MEMBERID);
            }
            catch (OverflowException)
            {
                ShowMessage(GlobalMessages.sMSG_MEMBERID_TOO_MANY_DIGITS);
            }
            catch (Exception ex)
            {
                ShowMessage(ex.Message);
            }
        }

        private void SearchAndRenderGridView(Nullable<Int32> id, string firstName = "", string lastName = "", string email = "", string communityDisplayname = "")
        {
            using (MemberBL Members = new MemberBL())
            {
                //Search by params
                GlobalLists.ListSaltMemberModel = Members.GetMembersBySearchParms(firstName.Trim(), lastName.Trim(), communityDisplayname.Trim()).ToList();
                iRecordCount = GlobalLists.ListSaltMemberModel.Count();
                if (iRecordCount > 0)
                {
                    MemberDataGridView.DataSource = GlobalLists.ListSaltMemberModel;
                    MemberDataGridView.DataBind();

                    if (iRecordCount == 1)
                    {
                        selectedMemberID = GlobalLists.ListSaltMemberModel[0].MemberID != null ? GlobalLists.ListSaltMemberModel[0].MemberID.ToString() : null;
                        MemberDataGridView.SelectedIndex = 0; //select the only existing row
                        GridViewRow selectedRow = MemberDataGridView.SelectedRow;
                        selectedUserName = selectedRow.Cells[4].Text;
                        RedirectToDetails(selectedMemberID, selectedUserName);
                    }
                }
                else
                {
                    ShowMessage(GlobalMessages.sMSG_NORECORDSFOUND);
                }
            }
        }

        private void SearchPartialEmailAndRenderGridView(string email = "")
        {
            using (MemberBL Members = new MemberBL())
            {
                //Search by params
                GlobalLists.ListMemberResults = Members.GetMembersBySearchParms(null, "", "", email, "").ToList();
                iRecordCount = GlobalLists.ListMemberResults.Count();
                if (iRecordCount > 0)
                {
                    MemberDataGridView.DataSource = GlobalLists.ListMemberResults;
                    MemberDataGridView.DataBind();

                    if (iRecordCount == 1)
                    {
                        selectedMemberID = GlobalLists.ListMemberResults[0].MemberID > 0 ? GlobalLists.ListMemberResults[0].MemberID.ToString() : null;
                        MemberDataGridView.SelectedIndex = 0; //select the only existing row
                        GridViewRow selectedRow = MemberDataGridView.SelectedRow;
                        selectedUserName = selectedRow.Cells[4].Text;
                        RedirectToDetails(selectedMemberID, selectedUserName);
                    }
                    this.displayRowCount();
                }
                else
                {
                    ShowMessage(GlobalMessages.sMSG_NORECORDSFOUND);
                }
            }
        }

        private void RedirectToDetails(string selectedMemberID, string selectedUserName, string errorMSG = GlobalMessages.sMSG_NORECORDSFOUND)
        {
            if (!string.IsNullOrEmpty(selectedMemberID))
            {
                Response.Redirect("Detail.aspx?memberID=" + selectedMemberID);
            }
            else if (!String.IsNullOrEmpty(selectedUserName))
            {
                Response.Redirect("Detail.aspx?userName=" + selectedUserName);
            }
            else
            {
                ShowMessage(errorMSG);
            }
        }
        protected void MemberDataGridView_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridViewRow selectedRow = MemberDataGridView.SelectedRow;
            selectedMemberID = selectedRow.Cells[1].Text;
            selectedUserName = selectedRow.Cells[4].Text;
            MemberRow_Click(sender, e);
        }

        protected void MemberDataGridView_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            if (GlobalLists.ListSaltMemberModel.Count() > 0)
            {
                if (string.IsNullOrEmpty(_sortDirection))
                {
                    MemberDataGridView.DataSource = GlobalLists.ListSaltMemberModel; // ds.Tables[0];
                    MemberDataGridView.PageIndex = e.NewPageIndex;
                    MemberDataGridView.DataBind();
                }
                else
                {
                    //Sort the data. SWD-6936
                    if (string.IsNullOrEmpty(sortExpression) && SaltShakerSession.MemberSortExpression != null)
                    {
                        sortExpression = SaltShakerSession.MemberSortExpression;
                    }
                    this.SortHelper(sortExpression, e.NewPageIndex);
                }

            }
        }

        protected void MemberDataGridView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                ///Add this javascript to the ondblclick Attribute of the row
                //e.Row.Attributes.Add("ondblclick", "CreateTrigger();");
                e.Row.Attributes.Add("onclick", Page.ClientScript.GetPostBackEventReference((Control)sender, "Select$" + e.Row.RowIndex.ToString()));

            }
        }

        protected void MemberDataGridView_Sorting(object sender, GridViewSortEventArgs e)
        {
            sortExpression = e.SortExpression;
            SaltShakerSession.MemberSortExpression = sortExpression;
            SetSortDirection(SortDireaction);
            this.SortHelper(sortExpression, 0);
        }

        protected void SortHelper(string sortExpression, int pageIndex)
        {

            if (GlobalLists.ListSaltMemberModel.Count >= 1)
            {
                List<SaltMemberModel> sortedOrganization;
                //Sort if possible SWD-6936
                if (!string.IsNullOrEmpty(sortExpression))
                {
                    //Sort the data.
                    var prop = typeof(SaltMemberModel).GetProperty(sortExpression);

                    if (_sortDirection == "ASC")
                    {
                        sortedOrganization = GlobalLists.ListSaltMemberModel.OrderBy(lr => prop.GetValue(lr, null)).ToList();
                        sortImage.ImageUrl = descImagePath;
                        //set global list to new sorted list
                        GlobalLists.ListSaltMemberModel = sortedOrganization;
                    }
                    else
                    {
                        sortedOrganization = GlobalLists.ListSaltMemberModel.OrderByDescending(lr => prop.GetValue(lr, null)).ToList();
                        sortImage.ImageUrl = ascImagePath;
                        //set global list to new sorted list
                        GlobalLists.ListSaltMemberModel = sortedOrganization;
                    }
                }
                else
                {
                    //use the previous list
                    sortedOrganization = GlobalLists.ListSaltMemberModel;
                }

                MemberDataGridView.DataSource = sortedOrganization;
                MemberDataGridView.PageIndex = pageIndex;
                MemberDataGridView.DataBind();
                SortDireaction = _sortDirection;
                int columnIndex = 0;
                foreach (DataControlFieldHeaderCell headerCell in MemberDataGridView.HeaderRow.Cells)
                {
                    if (headerCell.ContainingField.SortExpression == sortExpression)
                    {
                        columnIndex = MemberDataGridView.HeaderRow.Cells.GetCellIndex(headerCell);
                    }
                }

                MemberDataGridView.HeaderRow.Cells[columnIndex].Controls.Add(sortImage);
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

        protected void SetSortDirection(string sortDirection)
        {
            if (sortDirection == "ASC")
            {
                _sortDirection = "DESC";
                sortImage.ImageUrl = descImagePath;
            }
            else
            {
                _sortDirection = "ASC";
                sortImage.ImageUrl = ascImagePath;
            }
        }

        private void displayRowCount()
        {
            int stIdx = startIndex + 1;
            int edIdx = startIndex;
            if (pageNumber == NumofPages)
            {
                edIdx = totalNoRows; //set to lastcount if in last page
            }
            else
            {
                edIdx += NumofMemberRecordsToRetrieve; //increment by the no of records to retrieve
            }
        }


        #endregion

        #region BUTTONS AND CLICKS Related functionality

        private void showtabItem(int index)
        {
            MultiView c = (MultiView)this.FindControl("MultiView1");
            c.ActiveViewIndex = index;
        }

        protected void tabMenu1_MenuItem_Click(object sender, MenuEventArgs e)
        {
            int tabNo = Int32.Parse(e.Item.Value);
            //if member list tab is selected
            if (tabNo == TabGridIndx)
            {
                pnl_SearchResultCount.Visible = false;
                //if user had previous search results reset and load first range
                clearSearchParams();
                selectedMemberID = null;
                selectedUserName = null;
                pageNumber = 1; // reset to startpage
                startIndex = 0;
                this.displayRowCount();
            }
            else
            {
                //if search tab is selected reset grid selections
                resetGridAndPageSelection();
            }

            this.hideSpinner();
            showtabItem(tabNo);
        }

        private bool isSearchParamCleared()
        {
            return (TextBoxFirstName.Text.Trim().Length == 0 && TextBoxEmail.Text.Trim().Length == 0 && TextBoxLastName.Text.Trim().Length == 0 && TextBoxMemberID.Text.Trim().Length == 0);
        }

        private void clearSearchParams()
        {
            TextBoxMemberID.Text = "";
            TextBoxFirstName.Text = "";
            TextBoxLastName.Text = "";
            TextBoxEmail.Text = "";
        }

        private void resetGridAndPageSelection()
        {
            MemberDataGridView.SelectedIndex = -1;
            MemberDataGridView.PageIndex = 0;
        }


        protected void ClearSearch_Click(object sender, EventArgs e)
        {
            TextBoxMemberID.Text = "";
            TextBoxFirstName.Text = "";
            TextBoxLastName.Text = "";
            TextBoxEmail.Text = "";
        }

        private bool ValidateInput(string memberID, string lastName, string firstName, string email, string communityDisplayName)
        {
            HideMessage();

            //First check: at least one parameter must be passed
            if (string.IsNullOrEmpty(memberID + lastName + firstName + email + communityDisplayName))
            {
                ShowMessage(GlobalMessages.sMSG_AT_LEAST_ONE_VALUE_REQUIRED);
                return false;
            }
            //Second check: Are these a valid patern
            if (Regex.IsMatch(memberID, memberIDPattern) && Regex.IsMatch(lastName, namePattern) && Regex.IsMatch(firstName, namePattern) && Regex.IsMatch(communityDisplayName, communityPattern))
            {
                return true;
            }
            else
            {
                ShowMessage(GlobalMessages.sMSG_ENTER_VALID_SEARCH);
            }
            //default to false
            return false;
        }

        protected void SearchBtn_Click(object sender, EventArgs e)
        {
            int TabActiveIndex = ((MultiView)this.FindControl("MultiView1")).ActiveViewIndex;

            string memberID = TextBoxMemberID.Text.Trim();
            string lastName = TextBoxLastName.Text.Trim();
            string firstName = TextBoxFirstName.Text.Trim();
            string email = TextBoxEmail.Text.Trim();
            string communityDisplayName = String.Empty;

            if (ValidateInput(memberID, lastName, firstName, email, communityDisplayName))
            {

                Nullable<int> memID = null;
                if (!String.IsNullOrEmpty(memberID))
                {
                    memID = Int32.Parse(memberID);
                }

                if (!String.IsNullOrEmpty(memberID) || !String.IsNullOrEmpty(email))
                {
                    this.UniqueSearch(memberID, email);
                }

                else
                {
                    this.SearchAndRenderGridView(memID, firstName, lastName, email, communityDisplayName);
                    this.displayRowCount();
                }

                if (iRecordCount > 1)
                {
                    showtabItem(TabGridIndx);
                    lbl_SearchResultCount.Text = iRecordCount + " Records found";
                    iRecordCount = 0;
                    pnl_SearchResultCount.Visible = true;
                }
                else
                {
                    ShowMessage(GlobalMessages.sMSG_NORECORDSFOUND);
                }

            }
            this.hideSpinner();

            //if grid is shown switch to searchbox
            //so user can add parameters
            if (TabActiveIndex == TabGridIndx)
            {
                showtabItem(TabSeachIndx);
            }
        }

        protected void MemberRow_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(selectedMemberID) && selectedMemberID != "Prospect")
            {
                Response.Redirect("Detail.aspx?memberID=" + selectedMemberID);
            }
            else if (!String.IsNullOrEmpty(selectedUserName))
            {
                Response.Redirect("Detail.aspx?userName=" + selectedUserName);
            }
            else
            {
                ShowMessage(GlobalMessages.sMSG_SELECT_FIRST_NAME);
            }

            //if searchbox is shown switch to gridvew
            //so user can select from grid
            int TabActiveIndex = ((MultiView)this.FindControl("MultiView1")).ActiveViewIndex;

            if (TabActiveIndex == TabSeachIndx)
            {
                showtabItem(TabGridIndx);
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

        protected void btnLoadFirstRange_Click(object sender, EventArgs e)
        {
            if (SaltShakerSession.UserId != null)
            {
                selectedMemberID = null;
                pageNumber = 1; // reset to startpage
                startIndex = 0;
                //pass starting index and num of records to retrieve
                this.displayRowCount();
                this.hideSpinner();
            }
            else
            {
                Response.Redirect("Index.aspx");
            }
        }

        protected void btnLoadLastRange_Click(object sender, EventArgs e)
        {
            if (SaltShakerSession.UserId != null)
            {
                selectedMemberID = null;
                pageNumber = NumofPages; //set to lastpage
                startIndex = totalNoRows - NumofMemberRecordsToRetrieve;
                //pass starting index and num of records to retrieve
                this.displayRowCount();
                this.hideSpinner();
            }
            else
            {
                Response.Redirect("Index.aspx");
            }
        }

        protected void btnLoadNextRange_Click(object sender, EventArgs e)
        {
            if (SaltShakerSession.UserId != null)
            {
                selectedMemberID = null;
                pageNumber += 1; //increment pageNumber                
                startIndex = (pageNumber - 1) * NumofMemberRecordsToRetrieve;
                //pass starting index and num of records to retrieve
                this.displayRowCount();
                this.hideSpinner();
            }
            else
            {
                Response.Redirect("Index.aspx");
            }
        }

        protected void btnLoadPreviousRange_Click(object sender, EventArgs e)
        {
            if (SaltShakerSession.UserId != null)
            {
                selectedMemberID = null;
                pageNumber -= 1; //decrement pageNumber         
                startIndex = (pageNumber - 1) * NumofMemberRecordsToRetrieve;
                //pass starting index and num of records to retrieve  
                this.displayRowCount();
                this.hideSpinner();
            }
            else
            {
                Response.Redirect("Index.aspx");
            }
        }

        #endregion

        private void hideSpinner()
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "click", "javascript:DBTool.Spinner.hide();", true);
        }

    }
}