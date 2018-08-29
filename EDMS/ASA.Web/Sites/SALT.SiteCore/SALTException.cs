using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ASA.Web.Sites.SALT
{
    [Serializable]
    public class SALTException : Exception
    {
        public SALTException() { }
        public SALTException(string message) : base(message) { }
        public SALTException(string message, Exception inner) : base(message, inner) { }
    }
}
