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
    [KnownType(typeof(MemberQuestionAnswerContract))]
    [KnownType(typeof(RefSourceQuestionContract))]
    public class RefQuestionContract : IEntity 
    {
        public RefQuestionContract()
        {
            this.MemberQuestionAnswers = new HashSet<MemberQuestionAnswerContract>();
            this.RefSourceQuestions = new HashSet<RefSourceQuestionContract>();
        }
        [DataMember]
        public int RefQuestionID { get; set; }
        [DataMember]
        public string StandardQuestionText { get; set; }
        [DataMember]
        public bool IsRefQuestionActive { get; set; }
        [DataMember]
        public string CreatedBy { get; set; }
        [DataMember]
        public System.DateTime CreatedDate { get; set; }
        [DataMember]
        public string ModifiedBy { get; set; }
        [DataMember]
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        [DataMember]
        public virtual ICollection<MemberQuestionAnswerContract> MemberQuestionAnswers { get; set; }
        [DataMember]
        public virtual ICollection<RefSourceQuestionContract> RefSourceQuestions { get; set; }
    }
    
}
