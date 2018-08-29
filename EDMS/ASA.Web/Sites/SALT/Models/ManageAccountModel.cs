using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ASA.Web.Sites.SALT.Models
{
    public class ManageAccountModel
    {
        
        [Display(Name = "Membership PIN")]
        public string MembershipPIN { get; set; }

        public string  AvectraMemberId { get; set; }
        [Display(Name = "Legal First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Nickname")]
        public string Nickname { get; set; }

        
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        
        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public string DOB { get; set; }

        [Required(ErrorMessage = "Enrollment Status is required!")]
        [Display(Name = "Enrollment Status")]
        public string EnrollmentStatus { get; set; }

        private string _graduationDate = string.Empty;
        public string GraduationDate
        {
            get {
                return _graduationDate;
            } 
            
            set {
                _graduationDate = value;
        } }
        private string _graduationYear = string.Empty;
        public string MembershipStartDate { get; set; }
        public string GraduationYear { get {
            if (!string.IsNullOrEmpty(_graduationDate) )
            {
                DateTime gDate = DateTime.Parse(_graduationDate);
                _graduationYear = gDate.Year.ToString();
                
            }
            return _graduationYear; 
        }  }
        public string School { get; set; }


        // --------------------

        [Display(Name = "Country")]
        public string Country { get; set; }

        [Display(Name = "Address Line 1")]
        public string Address1 { get; set; }

        [Display(Name = "Address Line 2")]
        public string Address2 { get; set; }

        [Display(Name = "City")]
        public string City { get; set; }

        [Display(Name = "State/Province")]
        public string State { get; set; }
        public string HiddenState { get; set; }

        [Display(Name = "Zip Code")]
        public string Zip { get; set; }

        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        public string PhoneNumberType { get; set; }

        // --------------------

        [Required(ErrorMessage = "Email Address is required!")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "EmailAddress address")]
        public string EmailAddress { get; set; }

        // set to EmailAddress
        public string UserName { get; set; }

        
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("NewPassword", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }


        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; }

        
        [Display(Name = "Security Question")]
        [DataType(DataType.Text)]
        public string SecurityQuestion { get; set; }

        
        [Display(Name = "Security Answer")]
        [DataType(DataType.Text)]
        public string SecurityAnswer { get; set; }

        [Display(Name = "Contact Frequency")]
        public bool ContactFrequency { get; set; }

        public string OECode { get; set; }
        public string BranchCode { get; set; }

            }
}