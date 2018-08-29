using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ASA.Web.Sites.SALT.Models
{
    public class SchoolLookup
    {
        public string School { get; set; }
        public string OECode { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)] 
        public string Email { get; set; }

    }
}