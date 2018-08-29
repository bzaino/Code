using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using ASA.Web.Common.Validation;
using ASA.Web.Services.Common;

namespace ASA.Web.Services.ReminderService.DataContracts
{
    public class ReminderModel : BaseWebModel
    {
        [DisplayName("ID")]
        [RegularExpression(RegexStrings.GUID)]
        public string ID { get; set; }

        [DisplayName("IndividualId")]
        [RegularExpression(RegexStrings.GUID)]
        public string IndividualId { get; set; }

        public int MemberId { get; set; }

        [Required]
        [DisplayName("Servicer Name")]
        [DefaultValue("")]
        [StringLength(80, ErrorMessage="{0} must be less than {1} characters.")] //from DB column length.
        [RegularExpression(@"^[a-zA-Z'.\s | \d | \- | \/ | \$ | \£ | \€ | \( | \) | \ | \! | \% | \+ | \& | \, | \! $]{1,200}$", ErrorMessage="Servicer Name is invalid!")]
        public string ServicerName { get; set; }

        [Required]
        [DisplayName("Number of Loans")]
        [DefaultValue(1)]
        public int NumberOfLoans { get; set; }

        [Required]
        [DisplayName("Day of Month")]
        [Range(1,31)]
        public int DayOfMonth { get; set; }

        public DateTime DueDate { get; set; }

        [Required]
        [DisplayName("Active Flag")]
        public bool IsActive { get; set; }

    }
}
