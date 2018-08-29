using System.Collections.Generic;
using ASA.Web.Services.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace ASA.Web.Services.AlertService.DataContracts
{
    public class ContentFeedBackModel : BaseWebModel
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        [DisplayName("Email Address")]
        [StringLength(100, ErrorMessage = "{0} must be less than {1} characters.")]
        public string FromEmailAddress { get; set; }

        [Required]
        [DisplayName("Email Subject")]
        [StringLength(300, ErrorMessage = "{0} must be less than {1} characters.")]
        public string emailSubject { get; set; }

        [Required]
        [DisplayName("Email Body")]
        [StringLength(2000, ErrorMessage = "{0} must be less than {1} characters.")]
        public string emailBody { get; set; }

        [Required]
        [DisplayName("Membership Id")]
        [StringLength(60, ErrorMessage = "{0} must be less than {1} characters.")]
        public string memberID { get; set; }

        [Required]
        [DisplayName("Rating Value")]
        [StringLength(60, ErrorMessage = "{0} must be less than {1} characters.")]
        public string ratingVal { get; set; }

        [Required]
        [DisplayName("Content ID")]
        [StringLength(60, ErrorMessage = "{0} must be less than {1} characters.")]
        public string contentID { get; set; }
    }
}
