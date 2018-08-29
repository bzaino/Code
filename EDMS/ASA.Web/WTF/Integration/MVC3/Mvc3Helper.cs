using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ASA.Web.WTF.Integration.MVC3
{
    /// <summary>
    /// Helper class for WTF.Integration.MVC3 classes.
    /// </summary>
    /// <remarks>Helper class initially created for SWD-5581 (cmak)</remarks>
    public class Mvc3Helper
    {
        /// <summary>
        /// Evalutes whether exception is an HTTP Request.Path exception.  Note that exception message string compare is case sensitive.
        /// </summary>
        /// <param name="e">Exception to evaluate</param>
        /// <returns>True if exception is a HttpException and the message includes "A potentially dangerous Request.Path value was detected from the client"</returns>
        /// <remarks>Helper method created for SWD-5581 (cmak)</remarks>
        public Boolean IsRequestPathException(Exception e)
        {
            if ((e.GetType() == typeof(System.Web.HttpException)) &&
                (e.Message.Contains("A potentially dangerous Request.Path value was detected from the client")))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
