///////////////////////////////////////////////////////////////////////////////
//
//  WorkFile Name:	ASAClientMessageInspector.cs in ASA.WCFExtensions
//
//  Description:
//  This class intercepts each message being sent to a service from a .NET client.
//  The TID information associated with the call is audited upon sending the request
//  and receiving a reply.
//
//  ASA Proprietary Information
//
///////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Configuration;
using System.ServiceModel.Channels;
using System.Xml;
using ASA.Log.ServiceLogger;

namespace ASA.WCFExtensions
{
    /// <summary>
    /// The ASAClientMessageInspector class implements the IClientMessageInspector 
    /// and IEndpointBehavior interfaces, and extends the BehaviorExtensionElement class.  
    /// (The BehaviorExtensionElement class represents a configuration element that 
    /// contains sub-elements that specify behavior extensions.  This enables the user 
    /// to customize service or endpoint behaviors through the associated .config file.  
    /// In order for a client proxy to use this custom behavior, a reference to the 
    /// ASA.WCFExtensions package must be added to the client’s .NET project .  Then 
    /// the behavior should be added to the client’s associated Web.config or App.config 
    /// file.)) 
    /// </summary>
    public class ASAClientMessageInspector : BehaviorExtensionElement, IClientMessageInspector, IEndpointBehavior
    {
        static readonly IASALog _Log = ASALogManager.GetLogger(typeof(ASAClientMessageInspector));
        ServiceEndpoint svcEndpoint = null;

        #region IEndpointBehavior Members

        /// <summary>
        /// AddBindingParameters():  This method is intended to be used if you need to 
        /// add parameters to the BindingParameterCollection.  This allows those 
        /// parameters to be used to customize how the service channel is built.  
        /// For our purposes, we do not need this functionality, so this method will 
        /// not do anything in this implementation.    
        /// It is only here because the IEndpointBehavior requires it be implemented.
        /// </summary>
        /// <param name="serviceEndpoint"></param>
        /// <param name="bindingParameters"></param>
        public void AddBindingParameters(ServiceEndpoint serviceEndpoint, System.ServiceModel.Channels.BindingParameterCollection bindingParameters) 
        { }

        /// <summary>
        /// ApplyClientBehavior():  This method will be used to add the 
        /// ASAClientMessageInspector behavior to the MessageInspectors collection of 
        /// the client-runtime of the client proxy.
        /// </summary>
        /// <param name="serviceEndpoint"></param>
        /// <param name="behavior"></param>
        public void ApplyClientBehavior(ServiceEndpoint serviceEndpoint, System.ServiceModel.Dispatcher.ClientRuntime behavior) 
        {
            svcEndpoint = serviceEndpoint;
            _Log.InfoFormat("Adding ASAClientmessageInspector behavior to {0}", serviceEndpoint.Address.Uri);
            behavior.MessageInspectors.Add(this);
        }

        /// <summary>
        /// ApplyDispatchBehavior():  This method will not do anything.  
        /// It is only here because the IEndpointBehavior requires it be implemented.
        /// </summary>
        /// <param name="serviceEndpoint"></param>
        /// <param name="endpointDispatcher"></param>
        public void ApplyDispatchBehavior(ServiceEndpoint serviceEndpoint, System.ServiceModel.Dispatcher.EndpointDispatcher endpointDispatcher) 
        { }

        /// <summary>
        /// Validate():  This method can be used to check the client proxy endpoint 
        /// to make sure it passes some sort of validation.  For our purposes, we 
        /// will not need this functionality.  
        /// It is only here because the IEndpointBehavior requires it be implemented.
        /// </summary>
        /// <param name="endpoint"></param>
        public void Validate(ServiceEndpoint endpoint)
        { }

        #endregion

        #region IClientMessageInspector Members

        /// <summary>
        /// BeforeSendRequest():  This method will be used to audit the TID information 
        /// before sending a request to the service.  [This will call the associated 
        /// LifetimeEvents.BeforeSendRequest(), which will contain the actual logic for 
        /// handling the TID, and performing the audit.]
        /// </summary>
        /// <param name="request"></param>
        /// <param name="channel"></param>
        /// <returns></returns>
        object IClientMessageInspector.BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            return (LifetimeEvents.GetInstance().BeforeSendRequest(ref request, channel, svcEndpoint));
        }

        /// <summary>
        /// AfterReceiveReply():  This method will be used to audit TID information upon 
        /// receiving a reply from the service.  [This will call the associated 
        /// LifetimeEvents.AfterReceiveReply(), which will contain the actual logic for 
        /// handling the TID, and performing the audit.]
        /// </summary>
        /// <param name="reply"></param>
        /// <param name="correlationState"></param>
        void IClientMessageInspector.AfterReceiveReply(ref Message reply, object correlationState) 
        {
            LifetimeEvents.GetInstance().AfterReceiveReply(ref reply, correlationState);
        }

        #endregion

        #region BehaviorExtensionElement Members

        /// <summary>
        /// CreateBehavior():  This method will be used to create an object of the ASAClientMessageInspector type.
        /// </summary>
        public override Type BehaviorType
        {
            get { return typeof(ASAClientMessageInspector); }
        }

        /// <summary>
        /// BehaviorType:  This property will return the ASAClientMessageInspector type.
        /// </summary>
        /// <returns></returns>
        protected override object CreateBehavior()
        {
            return new ASAClientMessageInspector();
        }

        #endregion
    }
}
