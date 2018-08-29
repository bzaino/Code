//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Asa.Salt.Web.Services.Contracts.Data
{
    [DataContract(IsReference = true)]
    public class vMemberQuestionAnswerContract : IEntity 
    {
        [DataMember]
        public int MemberID { get; set; }
        [DataMember]
        public string FirstName { get; set; }
        [DataMember]
        public string LastName { get; set; }
        [DataMember]
        public string EmailAddress { get; set; }
        [DataMember]
        public Nullable<int> ExternalSourceQuestionAnswerID { get; set; }
        [DataMember]
        public Nullable<int> ExternalSourceQuestionID { get; set; }
        [DataMember]
        public string AnswerText { get; set; }
        [DataMember]
        public string StandardAnswerText { get; set; }
        [DataMember]
        public string QuestionText { get; set; }
        [DataMember]
        public string StandardQuestionText { get; set; }
        [DataMember]
        public int MemberQuestionAnswerID { get; set; }
        [DataMember]
        public int RefAnswerID { get; set; }
        [DataMember]
        public int RefQuestionID { get; set; }
        [DataMember]
        public int RefSourceID { get; set; }
        [DataMember]
        public string SourceName { get; set; }
        [DataMember]
        public string SourceDescription { get; set; }
        [DataMember]
        public Nullable<int> RefSourceQuestionID { get; set; }
        [DataMember]
        public string SourceQuestionDescription { get; set; }
        [DataMember]
        public Nullable<byte> AnswerDisplayOrder { get; set; }
        [DataMember]
        public Nullable<byte> QuestionDisplayOrder { get; set; }
        [DataMember]
        public int RefQuestions_refQuestionID { get; set; }
        [DataMember]
        public int RefAnswer_refAnswerID { get; set; }
        [DataMember]
        public Nullable<int> RefSourceQuestionAnswer_refAnswerID { get; set; }
        [DataMember]
        public  Nullable<int>  RefSourceQuestionAnswerID { get; set; }
        [DataMember]
        public System.DateTime CreatedDate { get; set; }
        [DataMember]
        public string FreeformAnswerText { get; set; }
    }
    
}