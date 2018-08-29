using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace ASA.Web.Services.ASAMemberService.DataContracts
{
    public class MemberProductModel
    {
        [Required]
        public int MemberID { get; set; }
        public int RefProductID { get; set; }
        public string MemberProductValue { get; set; }
        public bool IsMemberProductActive { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }

        //public virtual Member Member { get; set; }
        //public virtual RefProduct RefProduct { get; set; }
    }
}
