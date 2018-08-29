using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Asa.Salt.Web.Services.Contracts.Data
{
    public class MemberProfileQAContract
    {
        [DataMember]
        public int RefProfileQuestionID { get; set; }
        [DataMember]
        public string ProfileQuestionName { get; set; }
        [DataMember]
        public int ProfileQuestionExternalID { get; set; }
        [DataMember]
        public int RefProfileAnswerID { get; set; }
        [DataMember]
        public string ProfileAnswerName { get; set; }
        [DataMember]
        public string ProfileAnswerDescription { get; set; }
        [DataMember]
        public int ProfileAnswerExternalID { get; set; }
        [DataMember]
        public string CustomValue { get; set; }
    }
}
