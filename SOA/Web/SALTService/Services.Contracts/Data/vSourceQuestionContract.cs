using System;
using System.Collections.Generic;
using System.Runtime.Serialization;


namespace Asa.Salt.Web.Services.Contracts.Data
{
    [DataContract(IsReference = true)]
    public class vSourceQuestionContract : IEntity 
    {
        [DataMember]
        public int StandardQuestion_refQuestionID { get; set; }
        [DataMember]
        public string StandardQuestionText { get; set; }
        [DataMember]
        public string SourceName { get; set; }
        [DataMember]
        public string SourceDescription { get; set; }
        [DataMember]
        public int RefSourceID { get; set; }
        [DataMember]
        public int RefSourceQuestionID { get; set; }
        [DataMember]
        public string QuestionText { get; set; }
        [DataMember]
        public string SourceQuestionDescription { get; set; }
        [DataMember]
        public Nullable<int> ExternalSourceQuestionID { get; set; }
        [DataMember]
        public byte QuestionDisplayOrder { get; set; }
        [DataMember]
        public Nullable<int> MemberQuestionAnswerID { get; set; }
        [DataMember]
        public Nullable<int> RefAnswerID { get; set; }
        [DataMember]
        public Nullable<int> MemberID { get; set; }
    }
    
}
