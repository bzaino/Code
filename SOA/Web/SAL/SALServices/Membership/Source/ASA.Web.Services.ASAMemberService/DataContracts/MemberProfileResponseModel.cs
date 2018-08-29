using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using ASA.Web.Services.Common;

namespace ASA.Web.Services.ASAMemberService.DataContracts
{
    public class MemberProfileResponseModel : BaseWebModel
    {
        public int MemberID { get; set; }
        public int ProfileAnswerExternalID { get; set; }
        public string CustomValue { get; set; }
    }
}
