///////////////////////////////////////////////////////////////////////////////
//
//  WorkFile Name:	LifetimeEvents.cs in ASA.WCFExtensions
//
//  Description:
//  This class contains methods that are called when the web service is initialized and
//  when it shuts down. 
//  These methods are called by the ASAServiceHost class.
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
using System.ServiceModel.Channels;
using System.Collections;
using ASA.Common;
using ASA.Security;
using ASA.Log.ServiceLogger;
using ASA.ErrorHandling;
using ASA.TID;
using System.Xml;
using System.ServiceModel.Administration;
using log4net;
using log4net.Config;

namespace ASA.WCFExtensions
{
    /// <summary>
    /// The LifetimeEvents class contains the actual implementation of methods used to 
    /// handle various events in the life-cycle of a WCF service.  
    /// The methods in this class will be used by the ASA message interceptors and 
    /// service host classes. 
    /// </summary>
    public class LifetimeEvents
    {
        private Guid strCorrelationIDForLogging;
        static ASA.Log.ServiceLogger.IASALog Log = ASA.Log.ServiceLogger.ASALogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region Startup and Shutdown methods

        /// <summary>
        /// o	Startup(): This method will be used to configure any internal 
        /// components of the service and log the startup of the service host
        /// </summary>
        public static void Startup()
        {
            //Setup Log4net
            log4net.Config.XmlConfigurator.Configure();

            Log.InfoFormat("Startup");
        }

        /// <summary>
        /// o	Close(): This method will be used to clean up any resources and 
        /// log the closing of the service host
        /// </summary>
        public static void Close()
        {
            //Log closing
            Log.InfoFormat("Close");
        }

        /// <summary>
        /// o	GetInstance():  This method creates and returns an object of 
        /// type LifetimeEvents
        /// </summary>
        /// <returns></returns>
        public static LifetimeEvents GetInstance()
        {
            LifetimeEvents lte = new LifetimeEvents();
            lte.strCorrelationIDForLogging = new Guid();
            return (lte);
        }

        #endregion

        #region Message Inspection Methods
        //
        //       _____________                 _____________
        //      |             | 1           2 |             |
        //      |             |-------------->|             |
        //      |   Client    |               | WCF Service |
        //      |             |<--------------|             |
        //      |             | 4           3 |             |
        //       -------------                 -------------
        //
        //   #1 = "BeforeSendRequest", which happens in the ASAClientMessageInspector
        //   #2 = "AfterReceiveRequest", which happens in the ASADispatchMessageInspector
        //   #3 = "BeforeSendReply", which happens in the ASADispatchMessageInspector
        //   #4 = "AfterReceiveReply", which happens in the ASAClientMessageInspector

        #region IDispatchMessageInspector interception methods

