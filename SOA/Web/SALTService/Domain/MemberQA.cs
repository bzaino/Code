using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Asa.Salt.Web.Services.Domain
{
    public partial class MemberQA : DomainObject<int>
    {
        public int ExternalSourceQuestionID { get; set; }
        public int ExternalSourceAnswerID { get; set; }
        public int RefSourceID { get; set; }
        public string AnswerText { get; set; }
        public Nullable<int> MemberQuestionAnswerID { get; set; }
        public string FreeformAnswerText { get; set; }
    }
}