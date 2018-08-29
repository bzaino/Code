using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SALTShaker.BLL;
using SALTShaker.DAL.DataContracts;
using SALTShaker.HelperClass;

namespace SALTShaker.usercontrol
{
    public partial class uc_RegistrationSources : System.Web.UI.UserControl
    {
        SaltRefDataRepository saltRefDataRepo = new SaltRefDataRepository();
        private static string regSourceCreatedBy;
        private static string selectedRegSourceTypeName;
        private static Nullable<int> selectedRegSourceId;
        private static Nullable<int> selectedRegSourceTypeId;
        private static string selectedChannelName;
        private static Nullable<int> selectedRefChannelId;
        private static string selectedCampaignName;
        private static Nullable<int> selectedRefCampaignId;
        private static Nullable<int> selectedRefCampaignId_upsert;
        private static string refCampaignCreatedBy;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (String.IsNullOrEmpty(SaltShakerSession.UserId.ToString()))
                {
                    Response.Redirect("Index.aspx");
                }
                LoadAndBindRegSources(true);
                LoadAndBindRegSourceTypes(true);
                LoadAndBindRefCampaigns(true);
                LoadAndBindRefChannels(true);
            }
            else
            {
                LoadAndBindRegSources(false);
                HideMessage();
            }            
        }

        protected void RegistrationSourceGridView_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            clearRegistrationSourceControls();
            RegistrationSourceGridView.PageIndex = e.NewPageIndex;
            RegistrationSourceGridView.DataBind();
        }       

        private void clearRegistrationSourceControls()
        {
            txtRegSourceName.Text = "";
            txtRegSourceDetail.Text = "";
            regSourceCreatedBy = "";
            selectedRegSourceId = null;
            selectedRegSourceTypeId = null;
            selectedRefCampaignId = null;
            selectedRefCampaignId_upsert = null;
            selectedRefChannelId = null;
            refCampaignCreatedBy = "";
            selectedCampaignName = "";
            txtCampaignName.Text = "";

            RegistrationSourceGridView.SelectedIndex = -1;
            DDLRegSourceType.SelectedIndex = 0;
            DDLRefCampaign.SelectedIndex = 0;
            DDLRefCampaign_Upsert.SelectedIndex = 0;
            DDLRefChannel.SelectedIndex = 0;
            CmdUpdate.Visible = false;
            CmdSubmit.Visible = true;
        }

        private void ShowMessage(string sMessage)
        {
            this.warningMessageControl.ShowMessage(sMessage);
        }

        private void HideMessage()
        {
            this.warningMessageControl.HideMessage();
        }

        private bool isValidRegistrationSource()
        {
 	        bool valid = false;

            if (selectedRegSourceTypeId != null && DDLRegSourceType.SelectedValue != "NA" && txtRegSourceName.Text != "")
            {
                valid = true;
            }

            return valid;
        }

        private bool isValidRefCampaign()
        {
            bool valid = false;

            if (selectedRefCampaignId != null && DDLRefCampaign.SelectedValue != "NA")
            {
                valid = true;
            }

            return valid;
        }

        private bool isValidRefCampaign_Upsert_Add()
        {
            bool valid = false;

            if (selectedRefCampaignId_upsert == null && DDLRefCampaign_Upsert.SelectedValue == "NA")
            {
                valid = true;
            }

            return valid;
        }

        private bool isValidRefCampaign_Upsert_Update()
        {
            bool valid = false;

            if (selectedRefCampaignId_upsert != null && DDLRefCampaign_Upsert.SelectedValue != "NA")
            {
                valid = true;
            }

            return valid;
        }

        private bool isValidCampaignName_Upsert()
        {
            bool valid = false;

            if (!string.IsNullOrWhiteSpace(txtCampaignName.Text))
            {
                valid = true;
            }

            return valid;
        }
        
        private bool isValidRefChannel()
        {
            bool valid = false;

            if (selectedRefChannelId != null && DDLRefChannel.SelectedValue != "NA")
            {
                valid = true;
            }

            return valid;
        }

        protected void cmdAddUpdateRefCampaign_Click(object sender, EventArgs e)
        {            
            clearRegistrationSourceControls();            
            NewRefCampaignPanel.Visible = true;
            NewRegSourcePanel.Visible = false;
        }

        protected void cmdAddRegSource_Click(object sender, EventArgs e)
        {
            clearRegistrationSourceControls();
            NewRegSourcePanel.Visible = true;
            NewRefCampaignPanel.Visible = false;
        }

        private void LoadAndBindRegSources(bool reloadFromService)
        {
            if (reloadFromService)
            {

                GlobalLists.ListRegistrationSources = saltRefDataRepo.GetAllRefRegistrationSources().ToList();
            }
            RegistrationSourceGridView.DataSource = GlobalLists.ListRegistrationSources;
            RegistrationSourceGridView.DataBind();
        }

        private void LoadAndBindRegSourceTypes(bool reloadFromService)
        {
            if (reloadFromService)
            {

                GlobalLists.ListRegistrationSourceTypes = saltRefDataRepo.GetAllRefRegistrationSourceTypes().ToList();
            }
            
            List<String> regSourceTypeNames = new List<String>();
            foreach(var elem in GlobalLists.ListRegistrationSourceTypes)
            {
                regSourceTypeNames.Add(elem.RegistrationSourceTypeName);
            }
            DDLRegSourceType.DataSource = regSourceTypeNames;
            DDLRegSourceType.DataBind();
            DDLRegSourceType.Items.Insert(0, new ListItem("Please Select","NA"));
        }

        private void LoadAndBindRefCampaigns(bool reloadFromService)
        {
            if (reloadFromService)
            {

                GlobalLists.ListRefCampaigns = saltRefDataRepo.GetAllRefCampaigns().ToList();
            }

            List<String> refCampaignNames = new List<String>();
            foreach (var elem in GlobalLists.ListRefCampaigns)
            {
                refCampaignNames.Add(elem.CampaignName);
            }
            DDLRefCampaign.DataSource = refCampaignNames;
            DDLRefCampaign.DataBind();
            DDLRefCampaign.Items.Insert(0, new ListItem("Please Select", "NA"));

            //on the RefCampaign add/update tab
            DDLRefCampaign_Upsert.DataSource = refCampaignNames;
            DDLRefCampaign_Upsert.DataBind();
            DDLRefCampaign_Upsert.Items.Insert(0, new ListItem("Please Select", "NA"));
        }

        private void LoadAndBindRefChannels(bool reloadFromService)
        {
            if (reloadFromService)
            {

                GlobalLists.ListRefChannels = saltRefDataRepo.GetAllRefChannels().ToList();
            }

            List<String> refChannelNames = new List<String>();
            foreach (var elem in GlobalLists.ListRefChannels)
            {
                refChannelNames.Add(elem.ChannelName);
            }
            DDLRefChannel.DataSource = refChannelNames;
            DDLRefChannel.DataBind();
            DDLRefChannel.Items.Insert(0, new ListItem("Please Select", "NA"));
        }

        protected void DDLRegSourceType_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach (var elem in GlobalLists.ListRegistrationSourceTypes)
            {
                if (elem.RegistrationSourceTypeName == DDLRegSourceType.SelectedValue)
                {
                    selectedRegSourceTypeName = elem.RegistrationSourceTypeName;
                    selectedRegSourceTypeId = elem.RefRegistrationSourceTypeId;
                    break;
                }
            }
        }

        protected void DDLRefCampaign_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach (var elem in GlobalLists.ListRefCampaigns)
            {
                if (elem.CampaignName == DDLRefCampaign.SelectedValue)
                {
                    selectedCampaignName = elem.CampaignName;
                    selectedRefCampaignId = elem.RefCampaignId;
                    break;
                }
            }
        }

        protected void DDLRefCampaign_Upsert_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach (var elem in GlobalLists.ListRefCampaigns)
            {
                if (elem.CampaignName == DDLRefCampaign_Upsert.SelectedValue)
                {
                    txtCampaignName.Text = elem.CampaignName;
                    selectedRefCampaignId_upsert = elem.RefCampaignId;
                    refCampaignCreatedBy = elem.CreatedBy;
                    break;
                }
            }
        }

        protected void DDLRefChannel_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach (var elem in GlobalLists.ListRefChannels)
            {
                if (elem.ChannelName == DDLRefChannel.SelectedValue)
                {
                    selectedChannelName = elem.ChannelName;
                    selectedRefChannelId = elem.RefChannelId;
                    break;
                }
            }
        }

        protected void RegistrationSourceGridView_SelectedIndexChanging(object sender, GridViewSelectEventArgs e)
        {
            GridViewRow row = RegistrationSourceGridView.Rows[e.NewSelectedIndex];
            String id = row.Cells[4].Text;
            if (id == "")
            {
                selectedRegSourceId = null;
            }
            else
            {
                selectedRegSourceId = Int32.Parse(id);
            }
            txtRegSourceName.Text = HttpUtility.HtmlDecode(row.Cells[3].Text);
            txtRegSourceDetail.Text = HttpUtility.HtmlDecode(row.Cells[5].Text);
            
            //registration Source Type
            foreach (var item in GlobalLists.ListRegistrationSourceTypes)
            {
                if (item.RefRegistrationSourceTypeId == Int32.Parse(row.Cells[12].Text))
                {
                    selectedRegSourceTypeName = HttpUtility.HtmlDecode(item.RegistrationSourceTypeName);
                    selectedRegSourceTypeId = item.RefRegistrationSourceTypeId;
                    break;
                }
            }
            DDLRegSourceType.SelectedValue = selectedRegSourceTypeName;

            //Campaign
            foreach (var item in GlobalLists.ListRefCampaigns)
            {
                if (item.RefCampaignId == Int32.Parse(row.Cells[11].Text))
                {
                    selectedCampaignName = HttpUtility.HtmlDecode(item.CampaignName);
                    selectedRefCampaignId = item.RefCampaignId;
                    break;
                }
            }
            DDLRefCampaign.SelectedValue = selectedCampaignName;

            //Channel
            foreach (var item in GlobalLists.ListRefChannels)
            {
                if (item.RefChannelId == Int32.Parse(row.Cells[10].Text))
                {
                    selectedChannelName = HttpUtility.HtmlDecode(item.ChannelName);
                    selectedRefChannelId = item.RefChannelId;
                    break;
                }
            }
            DDLRefChannel.SelectedValue = selectedChannelName;

            regSourceCreatedBy = row.Cells[6].Text;

            NewRegSourcePanel.Visible = true;
            NewRefCampaignPanel.Visible = false;
            CmdSubmit.Visible = false;
            CmdUpdate.Visible = true;
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string saltShakerUserName = string.Format("AMSA\\{0}", SaltShakerSession.UserId);
            //add new RefRegistrationSource
            if (isValidRegistrationSource() &&
                isValidRefCampaign() &&
                isValidRefChannel())
            {
                bool bSuccess = saltRefDataRepo.AddRefRegistrationSource(txtRegSourceName.Text, txtRegSourceDetail.Text, (int)selectedRegSourceTypeId, saltShakerUserName,(int)selectedRefCampaignId, (int)selectedRefChannelId);
                if (bSuccess)
                {
                    ShowMessage(GlobalMessages.sMSG_REGSOURCE_CREATE_SUCCESS);
                    LoadAndBindRegSources(true);
                    LoadAndBindRegSourceTypes(false);
                    LoadAndBindRefCampaigns(false);
                    LoadAndBindRefChannels(false);
                    clearRegistrationSourceControls();
                }
                else
                {
                    ShowMessage(string.Format(GlobalMessages.sMSG_WARNING, "Adding new registration source appears to have failed. Please try again later or contact support for assistance"));
                }
            }
            else
            {
                ShowMessage(string.Format(GlobalMessages.sMSG_WARNING, "Invalid inputs, please check and try again"));
            }
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            string saltShakerUserName = string.Format("AMSA\\{0}", SaltShakerSession.UserId);
            //update existing RefRegistrationSource
            if (isValidRegistrationSource() &&
                isValidRefCampaign() &&
                isValidRefChannel())
            {
                bool bSuccess = saltRefDataRepo.UpdateRefRegistrationSource((int)selectedRegSourceId, txtRegSourceName.Text, txtRegSourceDetail.Text, (int)selectedRegSourceTypeId, regSourceCreatedBy, saltShakerUserName, (int)selectedRefCampaignId, (int)selectedRefChannelId);
                if (bSuccess)
                {                    
                    ShowMessage(GlobalMessages.sMSG_REGSOURCE_UPDATE_SUCCESS);
                    LoadAndBindRegSources(true);
                    LoadAndBindRegSourceTypes(false);
                    LoadAndBindRefCampaigns(false);
                    LoadAndBindRefChannels(false);
                    clearRegistrationSourceControls();
                }
                else
                {
                    ShowMessage(string.Format(GlobalMessages.sMSG_WARNING, "Updating registration source appears to have failed. Please try again later or contact support for assistance"));
                }
            }
            else
            {
                ShowMessage(string.Format(GlobalMessages.sMSG_WARNING, "Invalid inputs, please check and try again"));
            }       
        }
        protected void btnSubmit_Campaign_Click(object sender, EventArgs e)
        {
            string saltShakerUserName = string.Format("AMSA\\{0}", SaltShakerSession.UserId);
            //add new RefCampaign
            if (isValidRefCampaign_Upsert_Add() &&
                isValidCampaignName_Upsert())
            {
                HideMessage();
                bool bSuccess = saltRefDataRepo.AddRefCampaign(txtCampaignName.Text, saltShakerUserName);
                if (bSuccess)
                {
                    ShowMessage(GlobalMessages.sMSG_REFCAMPAIGN_CREATE_SUCCESS);
                    LoadAndBindRegSources(true);
                    LoadAndBindRefCampaigns(true);
                    clearRegistrationSourceControls();
                }
                else
                {
                    ShowMessage(string.Format(GlobalMessages.sMSG_WARNING, "Adding new campaign appears to have failed. Please try again later or contact support for assistance"));
                }
            }
            else
            {
                ShowMessage(string.Format(GlobalMessages.sMSG_WARNING, "Invalid inputs, please check and try again"));
            }
        }

        protected void btnUpdate_Campaign_Click(object sender, EventArgs e)
        {
            string saltShakerUserName = string.Format("AMSA\\{0}", SaltShakerSession.UserId);
            //update existing RefCampaign
            if (isValidRefCampaign_Upsert_Update() &&
                isValidCampaignName_Upsert())
            {
                HideMessage();
                bool bSuccess = saltRefDataRepo.UpdateRefCampaign((int)selectedRefCampaignId_upsert, txtCampaignName.Text, refCampaignCreatedBy, saltShakerUserName);
                if (bSuccess)
                {
                    ShowMessage(GlobalMessages.sMSG_REFCAMPAIGN_UPDATE_SUCCESS);
                    LoadAndBindRegSources(true);
                    LoadAndBindRefCampaigns(true);
                    clearRegistrationSourceControls();
                }
                else
                {
                    ShowMessage(string.Format(GlobalMessages.sMSG_WARNING, "Updating campaign appears to have failed. Please try again later or contact support for assistance"));
                }
            }
            else
            {
                ShowMessage(string.Format(GlobalMessages.sMSG_WARNING, "Invalid inputs, please check and try again"));
            }
        }
    }
}