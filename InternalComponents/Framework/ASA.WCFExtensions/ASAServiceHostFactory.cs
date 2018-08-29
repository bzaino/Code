///////////////////////////////////////////////////////////////////////////////
//
//  WorkFile Name:	ASAServiceHostFactory.cs in ASA.WCFExtensions
//
//  Description:
//  This class represents a custom WCF ServiceHostFactory that creates the 
//  custom ServiceHost implmented in ASAServiceHost
//  To use this, the svc file for the service should have a 
//  Factory="ASAWebService.WCFExtensions.ASAServiceHostFactory" added to the 
//  <%@ServiceHost tag.
//
//  ASA Proprietary Information
//
///////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using Spring.ServiceModel.Activation;
using Spring.Util;
using Spring.Context.Support;
using Spring.Context;
using System.ServiceModel.Activation;

namespace ASA.WCFExtensions
{
    /// <summary>
    /// The ASAServiceHostFactory class extends from Spring.ServiceModel.Activation.ServiceHostFactory
    /// class (which extends the standard WCF ServiceHostFactory class).  It extends 
    /// ServiceHostFactory so that dependency injection can be used in ASA’s hosted services.  
    /// </summary>
    public class ASAServiceHostFactory : Spring.ServiceModel.Activation.ServiceHostFactory 
    {
        /// <summary>
        /// This method will be used to create a service host of type ASAServiceHost
        /// </summary>
        /// <param name="reference"></param>
        /// <param name="baseAddresses"></param>
        /// <returns></returns>
        public override System.ServiceModel.ServiceHostBase CreateServiceHost(string reference, Uri[] baseAddresses)
        {
            if (StringUtils.IsNullOrEmpty(reference))
            {
                return (new ASAServiceHost(reference, baseAddresses));
            }

            IApplicationContext applicationContext = ContextRegistry.GetContext();
            if (applicationContext.ContainsObject(reference))
            {
                return new ASAServiceHost(reference, applicationContext, baseAddresses);
            }

            return (new ASAServiceHost(reference, baseAddresses));
        }
    }

}
///////////////////////////////////////////////////////////////////////////////
//
//	$Log: $
///////////////////////////////////////////////////////////////////////////////
