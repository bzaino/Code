using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SALTShaker.DAL.DataContracts
{
    public class RefRegistrationSourceModel
    {
        public int RegistrationSourceId { get; set; }
        public string RegistrationSourceName { get; set; }
        public string RegistrationDetail { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public Nullable<int> RefRegistrationSourceTypeId { get; set; }
        public Nullable<int> RefCampaignId { get; set; }
        public string CampaignName { get; set; }
        public Nullable<int> RefChannelId { get; set; }
        public string ChannelName { get; set; }
    }

    public class RefRegistrationSourceTypeModel
    {
        public int RefRegistrationSourceTypeId { get; set; }
        public string RegistrationSourceTypeName { get; set; }
        public string RegistrationSourceTypeDescription { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
    }

    public class RefCampaignModel
    {
        public int RefCampaignId { get; set; }
        public string CampaignName { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
    }

    public class RefChannelModel
    {
        public int RefChannelId { get; set; }
        public string ChannelName { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
    }

    //A dual purpose model for Ref and Member Roles
    public class vMemberRoleModel
    {
        public int RefMemberRoleID { get; set; }
        public string RoleName { get; set; }
        public string RoleDescription { get; set; }
        public bool IsMemberRoleActive { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
    }
}
