using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ASA.Web.Services.ASAMemberService.DataContracts
{
    public class EmailOptOutModel
    {
        public string EmailAddress { get; set; }
        public string OptOutKey { get; set; }
        public bool DeleteFlag { get; set; }
    }
}
