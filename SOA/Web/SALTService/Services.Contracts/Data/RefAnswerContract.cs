using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Asa.Salt.Web.Services.Contracts.Data
{
    [DataContract(IsReference = true)]
    [KnownType(typeof(MemberQuestionAnswerContract))]
    [KnownType(typeof(RefSourceQuestionAnswerContract))]
    public class RefAnswerContract : IEntity
    {
        public RefAnswerContract()
        {
            this.MemberQuestionAnswers = new HashSet<MemberQuestionAnswerContract>();
            this.RefSourceQuestionAnswers = new HashSet<RefSourceQuestionAnswerContract>();
        }
        [DataMember]
        public int RefAnswerID { get; set; }
        [DataMember]
        public string StandardAnswerText { get; set; }
        [DataMember]
        public bool IsRefAnswerActive { get; set; }
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
        public virtual ICollection<RefSourceQuestionAnswerContract> RefSourceQuestionAnswers { get; set; }
    }

}