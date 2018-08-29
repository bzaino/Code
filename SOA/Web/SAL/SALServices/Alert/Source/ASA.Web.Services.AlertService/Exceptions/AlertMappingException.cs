using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ASA.Web.Services.AlertService.Exceptions
{
    class AlertMappingException : System.Exception
    {
        public AlertMappingException() : base() { }
        public AlertMappingException(string message) : base(message) { }
        public AlertMappingException(string message, System.Exception inner) : base(message, inner) { }
    }
}
