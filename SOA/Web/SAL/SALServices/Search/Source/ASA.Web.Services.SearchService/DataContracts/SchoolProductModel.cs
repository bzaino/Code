using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace ASA.Web.Services.SearchService.DataContracts
{
    public class SchoolProductModel
    {
        [Required]
        public int RefSchoolID { get; set; }
        public int RefProductID { get; set; }
        public bool IsRefSchoolProductActive { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }

        //public virtual RefProduct RefProduct { get; set; }
        //public virtual School RefSchool { get; set; }
    }
}
