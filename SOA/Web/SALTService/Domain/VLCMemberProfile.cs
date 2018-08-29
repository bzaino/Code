using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Asa.Salt.Web.Services.Domain
{
    public class VLCMemberProfile
    {
        //public List<UserProfileDetail> Properties { get; set; }

        public int MemberID { get; set; }
        public Nullable<System.DateTime> GraduationDate { get; set; }
        public string RepaymentStatus { get; set; }
        public Nullable<int> AdjustedGrossIncome { get; set; }
        public Nullable<short> FamilySize { get; set; }
        public string StateOfResidence { get; set; }
        public string TaxFilingStatus { get; set; }
    }
}
