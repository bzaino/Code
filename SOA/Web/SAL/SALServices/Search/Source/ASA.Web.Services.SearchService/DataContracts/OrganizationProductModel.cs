using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace ASA.Web.Services.SearchService.DataContracts
{
    public class OrganizationProductModel
    {
        [Required]
        public int OrgID { get; set; }
        public int ProductID { get; set; }
        public bool IsOrgProductActive { get; set; }
    }

}
