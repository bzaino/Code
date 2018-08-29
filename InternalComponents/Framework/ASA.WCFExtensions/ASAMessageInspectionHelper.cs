///////////////////////////////////////////////////////////////////////////////
//
//  WorkFile Name:	ASAMessageInspectionHelper.cs in ASA.WCFExtensions
//
//  Description:
//  This class contains methods that are used to deal with MessageHeaders and TID objects.
//  when it shuts down. 
//  These methods are used by the LifetimeEvents class.
//
//  ASA Proprietary Information
//
///////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Xml;
using ASA.TID;

namespace ASA.WCFExtensions
{
	/// <summary>
	/// The ASAMessageInspectionHelper class contains helper methods provided as a 
	/// convenience for the LifetimeEvents class to simplify setting/getting objects 
	/// from the TID, Message, and MessageHeaders objects being inspected.  
	/// </summary>
	public static class ASAMessageInspectionHelper
	{
		#region TID Header
		/// <summary>
		/// AddTIDHeader():  This method will be used to add TID information to the 
		/// MessageHeaders collection passed in.  (If a TID already exists in the headers collection, 
		/// this method will remove it, and replace it with the TID data being passed.)
		/// </summary>
		/// <param name="tid"></param>
		/// <param name="headers"></param>
		/// <returns></returns>
		public static bool AddTIDHeader(ITID tid, ref MessageHeaders headers)
		{
			bool bAdded = false;
			if(headers != null && tid != null)
			{
				//this will remove 'tid' from the header if its already in there.. we don't want it added twice.
				RemoveTIDHeader(tid, ref headers);

				MessageHeader<TIDBase> mhMessageTID = new MessageHeader<TIDBase>((TIDBase)tid);
				headers.Add(mhMessageTID.GetUntypedHeader("TID", "TID"));

				bAdded = true;
			}

			return bAdded;
		}

		/// <summary>
		/// RemoveTIDHeader():  This method will be used to remove TID information 
		/// from the MessageHeaders collection passed in.
		/// </summary>
		/// <param name="tid"></param>
		/// <param name="headers"></param>
		/// <returns></returns>
		public static bool RemoveTIDHeader(ITID tid, ref MessageHeaders headers)
		{
			bool bRemoved = false;

			if(tid != null && headers != null)
			{
				int iHeaderIndex = headers.FindHeader("TID", "TID");

				// If we found the header, remove it.
				if(iHeaderIndex > -1)
				{
					headers.RemoveAt(iHeaderIndex);
					bRemoved = true;
				}
			}

			return bRemoved;
		}

		/// <summary>
		/// GetTIDHeaderIndex():  This method will be used to find the index of the 
		/// TID object in the MessageHeaders collection passed in.
		/// </summary>
		/// <param name="headers"></param>
		/// <returns></returns>
		public static int GetTIDHeaderIndex(MessageHeaders headers)
		{
			int iHeaderIndex = -1;
			int iLoopIndex = 0;
			if(headers != null)
			{
				foreach(MessageHeaderInfo mhi in headers)
				{
					if(mhi.Name != null && mhi.Name == "TID")
					{
						iHeaderIndex = iLoopIndex;
						break;
					}
					iLoopIndex++;
				}
			}

			return iHeaderIndex;
		}

		/// <summary>
		/// GetTIDHeaderNamespace():  This method will be used to find the namespace
		/// of the TID object in the MessageHeaders collection passed in.
		/// </summary>
		/// <param name="headers"></param>
		/// <returns></returns>
		public static string GetTIDHeaderNamespace(MessageHeaders headers)
		{
			string strNamespace = "";
			if(headers != null)
			{
				foreach(MessageHeaderInfo mhi in headers)
				{
					if(mhi.Name != null && mhi.Name == "TID")
					{
						strNamespace = mhi.Namespace;
						break;
					}
				}
			}

			return strNamespace;
		}

