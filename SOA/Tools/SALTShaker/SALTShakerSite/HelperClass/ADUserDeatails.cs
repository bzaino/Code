using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Globalization;

namespace SALTShaker.BLL
{
    public class ADUserDetails
    {

        public string EmailAddress { get; set; }

        public List<UserDetail> UserDetails { get; set; }

        public string ErrorMsg { get; set; }

        public ADUserDetails()
        {

            UserDetails = new List<UserDetail>();

        }
    }


    public class UserDetail
    {
        public string UserName { get; set; }

        public string DateOfCreation { get; set; }

        public string EnvironmentName { get; set; }

        public string UserPrincipalName { get; set; }

        public string ServiceStatus { get; set; }

        public string Mail { get; set; }

        public string CN { get; set; }
        
        public string DomainID { get; set; }

        public string title { get; set; } 

        public string Department { get; set; }

        public string Phone { get; set; }
        
        public string TN_Photo { get; set; }

        public bool bisActice { get; set; }

    }
}