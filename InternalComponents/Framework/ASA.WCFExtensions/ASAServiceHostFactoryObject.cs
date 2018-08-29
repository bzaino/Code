///////////////////////////////////////////////////////////////////////////////
//
//  WorkFile Name:	ASAServiceHostFactoryObject.cs in ASA.WCFExtensions
//
//  Description:
//  This class represents a custom WCF ServiceHostFactory used to create the 
//  custom ServiceHost implmented in ASAServiceHost programmatically.
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
using ASA.Log.ServiceLogger;

namespace ASA.WCFExtensions
{
    /// <summary>
    /// The ASAServiceHostFactoryObject class extends from 
    /// Spring.ServiceModel.Activation.ServiceHostFactoryObject class (which extends 
    /// the standard WCF ServiceHostFactory class).  This class overrides 
    /// Spring.NET’s ServiceHostFactoryObject methods such that when ASAServiceHost objects are created programmatically by the ASAServiceHostFactoryObject it creates/retrieves ASAServiceHost objects instead of SpringServiceHost objects.  
    /// </summary>
    public class ASAServiceHostFactoryObject : Spring.ServiceModel.Activation.ServiceHostFactoryObject
    {
        private static readonly IASALog LOG = ASALogManager.GetLogger(typeof(ASAServiceHostFactoryObject));

        /// <summary>
        /// The <see cref="ASAWebService.WCFExtensions.ASAServiceHost" /> instance managed by this factory.
        /// </summary>
        protected ASAServiceHost asaServiceHost;

        public ASAServiceHostFactoryObject()
            : base()
        {
        }

        #region IFactoryObject Members

        /// <summary>
        /// Return a <see cref="ASAWebService.WCFExtensions.ASAServiceHost" /> instance 
        /// managed by this factory.
        /// </summary>
        /// <returns>
        /// An instance of <see cref="ASAWebService.WCFExtensions.ASAServiceHost" /> 
        /// managed by this factory.
        /// </returns>
        public override object GetObject()
        {
            return asaServiceHost;
        }

        /// <summary>
        /// Return the <see cref="System.Type"/> of object that this
        /// <see cref="Spring.Objects.Factory.IFactoryObject"/> creates.
        /// </summary>
        public override Type ObjectType
        {
            get { return typeof(ASAServiceHost); }
        }

        #endregion

        #region IInitializingObject Members

        /// <summary>
        /// Publish the object.
        /// </summary>
        public override void AfterPropertiesSet()
        {
            ValidateConfiguration();

            asaServiceHost = new ASAServiceHost(TargetName, objectFactory, BaseAddresses);

            asaServiceHost.Open();

            #region Instrumentation

            if (LOG.IsInfoEnabled)
            {
                LOG.Info(String.Format("The service '{0}' is ready and can now be accessed.", TargetName));
            }

            #endregion
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Close the ASAServiceHost
        /// </summary>
        public new void Dispose()
        {
            if (asaServiceHost != null)
            {
                asaServiceHost.Close();
            }
        }

        #endregion

    }
}
