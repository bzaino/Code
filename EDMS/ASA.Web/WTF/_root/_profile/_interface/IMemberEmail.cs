using System;
using ASA.Web.Collections;

namespace ASA.Web.WTF
{
    public interface IMemberEmail : IPrimary, IContextDataObject
    {
        String Address { get; }
        Boolean PassedValidation { get; }
        DateTime? PassedValidationOn { get; }
    }
}