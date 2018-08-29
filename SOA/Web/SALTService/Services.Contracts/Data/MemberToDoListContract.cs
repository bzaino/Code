using System;
using System.Runtime.Serialization;

namespace Asa.Salt.Web.Services.Contracts.Data
{
    [DataContract(IsReference = true)]
    public class MemberToDoListContract : IEntity
    {
        [DataMember]
        public int MemberToDoListID { get; set; }
        [DataMember]
        public int MemberID { get; set; }
        [DataMember]
        public int RefToDoTypeID { get; set; }
        [DataMember]
        public int RefToDoStatusID { get; set; }
        [DataMember]
        public string ContentID { get; set; }
        [DataMember]
        public string CreatedBy { get; set; }
        [DataMember]
        public System.DateTime CreatedDate { get; set; }
        [DataMember]
        public string ModifiedBy { get; set; }
        [DataMember]
        public Nullable<System.DateTime> ModifiedDate { get; set; }
    }
}
