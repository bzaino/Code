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
    [KnownType(typeof(LessonContract))]
    [KnownType(typeof(MemberContract))]
    [KnownType(typeof(LessonQuestionResponseContract))]
    public class MemberLessonContract : IEntity 
    {
        public MemberLessonContract()
        {
            this.LessonQuestionResponses = new HashSet<LessonQuestionResponseContract>();
        }
    
        [DataMember]
        public int MemberLessonId { get; set; }
        [DataMember]
        public Nullable<int> MemberId { get; set; }
        [DataMember]
        public int LessonId { get; set; }
        [DataMember]
        public string CreatedBy { get; set; }
        [DataMember]
        public System.DateTime CreatedDate { get; set; }
        [DataMember]
        public string ModifiedBy { get; set; }
        [DataMember]
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        [DataMember]
        public Nullable<int> CurrentStep { get; set; }
        [DataMember]
        public int LessonUserId { get; set; }
    
        [DataMember]
        public virtual LessonContract Lesson { get; set; }
        [DataMember]
        public virtual MemberContract Member { get; set; }
        [DataMember]
        public virtual ICollection<LessonQuestionResponseContract> LessonQuestionResponses { get; set; }
    }
    
}
