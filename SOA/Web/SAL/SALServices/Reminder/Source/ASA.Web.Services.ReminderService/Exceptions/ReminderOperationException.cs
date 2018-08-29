using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ASA.Web.Services.ReminderService.Exceptions
{
    [Serializable()]
    class ReminderOperationException : System.Exception
    {
        public ReminderOperationException() : base() { }
        public ReminderOperationException(string message) : base(message) { }
        public ReminderOperationException(string message, System.Exception inner) : base(message, inner) { }

        // A constructor is needed for serialization when an
        // exception propagates from a remoting server to the client. 
        protected ReminderOperationException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) { }

    }
}
