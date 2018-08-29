///////////////////////////////////////////////////////////////////////////////
//
//  WorkFile Name:	ASADispatchMessageInspector.cs in ASA.WCFExtensions
//
//  Description:
//  This class intercepts each message received at a WCF service.
//  The TID information associated with the call is audited upon receiving the request
//  and sending a reply.
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
using ASA.Log.ServiceLogger;

namespace ASA.WCFExtensions
{
    /// <summary>
    /// The ASADispatchMessageInspector class implements the IDispatchMessageInspector 
    /// and IServiceBehavior interfaces. 
    /// </summary>
    public class ASADispatchMessageInspector : IDispatchMessageInspector, IServiceBehavior
    {
        static readonly IASALog _Log = ASALogManager.GetLogger(typeof(ASADispatchMessageInspector));
        ServiceDescription svcDescription = null;
        #region IDispatchMessageInspector Members

        /// <summary>
        /// AfterReceiveRequest():  This method will be used to audit TID information 
        /// upon receiving a message at the service.  [This will call the associated 
        /// LifetimeEvents.AfterReceiveRequest(), which will contain the actual logic 
        /// for handling the TID, and performing the audit.]
        /// </summary>
        /// <param name="request"></param>
        /// <param name="channel"></param>
        /// <param name="instanceContext"></param>
        /// <returns></returns>
        public object AfterReceiveRequest(ref System.ServiceModel.Channels.Message request, IClientChannel channel, InstanceContext instanceContext)
        {
            return (LifetimeEvents.GetInstance().AfterReceiveRequest(ref request, channel, instanceContext, svcDescription));
        }

        /// <summary>
        /// BeforeSendReply():  This method will be used to audit the TID information 
        /// before sending a request message back to the caller.  [This will call the 
        /// associated LifetimeEvents.BeforeSendReply(), which will contain the actual 
        /// logic for handling the TID, and performing the audit.]
        /// </summary>
        /// <param name="reply"></param>
        /// <param name="correlationState"></param>
        public void BeforeSendReply(ref System.ServiceModel.Channels.Message reply, object correlationState)
        {
            LifetimeEvents.GetInstance().BeforeSendReply(ref reply, correlationState, svcDescription);
        }
        #endregion

        #region IServiceBehavior Members

        /// <summary>
        /// AddBindingParameters():  This method is intended to be used if you need to 
        /// add parameters to the BindingParameterCollection.  This allows those 
        /// parameters to be used to customize how the service channel is built.  
        /// For our purposes, we do not need this functionality, so this method will 
        /// not do anything in this implementation.  It is only here because the 
        /// IServiceBehavior requires it be implemented.
        /// </summary>
        /// <param name="serviceDescription"></param>
        /// <param name="serviceHostBase"></param>
        /// <param name="endpoints"></param>
        /// <param name="bindingParameters"></param>
        public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, System.Collections.ObjectModel.Collection<ServiceEndpoint> endpoints, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
        {
        }

        /// <summary>
        /// ApplyDispatchBehavior():  This method will be used to add the 
        /// ASADispatchMessageInspector behavior to the MessageInspectors collection 
        /// of the dispatcher-runtime for all the endpoints of the hosted service.
        /// </summary>
        /// <param name="serviceDescription"></param>
        /// <param name="serviceHostBase"></param>
        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            svcDescription = serviceDescription;
            _Log.Info("ApplyDispatchBehavior");
            foreach (ChannelDispatcher channelDispatcher in serviceHostBase.ChannelDispatchers)
            {
                foreach (EndpointDispatcher endpointDispatcher in channelDispatcher.Endpoints)
                {
                    _Log.InfoFormat("Adding to {0}", endpointDispatcher.EndpointAddress.Uri);
                    endpointDispatcher.DispatchRuntime.MessageInspectors.Add(this);
                }

            } 
        }

        /// <summary>
        /// Validate():  This method can be used to check the service description 
        /// or service host to make sure they pass some sort of validation.  
        /// For our purposes, we will not need this functionality.  It is only here 
        /// because the IServiceBehavior requires it be implemented.
        /// </summary>
        /// <param name="serviceDescription"></param>
        /// <param name="serviceHostBase"></param>
        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
        }

        #endregion
    }
}
///////////////////////////////////////////////////////////////////////////////
//
//	$Log: $
///////////////////////////////////////////////////////////////////////////////
