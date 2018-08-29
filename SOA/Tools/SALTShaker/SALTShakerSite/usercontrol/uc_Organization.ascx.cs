using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using SALTShaker.BLL;
using SALTShaker.DAL;
using SALTShaker.DAL.DataContracts;
using System.Text.RegularExpressions;
using SALTShaker.HelperClass;

public partial class usercontrol_uc_Organization : System.Web.UI.UserControl
{

    static List<OrganizationModel> ListResults = new List<OrganizationModel>();
    string selectedRefOrganizationID;
    int iGridIndex = 0;
    int iRecordCount = 0;
    int TabSeachIndx = 1;

    //Regex
    static string idPattern = @"^$|^[0-9]+$";
    static string externalIdPattern = @"^$|^([a-zA-Z0-9]){1,25}$";
    static string organizationPattern = @"^$|^[a-zA-Z\)\(\@\,\&\.\'\’\-\–\s\/]+$";
    //Catch Exception errors
    const string sMSG_WARNING = "Oops that was unexpected: {0}";
    const string sMSG_NOORGANIZATIONFOUND = "No records matched your search. Please try again.";
    const string sMSG_NOPARAMATER_ERROR = "Please at least enter one of the below fields...";
    const string sMSG_INVALID_ENTRY = "Please enter valid info!";
    const string sMSG_SELECT_FIRST = "Please select first!";
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
            if (SaltShakerSession.UserId == null)
            {
                Response.Redirect("Index.aspx");
            }
        }
    }

    private void renderGridView()
    {
        renderGridViewHelper(null, null, null, null, null);
    }

    private void renderGridView(string id, string organizationName, string isContracted, string opeCode, string branchCode)
    {
        try
        {
            HideMessage();
            if (String.IsNullOrEmpty(id))
                renderGridViewHelper(null, organizationName, isContracted, opeCode, branchCode);
            else
                renderGridViewHelper(id, organizationName, isContracted, opeCode, branchCode);
        }
        catch (FormatException)
        {
            ShowMessage("invalid organization ref id please try another id or select display all");
        }
        catch (OverflowException)
        {
            ShowMessage("The organization ref id has too many digits please try another id or select display all");
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message);
        }

    }

    private void renderGridViewHelper(string id, string organizationName = "", string isContracted = "", string opeCode = "", string branchCode = "")
    {
        using (OrganizationBL Organization = new OrganizationBL())
        {
            Nullable<bool> bContracted = new bool();
            bContracted = null;
            if (!String.IsNullOrEmpty(isContracted))
            {
                //if user selected a value than update bool otherwise let it go as null
                if (isContracted.ToLower().IndexOf("please select") < 0)
                    bContracted = (isContracted == "True") ? true : false;
            }
            ListResults = Organization.GetOrganizationsBySearchParms(id, bContracted, organizationName, opeCode, branchCode).ToList();
            iRecordCount = ListResults.Count();
            if (ListResults.Count() > 0)
            {
                if (iRecordCount == 1)
                {
                    OrganizationGridView.DataSource = ListResults;
                    OrganizationGridView.DataBind();
                    OrganizationGridView.SelectedIndex = 0; //select the only existing row
                    this.OrganizationRow_Click();
                }
                else
                {
                    _sortDirection = "ASC";
                    sortExpression = "OrganizationName";
                    this.SortHelper("OrganizationName", 0);
                }

            }
            else
            {
                ShowMessage(sMSG_NOORGANIZATIONFOUND);
            }
        }
    }
    protected void OrganizationGridView_Sorting(object sender, GridViewSortEventArgs e)
    {
        sortExpression = e.SortExpression;
        SaltShakerSession.OrganizationSortExpression = sortExpression;
        SetSortDirection(SortDireaction);
        this.SortHelper(sortExpression, 0);
    }

    protected void SortHelper(string sortExpression, int pageIndex)
    {

        if (ListResults.Count >= 1)
        {
            List<OrganizationModel> sortedOrganization;
            //Sort if possible
            if (!string.IsNullOrEmpty(sortExpression))
            {
                //Sort the data. SWD-6936
                var prop = typeof(OrganizationModel).GetProperty(sortExpression);
                if (_sortDirection == "ASC")
                {
                    sortedOrganization = ListResults.OrderBy(lr => prop.GetValue(lr, null)).ToList();
                    sortImage.ImageUrl = descImagePath;
                    //set listResults to new sorted list
                    ListResults = sortedOrganization;
                }
                else
                {
                    sortedOrganization = ListResults.OrderByDescending(lr => prop.GetValue(lr, null)).ToList();
                    sortImage.ImageUrl = ascImagePath;
                    //set listResults to new sorted list
                    ListResults = sortedOrganization;
                }
            }
            else
            {
                //use the previous list
                sortedOrganization = ListResults;
            }
            OrganizationGridView.DataSource = sortedOrganization;
            OrganizationGridView.PageIndex = pageIndex;
            OrganizationGridView.DataBind();
            SortDireaction = _sortDirection;
            int columnIndex = 0;
            foreach (DataControlFieldHeaderCell headerCell in OrganizationGridView.HeaderRow.Cells)
            {
                if (headerCell.ContainingField.SortExpression == sortExpression)
                {
                    columnIndex = OrganizationGridView.HeaderRow.Cells.GetCellIndex(headerCell);
                }
            }

            OrganizationGridView.HeaderRow.Cells[columnIndex].Controls.Add(sortImage);
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
            //Sort the data. SWD-6936
            sortImage.ImageUrl = descImagePath;
        }
        else
        {
            _sortDirection = "ASC";
            //Sort the data. SWD-6936
            sortImage.ImageUrl = ascImagePath;
        }
    }

    protected void tabMenu1_MenuItem_Click(object sender, MenuEventArgs e)
    {
        showtabItem(Int32.Parse(e.Item.Value));
    }

    private void showtabItem(int index)
    {
        MultiView c = (MultiView)this.FindControl("MultiView1");
        c.ActiveViewIndex = index;
        if (index == TabSeachIndx)
        {
            clearSearch();
        }
    }

    private void displayRowCount()
    {
        if (iRecordCount != 0)
        {
            RecordNoLabel.Visible = true;
            RecordNoLabel.Text = "Row Count: " + iRecordCount;
        }
    }

    private void clearSearch()
    {
        TextBoxBranchCode.Text = "";
        TextBoxOpeCode.Text = "";
        TextBoxOrganizationName.Text = "";
        TextBoxRefOrganizationID.Text = "";
        DropDownListContracted.SelectedIndex = 0;
    }

    private void ShowSearchResults()
    {
        if (((MultiView)this.FindControl("MultiView1")).ActiveViewIndex != TabSeachIndx)
        {
            //show search if not visible
            showtabItem(TabSeachIndx);
        }
        if (TextBoxRefOrganizationID.Text.Trim().Length == 0 && TextBoxOrganizationName.Text.Trim().Length == 0 && DropDownListContracted.SelectedValue == "Please Select" && TextBoxOpeCode.Text.Trim().Length == 0 && TextBoxBranchCode.Text.Trim().Length == 0)
        {
            ShowMessage(sMSG_NOPARAMATER_ERROR);
        }
        else
        {

            HideMessage();
            string organizationID = TextBoxRefOrganizationID.Text;
            string organizationName = TextBoxOrganizationName.Text;
            Regex trimmer = new Regex(@"\s\s+");
            organizationName = trimmer.Replace(organizationName, " ").Trim();
            string isContracted = DropDownListContracted.SelectedValue;
            string opecode = TextBoxOpeCode.Text;
            string branchCode = TextBoxBranchCode.Text;
            //ds = new DataSet();
            if (System.Text.RegularExpressions.Regex.IsMatch(organizationID, externalIdPattern) && System.Text.RegularExpressions.Regex.IsMatch(organizationName, organizationPattern) && System.Text.RegularExpressions.Regex.IsMatch(opecode, idPattern) && System.Text.RegularExpressions.Regex.IsMatch(branchCode, idPattern))
            {
                this.renderGridView(organizationID, organizationName, isContracted, opecode, branchCode);
                this.displayRowCount();
                if (iRecordCount > 0)
                {
                    //if no valid results display grid
                    showtabItem(iGridIndex);
                }
                else
                {
                    ShowMessage(sMSG_NOORGANIZATIONFOUND);
                }
                clearSearch();
                //show/hide button that match need functionality if not visible
                RowRefOrganizationID.Visible = true;
            }
            else
            {
                ShowMessage(sMSG_INVALID_ENTRY);
            }
        }
        this.hideSpinner();
    }
    protected void OrganizationGridView_SelectedIndexChanged(object sender, EventArgs e)
    {
        OrganizationRow_Click();
    }
    protected void OrganizationGridView_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        if (ListResults.Count() > 0)
        {
            if (string.IsNullOrEmpty(_sortDirection))
            {
                OrganizationGridView.DataSource = ListResults; // ds.Tables[0];
                OrganizationGridView.PageIndex = e.NewPageIndex;
                OrganizationGridView.DataBind();
            }
            else
            {
                //Sort the data. SWD-6936
                if (string.IsNullOrEmpty(sortExpression) && SaltShakerSession.OrganizationSortExpression != null)
                {
                    sortExpression = SaltShakerSession.OrganizationSortExpression;
                }
                this.SortHelper(sortExpression, e.NewPageIndex);
            }
        }
    }

    protected void SearchButton_Click(object sender, EventArgs e)
    {
        ShowSearchResults();
    }

    protected void OrganizationGridView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            ///Add this javascript to the ondblclick Attribute of the row
            //e.Row.Attributes.Add("ondblclick", "CreateTrigger();");
            e.Row.Attributes.Add("onclick", Page.ClientScript.GetPostBackEventReference((Control)sender, "Select$" + e.Row.RowIndex.ToString()));
        }
    }


    private void ShowMessage(string sMessage)
    {
        this.warningMessageControl.ShowMessage(sMessage);
        //LabelRole.Text = sUpdatedRole;  Show message should be just showing the message, why updating the role text EX: what if update role is failed, we don't want to change the text then, correct?
    }

    private void HideMessage()
    {
        this.warningMessageControl.HideMessage();
    }

    protected void ClearSearch_Click(object sender, EventArgs e)
    {
        clearSearch();
    }

    private void hideSpinner()
    {
        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "click", "javascript:DBTool.Spinner.hide();", true);
    }

    protected void OrganizationRow_Click()
    {
        selectedRefOrganizationID = OrganizationGridView.SelectedDataKey.Value.ToString();
        int TabActiveIndex = ((MultiView)this.FindControl("MultiView1")).ActiveViewIndex;

        if (selectedRefOrganizationID != null && string.Empty != selectedRefOrganizationID)
        {
            Response.Redirect("OrganizationDetail.aspx?organizationID=" + selectedRefOrganizationID);
        }
        else
        {
            ShowMessage(sMSG_SELECT_FIRST);
        }
    }
}
