using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ASA.Web.WTF
{
    /// <summary>
    /// Core Data object for the WTF framework. The IContext dataobject defines specific members for Keys and identifiers that are crtitical for the framework to ensure propery sync with provider's back end data store.
    /// </summary>
    public interface IContextDataObject
    {
        Object MemberId { get; } //represents the ID of the member that owns this object
        Object Id { get; } //represents the objects own ID
        Dictionary<String, Object> ProviderKeys { get; } //Context data objects are created data providers, this store allows the framework to keep track of other keys the providers might need to attach to the object. The framework will pass this key chain set in to the providers with any request. 
    }
}
