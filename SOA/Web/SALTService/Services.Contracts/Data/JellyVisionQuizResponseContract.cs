using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Asa.Salt.Web.Services.Contracts.Data
{
    public class JellyVisionQuizResponseContract
    {
        [DataMember]
        public int MemberId { get; set; }
        [DataMember]
        public string QuizTakenSite { get; set; }
        [DataMember]
        public string QuizResponse { get; set; }
        [DataMember]
        public String QuizResult { get; set; }
        [DataMember]
        public String QuizName { get; set; }
    }
}
