using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ASA.Web.WTF
{
    public class MemberCredentials : IMemberCredentials
    {
        public IMemberAccountData Account
        { get; internal set; }
    }
}