		/// <summary>
		/// This should be used to create a new TID object when there is no TID already
		/// in the message or OperationContext's IncomingMessageHeaders.
		/// </summary>
		/// <returns></returns>
		public static ITID CreateNewTID()
		{
			Hashtable mapProps = new Hashtable();
			mapProps[TIDField.USERNAME] = GetUsername();

			//return a new TID object that has default fields populated.
			return TIDFactory.GetCreateTID(mapProps);
		}

		/// <summary>
		/// This is used to get the non-version specific serialized TID as 
		/// it appears in the message headers.
		/// </summary>
		/// <param name="headers"></param>
		/// <returns></returns>
		public static string GetTIDString(MessageHeaders headers)
		{
			string strTID = "";
			foreach(MessageHeaderInfo mhi in headers)
			{
				if(mhi.Name == "TID")
					strTID = mhi.ToString();
			}

			return strTID;
		}
		#endregion

		#region Get header data
		/// <summary>
		/// o	GetBeforeSendRequestData():  This method will encapsulate the logic 
		/// used to set the TID values appropriately for when a request message is 
		/// being sent from the client.  This will call many of the individual field 
		/// GetXXX() methods.  [e.g. GetMessageSequenceNumber(), GetServiceName(), 
		/// GetExpireDt()]
		/// </summary>
		/// <param name="headers"></param>
		/// <returns></returns>
		public static Hashtable GetBeforeSendRequestData(ITID tid, MessageHeaders headers, IClientChannel channel, ServiceEndpoint svcEndpoint)
		{
			Hashtable mapProps = new Hashtable();
            mapProps[TIDField.MESSAGEVERSION] = GetMessageVersion(svcEndpoint);
            mapProps[TIDField.SERVICENAME] = GetServiceName(svcEndpoint);
            mapProps[TIDField.OPERATIONNAME] = GetOperationName(headers);
            //if(OperationContext.Current != null)
            //    mapProps[TIDField.USERNAME] = GetUsername();
			//mapProps[TIDField.MESSAGEID] = GetMessageID(headers);           //<-- this is set by TID.CreateTID()
			//mapProps[TIDField.CORRELATIONID] = GetCorrelationID(headers);   //<-- this is set by TID.CreateTID()
			//mapProps[TIDField.ORIGINATIONDT] = GetOriginationDt(headers);   //<-- this is set by TID.CreateTID()
			//mapProps[TIDField.SENTDT] = GetSentDt(headers);                 //<-- this will be set by TIDFactory.GetSendTID()
			//mapProps[TIDField.RECEIVEDDT] = GetReceivedDt(headers);         //<-- this is set by TID.CreateTID() and again later by GetRecvTID()
			if(OperationContext.Current == null)
				mapProps[TIDField.EXPIREDT] = GetExpireDt((DateTime)tid.Get(TIDField.ORIGINATIONDT), channel.OperationTimeout);
			mapProps[TIDField.MESSAGESEQNUMBER] = GetMessageSequenceNumber(tid);
			mapProps[TIDField.SOURCE] = GetSendingMessageSource(headers);
			mapProps[TIDField.DESTINATION] = GetSendingMessageDestination(headers, channel);
			mapProps[TIDField.SYNCH] = GetSyncFlag(channel, (string)mapProps[TIDField.OPERATIONNAME]);
			mapProps[TIDField.REPLY] = false;
			mapProps[TIDField.PARAMLIST] = GetParamList(headers);

			return mapProps;
		}

        public static int GetMessageSeqNumberFromIncomingHeaders()
        {
            int seqNum = -1;

            if (OperationContext.Current != null && OperationContext.Current.IncomingMessageHeaders != null)
            {
                MessageHeaders headers = OperationContext.Current.IncomingMessageHeaders;

                // get the TID object from the header
                ITID tid = TIDFactory.GetTIDFromMessage(headers);

                // if the TID was found, then set username to the TID's username
                if (tid != null)
                    seqNum = (int)tid.Get(TIDField.MESSAGESEQNUMBER);
            }

            return seqNum;
        }

