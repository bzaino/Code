using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ASA.Web.Collections
{
    public interface IPrimaryObjectList<T> : IList<T> where T : IPrimary, new()
    {
        T Primary { get; }
        T PrimaryOrDefault { get; }
        void SetPrimary(T t);
        Boolean HasPrimary { get; }
    }
}