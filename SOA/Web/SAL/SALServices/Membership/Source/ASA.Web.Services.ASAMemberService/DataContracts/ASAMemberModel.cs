using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using ASA.Web.Common.Validation;
using ASA.Web.Services.Common;

namespace ASA.Web.Services.ASAMemberService.DataContracts
{
    public class ASAMemberModel : BaseWebModel
    {
        private const string CLASSNAME = "ASA.Web.Services.ASAMemberService.DataContracts";
        static ASA.Log.ServiceLogger.IASALog _log = ASA.Log.ServiceLogger.ASALogManager.GetLogger(CLASSNAME);

        public ASAMemberModel() : base() { }
        public ASAMemberModel(Boolean newRecord = false) : base(newRecord) { }

        public string IndividualId { get; set; } //Avectra

        public string MembershipId { get; set; } //Avectra

        public string ActiveDirectoryKey { get; set; } //Avectra

        [DisplayName(@"Membership Start Date")]
        public DateTime? MembershipStartDate { get; set; }

        public int PersonId { get; set; } //ODS

        public int CustomerId { get; set; } // ODS??

        public string ExternalId { get; set; }

        [DisplayName(@"Legal First Name")]
        public string LegalFirstName { get; set; }

        [Required(ErrorMessage = "Required*")]
        [RegularExpression(RegexStrings.REGISTRATION_FORM_NAME, ErrorMessage = "Invalid First Name")]
        [DisplayName(@"First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Required*")]
        [RegularExpression(RegexStrings.REGISTRATION_FORM_NAME, ErrorMessage = "Invalid Last Name")]
        [DisplayName("Last Name")]
        public string LastName { get; set; }

        [DisplayName(@"Display Name")]
        public string DisplayName { get; set; }

        [DisplayName(@"Date of Birth")]
        public DateTime? DOB { get; set; }

        [DisplayName(@"Enrollment Status")]
        public string EnrollmentStatus { get; set; }

        public DateTime? EnrollmentStatusEffective { get; set; }

        [DisplayName(@"Grade Level")]
        public string GradeLevel { get; set; }

        [DisplayName(@"Withdrawal Date")]
        public DateTime? WithdrawalDate { get; set; }

        [DisplayName(@"Part Time Date")]
        public DateTime? PartTimeDate { get; set; }

        [DisplayName(@"Middle Initial")]
        public string MiddleInitial { get; set; }

        [DisplayName(@"Contact Frequency")]
        public bool ContactFrequency { get; set; }
        public string ContactFrequencyKey { get; set; }

        public string MemberToken { get; set; }
        public string PrimaryEmailKey { get; set; }
        public string PrimaryOrganizationKey { get; set; }
        public string OrganizationIdForCourses { get; set; }
        public string Source { get; set; } //reverting back to a string because FaceTheRed.com uses "The Red"

        private List<MemberAddressModel> _addresses = new List<MemberAddressModel>();
        private List<MemberPhoneModel> _phones = new List<MemberPhoneModel>();
        private List<MemberEmailModel> _emails = new List<MemberEmailModel>();
        private List<OrganizationProductModel> _organizationProducts = new List<OrganizationProductModel>();
        private List<MemberRoleModel> _roles = new List<MemberRoleModel>();
        private List<MemberProductModel> _products = new List<MemberProductModel>();
        private List<MemberProfileQAModel> _profileQAndAs = new List<MemberProfileQAModel>();
        private IList<ProfileQAModel> _profileQAs = new List<ProfileQAModel>();
        //schools are still here for backwards compatibility
        //schools should be removed at some point when outside clients upgrade
        private List<MemberSchoolModel> _schools = new List<MemberSchoolModel>();
        private List<MemberOrganizationModel> _organizations = new List<MemberOrganizationModel>();

        [PasswordStandardsASAValidator(ErrorMessage = "Invalid Password")]
        public string Password { get; set; }
        [PasswordStandardsASAValidator(ErrorMessage = "Invalid New Password")]
        public string NewPassword { get; set; }
        [RegularExpression(RegexStrings.GUID, ErrorMessage = "Invalid Invitation Token")]
        public string InvitationToken { get; set; }

        public bool MemberShipFlag { get; set; }
        public bool ActivationStatusFlag { get; set; }

        public string CommunityDisplayName { get; set; }
        public bool IsCommunityActive { get; set; }
        public bool WelcomeEmailSent { get; set; }

        [DisplayName(@"5 Digit US ZIP Code")]
        public string USPostalCode { get; set; }

        [YOBValidator(ErrorMessage = "Year of birth is invalid.", MinimumAge = 13)]
        public Nullable<short> YearOfBirth { get; set; }
        
        public Nullable<int> ExpectedGraduationYear { get; set; }
        public Nullable<int> SALTSchoolTypeID { get; set; }
        public bool? DashboardEnabled { get; set; }

        [DisplayName("Known Addresses")]
        public List<MemberAddressModel> Addresses
        {
            get
            {
                return _addresses;
            }
            set
            {
                _addresses = value;
            }
        }

        [DisplayName("Phones")]
        public List<MemberPhoneModel> Phones
        {
            get
            {
                return _phones;
            }
            set
            {
                _phones = value;
            }
        }

        [DisplayName(@"Emails")]
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

        [DisplayName(@"OrganizationProducts")]
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

        public IList<ProfileQAModel> ProfileQAs
        {
            get
            {
                return _profileQAs;
            }
            set
            {
                _profileQAs = value;
            }
        }

        //schools are still here for backwards compatibility
        //schools should be removed at some point when outside clients upgrade
        [DisplayName(@"Schools")]
        public List<MemberSchoolModel> Schools
        {
            get
            {
                return _schools;
            }
            set
            {
                _schools = value;
            }
        }

        [DisplayName(@"Organizations")]
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

        public override bool IsValid()
        {
            bool bIsValid = false;
            var mv = new ASAModelValidator();
            bIsValid = base.IsValid();

            if (this._addresses != null)
                foreach (MemberAddressModel pam in this._addresses)
                    bIsValid &= pam.IsValid();

            if (this._emails != null)
                foreach (MemberEmailModel pem in this._emails)
                    bIsValid &= pem.IsValid();

            if (this._phones != null)
                foreach (MemberPhoneModel ppm in this._phones)
                    bIsValid &= ppm.IsValid();

            if (this._schools != null)
                foreach (MemberSchoolModel psm in this._schools)
                    bIsValid &= psm.IsValid();

            if (this._organizations != null)
                foreach (MemberOrganizationModel pom in this._organizations)
                    bIsValid &= pom.IsValid();

            return bIsValid;
        }

        public IList<ErrorModel> GetAllErrors()
        {
            IList<ErrorModel> errors = new List<ErrorModel>();

            foreach (ErrorModel err in this.ErrorList)
                errors.Add(err);

            if (this._addresses != null)
                foreach (MemberAddressModel pam in this._addresses)
                    foreach (ErrorModel err in pam.ErrorList)
                        errors.Add(err);

            if (this._emails != null)
                foreach (MemberEmailModel pem in this._emails)
                    foreach (ErrorModel err in pem.ErrorList)
                        errors.Add(err);

            if (this._phones != null)
                foreach (MemberPhoneModel ppm in this._phones)
                    foreach (ErrorModel err in ppm.ErrorList)
                        errors.Add(err);

            return errors;
        }
    }
}