		/// <summary>
		/// o	GetAfterReceiveRequestData():  This method will encapsulate the logic 
		/// used to set the TID values appropriately for when a request message is 
		/// being received at the service.  
		/// This will call many of the individual field GetXXX() methods.  
		/// </summary>
		/// <param name="headers"></param>
		/// <returns></returns>
		public static Hashtable GetAfterReceiveRequestData(ITID tid, MessageHeaders headers, IClientChannel channel)
		{
			Hashtable mapProps = new Hashtable();
			mapProps[TIDField.MESSAGESEQNUMBER] = GetMessageSequenceNumber(tid);
			mapProps[TIDField.DESTINATION] = GetReceivingMessageDestination(headers, channel);

			return mapProps;
		}

        public static void AddMissingDefaultDataToTID(ITID tid, MessageHeaders headers, ServiceDescription desc)
        {
            tid.Set(TIDField.MESSAGEID, Guid.NewGuid().ToString());
            tid.Set(TIDField.MESSAGEVERSION, GetMessageVersion(desc));
            tid.Set(TIDField.SERVICENAME, GetServiceName(desc));
            tid.Set(TIDField.OPERATIONNAME, GetOperationName(headers));
        }

		/// <summary>
		/// o	GetBeforeSendReplyData():  This method will encapsulate the logic 
		/// used to set the TID values appropriately for when a reply message is 
		/// being sent from the service.  
		/// This will call many of the individual field GetXXX() methods.
		/// </summary>
		/// <param name="headers"></param>
		/// <returns></returns>
		public static Hashtable GetBeforeSendReplyData(ITID tid, MessageHeaders headers, ServiceDescription desc)
		{
			Hashtable mapProps = new Hashtable();
            mapProps[TIDField.MESSAGEVERSION] = GetMessageVersion(desc);
            mapProps[TIDField.SERVICENAME] = GetServiceName(desc);
			mapProps[TIDField.OPERATIONNAME] = GetOperationName(headers);
			mapProps[TIDField.MESSAGESEQNUMBER] = GetMessageSequenceNumber(tid);

			//for the reply, just swap the Source and Dest:
			mapProps[TIDField.SOURCE] = (string)tid.Get(TIDField.DESTINATION);
			mapProps[TIDField.DESTINATION] = (string)tid.Get(TIDField.SOURCE);
			mapProps[TIDField.REPLY] = true;

			return mapProps;
		}

		/// <summary>
		/// o	GetAfterReceiveReplyData():  This method will encapsulate the logic 
		/// used to set the TID values appropriately for when a reply message is 
		/// being received at the client.  This will call many of the individual 
		/// field GetXXX() methods.  [e.g. GetMessageSequenceNumber()]
		/// </summary>
		/// <param name="headers"></param>
		/// <returns></returns>
		public static Hashtable GetAfterReceiveReplyData(ITID tid, MessageHeaders headers)
		{
			Hashtable mapProps = new Hashtable();
			mapProps[TIDField.MESSAGESEQNUMBER] = GetMessageSequenceNumber(tid);

			return mapProps;
		}
		#endregion

		#region Get TID field data

		/// <summary>
		/// Will retrieve the MESSAGE VERSION based on the Service contract namespace
		/// </summary>
		/// <param name="headers"></param>
		/// <returns></returns>
        public static string GetMessageVersion(ServiceEndpoint svcEndpoint)
        {
            string strMsgVersion = "";
            if (svcEndpoint != null)
            {
                strMsgVersion = svcEndpoint.Contract.Namespace;
            }

            return strMsgVersion;
        }

        public static string GetMessageVersion(ServiceDescription desc)
        {
            string strMsgVersion = "";
            if (desc != null)
            {
                strMsgVersion = desc.Namespace;
            }

            return strMsgVersion;
        }

		/// <summary>     
		/// Will retrieve the SERVICE NAME based on the Service contract name
		/// </summary>
		/// <param name="headers"></param>
		/// <returns></returns>
        public static string GetServiceName(ServiceEndpoint svcEndpoint)
        {
            string strServiceName = "";
            if (svcEndpoint != null)
            {
                strServiceName = svcEndpoint.Contract.Name;
            }

            return strServiceName;
        }

