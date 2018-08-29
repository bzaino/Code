using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Asa.Salt.Web.Services.Domain
{
    public partial class MemberProfileQA : DomainObject<int>
    {
        public string ProfileQuestionName { get; set; }
        public int ProfileQuestionExternalID { get; set; }
        public string ProfileAnswerName { get; set; }
        public string ProfileAnswerDescription { get; set; }
        public int ProfileAnswerExternalID { get; set; }
        public string CustomValue { get; set; }
    }
}
