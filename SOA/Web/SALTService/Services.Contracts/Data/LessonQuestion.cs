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
    [KnownType(typeof(LessonQuestionAttributeContract))]
    [KnownType(typeof(LessonStepContract))]
    public class LessonQuestionContract : IEntity 
    {
        public LessonQuestionContract()
        {
            this.LessonQuestionAttributes = new HashSet<LessonQuestionAttributeContract>();
        }
    
        [DataMember]
        public int LessonQuestionId { get; set; }
        [DataMember]
        public string CreatedBy { get; set; }
        [DataMember]
        public System.DateTime CreatedDate { get; set; }
        [DataMember]
        public string ModifiedBy { get; set; }
        [DataMember]
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        [DataMember]
        public int LessonStepId { get; set; }
        [DataMember]
        public string Question { get; set; }
        [DataMember]
        public int OrderNumber { get; set; }
        [DataMember]
        public bool HasMultipleValues { get; set; }
    
        [DataMember]
        public virtual ICollection<LessonQuestionAttributeContract> LessonQuestionAttributes { get; set; }
        [DataMember]
        public virtual LessonStepContract LessonStep { get; set; }
    }
    
}
