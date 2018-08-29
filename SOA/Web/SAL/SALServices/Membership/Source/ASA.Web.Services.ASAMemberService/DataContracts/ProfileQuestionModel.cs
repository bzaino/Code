using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ASA.Web.Services.ASAMemberService.DataContracts
{
    public class ProfileQuestionModel
    {
        public int QuestionID { get; set; }
        public string QuestionName { get; set; }
        public string QuestionDescription { get; set; }
        public string ProfileQuestionTypeName { get; set; }
        public bool IsProfileQuestionActive { get; set; }
        public Nullable<short> ProfileQuestionPriority { get; set; }
        public bool IsInLineProfileQuestion { get; set; }
    
    }
}
