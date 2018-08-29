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
    [KnownType(typeof(MemberContract))]
    public class EnrollmentStatusContract : IEntity 
    {
        public EnrollmentStatusContract()
        {
            this.Members = new HashSet<MemberContract>();
        }
    
        [DataMember]
        public int EnrollmentStatusId { get; set; }
        [DataMember]
        public string EnrollmentStatusCode { get; set; }
        [DataMember]
        public string EnrollmentStatusDescription { get; set; }
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
    }
    
}
