using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Asa.Salt.Web.Services.Contracts.Data
{
    [DataContract(IsReference = true)]
    [KnownType(typeof(MemberContract))]
    [KnownType(typeof(RefRegistrationSourceTypeContract))]
    [KnownType(typeof(RefCampaignContract))]
    [KnownType(typeof(RefChannelContract))]
    public class RefRegistrationSourceContract : IEntity
    {
        public RefRegistrationSourceContract()
        {
            this.Members = new HashSet<MemberContract>();
        }

        [DataMember]
        public int RefRegistrationSourceId { get; set; }
        [DataMember]
        public string RegistrationSourceName { get; set; }
        [DataMember]
        public string RegistrationDetail { get; set; }
        [DataMember]
        public string CreatedBy { get; set; }
        [DataMember]
        public System.DateTime CreatedDate { get; set; }
        [DataMember]
        public string ModifiedBy { get; set; }
        [DataMember]
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        [DataMember]
        public Nullable<int> RefRegistrationSourceTypeId { get; set; }
        [DataMember]
        public Nullable<int> RefCampaignId { get; set; }
        [DataMember]
        public Nullable<int> RefChannelId { get; set; }

        [DataMember]
        public virtual ICollection<MemberContract> Members { get; set; }
        [DataMember]
        public virtual RefRegistrationSourceTypeContract RefRegistrationSourceType { get; set; }
        [DataMember]
        public virtual RefCampaignContract RefCampaign { get; set; }
        [DataMember]
        public virtual RefChannelContract RefChannel { get; set; }
    }

}
