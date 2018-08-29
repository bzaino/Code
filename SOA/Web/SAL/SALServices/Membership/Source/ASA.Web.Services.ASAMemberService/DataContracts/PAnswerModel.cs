using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ASA.Web.Services.ASAMemberService.DataContracts
{
    public class PAnswerModel
    {
        public int AnsID { get; set; }
        public string AnsName { get; set; }
        public string AnsDescription { get; set; }
        public bool IsProfileAnswerActive { get; set; }
        public Nullable<short> ProfileAnswerDisplayOrder { get; set; }
        
    }
}
