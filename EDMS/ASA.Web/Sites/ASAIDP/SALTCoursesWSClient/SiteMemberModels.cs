using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SALTCoursesWSClient
{
    public class SiteMemberModel
    {
        public string MembershipId { get; set; }

        public string ExternalId { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DisplayName { get; set; }
        public DateTime? DOB { get; set; }
        public string EnrollmentStatus { get; set; }

        public DateTime? EnrollmentStatusEffective { get; set; }

        public string GradeLevel { get; set; }
        public DateTime? WithdrawalDate { get; set; }

        public string MemberToken { get; set; }
        public string PrimaryEmailKey { get; set; }
        public string PrimaryOrganizationKey { get; set; }
        public string OrganizationIdForCourses { get; set; }

        private List<MemberEmailModel> _emails = new List<MemberEmailModel>();
        private List<OrganizationProductModel> _organizationProducts = new List<OrganizationProductModel>();
        private List<MemberRoleModel> _roles = new List<MemberRoleModel>();
        private List<MemberProductModel> _products = new List<MemberProductModel>();
        private List<MemberProfileQAModel> _profileQAndAs = new List<MemberProfileQAModel>();
        private List<MemberOrganizationModel> _organizations = new List<MemberOrganizationModel>();

        public string CommunityDisplayName { get; set; }
        public bool IsCommunityActive { get; set; }
        public string USPostalCode { get; set; }

        public Nullable<short> YearOfBirth { get; set; }
        public Nullable<int> ExpectedGraduationYear { get; set; }

        public List<MemberEmailModel> Emails
        {
            get
            {
                return _emails;
            }
            set
            {
                _emails = value;
            }
        }

        public List<OrganizationProductModel> OrganizationProducts
        {
            get
            {
                return _organizationProducts;
            }

            set
            {
                _organizationProducts = value;
            }
        }

        public List<MemberRoleModel> Roles
        {
            get
            {
                return _roles;
            }
            set
            {
                _roles = value;
            }
        }

        public List<MemberProductModel> Products
        {
            get
            {
                return _products;
            }
            set
            {
                _products = value;
            }
        }

        public List<MemberProfileQAModel> ProfileQAndAs
        {
            get
            {
                return _profileQAndAs;
            }
            set
            {
                _profileQAndAs = value;
            }
        }

        public List<MemberOrganizationModel> Organizations
        {
            get
            {
                return _organizations;
            }
            set
            {
                _organizations = value;
            }
        }

    }

    public class MemberEmailModel
    {
        public string EmailKey { get; set; }
        public string IndividualId { get; set; }
        public string EmailAddress { get; set; }
        public string Type { get; set; }
        public string TypeID { get; set; }
        public bool IsPrimary { get; set; }
    }

    public class MemberRoleModel
    {
        public MemberRoleModel() : base() { }

        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public string RoleDescription { get; set; }
        public bool IsMemberRoleActive { get; set; }

    }

    public class MemberProductModel
    {
        public int MemberID { get; set; }
        public int RefProductID { get; set; }
        public string MemberProductValue { get; set; }
        public bool IsMemberProductActive { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
    }

    public class MemberProfileQAModel
    {
        public string QuestionName { get; set; }
        public int QuestionExternalId { get; set; }
        public string AnsName { get; set; }
        public int AnsExternalId { get; set; }
        public string CustomValue { get; set; }
    }

    public class MemberOrganizationModel
    {
        public int MemberId { get; set; }
        public int OrganizationId { get; set; }
        public string OrganizationExternalID { get; set; }
        public Nullable<int> ExpectedGraduationYear { get; set; }
        public string ReportingId { get; set; }
        public DateTime EffectiveStartDate { get; set; }
        public Nullable<DateTime> EffectiveEndDate { get; set; }
        public bool IsPrimary { get; set; }
        public string Brand { get; set; }        
        public string OrganizationName { get; set; }
        public bool IsContracted { get; set; }
        public string OECode { get; set; }
        public string BranchCode { get; set; }
        public string OrganizationLogoName { get; set; }
        public string OrganizationAliases { get; set; }

    }

    public class OrganizationProductModel
    {
        public int ProductID { get; set; }
        public bool IsOrgProductActive { get; set; }
        public string ProductName { get; set; }
        public int ProductTypeID { get; set; }
    }
}