using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASA.Web.Services.Common;

namespace ASA.Web.Services.ASAMemberService.DataContracts
{
    public class MemberProfileQAModel
    {
        public string QuestionName { get; set; }
        public int QuestionExternalId { get; set; }
        public string AnsName { get; set; }
        public string AnsDescription { get; set; }
        public int AnsExternalId { get; set; }
        public string CustomValue { get; set; }
    }
}