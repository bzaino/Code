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


namespace Services.Contracts.Data
{
    [DataContract(IsReference = true)]
    [KnownType(typeof(MemberContract))]
    [KnownType(typeof(RefSchoolCourseGradeLevelContract))]
    public partial class RefGradeLevelContract : IEntity 
    {
        public RefGradeLevelContract()
        {
            this.Members = new HashSet<MemberContract>();
            this.RefSchoolCourseGradeLevels = new HashSet<RefSchoolCourseGradeLevelContract>();
        }
    
        [DataMember]
        public int RefGradeLevelID { get; set; }
        [DataMember]
        public string GradeLevelCode { get; set; }
        [DataMember]
        public string GradeLevelDescription { get; set; }
        [DataMember]
        public Nullable<System.Guid> GradeLevelExternalID { get; set; }
        [DataMember]
        public string CreatedBy { get; set; }
        [DataMember]
        public System.DateTime CreatedDate { get; set; }
        [DataMember]
        public string ModifiedBy { get; set; }
        [DataMember]
        public Nullable<System.DateTime> ModifiedDate { get; set; }
    
        [DataMember]
        public virtual ICollection<MemberContract> Members { get; set; }
        [DataMember]
        public virtual ICollection<RefSchoolCourseGradeLevelContract> RefSchoolCourseGradeLevels { get; set; }
    }
    
}
