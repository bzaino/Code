#region Imports

using System;
using System.Runtime.Serialization;
using System.Text;
#endregion

namespace ASA.ErrorHandling
{
    /// <summary>
    /// Root of the hierarchy of ASA exceptions
    /// </summary>
    /// <author>OS</author>

    [Serializable]
    public class ASADemogBusinessException : ASAException
    {

        public ASADemogBusinessException(string message, string code, string sServiceName)
            : base(message, code, sServiceName)
        {
        }

        public ASADemogBusinessException(string sObjectID, string sRelatedObjectType, string sObjectTypeName, string sFunctionName, string sServiceName)
            : base(sObjectID, sRelatedObjectType, sObjectTypeName, sFunctionName, sServiceName)
        {
        }

        /// <summary>
        /// Creates a new instance of the
        /// <see cref="ASADemogBusinessException"/> class.
        /// </summary>
        /// <param name="message">
        /// A message about the exception.
        /// </param>
        public ASADemogBusinessException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Creates a new instance of the
        /// <see cref="ASADemogBusinessException"/> class.
        /// </summary>
        /// <param name="message">
        /// A message about the exception.
        /// </param>
        /// <param name="sServiceName">
        /// service throwing the exception.
        /// </param>
        public ASADemogBusinessException(string message, string sServiceName)
            : base(message, sServiceName)
        {
        }

        /// <summary>
        /// Creates a new instance of the
        /// <see cref="ASADemogBusinessException"/> class.
        /// </summary>
        /// <param name="message">
        /// A message about the exception.
        /// </param>
        /// <param name="sServiceName">
        /// service throwing the exception.
        /// </param>
        /// <param name="rootCause">
        /// Original exception.
        /// </param>
        public ASADemogBusinessException(string message, string sServiceName, Exception rootCause)
            : base(message, sServiceName, rootCause)
        {
        }

        public ASADemogBusinessException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }  

    }

}
