using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ASA.Web.WTF
{
    public interface IContextActionValidationRequest<T> where T : IContextActionValidator
    {
        Dictionary<Object, Object> Params { get; }
        IContextDataObject OrignalState { get; }
        IContextDataObject NewState { get; }
        
        Boolean Process();

    }
}
