using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using ASA.Web.Services.Common;

namespace ASA.Web.Services.ASAMemberService.DataContracts
{
    public class MemberSchoolModel : BaseWebModel
    {
        public MemberSchoolModel() : base() { }
        public MemberSchoolModel(Boolean newRecord = false) : base(newRecord) { }

        public bool IsPrimary { get; set; }

        public string SchoolId { get; set; }

        [DisplayName("School Name")]
        public string SchoolName { get; set; }

        [Required]
        [StringLength(6, ErrorMessage = "The {0} must be {1} characters long.", MinimumLength = 6)]
        public string OECode { get; set; }

        [Required]
        [StringLength(2, ErrorMessage = "The {0} must be {1} characters long.", MinimumLength = 2)]
        public string BranchCode { get; set; }

        public string Brand { get; set; }
        public Nullable<int> SchoolType { get; set; }
    }

    public class MemberOrganizationModel : BaseWebModel
    {
        public MemberOrganizationModel() : base() { }
        public MemberOrganizationModel(Boolean newRecord = false) : base(newRecord) { }
        
        public int MemberId { get; set; }

        public int OrganizationId { get; set; }

        public string OrganizationExternalID { get; set; }

        public Nullable<int> ExpectedGraduationYear { get; set; }
        public string ReportingId { get; set; }

        public DateTime EffectiveStartDate { get; set; }

        public Nullable<DateTime> EffectiveEndDate { get; set; }

        public bool IsPrimary { get; set; }

        public string Brand { get; set; }

        public Nullable<int> SchoolType { get; set; }

        /// <summary>
        /// A flag for member's org unaffiliation action
        /// </summary>
        public bool IsOrganizationDeleted { get; set; }

        //RefOrganization Data
        public string OrganizationName { get; set; }

        public bool IsContracted { get; set; }

        public string OECode { get; set; }

        public string BranchCode { get; set; }

        public string OrganizationLogoName { get; set; }

        public string OrganizationAliases { get; set; }

        public int? RefSALTSchoolTypeID { get; set; }

        public bool IsLookupAllowed { get; set; }

        public int RefOrganizationTypeId { get; set; }

        public string OrganizationTypeExternalId { get; set; }
    }
}