        /// <summary>
        /// o	AfterReceiveRequest():  This method will be used to audit TID information 
        /// upon receiving a request from the client.  This method will:
        ///   -	Retrieve TID information from the header of the received message
        ///   - Update TID fields for situation of a request being received.  
        ///     This will be done by the ASAMessageInspectionHelper.GetAfterReceivedRequestData() method.
        ///   - Call the TID class’ GetRecvTID() method
        ///   - send TID xml to the Logger for auditing.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="channel"></param>
        /// <param name="instanceContext"></param>
        /// <returns></returns>
        public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext, ServiceDescription svcDescription)
        {
            try
            {
                //if the SOAP envelope of the request is 'None", then someone is just trying to get the mex endpoint.
                // we dont want to audit for that.
                if (request.Version.Envelope != System.ServiceModel.EnvelopeVersion.None)
                {
                    //load Parameters with ASA settings
                    Parameters.Instance.SectionName = "asa";

                    if (Parameters.Instance.EnableServiceRequestAudit == true)
                    {
                        OperationContext opCtx = OperationContext.Current;
                        ITID tid = TIDFactory.GetTIDFromMessage(request.Headers);
                        if (tid == null)
                        {
                            // the caller did not supply a TID.  Let's populate one with the info available to us.
                            tid = ASAMessageInspectionHelper.CreateNewTID(); //create a new TID object
                            MessageHeaders headers = request.Headers;
                            ASAMessageInspectionHelper.AddParamListFromMessageToTID(tid, headers);
                            ASAMessageInspectionHelper.AddMissingDefaultDataToTID(tid, headers, svcDescription);
                            ASAMessageInspectionHelper.AddTIDHeader(tid, ref headers);
                        }

                        // update the TID
                        if (tid != null)
                        {
                            //Add message payload to dictionary
                            Payload.AddMessagePayLoad(ASATIDHelper.GetTIDCorrelationID(), request.ToString());

                            //populate hashtable with fields that need to be updated after receiving request message.
                            Hashtable mapProps = ASAMessageInspectionHelper.GetAfterReceiveRequestData(tid, request.Headers, channel);

                            //call update the tid for sending.
                            tid = TIDFactory.GetRecvTID(request.Headers, mapProps);

                            //put this TID into the outgoing Message object.
                            MessageHeaders headers = opCtx.IncomingMessageHeaders;
                            ASAMessageInspectionHelper.AddTIDHeader(tid, ref headers);

                            // Check for config file entry "Enable_ServiceRequestAudit"
                            //audit the TID information in 'tid' using the new ASA Logger
                            string strTID = ASAMessageInspectionHelper.GetTIDString(headers);
                            Log.LogTID(strTID);
                        }
                        else
                        {
                            //Log error
                            Log.Error("TID object was not created properly by the caller of this service:" +
                                channel.RemoteAddress.Uri.ToString(), this.strCorrelationIDForLogging.ToString());
                        }

                        if (Parameters.Instance.GetBoolValue("Enable_Authorization", false) == true)
                            AuthorizationManager.CheckAccess(TID.ASATIDHelper.GetTIDUsername(), request.Headers.Action);

                    }
                }
            }
            catch (ASAException exASA)
            {
                // log exception, then swallow it.
                Log.Error("Error in AfterReceiveRequest:" + exASA.Message + " Error_code:" + exASA.ExceptionError_id, exASA);
            }
            catch (ApplicationException exApp)
            {
                // log exception
                Log.Error("Error in AfterReceiveRequest", exApp);
                throw exApp;
            }
            catch (Exception ex)
            {
                // log exception, then swallow it.
                Log.Error("Error in AfterReceiveRequest", ex);
            }
            return null;
        }

        /// <summary>
        /// o	BeforeSendReply():  This method will be used to audit TID information before 
        /// sending a reply message to the client.  This method will:
        ///    - Retrieve TID information from the header of the received message
        ///    - Update TID fields for situation of a reply being sent.  
        ///      This will be done by the ASAMessageInspectionHelper.GetBeforeSendReplyData() method.
        ///    - Call the TID class’ GetSendTID() method
        ///    - send TID xml to the Logger for auditing.
        /// </summary>
        /// <param name="reply"></param>
        /// <param name="correlationState"></param>
        public void BeforeSendReply(ref Message reply, object correlationState, ServiceDescription svcDescription)
        {
            try
            {
                //if the SOAP envelope of the request is 'None", then someone is just trying to get the mex endpoint.
                // we dont want to audit for that.
                if (Parameters.Instance.EnableServiceResponseAudit == true)
                {
                    //load Parameters with ASA settings
                    Parameters.Instance.SectionName = "asa";

                    if (reply.Version.Envelope != System.ServiceModel.EnvelopeVersion.None)
                    {
                        OperationContext opCtx = OperationContext.Current;
                        ITID tid = TIDFactory.GetTIDFromMessage(opCtx.IncomingMessageHeaders);

                        // update the TID
                        if (tid != null)
                        {
                            //populate hashtable with fields needed before sending request message.
                            Hashtable mapProps = ASAMessageInspectionHelper.GetBeforeSendReplyData(tid, opCtx.IncomingMessageHeaders, svcDescription);
                            //call update the tid for sending.
                            tid = TIDFactory.GetSendTID(tid, mapProps);

                            //put the TID into the outgoing Message object.
                            MessageHeaders headers = reply.Headers;
                            ASAMessageInspectionHelper.AddTIDHeader(tid, ref headers);

                            // Check for config file entry "Enable_ServiceResponseAudit"
                            //audit the TID information in 'tid' using the new ASA Logger
                            string strTID = ASAMessageInspectionHelper.GetTIDString(headers);
                            Log.LogTID(strTID);

                            //remove message payload from dictionary
                            Payload.RemoveMessagePayLoad(ASATIDHelper.GetTIDCorrelationID());

                        }
                        else
                        {
                            //Log error
                            Log.Error("TID object was not created properly by this interception point:" + reply.Headers.To.ToString(),
                                this.strCorrelationIDForLogging.ToString());//TODO: get URI of this service here.
                        }
                    }
                }

                // Fix for defect 391
                Parameters.Instance.SectionName = "asa";
                bool enableForcedGarbageCollection = Parameters.Instance.GetBoolValue("EnableForcedGarbageCollection", false);
                if (enableForcedGarbageCollection)
                {
                    GC.Collect(2, GCCollectionMode.Forced);
                }
            }
            catch (ASAException exASA)
            {
                // log exception, then swallow it.
                Log.Error("Error in BeforeSendReply:" + exASA.Message + " Error_code:" + exASA.ExceptionError_id, exASA);
            }
            catch (Exception ex)
            {
                // log exception, then swallow it.
                Log.Error("Error in BeforeSendReply", ex);
            }
        }

