using System;
using System.Collections.Generic;
using System.Text;

namespace ASA.ErrorHandling
{
    [Serializable]
    public class ServiceRequestValidationException : ASAException
    {
        public ServiceRequestValidationException(string validationErrorDetail)
            : base(validationErrorDetail)
        {
        }
    }

    [Serializable]
    public class ServiceReplyValidationException : ASAException
    {
        public ServiceReplyValidationException(string validationErrorDetail)
            : base(validationErrorDetail)
        {
        }
    }
    




    
}
