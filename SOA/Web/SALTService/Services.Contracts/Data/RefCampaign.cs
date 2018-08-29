using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Asa.Salt.Web.Services.Contracts.Data
{
    [DataContract(IsReference = true)]
    [KnownType(typeof(RefRegistrationSourceContract))]
    public class RefCampaignContract : IEntity
    {
        public RefCampaignContract()
        {
            this.RefRegistrationSources = new HashSet<RefRegistrationSourceContract>();
        }

        [DataMember]
        public int RefCampaignID { get; set; }
        [DataMember]
        public string CampaignName { get; set; }
        [DataMember]
        public string CreatedBy { get; set; }
        [DataMember]
        public System.DateTime CreatedDate { get; set; }
        [DataMember]
        public string ModifiedBy { get; set; }
        [DataMember]
        public Nullable<System.DateTime> ModifiedDate { get; set; }

        [DataMember]
        public virtual ICollection<RefRegistrationSourceContract> RefRegistrationSources { get; set; }
    }

}