        #endregion

        #region IClientMessageInspector interception methods

        /// <summary>
        /// o	BeforeSendRequest():  This method will be used to audit TID information 
        /// before sending a request to the service.  This method will:
        ///   - Create a new TID object if this is the first call in a logical transaction.  
        ///     (It is possible to tell this by the non-existence of an OperationContext in the proxy.)
        ///   - Retrieve TID information from the header of the received message if this is not 
        ///     the first call in a logical transaction.   (It is possible to tell this by the 
        ///     existence of an OperationContext in the proxy.)
        ///   - Update TID fields for situation of a request being sent.  This will be done by 
        ///     the ASAMessageInspectionHelper.GetBeforeSendRequestData() method.
        ///   - Call the TID class’ GetSendTID() method
        ///   - send TID xml to the Logger for auditing.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="channel"></param>
        /// <returns></returns>
        public object BeforeSendRequest(ref Message request, IClientChannel channel, ServiceEndpoint svcEndpoint)
        {
            try
            {
                //load Parameters with ASA settings
                Parameters.Instance.SectionName = "asa";

                if (Parameters.Instance.EnableClientRequestAudit == true)
                {
                    OperationContext opCtx = OperationContext.Current;
                    ITID tid = null;
                    if (opCtx != null) //this call is from inside a service
                        tid = TIDFactory.GetTIDFromMessage(opCtx.IncomingMessageHeaders);
                    if (tid == null)
                    {
                        //this call is from a .NET client or from the SAL
                        tid = ASAMessageInspectionHelper.CreateNewTID(); //create a new TID object
                    }
                    // update the TID
                    if (tid != null)
                    {
                        Hashtable mapProps = ASAMessageInspectionHelper.GetBeforeSendRequestData(tid, request.Headers, channel, svcEndpoint);

                        //call update the tid for sending.
                        tid = TIDFactory.GetSendTID(tid, mapProps);

                        //put the TID into the outgoing Message object.
                        MessageHeaders headers = request.Headers;
                        ASAMessageInspectionHelper.AddTIDHeader(tid, ref headers);

                        // Check for config file entry "Enable_ClientRequestAudit"
                        //audit the TID information in 'tid' using the new ASA Logger
                        string strTID = ASAMessageInspectionHelper.GetTIDString(headers);
                        Log.LogTID(strTID);
                    }
                    else
                    {
                        if (opCtx != null)
                        {
                            //Log error
                            Log.Error("TID object was not created properly by the caller of this service:" + request.Headers.Action,
                                this.strCorrelationIDForLogging.ToString());//TODO: get URI of this service here.
                        }
                        else
                        {
                            //Log error
                            Log.Error("TID object was not created properly by this interception point on the client:" + Environment.MachineName,
                                this.strCorrelationIDForLogging.ToString());
                        }
                    }
                }
            }
            catch (ASAException exASA)
            {
                // log exception, then swallow it.
                Log.Error("Error in BeforeSendRequest:" + exASA.Message + " Error_code:" + exASA.ExceptionError_id, exASA);
            }
            catch (Exception ex)
            {
                // log exception, then swallow it.
                Log.Error("Error in BeforeSendRequest", ex);
            }

