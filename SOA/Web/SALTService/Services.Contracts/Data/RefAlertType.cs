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
    [KnownType(typeof(MemberAlertContract))]
    public partial class RefAlertTypeContract : IEntity 
    {
        public RefAlertTypeContract()
        {
            this.MemberAlerts = new HashSet<MemberAlertContract>();
        }
    
        [DataMember]
        public int RefAlertTypeID { get; set; }
        [DataMember]
        public string AlertTypeName { get; set; }
        [DataMember]
        public System.Guid ExternalID { get; set; }
        [DataMember]
        public string CreatedBy { get; set; }
        [DataMember]
        public System.DateTime CreatedDate { get; set; }
        [DataMember]
        public string ModifiedBy { get; set; }
        [DataMember]
        public Nullable<System.DateTime> ModifiedDate { get; set; }
    
        [DataMember]
        public virtual ICollection<MemberAlertContract> MemberAlerts { get; set; }
    }
    
}
