using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Asa.Salt.Web.Services.Contracts.Data
{
    public class MemberQAContract
    {
        [DataMember]
        public int ExternalSourceQuestionID { get; set; }
        [DataMember]
        public int ExternalSourceAnswerID { get; set; }
        [DataMember]
        public int RefSourceID { get; set; }
        [DataMember]
        public string AnswerText { get; set; }
        [DataMember]
        public Nullable<int> MemberQuestionAnswerID { get; set; }
        [DataMember]
        public string FreeformAnswerText { get; set; }
    }
}
