using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ASA.Web.WTF.Integration
{
    [Serializable]
    public class SecurityAdapterException : Exception
    {
        public SecurityAdapterException() { }
        public SecurityAdapterException(string message) : base(message) { }
        public SecurityAdapterException(string message, Exception inner) : base(message, inner) { }
    }
}
