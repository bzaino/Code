using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ASA.Web.Services.AlertService.Exceptions
{
    class AlertBadDataException : System.Exception
    {
        public AlertBadDataException() : base() { }
        public AlertBadDataException(string message) : base(message) { }
        public AlertBadDataException(string message, System.Exception inner) : base(message, inner) { }
    }
}
