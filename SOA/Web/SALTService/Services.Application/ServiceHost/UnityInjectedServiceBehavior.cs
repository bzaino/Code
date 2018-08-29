using System.Collections.ObjectModel;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using Microsoft.Practices.Unity;

namespace Asa.Salt.Web.Services.Application.ServiceHost
{
    /// <summary>
    /// 
    /// </summary>
    public class UnityInjectedServiceBehavior : IServiceBehavior
    {
        /// <summary>
        /// The unity container
        /// </summary>
        private readonly IUnityContainer _container;

        /// <summary>
        /// The instance provider using unity
        /// </summary>
        private UnityInjectedInstanceProvider _instanceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnityInjectedServiceBehavior"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        public UnityInjectedServiceBehavior(IUnityContainer container)
     {
          _container = container;
      }

        /// <summary>
        /// Provides the ability to inspect the service host and the service description to confirm that the service can run successfully.
        /// </summary>
        /// <param name="serviceDescription">The service description.</param>
        /// <param name="serviceHostBase">The service host that is currently being constructed.</param>
        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
        }

        /// <summary>
        /// Provides the ability to pass custom data to binding elements to support the contract implementation.
        /// </summary>
        /// <param name="serviceDescription">The service description of the service.</param>
        /// <param name="serviceHostBase">The host of the service.</param>
        /// <param name="endpoints">The service endpoints.</param>
        /// <param name="bindingParameters">Custom objects to which binding elements have access.</param>
        public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters)
        {
        }

        /// <summary>
        /// Provides the ability to change run-time property values or insert custom extension objects such as error handlers, message or parameter interceptors, security extensions, and other custom extension objects.
        /// </summary>
        /// <param name="serviceDescription">The service description.</param>
        /// <param name="serviceHostBase">The host that is currently being built.</param>
        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            foreach (var cdb in serviceHostBase.ChannelDispatchers)
            {
                var cd = cdb as ChannelDispatcher;
                if (cd != null)
                {
                    cd.ErrorHandlers.Add(_container.Resolve<IErrorHandler>());
                    foreach (var ed in cd.Endpoints)
                    {
                        _instanceProvider = new UnityInjectedInstanceProvider(_container,serviceDescription.ServiceType);
                        ed.DispatchRuntime.InstanceProvider = _instanceProvider;
                    }
                }
            }
        }
    }
}