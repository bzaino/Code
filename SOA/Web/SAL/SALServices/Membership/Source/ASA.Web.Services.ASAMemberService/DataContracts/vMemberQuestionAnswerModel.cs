using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace ASA.Web.Services.ASAMemberService.DataContracts
{
    public class vMemberQuestionAnswerModel
    {
        public int MemberID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public string StandardAnswerText { get; set; }
        public string QuestionText { get; set; }
        public string StandardQuestionText { get; set; }
        public int MemberQuestionAnswerID { get; set; }
        public int RefAnswerID { get; set; }
        public int RefQuestionID { get; set; }
        public int RefSourceID { get; set; }
        public string SourceName { get; set; }
        public string SourceDescription { get; set; }
        public Nullable<int> RefSourceQuestionID { get; set; }
        public string SourceQuestionDescription { get; set; }
        public Nullable<byte> AnswerDisplayOrder { get; set; }
        public Nullable<byte> QuestionDisplayOrder { get; set; }
        public int RefQuestions_refQuestionID { get; set; }
        public int RefAnswer_refAnswerID { get; set; }
        public Nullable<int> RefSourceQuestionAnswer_refAnswerID { get; set; }
        public Nullable<int> RefSourceQuestionAnswerID { get; set; }
        //The spelling/casing of these variables needs to match Unigo's api, in order to provide a consistent data model to the client side code
        public Nullable<int> choiceId { get; set; }
        public Nullable<int> questionId { get; set; }
        public string choiceText { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string FreeformAnswerText { get; set; }
    }
}
