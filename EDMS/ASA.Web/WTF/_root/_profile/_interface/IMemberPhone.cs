using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASA.Web.Collections;

namespace ASA.Web.WTF
{
    public interface IMemberPhone : IPrimary, IContextDataObject
    {
        String PhoneNumber  { get; }
        String PhoneType { get; }
        Boolean PassedValidation { get; }
        DateTime? PassedValidationOn { get; }

    }
}
