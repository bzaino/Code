using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ASA.Web.Services.SurveyService.Exceptions
{
    [Serializable()]
    class SurveyOperationException : System.Exception
    {
        public SurveyOperationException() : base() { }
        public SurveyOperationException(string message) : base(message) { }
        public SurveyOperationException(string message, System.Exception inner) : base(message, inner) { }

        // A constructor is needed for serialization when an
        // exception propagates from a remoting server to the client. 
        protected SurveyOperationException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) { }

    }
}
