using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ASA.Web.Services.ReminderService.Exceptions
{
    [Serializable()]
    class ReminderMappingException : System.Exception
    {
        public ReminderMappingException() : base() { }
        public ReminderMappingException(string message) : base(message) { }
        public ReminderMappingException(string message, System.Exception inner) : base(message, inner) { }

        // A constructor is needed for serialization when an
        // exception propagates from a remoting server to the client. 
        protected ReminderMappingException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) { }

    }
}
