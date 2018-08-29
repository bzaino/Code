using System;
using System.ComponentModel.DataAnnotations;

namespace ASA.Web.Services.ASAMemberService.DataContracts
{
    public class MemberToDoModel
    {
        public int MemberToDoListID { get; set; }
        [Required]
        public int MemberID { get; set; }
        public int RefToDoTypeID { get; set; }
        public int RefToDoStatusID { get; set; }
        [Required]
        public string ContentID { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
    }
}
