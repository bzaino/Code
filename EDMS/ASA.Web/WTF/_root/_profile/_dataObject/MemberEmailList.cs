using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASA.Web.Collections;

namespace ASA.Web.WTF
{
    public class MemberEmailList<T> : ContextDataObjectList<T>, IMemberEmailList<T> where T : IMemberEmail, new()
    {
        public MemberEmailList() : base() { }

    }
}
