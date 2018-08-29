using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Asa.Salt.Web.Common.Types.Enums;

namespace ASA.Web.Services.ASAMemberService.DataContracts
{
    public class RegistrationResultModel
    {
        public MemberUpdateStatus CreateStatus { get; set; }
        public ASAMemberModel Member { get; set; }
    }
}
