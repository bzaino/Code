using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using ASA.Web.Common.Validation;

namespace ASA.Web.Services.SurveyService.DataContracts
{
    public class JellyVisionQuizResponseModel
    {
        //memberId
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string referrerID { get; set; }

        [Required]
        [StringLength(20)]
        public string responses { get; set; }

        [Required]
        [StringLength(1)]
        public String personalityType { get; set; }

        [Required]
        [StringLength(100)]
        public String quizName { get; set; }
    }
}
