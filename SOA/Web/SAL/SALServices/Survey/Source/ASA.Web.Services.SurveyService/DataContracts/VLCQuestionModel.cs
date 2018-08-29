using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using ASA.Web.Common.Validation;

namespace ASA.Web.Services.SurveyService.DataContracts
{
    public class VLCQuestionModel
    {

        [Required]
        public int QuestionVersion { get; set; }

        [Required]
        public int QuestionID { get; set; }

        [Required]
        [StringLength(300)]
        public string QuestionText { get; set; }

    }
}
