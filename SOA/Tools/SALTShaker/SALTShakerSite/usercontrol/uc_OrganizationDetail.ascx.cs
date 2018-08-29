using SALTShaker.BLL;
using SALTShaker.DAL.DataContracts;
using SALTShaker.HelperClass;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;

using log4net;

using GlobalUtils = SALTShaker.HelperClass.GlobalUtils;


namespace SALTShaker.usercontrol
{
    public partial class uc_OrganizationDetail : System.Web.UI.UserControl
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(usercontrol_uc_MemberDetail));

        //Catch Exception errors
        const string sMSG_WARNING = "Oops that was unexpected: {0}";
        const string sMSG_NO_ORGANIZATIONPRODUCT_WARNING = "There are no product associated with this organization.";
        //EMPTY paramater errror
        const string sMSG_EMPTYPARAM = "The value for {0} is empty or has not been provided. Please enter a value in {0} and try again.";
        //ButtonSearch_Click
        const string sMSG_OrganizationID_ISNULL = "Selected Organization ID is null!";
        const string sMSG_NOOrganizationFoundBYID = "No organization found.";
        const string sMSG_UPDATE_SUCCESS = "Successfully updated! ";
        string selectedRefOrganizationID;
        static List<ProductModel> ListOfProducts;
        static List<OrganizationProductModel> ListOfOrganizationProducts = new List<OrganizationProductModel>();
        static List<OrganizationProductModel> OrganizationGoals = new List<OrganizationProductModel>();
        static List<OrganizationProductModel> OrganizationCourses = new List<OrganizationProductModel>();
        static List<OrganizationProductModel> OrganizationProducts = new List<OrganizationProductModel>();
        static OrganizationModel selectedOrganization;
        static OrganizationBL organization = new OrganizationBL();
        static ContentBL content = new ContentBL();
        static List<ContentModel> contentItems;
        static List<ContentModel> combinedContentItems;
        static private bool isDirty = false, isGridDirty = false, isFormDirty = false;
        DataTable productTable; //used for batch update in sql
        //sorting params
        System.Web.UI.WebControls.Image sortImage = new System.Web.UI.WebControls.Image();
        static string ascImagePath = "/Images/icon_desc.gif"; //images are backwards
        static string descImagePath = "/Images/icon_asc.gif"; //images are backwards

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                selectedRefOrganizationID = Request["organizationID"];
                SaltShakerSession.selectedRefOrganizationID = selectedRefOrganizationID;

                LoadOrganizationDetail(selectedRefOrganizationID);

                this.Page.Title = "OrganizationDetail - SALTShaker";
		        GlobalUtils.SetAccessToControls(this.Page, SaltShakerSession.CurrentRole);

                //showtabItem(0);
            }
        }

        private void LoadOrganizationDetail(string selectedRefOrganizationID)
        {
            if (!string.IsNullOrEmpty(selectedRefOrganizationID))
            {
                List<OrganizationModel> organizationList = organization.GetOrganizationByID(int.Parse(selectedRefOrganizationID.Trim())).ToList();
                if (organizationList.Count == 1)
                {
                    selectedOrganization = organizationList[0];

                    //match endeca content items with orgData
                    contentItems = content.GetContentItems();
                    combinedContentItems = content.CombineOrgToDosAndContentItems(contentItems, selectedOrganization.OrganizationToDoItems);

                    this.renderDetailFields(selectedOrganization);
                    this.LoadProducts(selectedOrganization);
                }
                else
                {
                    ShowMessage(sMSG_NOOrganizationFoundBYID);
                }
            }
        }

        private void LoadProducts(OrganizationModel selectedOrganization)
        {
            if (selectedOrganization.ProductsSubscriptionList == null)
            {
                selectedOrganization.ProductsSubscriptionList = new List<OrganizationProductModel>();
            }
            else
            {
                selectedOrganization.ProductsSubscriptionList.Clear();
            }

            ListOfProducts = organization.GetAllProducts().ToList();
            if (SaltShakerSession.selectedRefOrganizationID != null)
            {
                ListOfOrganizationProducts = organization.GetOrganizationProducts(int.Parse(SaltShakerSession.selectedRefOrganizationID.Trim())).ToList();

                foreach (var product in ListOfProducts)
                {
                    if (product.IsProductActive)
                    {
                        OrganizationProductModel prodSub = new OrganizationProductModel();
                        prodSub.RefProductID = product.RefProductID;
                        prodSub.ProductName = product.ProductName;
                        prodSub.ProductDescription = product.ProductDescription;
                        prodSub.RefProductTypeID = product.RefProductTypeID;
                        foreach (var orgProduct in ListOfOrganizationProducts)
                        {
                            if (orgProduct.RefProductID == product.RefProductID)
                            {
                                prodSub.RefOrganizationID = orgProduct.RefOrganizationID;
                                prodSub.IsRefOrganizationProductActive = orgProduct.IsRefOrganizationProductActive;
                                prodSub.CreatedBy = orgProduct.CreatedBy;
                                prodSub.CreatedDate = orgProduct.CreatedDate;
                                prodSub.ModifiedBy = orgProduct.ModifiedBy;
                                prodSub.ModifiedDate = orgProduct.ModifiedDate;
                                break;
                            }
                        }
                        selectedOrganization.ProductsSubscriptionList.Add(prodSub);
                    }
                }
            }

            this.setProductsByTypes(selectedOrganization.ProductsSubscriptionList);

        }

        private void renderDetailFields(OrganizationModel organization)
        {
            this.HideMessage();
            LabelOrgNameHeader.Text = organization.OrganizationName;
            LabelOrgname.Text = organization.OrganizationName;
            LabelOrgAlias.Text = organization.OrganizationAliases;
            LabelOECode.Text = organization.OPECode;
            LabelBranchCode.Text = organization.BranchCode;
            LabelOrganizationLogo.Text = organization.OrganizationLogoName;
            CheckBoxIsContracted.Checked = organization.IsContracted;

            CheckBoxAllowedLookupFlag.Checked = organization.IsLookupAllowed;
            LabelOrgID.Text = organization.RefOrganizationID.ToString();
            LabelExtOrgID.Text = organization.OrganizationExternalID.ToString();
            LabelModifiedBy.Text = organization.ModifiedBy;
            LabelModifiedDate.Text = organization.ModifiedDate.ToString();
        }

        private void ShowMessage(string sMessage)
        {
            this.warningMessageControl.ShowMessage(sMessage);
        }

        private void HideMessage()
        {
            this.warningMessageControl.HideMessage();
        }

        protected void Savechanges_Click(object sender, EventArgs e)
        {
            bool bSuccess = false;
            if (isDirty && !String.IsNullOrEmpty(LabelOrgID.Text) && !String.IsNullOrEmpty(SaltShakerSession.UserId))
            {
                if (isFormDirty)
                {
                    bSuccess = organization.UpdateRefOrganizationInfoFlag(int.Parse(LabelOrgID.Text.ToString()), CheckBoxAllowedLookupFlag.Checked, SaltShakerSession.UserId);
                }

                if (bSuccess || isGridDirty)
                {
                    isFormDirty = false;
                    //set button save flag to prevent update with no change
                    isDirty = false;
                    //set html textbox val to allow exit page
                    isDirtyHidden.Value = "false";
                    ShowMessage(sMSG_UPDATE_SUCCESS);
                    if (isGridDirty)
                    {
                        bSuccess = saveOrganizationProductSubscription();
                        if (bSuccess)
                            isGridDirty = false;
                    }
                }
                else
                {
                    ShowMessage(string.Format(sMSG_WARNING, "your update appears to have failed. Please contact support for assistance"));
                }
            }
        }

        private bool saveOrganizationProductSubscription()
        {
            bool bSuccess = false;
            bool atLeastOneGoal = false;
            try
            {
                MultiView c = (MultiView)FindControl("detailMultiView");
                GridView activeGView;
                CheckBox cbxProdActive;
                TextBox txtbProdID;
                if (c.ActiveViewIndex == 0)
                {
                    activeGView = ProductGridView;
                }
                else if (c.ActiveViewIndex == 1)
                {
                    activeGView = CoursesGridView;
                }
                else
                {
                    ShowMessage(string.Format(sMSG_WARNING, "your organization products update appears to have failed. Please contact support for assistance"));
                    return bSuccess;
                }
                productTable = new System.Data.DataTable();
                productTable.Columns.Add("RefProductID", typeof(int));
                productTable.Columns.Add("IsActive", typeof(bool));
                productTable.TableName = "RefOrganizationProductTableType";

                //here you can find your control and get value(Id).
                for (int i = 0; i < activeGView.Rows.Count; i++)
                {
                    cbxProdActive = (CheckBox)activeGView.Rows[i].FindControl("IsRefOrganizationProductActive");
                    txtbProdID = (TextBox)activeGView.Rows[i].FindControl("RefProductID");

                    //While we are looping through the rows in the table, set a flag variable to true when we find at least one goal product with an opt-in
                    Int32 productID = Int32.Parse(txtbProdID.Text.Trim());
                    if (!string.IsNullOrEmpty(txtbProdID.Text))
                    {
                        var prodMatch = ListOfOrganizationProducts.Where(entity => entity.RefProductID == productID).FirstOrDefault();
                        if (prodMatch != null)
                        {
                            //check if the value changed and only add if it does
                            if (prodMatch.IsRefOrganizationProductActive != cbxProdActive.Checked)
                            {
                                productTable.Rows.Add(int.Parse(txtbProdID.Text), cbxProdActive.Checked);
                            }
                        }
                        //checked active box on a none existing (ACTIVE) product means add it to 
                        //table of ACTIVE products associated with this organization
                        else if (cbxProdActive.Checked)
                        {
                            productTable.Rows.Add(int.Parse(txtbProdID.Text), cbxProdActive.Checked);
                        }
                    }
                }
                //verify that there's at least one goal product active
                atLeastOneGoal = hasActiveGoal(productTable);

                //No goal products were selected, show an error message as this in an invalid state, break out of this method, we don't want to save these changes
                if (!atLeastOneGoal)
                {
                    ShowMessage("You must enable at least one goal product.  These updates have not been saved to the DB.");
                    return false;
                }
                if (productTable.Rows.Count > 0)
                {
                    bSuccess = organization.UpdateOrganizationProductSubscription(int.Parse(LabelOrgID.Text.ToString()), productTable, SaltShakerSession.UserId);
                    if (bSuccess)
                    {
                        //give it a second to update
                        //System.Diagnostics.Stopwatch stopwatch = System.Diagnostics.Stopwatch.StartNew();
                        GlobalUtils.WaiTTime();
                        LoadOrganizationDetail(SaltShakerSession.selectedRefOrganizationID);                     
                        if (c.ActiveViewIndex == 0)
                        {
                            //Keep products tab active
                            showtabItem(0);
                        }
                        else if (c.ActiveViewIndex == 1)
                        {
                            //keep courses tab active
                            showtabItem(1);
                        }
                        ShowMessage(sMSG_UPDATE_SUCCESS);
                    }
                    else
                    {
                        ShowMessage(string.Format(sMSG_WARNING, "your organization products update appears to have failed. Please contact support for assistance"));
                    }
                }
            }
            catch (Exception saveEx)
            {
                ShowMessage(string.Format(sMSG_WARNING, "your organization products update appears to have failed. Please contact support for assistance " + saveEx));
            }
            return bSuccess;
        }


        protected void SetDirtyFlagTrue(object sender, EventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            var cbID = cb.ID;
            if (cbID == "IsRefOrganizationProductActive")
            {
                isGridDirty = true;
            }
            else
            {
                isFormDirty = true;
            }
            
            isDirty = true;
            //set html textbox val to prevent leaving when change has happend
            isDirtyHidden.Value = "true";
        }

        protected override void OnUnload(EventArgs e)
        {
            base.OnUnload(e);
        }

        protected void chkview_CheckedChanged(object sender, EventArgs e)
        {
             SetDirtyFlagTrue(sender, e);
        }
        protected void tabMenu1_MenuItem_Click(object sender, MenuEventArgs e)
        {
            showtabItem(int.Parse(e.Item.Value));
        }

        private void showtabItem(int index)
        {
            try
            {
                EventArgs e = new EventArgs();
                MultiView c = (MultiView)FindControl("detailMultiView");
                //check for valid parameters before hiding taking any others actions
                //if (ValidateGridDisplayParameters(index))
                {
                    switch (index)
                    {
                        case 0:                            
                            ProductGridViewPanel.Visible = true;
                            renderProductGridView();
                            break;
                        case 1:
                            CoursesViewPanel.Visible = true;
                            renderCoursesGridView();
                            break;
                        case 2:
                            TodoContentGridViewPanel.Visible = true;
                            renderTodoContentGridView();
                            break;
                    }
                }
                c.ActiveViewIndex = index;
                GlobalUtils.SetAccessToControls(this.Page, SaltShakerSession.CurrentRole);
            }
            catch (Exception ex)
            {
                logger.Error("Exception in uc_OrganizationDetail.showtabItem:" + ex.Message);
                ExceptionMessageException Oops = new ExceptionMessageException(ex.Message);
                ShowMessage(String.Format(GlobalMessages.sMSG_WARNING, Oops.Message));
            }
        }
        private void renderCoursesGridView()
        {
            CoursesGridView.DataSource = OrganizationCourses;
            CoursesGridView.DataBind();
        }
        private void renderProductGridView()
        {
            ProductGridView.DataSource = OrganizationProducts;
            ProductGridView.DataBind();
        }
        private void setProductsByTypes(List<OrganizationProductModel> orgProducts)
        {
            OrganizationGoals = orgProducts.Where(p => p.RefProductTypeID == 3).ToList();
            OrganizationCourses = orgProducts.Where(p => p.RefProductTypeID == 1).OrderBy(c => c.ProductName).ToList();
            OrganizationProducts = orgProducts.Where(p => p.RefProductTypeID != 1).ToList(); //all products except courses
        }
        private bool hasActiveGoal(System.Data.DataTable orgProductsUpsertDT)
        {
            bool ruleSatisfied = false;
            int goalsCountFromDB = OrganizationGoals.Where(p => p.IsRefOrganizationProductActive).ToList().Count();
            DataView goalsViewFromUI = new DataView(orgProductsUpsertDT);
            //initial filter to get the count of goals' subscriptions
            goalsViewFromUI.RowFilter = "RefProductID IN (8, 9, 10, 11)";
            int goalsCountFromUI = goalsViewFromUI.Count;
            //further filter to get the count of active goals
            goalsViewFromUI.RowFilter = goalsViewFromUI.RowFilter + "AND IsActive = True";
            int activeGoalsCountFromUI = goalsViewFromUI.Count;
            if ((goalsCountFromUI == 0 && goalsCountFromDB > 0) || //no change to goals subscriptions and already has a goal
                activeGoalsCountFromUI > 0 || //there's at least one goal being activated
                (goalsCountFromDB > (goalsCountFromUI - activeGoalsCountFromUI))) //active count in DB > deactivate count from UI
            {
                ruleSatisfied = true;
            }

            return ruleSatisfied;
        }
        private void renderTodoContentGridView()
        {
            HideMessage();
            TodoContentGridView.DataSource = combinedContentItems;
            TodoContentGridView.DataBind();
            TodoContentGridView.SelectedIndex = -1;
        }
        protected void TodoContentGridView_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            TodoContentGridView.DataSource = combinedContentItems;
            TodoContentGridView.PageIndex = e.NewPageIndex;
            TodoContentGridView.DataBind();
        }
        protected void TodoContentGridView_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            txtUpdateHeadline.Text = TodoContentGridView.SelectedRow.Cells[0].Text.Replace("&nbsp;", String.Empty);
            txtUpdateDueDate.Text = TodoContentGridView.SelectedRow.Cells[7].Text.Replace("&nbsp;", String.Empty);
            toDoId.Value = TodoContentGridView.DataKeys[TodoContentGridView.SelectedIndex].Values[0].ToString();
            toDoType.Value = TodoContentGridView.DataKeys[TodoContentGridView.SelectedIndex].Values[1].ToString();
            ddlUpdateToDoStatus.SelectedValue = TodoContentGridView.DataKeys[TodoContentGridView.SelectedIndex].Values[2].ToString();
            modalPopup.Show();
        }
        protected void TodoContentGridView_Sorting(object sender, GridViewSortEventArgs e)
        {
            if (combinedContentItems.Count > 1)
            {
                //Sort the data.
                var prop = typeof(ContentModel).GetProperty(e.SortExpression);
                SortDirection direction = GetSortDirection(e.SortExpression);
                if (direction == SortDirection.Ascending)
                {
                    combinedContentItems = combinedContentItems.OrderBy(ll => prop.GetValue(ll, null)).ToList();
                }
                else
                {
                    combinedContentItems = combinedContentItems.OrderByDescending(ll => prop.GetValue(ll, null)).ToList();
                }
                TodoContentGridView.DataSource = combinedContentItems;
                TodoContentGridView.DataBind();
                int columnIndex = 0;
                foreach (DataControlFieldHeaderCell headerCell in TodoContentGridView.HeaderRow.Cells)
                {
                    if (headerCell.ContainingField.SortExpression == e.SortExpression)
                    {
                        columnIndex = TodoContentGridView.HeaderRow.Cells.GetCellIndex(headerCell);
                    }
                }
                TodoContentGridView.HeaderRow.Cells[columnIndex].Controls.Add(sortImage);
            }
        }
        protected SortDirection GetSortDirection(string column)
        {
            // Default next sort expression behaviour.
            SortDirection nextDir = SortDirection.Ascending;
            sortImage.ImageUrl = ascImagePath;

            if (ViewState["sort"] != null && ViewState["sort"].ToString() == column)
            {   // Exists... DESC.
                nextDir = SortDirection.Descending;
                sortImage.ImageUrl = descImagePath;
                ViewState["sort"] = null;
            }
            else
            {   // Doesn't exists, set ViewState.
                ViewState["sort"] = column;
            }
            return nextDir;
        }
        protected void ProductGridView_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            ProductGridView.DataSource = OrganizationProducts;
            ProductGridView.PageIndex = e.NewPageIndex;
            ProductGridView.DataBind();
            GlobalUtils.SetAccessToControls(this.Page, SaltShakerSession.CurrentRole);
        }
        /// <summary>
        /// Convert RefToDoTypeId to text value; hardcoded
        /// </summary>
        /// <param name="typeId">RefToDoTypeId</param>
        /// <returns>ToDoTypeName value</returns>
        protected string ConvertTypeIdToText(object typeId)
        {
            string idTextValue;
            switch (typeId.ToString())
            {
                case "1":
                    idTextValue = "Content Consumption";
                    break;
                case "2":
                    idTextValue = "User Added";
                    break;
                case "3":
                    idTextValue = "Offline";
                    break;
                case "4":
                    idTextValue = "Admin Added";
                    break;
                default:
                    idTextValue = typeId.ToString();
                    break;
            }
            return idTextValue;
        }
        /// <summary>
        /// Convert RefToDoStatusID to text value; hardcoded
        /// </summary>
        /// <param name="statusId">RefToDoStatusID</param>
        /// <returns>ToDoStatusName value</returns>
        protected string ConvertStatusIdToText(object statusId)
        {
            string idTextValue;
            switch (statusId.ToString())
            {
                case "1":
                    idTextValue = "Added";
                    break;
                case "2":
                    idTextValue = "Complete";
                    break;
                case "3":
                    idTextValue = "Cancelled";
                    break;
                case "4":
                    idTextValue = "In Progress";
                    break;
                default:
                    idTextValue = statusId.ToString();
                    break;
            }

            return idTextValue;
        }

        protected void btnUpdateToDo_Click(object sender, EventArgs e)
        {
            bool isDirtyToDo = (Boolean.Parse(isDirtyHiddenUpdateToDoText.Value) || Boolean.Parse(isDirtyHiddenUpdateToDoDate.Value) || Boolean.Parse(isDirtyHiddenUpdateToDoStatus.Value));
            isDirty = isDirtyToDo;
            bool bSuccess = false;

            if (isDirty)
            {
                if (!String.IsNullOrEmpty(LabelOrgID.Text) && !String.IsNullOrEmpty(SaltShakerSession.UserId))
                {
                    string sToDoHeadlineUpdated = txtUpdateHeadline.Text;
                    string sToDoDateUpdated = txtUpdateDueDate.Text;
                    string sContentId = toDoId.Value;
                    int iToDoTypeId = int.Parse(toDoType.Value);
                    int iToDoStatusId = int.Parse(ddlUpdateToDoStatus.SelectedValue);
                    bSuccess = organization.UpsertOrganizationToDoItem(int.Parse(LabelOrgID.Text.ToString()), sToDoHeadlineUpdated, sToDoDateUpdated, SaltShakerSession.UserId, sContentId, iToDoTypeId, iToDoStatusId);

                    if (bSuccess)
                    {
                        //set button save flag to prevent update with no change
                        isDirty = false;
                        //set html textbox val to allow exit page
                        isDirtyHidden.Value = "false";
                        ShowMessage(sMSG_UPDATE_SUCCESS);

                        //Clear out textbox
                        txtOrgToDo.Text = "";
                        txtToDoDate.Text = "";
                        txtUpdateDueDate.Text = "";
                        txtUpdateHeadline.Text = "";

                        //Keep ToDo tab active
                        TodoContentGridViewPanel.Visible = true;

                        LoadOrganizationDetail(SaltShakerSession.selectedRefOrganizationID);

                        showtabItem(2);
                    }
                }
                else
                {
                    ShowMessage(string.Format(sMSG_WARNING, "your update appears to have failed. Please contact support for assistance"));
                }

                //reset dirty flags
                isDirtyHiddenUpdateToDoText.Value = "false";
                isDirtyHiddenUpdateToDoDate.Value = "false";
                isDirtyHiddenUpdateToDoStatus.Value = "false";
            }

        }

        protected void btnAddOrgToDo_Click(object sender, EventArgs e)
        {

            try
            {

                bool isDirtyToDo = (Boolean.Parse(isDirtyHiddenAddToDoText.Value) || Boolean.Parse(isDirtyHiddenAddToDoDate.Value));
                isDirty = isDirtyToDo;
                bool bSuccess = false;

                if (isDirty)
                {                    
                    if (!String.IsNullOrEmpty(LabelOrgID.Text) && !String.IsNullOrEmpty(SaltShakerSession.UserId))
                    {
                        string sToDoHeadline = txtOrgToDo.Text;
                        string sToDoDate = txtToDoDate.Text;
                        int iToDoTypeId = 3; //Offline to do type
                        int iToDoStatusId = 1; //Added to do type
                        bSuccess = organization.UpsertOrganizationToDoItem(int.Parse(LabelOrgID.Text.ToString()), sToDoHeadline, sToDoDate, SaltShakerSession.UserId, null, iToDoTypeId, iToDoStatusId);

                        if (bSuccess)
                        {
                            //set button save flag to prevent update with no change
                            isDirty = false;
                            //set html textbox val to allow exit page
                            isDirtyHidden.Value = "false";
                            ShowMessage(sMSG_UPDATE_SUCCESS);

                            //Clear out textbox
                            txtOrgToDo.Text = "";
                            txtToDoDate.Text = "";
                            isDirtyHiddenAddToDoText.Value = "false";
                            isDirtyHiddenAddToDoDate.Value = "false";

                            //Keep ToDo tab active
                            TodoContentGridViewPanel.Visible = true;

                            LoadOrganizationDetail(SaltShakerSession.selectedRefOrganizationID);

                            showtabItem(2);
                        }
                    }
                    else
                    {
                        ShowMessage(string.Format(sMSG_WARNING, "your update appears to have failed. Please contact support for assistance"));
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage(string.Format(sMSG_WARNING, "An error occurred when adding a To Do item - " + ex.Message));
            }
        }

        protected void CoursesGridView_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            CoursesGridView.DataSource = OrganizationCourses;
            CoursesGridView.PageIndex = e.NewPageIndex;
            CoursesGridView.DataBind();
            GlobalUtils.SetAccessToControls(this.Page, SaltShakerSession.CurrentRole);
        }
    }
}
