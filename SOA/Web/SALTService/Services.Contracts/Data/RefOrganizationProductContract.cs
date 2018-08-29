using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Asa.Salt.Web.Services.Contracts.Data
{
    [DataContract(IsReference = true)]
    [KnownType(typeof(RefProductContract))]
    [KnownType(typeof(RefOrganizationContract))]
    public class RefOrganizationProductContract : IEntity
    {
        [DataMember]
        public int RefOrganizationProductID { get; set; }
        [DataMember]
        public int RefOrganizationID { get; set; }
        [DataMember]
        public int RefProductID { get; set; }
        [DataMember]
        public bool IsRefOrganizationProductActive { get; set; }
        [DataMember]
        public string CreatedBy { get; set; }
        [DataMember]
        public System.DateTime CreatedDate { get; set; }
        [DataMember]
        public string ModifiedBy { get; set; }
        [DataMember]
        public Nullable<System.DateTime> ModifiedDate { get; set; }

        [DataMember]
        public virtual RefProductContract RefProduct { get; set; }
        [DataMember]
        public virtual RefOrganizationContract RefOrganization { get; set; }
    }
}
