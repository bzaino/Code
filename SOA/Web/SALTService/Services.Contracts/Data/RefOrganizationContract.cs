using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Asa.Salt.Web.Services.Contracts.Data
{
    [DataContract(IsReference = true)]
    [KnownType(typeof(MemberOrganizationContract))]
    [KnownType(typeof(RefOrganizationTypeContract))]
    [KnownType(typeof(RefSALTSchoolTypeContract))]
    [KnownType(typeof(RefStateContract))]
    [KnownType(typeof(RefOrganizationProductContract))]
    [KnownType(typeof(OrganizationToDoListContract))]
    public class RefOrganizationContract : IEntity
    {
         public RefOrganizationContract()
        {
            this.MemberOrganizations = new HashSet<MemberOrganizationContract>();
            this.RefOrganizationProducts = new HashSet<RefOrganizationProductContract>();
            this.RefOrganizationType = new RefOrganizationTypeContract();
            this.OrganizationToDoLists = new HashSet<OrganizationToDoListContract>();
        }

        [DataMember]
        public int RefOrganizationID { get; set; }
        [DataMember]
        public string OrganizationName { get; set; }
        [DataMember]
        public string OrganizationDescription { get; set; }
        [DataMember]
        public bool IsContracted { get; set; }
        [DataMember]
        public string OrganizationExternalID { get; set; }
        [DataMember]
        public string OPECode { get; set; }
        [DataMember]
        public string BranchCode { get; set; }
        [DataMember]
        public string OrganizationLogoName { get; set; }
        [DataMember]
        public string CreatedBy { get; set; }
        [DataMember]
        public System.DateTime CreatedDate { get; set; }
        [DataMember]
        public string ModifiedBy { get; set; }
        [DataMember]
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        [DataMember]
        public string OrganizationAliases { get; set; }
        [DataMember]
        public Nullable<int> RefSALTSchoolTypeID { get; set; }
        [DataMember]
        public bool IsLookupAllowed { get; set; }
        [DataMember]
        public Nullable<int> RefStateID { get; set; }
        [DataMember]
        public int RefOrganizationTypeID { get; set; }

        [DataMember]
        public virtual ICollection<MemberOrganizationContract> MemberOrganizations { get; set; }
        [DataMember]
        public virtual RefOrganizationTypeContract RefOrganizationType { get; set; }
        [DataMember]
        public virtual RefSALTSchoolTypeContract RefSALTSchoolType { get; set; }
        [DataMember]
        public virtual RefStateContract RefState { get; set; }
        [DataMember]
        public virtual ICollection<RefOrganizationProductContract> RefOrganizationProducts { get; set; }
        [DataMember]
        public virtual ICollection<OrganizationToDoListContract> OrganizationToDoLists { get; set; }
    }
}
