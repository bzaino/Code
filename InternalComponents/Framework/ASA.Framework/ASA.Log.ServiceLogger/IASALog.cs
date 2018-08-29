///////////////////////////////////////////////////////////////////////////////
//
//  WorkFile Name:	IASALog.cs
//
//  ASA Proprietary Information
//
///////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Text;
using log4net;

namespace ASA.Log.ServiceLogger
{
    public interface IASALog : ILog
    {
        #region Exception

        /// <summary>
        /// Generate a log record for exception.
        /// </summary>
        /// <param name="CorrelationID">Correlation identifier of business activity</param>
        void LogException(string CorrelationID, string message, Exception t, string errCode, string payload);

        #endregion Exception

        #region Messaging

        /// <summary>
        /// Generate an audit record for TID.
        /// </summary>
        void LogTID(string tid);

        #endregion Messaging

        #region Business Activity

        /// <summary>
        /// Generate a log record for starting of service.
        /// </summary>
        void LogServiceEntry(string CorrelationID);

        /// <summary>
        /// Generate a log record for ending of service.
        /// </summary>
        void LogServiceExit(string CorrelationID, EndingStatus endingStatus);

        /// <summary>
        /// Generate a log record for starting of service invocation.
        /// </summary>
        void LogServiceCall(string CorrelationID, string targetService);

        /// <summary>
        /// Generate a log record for ending of service invocation.
        /// </summary>
        void LogServiceReturn(string CorrelationID, string targetService, EndingStatus endingStatus);

        #endregion Business Activity

        #region Processing Status

        /// <summary>
        /// Generate a log record for starting of method call.
        /// </summary>
        void LogMethodEntry(string method, string arguments);

        /// <summary>
        /// Generate a log record for ending of method call.
        /// </summary>
        void LogMethodExit(string method, string returnValue);


        /// <summary>
        /// Generate a log record for method call.
        /// </summary>
        void LogMethodExit(string method, string returnValue, string arguments, TimeSpan executionTime);

        #endregion Processing Status

        #region Routine Logging

        /// <summary>
        /// Generate a log record with Debug serverity level and correlation ID.
        /// </summary>
        void Debug(object message, string CorrelationID);
        void Debug(object message, Exception e, string CorrelationID);

        /// <summary>
        /// Generate a log record with Info serverity level and correlation ID.
        /// </summary>
        void Info(object message, string CorrelationID);
        void Info(object message, Exception e, string CorrelationID);

        /// <summary>
        /// Generate a log record with Warn serverity level and correlation ID.
        /// </summary>
        void Warn(object message, string CorrelationID);
        void Warn(object message, Exception e, string CorrelationID);

        /// <summary>
        /// Generate a log record with Error serverity level and correlation ID.
        /// </summary>
        void Error(object message, string CorrelationID);
        void Error(object message, Exception e, string CorrelationID);

        /// <summary>
        /// Generate a log record with Fatal serverity level and correlation ID.
        /// </summary>
        void Fatal(object message, string CorrelationID);
        void Fatal(object message, Exception e, string CorrelationID);

        #endregion Routine Logging
    }
}
