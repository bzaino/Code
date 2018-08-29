using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ASA.Web.WTF
{
    public interface IMemberCredentials
    {
        IMemberAccountData Account { get; }
        //String Username { get; }
        //Object MemberId { get; }
    }
}
