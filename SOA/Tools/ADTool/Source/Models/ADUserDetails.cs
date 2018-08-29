using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Security;

namespace ADTool.Models
{
    public class ADUserDetails
    {
       
            [Required]
            [Display(Name = "EmailAddress Name")]
            public string EmailAddress { get; set; }

            [Required]
            [DataType(DataType.Custom)]
            [Display(Name = "UserDetail")]
            public List<UserDetail> UserDetails { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Error")]
            public string ErrorMsg { get; set; }

            public ADUserDetails()
            {
        
                UserDetails = new List<UserDetail>();
            
            }
    }


        public class UserDetail
        {
            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "User Name")]
            public string UserName { get; set; }
            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "DateOfCreation")]
            public string DateOfCreation { get; set; }
            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Environment Name")]
            public string EnvironmentName { get; set; }
            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "User Principal Name")]
            public string UserPrincipalName { get; set; }
            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Service Status")]
            public string ServiceStatus { get; set; }

            [Required]
            [Display(Name = "EmailAddress Name")]
            public string Mail { get; set; }

            [Required]
            [Display(Name = "CN")]
            public string CN { get; set; }

        }
    
}