using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SALTShaker.DAL.DataContracts;
using SALTShaker.BLL;
using SALTShaker.DAL;
using System.Web.UI.WebControls;
namespace SALTShaker
{
    public static class GlobalLists
    {
        public static SaltMemberModel SaltMemberModel = new SaltMemberModel();
        public static List<vMemberAcademicInfoModel> ListMemberResults = new List<vMemberAcademicInfoModel>();
        public static List<MemberOrganizationModel> MemberOrganizations = new List<MemberOrganizationModel>();
        public static List<MemberActivityHistoryModel> ListActivityHistory = new List<MemberActivityHistoryModel>();
        public static List<SaltMemberModel> ListSaltMemberModel = new List<SaltMemberModel>();
        public static List<RefRegistrationSourceModel> ListRegistrationSources = new List<RefRegistrationSourceModel>();
        public static List<RefRegistrationSourceTypeModel> ListRegistrationSourceTypes = new List<RefRegistrationSourceTypeModel>();
        public static List<RefCampaignModel> ListRefCampaigns = new List<RefCampaignModel>();
        public static List<RefChannelModel> ListRefChannels = new List<RefChannelModel>();
        public static List<vMemberRoleModel> RefUserRoles = new List<vMemberRoleModel>();
        public static List<vMemberRoleModel> MemberRoles = new List<vMemberRoleModel>();
    }

    //**********************************
    //ProtoType for salt list NOT READY FOR USE
    //*********************************
    //public class SaltList<T> : List<T>
    //{
    //    private List<T> MYList;

    //    public List<T> SALTList<T>(IEnumerable<T> item)
    //    {
    //        List<T> MYList = new List<T>();
    //        MYList = item.ToList();
    //        return MYList;
    //    }

    //    public List<T> List
    //    {
    //        get { return MYList; }
    //        set { this.MYList = value; }
    //    }

    //    public void BindToGrid(GridView Grid)
    //    {
    //        Grid.DataSource = List;
    //        Grid.DataBind();
    //    }
    //}

}
