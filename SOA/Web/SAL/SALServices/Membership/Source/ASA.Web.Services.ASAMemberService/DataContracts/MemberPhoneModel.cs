using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ASA.Web.Services.Common;

namespace ASA.Web.Services.ASAMemberService.DataContracts
{
    public class MemberPhoneModel : BaseWebModel
    {
        public MemberPhoneModel() : base() { }
        public MemberPhoneModel(Boolean newRecord = false) : base(newRecord) { }

        public string PhoneKey { get; set; }
        public bool IsPrimary { get; set; }

        [DisplayName("Phone Number")]
        public string PhoneNumber { get; set; }

        [DisplayName("Type")]
        public string Type { get; set; }

        [DisplayName("Type ID")]
        public string TypeID { get; set; }
    }
}
