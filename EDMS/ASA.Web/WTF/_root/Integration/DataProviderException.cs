using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ASA.Web.WTF.Integration
{
    [Serializable]
    public class DataProviderException : Exception
    {
        public DataProviderException() { }
        public DataProviderException(string message) : base(message) { }
        public DataProviderException(string message, Exception inner) : base(message, inner) { }
    }
}
