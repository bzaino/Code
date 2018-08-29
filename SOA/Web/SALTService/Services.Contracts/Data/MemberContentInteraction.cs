using System;
using System.Collections.Generic;
using System.Runtime.Serialization;


namespace Asa.Salt.Web.Services.Contracts.Data
{
    [DataContract(IsReference = true)]
    [KnownType(typeof(MemberContract))]
    [KnownType(typeof(RefContentInteractionTypeContract))]
    public class MemberContentInteractionContract : IEntity 
    {
        [DataMember]
        public int MemberContentInteractionID { get; set; }
        [DataMember]
        public Nullable<int> MemberID { get; set; }
        [DataMember]
        public string ContentID { get; set; }
        [DataMember]
        public int RefContentInteractionTypeID { get; set; }
        [DataMember]
        public string MemberContentInteractionValue { get; set; }
        [DataMember]
        public System.DateTime InteractionDate { get; set; }
        [DataMember]
        public string CreatedBy { get; set; }
        [DataMember]
        public System.DateTime CreatedDate { get; set; }
        [DataMember]
        public string ModifiedBy { get; set; }
        [DataMember]
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        [DataMember]
        public string MemberContentComment { get; set; }
    
        [DataMember]
        public virtual MemberContract Member { get; set; }
        [DataMember]
        public virtual RefContentInteractionTypeContract RefContentInteractionType { get; set; }
    }
    
}
