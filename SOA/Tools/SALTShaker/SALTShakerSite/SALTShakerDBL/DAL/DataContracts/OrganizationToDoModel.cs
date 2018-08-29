using System;

namespace SALTShaker.DAL.DataContracts
{
    public class OrganizationToDoModel
    {
        public string ContentId { get; set; }
        public string Headline { get; set; }
        public int OrganizationId { get; set; }
        public int TypeId { get; set; }
        public int StatusId { get; set; }
        public Nullable<System.DateTime> DueDate { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
    }
}
