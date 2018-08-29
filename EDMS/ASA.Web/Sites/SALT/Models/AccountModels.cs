using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Security;

namespace ASA.Web.Sites.SALT.Models
{

    public class ChangePasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class LogOnModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class ResendPasswordModel
    {

        [Required(ErrorMessage = "Last Name is required!")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        public string ResendPasswordUrl { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Security Answer is required!")]
        [Display(Name = "Security Answer")]
        [DataType(DataType.Text)]
        public string SecurityAnswer { get; set; }

        public string Token { get; set; }
    }

    public class NewPasswordModel
    {
        
        
        
        public string Email { get; set; }
        

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Security Answer is required!")]
        [Display(Name = "Security Answer")]
        [DataType(DataType.Text)]
        public string SecurityAnswer { get; set; }

        [Required(ErrorMessage = "Last name is required!")]
        [Display(Name = "Last name")]
        [DataType(DataType.Text)]
        public string LastName { get; set; }

        public string SecurityQuestion { get; set; }
        public string Token { get; set; }


    }
    public class ForgotPasswordModel
    {
        [Required]
        [DataType(DataType.EmailAddress, ErrorMessage = "Invalid Email Address")]
        [Display(Name = "Email Address")]
        public string Email { get; set; }

        public string ResendPasswordUrl { get; set; }

    }
    public class ResendPin
    {
        [Required]
        [DataType(DataType.EmailAddress, ErrorMessage = "Invalid Email Address")]
        [Display(Name = "Email Address")]
        public string Email { get; set; }
    }
    public class Contact
    {
        [Required]
        [Display(Name = "Question Type")]
        public string QuestionType { get; set; }

        [Required]
        [Display(Name = "Followup Type")]
        public string FollowupType { get; set; }

        [Required]
        [Display(Name = "Subject")]
        public string Subject { get; set; }

        [Required]
        [Display(Name = "Message")]
        public string Message { get; set; }
        
        //[DataType(DataType.EmailAddress, ErrorMessage = "Invalid Email Address")]
        //[Display(Name = "Email Address")]
        //public string Email { get; set; }
    }

    public class HasMessagesModel
    {
        [Display(Name = "Nickname")]
        public string Nickname { get; set; }
    }

    public class ReminderModel
    {
        [Required]
        [Display(Name = "Pick a Servicer")]
        public string ServicerName { get; set; }

        [Required]
        [Display(Name = "Number of Loans")]
        public int NumberOfLoans { get; set; }

        [Required]
        [Display(Name = "Payment Due Date")]
        public DateTime DueDate { get; set; }

        public int DayOfMonth { get; set; }

        [Display(Name = "Nickname")]
        public string Nickname { get; set; }
    }
}
