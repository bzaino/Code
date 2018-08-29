using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using ASA.Web.Common.Validation;
using ASA.Web.Services.Common;

namespace ASA.Web.Services.AlertService.DataContracts
{
    public class AlertModel : BaseWebModel
    {
        [Required]
        [DisplayName("Alert ID")]
        [DefaultValue("")]
        public string ID { get; set; }

        [Required]
        [RegularExpression(RegexStrings.GUID)]
        public string IndividualId { get; set; } //Avectra

        [Required]
        [DisplayName("Date Issued")]
        public DateTime? DateIssued { get; set; }

        [DisplayName("Title")]
        [DefaultValue("")]
        public string Title { get; set; }

        [Required]
        [DisplayName("Message")]
        [DefaultValue("")]
        public string Message { get; set; }

        [DisplayName("Link")]
        public string Link { get; set; }

        [DisplayName("Logo")]
        public string Logo { get; set; }

        [DisplayName("Alert Type")]
        public string AlertType { get; set; }

        [Required]
        [DisplayName("Active Flag")]
        public bool IsActive { get; set; }
    }
}
