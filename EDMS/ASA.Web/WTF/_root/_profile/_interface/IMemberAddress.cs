using System;
using ASA.Web.Collections;

namespace ASA.Web.WTF
{
    public interface IMemberAddress : IMailAddress, IPrimary, IContextDataObject
    {
        DateTime? PassedValidationOn { get; }
        Boolean PassedValidation { get; }
    }
}