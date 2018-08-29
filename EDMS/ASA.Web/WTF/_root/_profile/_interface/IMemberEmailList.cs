using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASA.Web.Collections;

namespace ASA.Web.WTF
{
    public interface IMemberEmailList<T> : IContextDataObjectList<T>, IPrimaryObjectList<T>, IList<T> where T : IPrimary, IMemberEmail, new() { }
}
