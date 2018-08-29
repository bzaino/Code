using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASA.Web.Collections;
using ASA.Web.WTF;

namespace ASA.Web.WTF
{
    public class MemberPhoneList<T> : ContextDataObjectList<T>, IMemberPhoneList<T> where T : IMemberPhone, new()
    {
        public MemberPhoneList() : base() { }
    }
}
