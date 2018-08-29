using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ASA.Web.Services.AlertService.Exceptions
{
    class AlertOperationException : System.Exception
    {
        public AlertOperationException() : base() { }
        public AlertOperationException(string message) : base(message) { }
        public AlertOperationException(string message, System.Exception inner) : base(message, inner) { }
    }
}
