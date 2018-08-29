using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ASA.Web.WTF
{
    public interface IContextDataObjectList<T> : IList<T> where T : IContextDataObject, new()
    {
        Boolean Update(T dataItem);
    }
}
