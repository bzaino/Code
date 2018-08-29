using System;
using Microsoft.Practices.Unity;

namespace Asa.Salt.Web.Services.Application.ServiceHost
{
    public class SaltServiceHost : System.ServiceModel.ServiceHost
    {
        /// <summary>
        /// The unity container
        /// </summary>
       private readonly IUnityContainer _unityContainer;

        /// <summary>
        /// Initializes a new instance of the <see cref="SaltServiceHost"/> class.
        /// </summary>
        /// <param name="unityContainer">The unity container.</param>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="baseAddresses">The base addresses.</param>
        public SaltServiceHost(IUnityContainer unityContainer, Type serviceType, Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
            _unityContainer = unityContainer;
        }

        /// <summary>
        /// Invoked during the transition of a communication object into the opening state.
        /// </summary>
        protected override void OnOpening()
        {
            base.OnOpening();

            if (this.Description.Behaviors.Find<UnityInjectedServiceBehavior>() == null)
            {
                this.Description.Behaviors.Add(new UnityInjectedServiceBehavior(this._unityContainer));
            }
        }
        protected override void ApplyConfiguration()
        {
            base.ApplyConfiguration();
        }

        /// <summary>
        /// Gets the service credentials,service authentication and authorization behavior for the hosted service.
        /// </summary>
        protected override void OnOpened()
        {
            base.OnOpened();
        }

        /// <summary>
        /// Invoked during the transition of a communication object into the closing state.
        /// </summary>
        protected override void OnClosing()
        {
            base.OnClosing();
        }
    }
}