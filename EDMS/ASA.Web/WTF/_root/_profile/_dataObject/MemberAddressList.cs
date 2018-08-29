using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASA.Web.Collections;

namespace ASA.Web.WTF
{
    public class MemberAddressList<T> : ContextDataObjectList<T>, IMemberAddressList<T> where T : IMemberAddress, new()
    {
        public MemberAddressList() : base() {}
    }
}
