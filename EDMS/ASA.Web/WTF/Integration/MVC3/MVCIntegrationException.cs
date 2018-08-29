using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ASA.Web.WTF.Integration.MVC3
{
    [Serializable]
    public class MVCIntegrationException : Exception
    {
        public MVCIntegrationException() { }
        public MVCIntegrationException(string message) : base(message) { }
        public MVCIntegrationException(string message, Exception inner) : base(message, inner) { }
    }
}
