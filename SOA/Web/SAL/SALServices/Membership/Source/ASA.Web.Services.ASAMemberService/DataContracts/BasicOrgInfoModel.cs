using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASA.Web.Services.Common;

namespace ASA.Web.Services.ASAMemberService.DataContracts
{
    public class BasicOrgInfoModel : BaseWebModel
    {
        public string OrgLogo { get; set; }
        public string OrgName { get; set; }
        public bool IsBranded { get; set; }
        public string OeCode { get; set; }
        public string OeBranch { get; set; }
        public string ExtOrgId { get; set; }
        public int OrgId { get; set; }
        public int SchoolType { get; set; }
        public bool IsSchool { get; set; }
    }
}
