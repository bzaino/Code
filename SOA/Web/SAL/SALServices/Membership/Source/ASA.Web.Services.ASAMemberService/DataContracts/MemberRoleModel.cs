using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASA.Web.Services.Common;
using ASA.Web.Collections;

namespace ASA.Web.Services.ASAMemberService.DataContracts
{
    public class MemberRoleModel: BaseWebModel
    {
        public MemberRoleModel() : base() { }

        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public string RoleDescription { get; set; }
        public bool IsMemberRoleActive { get; set; }

    }
}
