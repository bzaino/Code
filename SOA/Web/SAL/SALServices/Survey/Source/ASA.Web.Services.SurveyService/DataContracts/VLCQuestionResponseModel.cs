using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using ASA.Web.Common.Validation;


namespace ASA.Web.Services.SurveyService.DataContracts
{
    public class VLCQuestionResponseModel
    {

        [Required]
        public int MemberID { get; set; }

        [Required]
        [StringLength(300)]
        public string ResponseText { get; set; }

        public VLCQuestionModel Question { get; set; }
        public DateTime ResponseDate { get; set; }

        [DefaultValue(0)]
        public int VLCQuestionID { get; set; }
        [DefaultValue(0)]
        public int VLCUserResponseID { get; set; }

    }
}
