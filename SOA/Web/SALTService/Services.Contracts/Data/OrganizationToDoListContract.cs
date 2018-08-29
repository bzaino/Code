using System;
using System.Runtime.Serialization;

namespace Asa.Salt.Web.Services.Contracts.Data
{
    [DataContract(IsReference = true)]
    public class OrganizationToDoListContract : IEntity
    {
        [DataMember]
        public int OrganizationToDoListID { get; set; }
        [DataMember]
        public int RefOrganizationID { get; set; }
        [DataMember]
        public int RefToDoTypeID { get; set; }
        [DataMember]
        public int RefToDoStatusID { get; set; }
        [DataMember]
        public string Headline { get; set; }
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
        [DataMember]
        public Nullable<System.DateTime> DueDate { get; set; }
    }
}
