using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Asa.Salt.Web.Services.Logging;

namespace Asa.Salt.Web.Services.Application.ServiceHost
{
    public class SaltServiceErrorHandler : System.ServiceModel.Dispatcher.IErrorHandler
    {

        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILog _log;

        /// <summary>
        /// Initializes a new instance of the <see cref="SaltServiceErrorHandler"/> class.
        /// </summary>
        /// <param name="log">The log.</param>
        public SaltServiceErrorHandler(ILog log)
        {
            _log = log;
        }

        /// <summary>
        /// Enables the creation of a custom <see cref="T:System.ServiceModel.FaultException`1" /> that is returned from an exception in the course of a service method.
        /// </summary>
        /// <param name="error">The <see cref="T:System.Exception" /> object thrown in the course of the service operation.</param>
        /// <param name="version">The SOAP version of the message.</param>
        /// <param name="fault">The <see cref="T:System.ServiceModel.Channels.Message" /> object that is returned to the client, or service, in the duplex case.</param>
        public void ProvideFault(Exception error, MessageVersion version, ref Message fault)
        {
            var exception = new FaultException(error.Message);
            var messageFault = exception.CreateMessageFault();
            // we are only returning a generic fault exception, therefore using an empty action.
            fault = Message.CreateMessage(version,messageFault,string.Empty);
        }

        /// <summary>
        /// Enables error-related processing and returns a value that indicates whether the dispatcher aborts the session and the instance context in certain cases.
        /// </summary>
        /// <param name="error">The exception thrown during processing.</param>
        /// <returns>
        /// true if  should not abort the session (if there is one) and instance context if the instance context is not <see cref="F:System.ServiceModel.InstanceContextMode.Single" />; otherwise, false. The default is false.
        /// </returns>
        public bool HandleError(Exception error)
        {
            _log.Error(error.Message, error);

            return true;
        }

    }
}