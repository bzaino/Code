using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ASA.Web.WTF
{
    public interface IContextActionValidator
    {
        Boolean Validate(
            IContextDataObject oldState, 
            IContextDataObject newState, 
            Dictionary<Object, Object> validationParams = null);

    }
}
