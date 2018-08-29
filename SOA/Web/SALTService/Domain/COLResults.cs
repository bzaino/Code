using System;
using System.Collections.Generic;

namespace Asa.Salt.Web.Services.Domain
{
    public class COLResults
    {
        public decimal ComparableSalary { get; set; }
        public decimal IncomeToMaintain { get; set; }
        public decimal IncomeToSustain { get; set; }
        public decimal Groceries { get; set; }
        public decimal Housing { get; set; }
        public decimal Utilities { get; set; }
        public decimal Transportation { get; set; }
        public decimal Health { get; set; }
        public decimal Miscellaneous { get; set; }
    }
}
