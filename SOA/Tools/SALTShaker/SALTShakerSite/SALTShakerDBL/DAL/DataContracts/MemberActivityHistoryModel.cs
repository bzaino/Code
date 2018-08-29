using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SALTShaker.DAL.DataContracts
{
    public class MemberActivityHistoryModel
    {
        public int MemberActivityHistoryID { get; set; }
        public int MemberID { get; set; }
        public int RefActivityTypeID { get; set; }
        public string RefActivityTypeName { get; set; }
        public Nullable<System.DateTime> ActivityDate { get; set; }
        public string AdditionalDetails { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public virtual ActivityTypeModel RefActivityType { get; set; }
    }
}
