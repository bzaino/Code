using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using ASA.Web.Common.Validation;
using ASA.Web.Services.Common;

namespace ASA.Web.Sites.SALT.Models
{

    [RequiredIf("Password", "NewPassword", ErrorMessage = "Password must be filled out if New Password is populated!")]
    public class ManageAccountModel : BaseWebModel
    {

        [Display(Name = "Member ID")]
        public string MembershipAccoundId { get; set; }

        [Required(ErrorMessage = "Legal First Name is required!")]
        [Display(Name = "Legal First Name")]
        [StringLength(30, ErrorMessage = "The {0} must be between {2} and {1} characters long.", MinimumLength = 1)]
        [RegularExpression(RegexStrings.NAME, ErrorMessage = "Please enter a valid name!")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [StringLength(30, MinimumLength = 0)]
        [RegularExpression(RegexStrings.NAME, ErrorMessage = "Please enter a valid name!")]
        public string LastName { get; set; }

        public string DisplayName { get; set; }
        public string Source { get; set; }

        [Display(Name = "Year of Birth")]
        [YOBValidator(ErrorMessage="Year of birth is invalid.", MinimumAge=13)]
        public Nullable<short> YOB { get; set; }

        [RegularExpression(RegexStrings.USPostalCode, ErrorMessage = "Please enter a valid 5 digit US postal code!")]
        [Display(Name = "US Postal Code")]
        public string USPostalCode { get; set; }

        public Nullable<int> SALTSchoolTypeID { get; set; }

        [Display(Name = "Enrollment Status")]
        [StringLength(1, MinimumLength = 1)]
        public string EnrollmentStatus { get; set; }

        [Display(Name = "Grade Level")]
        public string GradeLevel { get; set; }

        public string MembershipStartDate { get; set; }

        private int? _expectedGraduationYear = null;
        public int? ExpectedGraduationYear
        {
            get
            {
                return _expectedGraduationYear;
            }

            set
            {
                _expectedGraduationYear = value;
            }
        }
        public string OrganizationName { get; set; }

        // --------------------

        [RegularExpression(RegexStrings.PHONEus, ErrorMessage = "Must be in US phone number format")]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
        [RegularExpression("(^Home$)|(^Mobile$)", ErrorMessage = "Phone Number Type must be either 'Mobile' or 'Home'.")]
        public string PhoneNumberType { get; set; }

        // --------------------

        [Required(ErrorMessage = "Email Address is required!")]
        //64 max due to overloading Email as AD username
        [StringLength(64, ErrorMessage = "The {0} must be between {2} and {1} characters long.", MinimumLength = 8)]
        [Display(Name = "EmailAddress address")]
        [EmailValidator(ErrorMessage = "Please enter a valid email.")]
        public string EmailAddress { get; set; }

        // set to EmailAddress
        [StringLength(64, ErrorMessage = "The {0} must be between {2} and {1} characters long.", MinimumLength = 8)]
        [EmailValidator(ErrorMessage = "Please enter a valid user name.")]
        public string UserName { get; set; }

        [Display(Name = "Password")]
        [StringLength(32, ErrorMessage = "The {0} must be between {2} and {1} characters long.", MinimumLength = 8)]
        [PasswordStandardsASAValidator(ErrorMessage = "Invalid Password")]
        public string Password { get; set; }

        [Compare("NewPassword", ErrorMessage = "The password and confirmation password do not match.")]
        [Display(Name = "Confirm Password")]
        [StringLength(32, ErrorMessage = "The {0} must be between {2} and {1} characters long.", MinimumLength = 8)]
        [PasswordStandardsASAValidator(ErrorMessage = "Invalid Password")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "New Password")]
        [StringLength(32, ErrorMessage = "The {0} must be between {2} and {1} characters long.", MinimumLength = 8)]
        [PasswordStandardsASAValidator(ErrorMessage = "Invalid Password")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Contact Frequency is required!")]
        [Display(Name = "Contact Frequency")]
        public bool ContactFrequency { get; set; }

        public int OrganizationId { get; set; }
        public string OECode { get; set; }
        public string BranchCode { get; set; }
        public bool AddressValidated { get; set; }

        public bool IsCommunityActive { get; set; }

        public bool WelcomeEmailSent { get; set; }
    }
}
