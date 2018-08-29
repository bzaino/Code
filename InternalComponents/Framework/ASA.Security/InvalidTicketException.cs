using System;
using System.Collections.Generic;
using System.Text;

namespace ASA.Security
{
    public class InvalidTicketException : ApplicationException
    {
        public InvalidTicketException(string Message) : base(Message)
        {
        }
    }
}
