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
    public class JSIQuizModel : BaseWebModel
    {
        //memberId can be null if not authenticated
        public int MemberId { get; set; }

        [DisplayName("JSI Major Id")]
        public string MajorId { get; set; }

        [DisplayName("JSI Major")]
        public string Major { get; set; }

        [DisplayName("JSI School Id")]
        public string SchoolId { get; set; }

        [DisplayName("JSI State")]
        public string School { get; set; }

        [DisplayName("JSI State Id")]
        public string StateId { get; set; }

        [DisplayName("JSI State Name")]
        public string State { get; set; }

        [DisplayName("JSI State Abbreviation")]
        public string StateCd { get; set; }

        [DisplayName("JSI Occupation Name")]
        public string OccupationName { get; set; }

        [DisplayName("JSI Estimated Salary Amount")]
        public string EstimatedSalaryAmount { get; set; }
    }
}
