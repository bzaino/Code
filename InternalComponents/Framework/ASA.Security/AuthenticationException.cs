using System;
using System.Collections.Generic;
using System.Text;

namespace ASA.Security
{
    public class AuthenticationException : ApplicationException
    {
        public AuthenticationException(string Message) : base(Message)
        {
        }
    }
}
