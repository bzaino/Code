using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ASA.Web.Services.ASAMemberService
{
    public class SourceType
    {
        //QC 4300 is adding these three ref data values for the cst_src_code
        // to represent users that are registered through the web.
        public const string SELF_REGISTERED_NO_MATCH = "Self-registered no match";
        public const string SELF_REGISTERED_MATCHED = "Self-registered matched";
        public const string ACTIVATION_EMAIL = "Activation email";
    }
}