        public static string GetServiceName(ServiceDescription desc)
        {
            string strServiceName = "";
            foreach (ServiceEndpoint endpt in desc.Endpoints)
            {
                //make sure we're not getting contract info for the mex endpoint
                if (endpt.Contract.Name.CompareTo("IMetadataExchange") != 0 &&
                    !endpt.Contract.Namespace.ToLower().EndsWith("/mex"))
                {
                    strServiceName = endpt.Contract.Name;
                }
            }

            return strServiceName;
        }

		/// <summary>
		/// Will retrieve the OPERATION NAME based on the SOAP action
		/// </summary>
		/// <param name="headers"></param>
		/// <returns></returns>
		public static string GetOperationName(MessageHeaders headers)
		{
			// If SOAP Action is -->  <Action>http://MyService.ServiceContracts/2009/02/ICalculator/Add</Action>
			// then, the OperationName is 'Add'

			string strOperationName = "";
			if(headers != null && headers.Action != null)
			{
		//QC 2364: need to retrieve OperationName as last component of SOAP Action, not the whole soap action.
                string strAction = headers.Action;
                string[] strActionArray = strAction.Split('/');
                if (strActionArray.Length > 0)
                    strOperationName = strActionArray[strActionArray.Length - 1];
			}

			return strOperationName;
		}

        public static Hashtable GetReplyDataWhenNoTidWasFound(ITID tid, MessageHeaders headers)
        {
            OperationContext ctx = OperationContext.Current;
            IClientChannel channel = null;
            if (ctx == null && ctx.Channel is IClientChannel)
                channel = (IClientChannel)ctx.Channel;

            Hashtable mapProps = new Hashtable();
            mapProps[TIDField.MESSAGESEQNUMBER] = ASAMessageInspectionHelper.GetMessageSeqNumberFromIncomingHeaders() + 1;
            ASAMessageInspectionHelper.WriteMessageSeqNumToIncomingHeaders((int)mapProps[TIDField.MESSAGESEQNUMBER]);
            mapProps[TIDField.CORRELATIONID] = ASATIDHelper.GetTIDCorrelationID();
            mapProps[TIDField.OPERATIONNAME] = GetOperationName(headers); // --> try to get from SOAP Action, but will probably be empty.
            mapProps[TIDField.SENTDT] = DateTime.UtcNow; //dont' know when it was actually sentin this scenario, so our best guess is "now".

            // desintination = this machine
            mapProps[TIDField.DESTINATION] = GetReceivingMessageDestination(headers, channel);
            // source --> in message?
            mapProps[TIDField.SOURCE] = GetReceivingMessageSource(headers);

            // set reply = true
            // set synch = true
            mapProps[TIDField.SYNCH] = GetSyncFlag(channel, (string)mapProps[TIDField.OPERATIONNAME]);
            mapProps[TIDField.REPLY] = true;

            if (ctx != null)
            {
                mapProps[TIDField.USERNAME] = GetUsername();
            }

            //mapProps[TIDField.MESSAGEID] = GetMessageID(headers);           //<-- this is set by TID.CreateTID()
            //mapProps[TIDField.ORIGINATIONDT] = GetOriginationDt(headers);   //<-- this is set by TID.CreateTID()
            //mapProps[TIDField.RECEIVEDDT] = GetReceivedDt(headers);         //<-- this is set by TID.CreateTID() and again later by GetRecvTID()
            //mapProps[TIDField.MESSAGEVERSION] = GetMessageVersion(svcEndpoint);  //--> no svc endpoint to get this from in context.
            //mapProps[TIDField.SERVICENAME] = GetServiceName(svcEndpoint); //--> no svc endpoint to get this from in context.

            if (OperationContext.Current == null)
                mapProps[TIDField.EXPIREDT] = GetExpireDt((DateTime)tid.Get(TIDField.ORIGINATIONDT), channel.OperationTimeout);

            return mapProps;
        }

