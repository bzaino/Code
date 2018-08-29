using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASA.Web.Services.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace ASA.Web.Services.ASAMemberService.DataContracts
{
    public class MemberQAModel : BaseWebModel
    {
        [DisplayName("questionId")]
        [Required(ErrorMessage = "The questionId is required.")]
        public string questionId { get; set; }

        [DisplayName("questionId")]
        [Required(ErrorMessage = "The questionId is required.")]
        public string choiceId { get; set; }

        [DisplayName("sourceId")]
        [Required(ErrorMessage = "The sourceId is required.")]
        public string sourceId { get; set; }

        [StringLength(500, ErrorMessage = "The {0} must be between {2} and {1} characters long.", MinimumLength = 1)]
        public string choiceText { get; set; }

        [StringLength(1000, ErrorMessage = "The {0} must be between less than 1001 characters")]
        public string FreeformAnswerText { get; set; }

        public Nullable<int> MemberQuestionAnswerID { get; set; }
    }
}
