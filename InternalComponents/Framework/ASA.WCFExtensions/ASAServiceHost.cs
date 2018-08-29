///////////////////////////////////////////////////////////////////////////////
//
//  WorkFile Name:	ASAServiceHost.cs in ASA.WCFExtensions
//
//  Description:
//  This class implments a custom ServiceHost so that the service can be initialized
//  the first time the service is accessed
//
//  ASA Proprietary Information
//
///////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using ASA.Common;
using ASA.Log.ServiceLogger;
using ASA.ErrorHandling;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using Spring.ServiceModel;
using Spring.Context;

namespace ASA.WCFExtensions
{
    /// <summary>
    /// The ASAServiceHost class extends from Spring.ServiceModel.SpringServiceHost class 
    /// (which extends the standard WCF ServiceHost class).  It extends SpringServiceHost 
    /// so that dependency injection can be used in ASA’s hosted services.  
    /// </summary>
    public class ASAServiceHost : SpringServiceHost
    {
        public ASAServiceHost(string reference, params Uri[] baseAddresses)
            : base(reference, baseAddresses)
        {

        }

        public ASAServiceHost(string reference, Spring.Objects.Factory.IObjectFactory applicationContext, params Uri[] baseAddresses)
            : base(reference, applicationContext, baseAddresses)
        {

        }

        /// <summary>
        /// InitializeRuntime():  This method is called when WCF creates the host process.  
        /// It will be overridden to:
        ///  -- Initialize any internal components (like the Logger - if necessary) 
        ///     and log the starting of the hosting of the service.
        ///  -- Add custom behaviors to the ServiceHost by making calls to 
        ///     AddCustomAuthorizationBehavior() and AddCustomMessageInspectorBehavior()
        ///  -- Perform standard WCF service-host initialization with a call to base.InitializeRuntime()
        /// </summary>
        protected override void InitializeRuntime()
        {
            //Starting up service
            LifetimeEvents.Startup();

            //initialize Parameters with ASA settings
            Parameters.Instance.SectionName = "asa";

            //Add custom Behaviors
            AddCustomMessageInspectorBehavior();

            if (Parameters.Instance.EnableASAServiceRequestSchemaValidation == true
            || Parameters.Instance.EnableASAServiceReplySchemaValidation == true)
            {
                AddCustomSchemaValidation();
            }

            //Let base finish
            base.InitializeRuntime();

            //Add custom Error Handler
            if (Parameters.Instance.EnableASAFaultHandling == true)
                AddCustomFaultHandling();

        
            
        }

        private void AddCustomSchemaValidation()
        {
            try
            {
                
                bool validateRequest = Parameters.Instance.EnableASAServiceRequestSchemaValidation;
                bool validateReply = Parameters.Instance.EnableASAServiceReplySchemaValidation;
                string messageContractSchemasPath = Parameters.Instance.TargetMessageContractSchemasPath;

                this.Description.Behaviors.Add(new ASASchemaValidationServiceBehavior(messageContractSchemasPath, validateRequest, validateReply));
            }
            catch (ASAException exASA)
            {
                IASALog Log = ASALogManager.GetLogger(typeof(ASAServiceHost));
                Log.Error("Couldn't add ASAFaultErrorHandler to the ASAServiceHost", exASA);
                throw new ApplicationException("Couldn't add ASAFaultErrorHandler to the ASAServiceHost:" + exASA.Message +
                    " Error_code:" + exASA.ExceptionError_id + " Business Message:" + exASA.BusinessDescription);
            }
            catch (Exception ex)
            {
                IASALog Log = ASALogManager.GetLogger(typeof(ASAServiceHost));
                Log.Error("Couldn't add ASAFaultErrorHandler to the ASAServiceHost", ex);
                throw new ApplicationException("Couldn't add ASAFaultErrorHandler to the ASAServiceHost:" + ex.Message);
            }
        }

        /// <summary>
        /// This method will be used to add the ASADispatchMessageInspector behavior 
        /// to the Behaviors collection of the ServiceHost’s service description.
        /// </summary>
        public void AddCustomMessageInspectorBehavior()
        {
            try
            {
                //Add Custom Behavior to all services
                this.Description.Behaviors.Add(new ASADispatchMessageInspector());


            }
            catch (ASAException exASA)
            {
                IASALog Log = ASALogManager.GetLogger(typeof(ASAServiceHost));
                Log.Error("Couldn't add custom message inspection behavior", exASA);
                throw new ApplicationException("Couldn't add custom message inspection behavior:" + exASA.Message +
                    " Error_code:" + exASA.ExceptionError_id + " Business Message:" + exASA.BusinessDescription);
            }
            catch (Exception ex)
            {
                IASALog Log = ASALogManager.GetLogger(typeof(ASAServiceHost));
                Log.Error("Couldn't add custom message inspection behavior", ex);
                throw new ApplicationException("Couldn't add custom message inspection behavior:" + ex.Message);
            }
        }

        /// <summary>
        /// Add ASAFaultHandler to the ASAServiceHost
        /// </summary>
        public void AddCustomFaultHandling()
        {
            try
            {
                foreach (ChannelDispatcher dispatcher in    this.ChannelDispatchers)
                {
                    dispatcher.ErrorHandlers.Add(new ASAFaultErrorHandler());
                     
                }
            }
            catch (ASAException exASA)
            {
                IASALog Log = ASALogManager.GetLogger(typeof(ASAServiceHost));
                Log.Error("Couldn't add ASAFaultErrorHandler to the ASAServiceHost", exASA);
                throw new ApplicationException("Couldn't add ASAFaultErrorHandler to the ASAServiceHost:" + exASA.Message +
                    " Error_code:" + exASA.ExceptionError_id + " Business Message:" + exASA.BusinessDescription);
            }
            catch (Exception ex)
            {
                IASALog Log = ASALogManager.GetLogger(typeof(ASAServiceHost));
                Log.Error("Couldn't add ASAFaultErrorHandler to the ASAServiceHost", ex);
                throw new ApplicationException("Couldn't add ASAFaultErrorHandler to the ASAServiceHost:" + ex.Message);
            }
        }

        /// <summary>
        /// This method is called when WCF ends the host process.  
        /// It will be overridden to perform releasing of resources from the host, 
        /// and log the closing of the host.
        /// </summary>
        protected override void OnClosed()
        {
            //Service is closed
            LifetimeEvents.Close();
            //Base can now close
            base.OnClosed();
        }

        
    }
}
///////////////////////////////////////////////////////////////////////////////
//
//	$Log: $
///////////////////////////////////////////////////////////////////////////////
