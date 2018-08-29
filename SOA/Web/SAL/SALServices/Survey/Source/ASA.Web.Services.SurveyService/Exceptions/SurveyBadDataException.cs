using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ASA.Web.Services.SurveyService.Exceptions
{
    [Serializable()]
    class SurveyBadDataException : System.Exception
    {
        public SurveyBadDataException() : base() { }
        public SurveyBadDataException(string message) : base(message) { }
        public SurveyBadDataException(string message, System.Exception inner) : base(message, inner) { }

        // A constructor is needed for serialization when an
        // exception propagates from a remoting server to the client. 
        protected SurveyBadDataException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) { }

    }
}
