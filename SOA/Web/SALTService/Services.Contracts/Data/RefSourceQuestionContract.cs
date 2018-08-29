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
    [KnownType(typeof(RefSourceQuestionAnswerContract))]
    public class RefSourceQuestionContract : IEntity 
    {
        public RefSourceQuestionContract()
        {
            this.RefSourceQuestionAnswers = new HashSet<RefSourceQuestionAnswerContract>();
        }
        [DataMember]
        public int RefSourceQuestionID { get; set; }
        [DataMember]
        public int RefSourceID { get; set; }
        [DataMember]
        public int RefQuestionID { get; set; }
        [DataMember]
        public Nullable<int> ExternalSourceQuestionID { get; set; }
        [DataMember]
        public string QuestionText { get; set; }
        [DataMember]
        public string SourceQuestionDescription { get; set; }
        [DataMember]
        public byte QuestionDisplayOrder { get; set; }
        [DataMember]
        public bool IsRefSourceQuestionActive { get; set; }
        [DataMember]
        public string CreatedBy { get; set; }
        [DataMember]
        public System.DateTime CreatedDate { get; set; }
        [DataMember]
        public string ModifiedBy { get; set; }
        [DataMember]
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        [DataMember]
        public virtual RefQuestionContract RefQuestion { get; set; }
        [DataMember]
        public virtual RefSourceContract RefSource { get; set; }
        [DataMember]
        public virtual ICollection<RefSourceQuestionAnswerContract> RefSourceQuestionAnswers { get; set; }
    }
    
}