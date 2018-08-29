using System;
using System.Runtime.Serialization;

namespace ASA.Web.WTF
{    
    [Serializable]
    public class WtfException : Exception
    {
        public WtfException() { }
        public WtfException(string message) : base(message) { }
        public WtfException(string message, Exception inner) : base(message, inner) { }
    }
}
