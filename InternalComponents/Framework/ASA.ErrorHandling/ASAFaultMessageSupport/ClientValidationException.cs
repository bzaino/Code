
using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using System.Threading;

namespace ASA.ErrorHandling
{
    [Serializable]
    public class RequestClientValidationException : ASAException
    {
        public RequestClientValidationException(string validationErrorDetail)
            :
            base(validationErrorDetail)
        {
        }
    }

    [Serializable]
    public class ReplyClientValidationException : ASAException
    {
        public ReplyClientValidationException(string validationErrorDetail)
            :
            base(validationErrorDetail)
        {
        }
    }
}