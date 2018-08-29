using System;
using System.ServiceModel.Activation;
using Asa.Salt.Web.Services.Application.Implementation.Mapping;
using Asa.Salt.Web.Services.Data.Model.Database;

namespace Asa.Salt.Web.Services.Application.ServiceHost
{
    public class SaltServiceHostFactory : ServiceHostFactory
    {

        /// <summary>
        /// Creates a <see cref="T:System.ServiceModel.ServiceHost" /> for a specified type of service with a specific base address.
        /// </summary>
        /// <param name="serviceType">Specifies the type of service to host.</param>
        /// <param name="baseAddresses">The <see cref="T:System.Array" /> of type <see cref="T:System.Uri" /> that contains the base addresses for the service hosted.</param>
        /// <returns>
        /// A <see cref="T:System.ServiceModel.ServiceHost" /> for the type of service specified with a specific base address.
        /// </returns>
        protected override System.ServiceModel.ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            AutoMapperBootStrapper.ConfigureAutoMapper();

            var container = UnityBootStrapper.GetUnityConfiguration();
           
            var serviceHost = new SaltServiceHost(container, serviceType, baseAddresses);

            return serviceHost;

        }
    }
}
