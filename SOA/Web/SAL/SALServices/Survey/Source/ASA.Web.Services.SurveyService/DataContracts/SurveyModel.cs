using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using ASA.Web.Common.Validation;
using ASA.Web.Services.Common;

namespace ASA.Web.Services.SurveyService.DataContracts
{
    public class SurveyModel : BaseWebModel
    {
        [Required]
        [DisplayName("Survey Id")]
        public string SurveyId { get; set; }

        [DisplayName("Survey Question Id")]
        public string SurveyQuestionId { get; set; }

        [DisplayName("Question")]
        [DefaultValue("")]
        [StringLength(200, ErrorMessage="{0} must be less than {1} characters.")] //from DB column length.
        public string QuestionText { get; set; }

        [DisplayName("Answer Options")]
        [DefaultValue("")]
        [StringLength(500, ErrorMessage = "{0} must be less than {1} characters.")] //from DB column length.
        public string ListOfAnswerOptions { get; set; }

        [DisplayName("Response")]
        [DefaultValue("")]
        [StringLength(2000, ErrorMessage = "{0} must be less than {1} characters.")] //from DB column length.
        public string Response { get; set; }

        [DisplayName("Response Count")]
        [DefaultValue("0")]
        public int ResponseCount { get; set; }

        [Required]
        [DisplayName("Individual Id")]
        [RegularExpression(RegexStrings.GUID)]
        public string IndividualId { get; set; } //Avectra

        [Required]
        public int MemberId { get; set; }

        [Required]
        [DisplayName("Response Date")]
        public DateTime? ResponseDate { get; set; }

        [Required]
        [DisplayName("Response Status")]
        [RegularExpression(RegexStrings.GUID)]
        public string ResponseStatus { get; set; } 
    }
}
