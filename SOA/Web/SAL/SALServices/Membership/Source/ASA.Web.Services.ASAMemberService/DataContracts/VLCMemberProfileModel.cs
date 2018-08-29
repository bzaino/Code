using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace ASA.Web.Services.ASAMemberService.DataContracts
{
    public class VLCMemberProfileModel
    {
        
        [Required]
        public int MemberID { get; set; }
        public string EnrollmentStatus { get; set; }
        public Nullable<System.DateTime> GraduationDate { get; set; }
        public string RepaymentStatus { get; set; }
        public Nullable<int> AdjustedGrossIncome { get; set; }
        public Nullable<byte> FamilySize { get; set; }
        public string StateOfResidence { get; set; }
        public string TaxFilingStatus { get; set; }

    }
}
