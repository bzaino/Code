using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Asa.Salt.Web.Services.Contracts.Data
{
    [DataContract(IsReference = true)]
    //[KnownType(typeof(RefStateContract))]
    public class RefGeographicIndexContract : IEntity 
    {
        public RefGeographicIndexContract()
        {
            //    this.RefStates = new HashSet<RefStateContract>();
        }

        [DataMember]
        public int RefGeographicIndexID { get; set; }
        [DataMember]
        public int RefStateID { get; set; }
        [DataMember]
        public string MetroName { get; set; }
        [DataMember]
        public string UrbanAreaName { get; set; }
        [DataMember]
        public string CreatedBy { get; set; }
        [DataMember]
        public System.DateTime CreatedDate { get; set; }
        [DataMember]
        public string ModifiedBy { get; set; }
        [DataMember]
        public Nullable<System.DateTime> ModifiedDate { get; set; }
    
        //[DataMember]
        //public virtual ICollection<RefStateContract> RefStates { get; set; }
    }
    
}
