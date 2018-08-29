using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Asa.Salt.Web.Services.Contracts.Data
{
    [DataContract(IsReference = true)]
    [KnownType(typeof(RefProductTypeContract))]
    public class RefProductTypeContract : IEntity
    {
        public RefProductTypeContract()
        {
            this.RefProducts = new HashSet<RefProductTypeContract>();
        }

        [DataMember]
        public int RefProductTypeID { get; set; }
        [DataMember]
        public string ProductTypeName { get; set; }
        [DataMember]
        public string CreatedBy { get; set; }
        [DataMember]
        public System.DateTime CreatedDate { get; set; }
        [DataMember]
        public string ModifiedBy { get; set; }
        [DataMember]
        public Nullable<System.DateTime> ModifiedDate { get; set; }

        [DataMember]
        public virtual ICollection<RefProductTypeContract> RefProducts { get; set; }
    }
}