	// This method updates the MessageSeqNumber in the IncomingMessage headers so it is
	// up-to-date for things later in the process during the multi-hop scenario.
        private static void WriteMessageSeqNumToIncomingHeaders(int iNewSeqNumber)
        {
            if (OperationContext.Current != null && OperationContext.Current.IncomingMessageHeaders != null)
            {
                MessageHeaders headers = OperationContext.Current.IncomingMessageHeaders;
                ITID tid = TIDFactory.GetTIDFromMessage(OperationContext.Current.IncomingMessageHeaders);
                if(tid != null)
                {
                    tid.Set(TIDField.MESSAGESEQNUMBER, iNewSeqNumber);
                    ASAMessageInspectionHelper.AddTIDHeader(tid, ref headers);
                }
            }
        }

		/// <summary>
		/// Will retrieve the USERNAME from ...
		/// </summary>
		/// <returns></returns>
		public static string GetUsername()
		{
			//TODO: get App user login from somewhere TBD if call is from a client, 
			// and get user from security context of service if call is from service
			//... use CurrentUserHelper here?
			string strUser;
			//when the request enters the WCF ServiceHost HttpContext gets nulled and all impersonation is undone.
			//WCF does take care for OperationContext which Provides access to the execution context
			//to make less changes to existing WCFExtension 
			//this method will rely on OperationContext or ServiceSecurityContext
			//if security mode = None, i.e. SecurityContext = null - WindowsIdentity
			CurrentUserHelper userHelper = null;
			if(OperationContext.Current != null)
			{
				userHelper = new CurrentUserHelper(OperationContext.Current.ServiceSecurityContext);
			}
			else if(ServiceSecurityContext.Current != null)
			{
				userHelper = new CurrentUserHelper(ServiceSecurityContext.Current);
			}
			else
			{
				userHelper = new CurrentUserHelper();
			}

			strUser = userHelper.ToString();

			return strUser;
		}

		/// <summary>
		/// Will retrieve the EXPIRE DATE of the TID based on the client timeout
		/// and the origination date passed in.
		/// </summary>
		/// <param name="dtOrigination"></param>
		/// <param name="tsTimeout"></param>
		/// <returns></returns>
		public static DateTime GetExpireDt(DateTime dtOrigination, TimeSpan tsTimeout)
		{
			return dtOrigination.AddMilliseconds(tsTimeout.TotalMilliseconds);
		}

		/// <summary>
		/// Will retrieve the next MESSAGE SEQUENCE NUMBER based upon the TID passed in.
		/// </summary>
		/// <param name="dtOrigination"></param>
		/// <param name="tsTimeout"></param>
		/// <returns></returns>
		public static int GetMessageSequenceNumber(ITID tid)
		{
			return ((int)tid.Get(TIDField.MESSAGESEQNUMBER)) + 1;
		}

		/// <summary>
		/// Will retrieve the SOURCE  (SENDING) of the message.
		/// </summary>
		/// <param name="headers"></param>
		/// <returns></returns>
		public static string GetSendingMessageSource(MessageHeaders headers)
		{
			string strSource = "";

			// Use 'From' for SOAP 1.2, and use 'Environment.MachineName' otherwise for SOAP 1.1.
			if(headers != null && headers.From != null)
				strSource = headers.From.Uri.ToString();
			else if(OperationContext.Current != null && OperationContext.Current.Channel.LocalAddress != null)
				strSource = OperationContext.Current.Channel.LocalAddress.Uri.ToString();
			else
				strSource = Environment.MachineName;

			return strSource;
		}

        public static string GetReceivingMessageSource(MessageHeaders headers)
        {
            string strSource = "";

            // Use 'From' for SOAP 1.2, and Remote address otherwise for SOAP 1.1.
            if (headers != null && headers.From != null)
                strSource = headers.From.Uri.ToString();
            else if (OperationContext.Current != null && OperationContext.Current.Channel.RemoteAddress != null)
                strSource = OperationContext.Current.Channel.RemoteAddress.Uri.ToString();
            else
                strSource = "Unknown";

            return strSource;
        }

