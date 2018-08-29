using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SALTShaker.DAL.DataContracts
{
    public class ActivityTypeModel
    {
        public int ActivityTypeId { get; set; }
        public string ActivityTypeName { get; set; }
        public string ActivityTypeDescription { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
    }
}
