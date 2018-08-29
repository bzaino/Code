///////////////////////////////////////////////////////////////////////////////
//
//  WorkFile Name:	ASATIDHelper.cs in ASA.TID
//
//  Description:
//  This class contains methods that are used to retrieve information from
//  the operation context from within the scope of a service operation.
//  These methods are to be used by the service implementation classes.
//
//  ASA Proprietary Information
//
///////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Xml;

namespace ASA.TID
{
    public static class ASATIDHelper
    {
        /// <summary>
        /// This method can be called from within a service operation
        /// to retrieve the CorrelationID associated with the TID of
        /// the message being processed by the service
        /// </summary>
        /// <returns>Returns the correlationID of the TID.  If the 
        /// OperationContext, the TID or the correlationID cannot be found, it returns
        /// the empty string.</returns>
        public static string GetTIDCorrelationID()
        {
            string strCorrelationID = "";

            if (OperationContext.Current != null && OperationContext.Current.IncomingMessageHeaders != null)
            {
                MessageHeaders headers = OperationContext.Current.IncomingMessageHeaders;

                // get the TID object from the header
                ITID tid = TIDFactory.GetTIDFromMessage(headers);

                // if the TID was found, then set correlationID to the TID's correlationID
                if (tid != null)
                    strCorrelationID = (string)tid.Get(TIDField.CORRELATIONID);
            }

            return strCorrelationID;
        }


        /// <summary>
        /// This method can be called from within a service operation
        /// to retrieve the UserName associated with the TID of
        /// the message being processed by the service
        /// </summary>
        /// <returns>Returns the username of the TID.  If the 
        /// OperationContext, the TID or the username cannot be found, it returns
        /// the empty string.</returns>
        public static string GetTIDUsername()
        {
            string strUsername = "";

            if (OperationContext.Current != null && OperationContext.Current.IncomingMessageHeaders != null)
            {
                MessageHeaders headers = OperationContext.Current.IncomingMessageHeaders;

                // get the TID object from the header
                ITID tid = TIDFactory.GetTIDFromMessage(headers);

                // if the TID was found, then set username to the TID's username
                if (tid != null)
                    strUsername = (string)tid.Get(TIDField.USERNAME);
            }

            return strUsername;
        }

        /// <summary>
        /// This method can be called from within a service operation
        /// to retrieve the Action associated with the message
        /// being processed by the service
        /// </summary>
        /// <returns>Returns the service name of the TID.  If the 
        /// OperationContext, the TID or the service name cannot be found, it returns
        /// the empty string.</returns>
        public static string GetTIDAction()
        {
            string strAction = "";

            if (OperationContext.Current != null && OperationContext.Current.IncomingMessageHeaders != null)
            {
                MessageHeaders headers = OperationContext.Current.IncomingMessageHeaders;

                strAction = headers.Action;
            }

            return strAction;
        }        

        /// <summary>
        ///  This method can be called from within a service operation
        /// to retrieve the ParamList associated with the TID of
        /// the message being processed by the service
        /// </summary>
        /// <returns>Returns the ParamList of the TID.  If the 
        /// OperationContext, the TID or the ParamList cannot be found, 
        /// it returns null.</returns>
        public static Dictionary<string, string> GetTIDParamList()
        {
            Dictionary<string, string> paramList = null;

            if (OperationContext.Current != null && OperationContext.Current.IncomingMessageHeaders != null)
            {
                MessageHeaders headers = OperationContext.Current.IncomingMessageHeaders;

                // get the TID object from the header
                ITID tid = TIDFactory.GetTIDFromMessage(headers);

                // if the TID was found, then set correlationID to the TID's correlationID
                if (tid != null)
                    paramList = (Dictionary<string,string>)tid.Get(TIDField.PARAMLIST);
            }

            return paramList;
        }
    }
}
