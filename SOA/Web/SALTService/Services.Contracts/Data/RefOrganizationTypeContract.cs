using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Asa.Salt.Web.Services.Contracts.Data
{
    [DataContract(IsReference = true)]
    [KnownType(typeof(RefOrganizationContract))]
    public class RefOrganizationTypeContract : IEntity
    {
        public RefOrganizationTypeContract()
        {
            this.RefOrganizations = new HashSet<RefOrganizationContract>();
        }

        [DataMember]
        public int RefOrganizationTypeID { get; set; }
        [DataMember]
        public string OrganizationTypeName { get; set; }
        [DataMember]
        public string OrganizationTypeDescription { get; set; }
        [DataMember]
        public string OrganizationTypeExternalID { get; set; }
        [DataMember]
        public string CreatedBy { get; set; }
        [DataMember]
        public System.DateTime CreatedDate { get; set; }
        [DataMember]
        public string ModifiedBy { get; set; }
        [DataMember]
        public Nullable<System.DateTime> ModifiedDate { get; set; }

        [DataMember]
        public virtual ICollection<RefOrganizationContract> RefOrganizations { get; set; }
    }
}
