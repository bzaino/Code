using System.Collections.Generic;

using ASA.Web.Collections;

namespace ASA.Web.WTF
{
    public interface IMemberOrganizationList<T> : IContextDataObjectList<T>, IPrimaryObjectList<T>, IList<T> where T : IPrimary, IMemberOrganization, new() { }
}
