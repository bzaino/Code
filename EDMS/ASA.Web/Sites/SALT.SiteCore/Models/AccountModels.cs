using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Security;

using ASA.Web.Services.Common;
using ASA.Web.Common.Validation;

namespace ASA.Web.Sites.SALT.Models
{

    public class ChangePasswordModel
    {
        [Required(ErrorMessage = "Password is required!")]
        [StringLength(32, ErrorMessage = "The {0} must be between {2} and {1} characters long.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        [PasswordStandardsASAValidator(ErrorMessage = "Invalid Password")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "Password is required!")]
        [StringLength(32, ErrorMessage = "The {0} must be between {2} and {1} characters long.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        [PasswordStandardsASAValidator(ErrorMessage = "Invalid Password")]
        public string NewPassword { get; set; }

        [StringLength(32, ErrorMessage = "The {0} must be between {2} and {1} characters long.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation of new password do not match.")]
        [PasswordStandardsASAValidator(ErrorMessage = "Invalid Password")]
        public string ConfirmPassword { get; set; }
    }

    public class LogOnModel
    {
        [Required(ErrorMessage = "User name is required!")]
        [Display(Name = "User name")]
        [EmailValidator(ErrorMessage = "Please enter a valid user name.")]
        [StringLength(64, ErrorMessage = "The {0} must be between {2} and {1} characters long.", MinimumLength = 8)]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password is required!")]
        [StringLength(32, ErrorMessage = "The {0} must be between {2} and {1} characters long.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        // Code to be removed
        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }

        public string checkRememberMe { get; set; }
    }

    public class ResendPasswordModel
    {

        [Required(ErrorMessage = "Last Name is required!")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        public string ResendPasswordUrl { get; set; }

        [Required(ErrorMessage = "Password is required!")]
        [StringLength(32, ErrorMessage = "The {0} must be between {2} and {1} characters long.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [StringLength(32, ErrorMessage = "The {0} must be between {2} and {1} characters long.", MinimumLength = 8)]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string Token { get; set; }
    }

    public class NewPasswordModel
    {
        [EmailValidator(ErrorMessage = "Please enter a valid user name.")]
        [StringLength(64, MinimumLength = 8)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required!")]
        [StringLength(32, ErrorMessage = "The {0} must be between {2} and {1} characters long.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        [PasswordStandardsASAValidator(ErrorMessage = "Invalid Password")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Confirm Password is required!")]
        [StringLength(32, ErrorMessage = "The {0} must be between {2} and {1} characters long.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [PasswordStandardsASAValidator(ErrorMessage = "Invalid Password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match!")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Email token is required!")]
        public string Token { get; set; }
    }

    public class ForgotPasswordModel
    {
        [Required(ErrorMessage = "Email Address is required!")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Invalid Email Address")]
        [Display(Name = "Email Address")]
        [StringLength(64, ErrorMessage = "The {0} must be between {2} and {1} characters long.", MinimumLength = 8)]
        [EmailValidator(ErrorMessage = "Please enter a valid email.")]
        public string Email { get; set; }

        public string ResendPasswordUrl { get; set; }

    }

    public class HasMessagesModel
    {
        [Display(Name = "Nickname")]
        public string Nickname { get; set; }
    }
}
