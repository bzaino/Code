using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Asa.Salt.Web.Services.Contracts.Data
{
    [DataContract(IsReference = true)]
    [KnownType(typeof(RefRegistrationSourceContract))]
    public class RefChannelContract : IEntity
    {
        public RefChannelContract()
        {
            this.RefRegistrationSources = new HashSet<RefRegistrationSourceContract>();
        }

        [DataMember]
        public int RefChannelID { get; set; }
        [DataMember]
        public string ChannelName { get; set; }
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