		/// <summary>
		/// Will retrieve the DESTINATION (SENDING) of the message.
		/// </summary>
		/// <param name="headers"></param>
		/// <returns></returns>
		public static string GetSendingMessageDestination(MessageHeaders headers, IClientChannel channel)
		{
			string strDestination = "";
			// Use 'To' field when soap 1.2... otherwise use channel.RemoteAddress for SOAP 1.1.
			// Destination will be the 'To' SOAP header --> <To>http://localhost:4002/MyService.Host/MyService.svc</To>
			if(headers != null && headers.To != null)
				strDestination = headers.To.ToString();
			else if(channel != null && channel.RemoteAddress != null)
				strDestination = channel.RemoteAddress.Uri.ToString();

			return strDestination;
		}

		/// <summary>
		/// Will retrieve the DESTINATION (RECEIVING) of the message.
		/// </summary>
		/// <param name="headers"></param>
		/// <returns></returns>
		public static string GetReceivingMessageDestination(MessageHeaders headers, IClientChannel channel)
		{
			string strDestination = "";
			// Use 'To' field when soap 1.2... otherwise use channel.RemoteAddress for SOAP 1.1.
			// Destination will be the 'To' SOAP header --> <To>http://localhost:4002/MyService.Host/MyService.svc</To>
			if(headers != null && headers.To != null)
				strDestination = headers.To.ToString();
			else if(channel != null && channel.LocalAddress != null)
				strDestination = channel.LocalAddress.Uri.ToString();
			else
				strDestination = Environment.MachineName;

			return strDestination;
		}
		/// <summary>
		/// Will retrieve SYNC FLAG based upon the value of the IsOneWay 
		/// attribute for the operation being invoked.
		/// </summary>
		/// <param name="channel"></param>
		/// <param name="opName"></param>
		/// <returns></returns>
		public static bool GetSyncFlag(IClientChannel channel, string opName)
		{
			bool bSync = true;
			if(channel != null)
			{
				Type t = Type.GetType(channel.GetType().AssemblyQualifiedName);
				if(t != null)
				{
					OperationDescription opDesc = GetOperationDescription(t, opName);
					if(opDesc != null && opDesc.IsOneWay == true)
						bSync = false;
				}
			}

			return bSync;
		}

		/// <summary>
		/// Will retrieve the custom PARAMETER LIST passed in by the client from
		/// the ASABaseMessageContract
		/// </summary>
		/// <param name="headers"></param>
		/// <returns></returns>
		public static Dictionary<string, string> GetParamList(MessageHeaders headers)
		{
			Dictionary<string, string> paramList = null;
			if(headers != null)
			{
				int iHeaderIndex = headers.FindHeader("ParamList", ASA.WCFExtensions.ASABaseRequestContract.PARAMLIST_NAMESPACE);

				// If we found the header, remove it.
				if(iHeaderIndex > -1)
				{
					paramList = headers.GetHeader<Dictionary<string, string>>(iHeaderIndex);

					//remove paramlist from message headers.. it will get re-added later inside TID.
					headers.RemoveAt(iHeaderIndex);
				}
			}

			return paramList;
		}

		/// <summary>
		/// This will retrieve the ParamList (if there is one) from the message headers
		/// and set that as the ParamList for the TID.
		/// </summary>
		/// <param name="tid"></param>
		/// <param name="headers"></param>
		public static void AddParamListFromMessageToTID(ITID tid, MessageHeaders headers)
		{
			Dictionary<string, string> paramList = GetParamList(headers);
			if(paramList != null)
				tid.Set(TIDField.PARAMLIST, paramList);
		}
		#endregion

		#region Helper's helpers
		/// <summary>
		/// This will be used to retrieve the OperationDescription object for the service contract
		/// type and operation name passed in.
		/// </summary>
		/// <param name="contractType"></param>
		/// <param name="operationName"></param>
		/// <returns></returns>
		public static OperationDescription GetOperationDescription(Type contractType, string operationName)
		{
			OperationDescription operation = null;
			if(contractType != null)
			{
				ContractDescription contract = new ContractDescription(contractType.Name);
				operation = new OperationDescription(operationName, contract);
				operation.SyncMethod = contractType.GetMethod(operationName);
			}
			return operation;
		}
		#endregion

    }
}
