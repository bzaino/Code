using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ASA.Web.Services.SurveyService.Exceptions
{
    [Serializable()]
    class SurveyMappingException : System.Exception
    {
        public SurveyMappingException() : base() { }
        public SurveyMappingException(string message) : base(message) { }
        public SurveyMappingException(string message, System.Exception inner) : base(message, inner) { }

        // A constructor is needed for serialization when an
        // exception propagates from a remoting server to the client. 
        protected SurveyMappingException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) { }

    }
}
