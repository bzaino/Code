using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SALTShaker.BLL
{
    public class ExceptionMessageException : Exception
    {
        public ExceptionMessageException(string message)
            : base(message)
        {
        }
    }
}