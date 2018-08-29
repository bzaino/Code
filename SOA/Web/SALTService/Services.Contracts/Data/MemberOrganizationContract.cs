using System;
using System.Runtime.Serialization;

namespace Asa.Salt.Web.Services.Contracts.Data
{
    [DataContract(IsReference = true)]
    [KnownType(typeof(MemberContract))]
    [KnownType(typeof(RefOrganizationContract))]
    public class MemberOrganizationContract : IEntity
    {
        [DataMember]
        public int MemberOrganizationID { get; set; }
        [DataMember]
        public int MemberID { get; set; }
        [DataMember]
        public int RefOrganizationID { get; set; }
        [DataMember]
        public Nullable<int> ExpectedGraduationYear { get; set; }
        [DataMember]
        public System.DateTime EffectiveStartDate { get; set; }
        [DataMember]
        public Nullable<System.DateTime> EffectiveEndDate { get; set; }
        [DataMember]
        public string CreatedBy { get; set; }
        [DataMember]
        public System.DateTime CreatedDate { get; set; }
        [DataMember]
        public string ModifiedBy { get; set; }
        [DataMember]
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        [DataMember]
        public bool IsOrganizationDeleted { get; set; }
        [DataMember]
        public string SchoolReportingID { get; set; }

        //data from RefOrganization table
        [DataMember]
        public int OrganizationId { get; set; }

        [DataMember]
        public string OrganizationExternalID { get; set; }

        [DataMember]
        public string OrganizationName { get; set; }

        [DataMember]
        public bool IsContracted { get; set; }

        [DataMember]
        public string OECode { get; set; }

        [DataMember]
        public string BranchCode { get; set; }

        [DataMember]
        public string OrganizationLogoName { get; set; }

        [DataMember]
        public string OrganizationAliases { get; set; }

        [DataMember]
        public int? RefSALTSchoolTypeID { get; set; }

        [DataMember]
        public bool IsLookupAllowed { get; set; }

        [DataMember]
        public int RefOrganizationTypeID { get; set; }

        [DataMember]
        public string OrganizationTypeExternalID { get; set; }

        public virtual MemberContract Member { get; set; }
        public virtual RefOrganizationContract RefOrganization { get; set; }
    }
}