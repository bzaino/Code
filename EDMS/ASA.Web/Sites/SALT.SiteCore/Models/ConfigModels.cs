using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace ASA.Web.Sites.SALT.Models
{
    public class ConfigModel
    {
        //SAL Endpoints
        [Required]
        public string AddrValidationServiceEndpoint { get; set; }

        [Required]
        public string AlertServiceEndpoint { get; set; }        
        
        [Required]
        public string MemberService { get; set; }

        [Required]
        public string LoanServiceEndpoint { get; set; }

        [Required]
        public string ReminderService { get; set; }

        [Required]
        public string SearchServiceEndpoint { get; set; }

        [Required]
        public string SelfReportedServiceEndpoint { get; set; }

        [Required]
        public string SurveyServiceEndpoint { get; set; }

        //MVC Endpoints
        //[Required]
        //public string RegisterMatchUser { get; set; }

        //[Required]
        //public string SchoolLookup { get; set; }

        //[Required]
        //public string SchoolLookupIneligibleEmail { get; set; }

        [Required]
        public string TemplateDirectory { get; set; }

        //Auth and Timeout Config
        [Required]
        public string IsAuthenticated { get; set; }

        [Required]
        public int FormsAuthTimeoutValue { get; set; }

        [Required]
        public string LoginRedirectPage { get; set; }
    }
}
