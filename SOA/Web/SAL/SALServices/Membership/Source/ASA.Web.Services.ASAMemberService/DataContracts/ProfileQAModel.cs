using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ASA.Web.Services.ASAMemberService.DataContracts
{
    public class ProfileQAModel
    {
        public int QuestionID { get; set; }
        public string QuestionName { get; set; }
        public string QuestionDescription { get; set; }
        public string ProfileQuestionTypeName { get; set; }
        public bool IsProfileQuestionActive { get; set; }
        public Nullable<short> ProfileQuestionPriority { get; set; }
        public bool IsInLineProfileQuestion { get; set; }
        public List<PAnswerModel> Answers { get; set; }
        public List<ProfileResponseModel> Responses { get; set; }

    }
}