            return null;
        }

        /// <summary>
        /// o	AfterReceiveReply():  This method will be used to audit TID information 
        /// upon receiving a reply from the service.  This method will:
        ///   - Retrieve TID information from the header of the received message
        ///   - Update TID fields for situation of a reply being received.  This will 
        ///     be done by the ASAMessageInspectionHelper.GetAfterReceiveReplyData() method.
        ///   - Call the TID class’ GetRecvTID() method
        ///   - send TID xml to the Logger for auditing.
        /// </summary>
        /// <param name="reply"></param>
        /// <param name="correlationState"></param>
        public void AfterReceiveReply(ref Message reply, object correlationState)
        {
            try
            {
                //load Parameters with ASA settings
                Parameters.Instance.SectionName = "asa";

                if (Parameters.Instance.EnableClientResponseAudit == true)
                {
                    ITID tid = TIDFactory.GetTIDFromMessage(reply.Headers);

                    //added for QC 2364: populate some TID info even if service didnt return one,
                    // so long as we have an opCtx to pull some info from.
                    if (tid == null)
                    {
                        OperationContext opCtx = OperationContext.Current;
                        if (opCtx != null) //this call is from inside a service
                        {
                            MessageHeaders headers = reply.Headers;
                            tid = ASAMessageInspectionHelper.CreateNewTID();
                            Hashtable mapProps = ASAMessageInspectionHelper.GetReplyDataWhenNoTidWasFound(tid, headers);
                            tid = TIDFactory.GetCreateTID(mapProps);
                            ASAMessageInspectionHelper.AddTIDHeader(tid, ref headers);
                        }
                    }

                    // update the TID
                    if (tid != null)
                    {
                        MessageHeaders headers = reply.Headers;

                        //populate hashtable with fields needed when receiving the reply message.
                        Hashtable mapProps = ASAMessageInspectionHelper.GetAfterReceiveReplyData(tid, headers);

                        //call update the tid for sending.
                        tid = TIDFactory.GetRecvTID(headers, mapProps);
                        ASAMessageInspectionHelper.AddTIDHeader(tid, ref headers);

                        // Check for config file entry "Enable_ClientResponseAudit"
                        //audit the TID information in 'tid' using the new ASA Logger
                        string strTID = ASAMessageInspectionHelper.GetTIDString(headers);
                        Log.LogTID(strTID);

                        //Update MessageSequenceNumber within the OperationContext's tid for multihop scenario.
                        OperationContext opCtx = OperationContext.Current;
                        if (opCtx != null)
                        {
                            ITID tidFromOpContext = TIDFactory.GetTIDFromMessage(opCtx.IncomingMessageHeaders);
                            if (tidFromOpContext != null)
                            {
                                tidFromOpContext.Set(TIDField.MESSAGESEQNUMBER, ((int)tid.Get(TIDField.MESSAGESEQNUMBER)));
                                MessageHeaders opCtxHeaders = opCtx.IncomingMessageHeaders;
                                ASAMessageInspectionHelper.AddTIDHeader(tidFromOpContext, ref opCtxHeaders);
                            }
                        }
                    }
                    else
                    {
                        //Log error
                        Log.Error("TID object was not created properly by this interception point on the client or service:" + Environment.MachineName,
                            this.strCorrelationIDForLogging.ToString());  //TODO: get URL of the service it's returning from?
                    }
                }
            }
            catch (ASAException exASA)
            {
                // log exception, then swallow it.
                Log.Error("Error in AfterReceiveReply:" + exASA.Message + " Error_code:" + exASA.ExceptionError_id, exASA);
            }
            catch (Exception ex)
            {
                // log exception, then swallow it.
                Log.Error("Error in AfterReceiveReply", ex);
            }
        }

        #endregion

        #endregion
    }
}

