using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Asa.Salt.Web.Services.Contracts.Data
{
    public class COLResultsContract
    {
        [DataMember]
        public decimal ComparableSalary { get; set; }
        [DataMember]
        public decimal IncomeToMaintain { get; set; }
        [DataMember]
        public decimal IncomeToSustain { get; set; }
        [DataMember]
        public decimal Groceries { get; set; }
        [DataMember]
        public decimal Housing { get; set; }
        [DataMember]
        public decimal Utilities { get; set; }
        [DataMember]
        public decimal Transportation { get; set; }
        [DataMember]
        public decimal Health { get; set; }
        [DataMember]
        public decimal Miscellaneous { get; set; }
    }
}