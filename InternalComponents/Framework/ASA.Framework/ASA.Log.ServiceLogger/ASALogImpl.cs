///////////////////////////////////////////////////////////////////////////////
//
//  WorkFile Name:	ASALogImpl.cs
//
//  ASA Proprietary Information
//
///////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using log4net.Core;

namespace ASA.Log.ServiceLogger
{
    /// <summary>
    /// Logging interface implementation
    /// </summary>
    public class ASALogImpl : LogImpl, IASALog
    {
		/// <summary>
		/// The fully qualified name of this declaring type not the type of any subclass.
		/// </summary>
        private readonly static Type ThisDeclaringType = typeof(ASALogImpl);

        /// <summary>
        /// This level is reserved for auditing record. Used for logging TID
        /// </summary>
        private readonly static Level Audit = new Level(115000, "AUDIT");

        public ASALogImpl(ILogger logger)
            : base(logger)
		{
		}

		#region Implementation of ILogExt

        #region Exception

        /// <summary>
        /// Generate a log record for exception.
        /// </summary>
        /// <param name="CorrelationID">Correlation identifier of business activity</param>
        public void LogException(string CorrelationID, string message, Exception t, string errCode, string payload)
        {

            LogMessage(message, t, Level.Error,
                new KeyValuePair<string, object>("CorrelationID", CorrelationID),
                new KeyValuePair<string, object>("ErrorCode", errCode),
                new KeyValuePair<string, object>("EventType", (int)EventType.Exception),
                new KeyValuePair<string, object>("Payload", payload));
        }

        #endregion Exception

        #region Messaging

        public void LogTID(string tid)
        {
            LogMessage(null, null, ASALogImpl.Audit,
                new KeyValuePair<string, object>("TID", tid),
                new KeyValuePair<string, object>("EventType", (int)EventType.TID));
        }

        #endregion Messaging

        #region Business Activity

        public void LogServiceEntry(string CorrelationID)
        {
            LogMessage(null, null, Level.Info,
                new KeyValuePair<string, object>("EventType", (int)EventType.ServiceEntry),
                new KeyValuePair<string, object>("CorrelationID", CorrelationID));
        }

        public void LogServiceExit(string CorrelationID, EndingStatus endingStatus)
        {
            LogMessage(null, null, Level.Info,
                new KeyValuePair<string, object>("EventType", (int)EventType.ServiceExit),
                new KeyValuePair<string, object>("CorrelationID", CorrelationID),
                new KeyValuePair<string, object>("EndingStatus", (int)endingStatus));
        }

        public void LogServiceCall(string CorrelationID, string targetService)
        {
            LogMessage(targetService, null, Level.Info,
                new KeyValuePair<string, object>("EventType", (int)EventType.ServiceCall),
                new KeyValuePair<string, object>("CorrelationID", CorrelationID));
        }

        public void LogServiceReturn(string CorrelationID, string targetService, EndingStatus endingStatus)
        {
            LogMessage(targetService, null, Level.Info,
                new KeyValuePair<string, object>("EventType", (int)EventType.ServiceReturn),
                new KeyValuePair<string, object>("CorrelationID", CorrelationID),
                new KeyValuePair<string, object>("EndingStatus", (int)endingStatus));
        }

        #endregion Business Activity

        #region Processing Status

        public void LogMethodEntry(string method, string arguments)
        {
            LogMessage(method, null, Level.Debug,
                new KeyValuePair<string, object>("EventType", (int)EventType.MethodEntry),
                new KeyValuePair<string, object>("Arguments", arguments));
        }

        public void LogMethodExit(string method, string returnValue)
        {
            LogMessage(method, null, Level.Debug,
                new KeyValuePair<string, object>("EventType", (int)EventType.MethodExit),
                new KeyValuePair<string, object>("ReturnValue", returnValue));
        }

        public void LogMethodExit(string method, string returnValue, string arguments, TimeSpan executionTime)
        {
            LogMessage(method, null, Level.Debug,
                new KeyValuePair<string, object>("EventType", (int)EventType.MethodExit),
                new KeyValuePair<string, object>("Arguments", arguments),
                new KeyValuePair<string, object>("ReturnValue", returnValue),
                new KeyValuePair<string, object>("ExecutionTime", executionTime));
        }

        #endregion Processing Status

        #region Routine Logging

        public void Debug(object message, string CorrelationID)
        {
            LogMessage(message, null, Level.Debug,
                new KeyValuePair<string, object>("CorrelationID", CorrelationID));
        }

        public void Debug(object message, Exception e, string CorrelationID)
        {
            LogMessage(message, e, Level.Debug,
                new KeyValuePair<string, object>("CorrelationID", CorrelationID));
        }

        public void Info(object message, string CorrelationID)
        {
            LogMessage(message, null, Level.Info,
                new KeyValuePair<string, object>("CorrelationID", CorrelationID));
        }

        public void Info(object message, Exception e, string CorrelationID)
        {
            LogMessage(message, e, Level.Info,
                new KeyValuePair<string, object>("CorrelationID", CorrelationID));
        }

        public void Warn(object message, string CorrelationID)
        {
            LogMessage(message, null, Level.Warn,
                new KeyValuePair<string, object>("CorrelationID", CorrelationID));
        }

        public void Warn(object message, Exception e, string CorrelationID)
        {
            LogMessage(message, e, Level.Warn,
                new KeyValuePair<string, object>("CorrelationID", CorrelationID));
        }

        public void Error(object message, string CorrelationID)
        {
            LogMessage(message, null, Level.Error,
                new KeyValuePair<string, object>("CorrelationID", CorrelationID));
        }

        public void Error(object message, Exception e, string CorrelationID)
        {
            LogMessage(message, e, Level.Error,
                new KeyValuePair<string, object>("CorrelationID", CorrelationID));
        }

        public void Fatal(object message, string CorrelationID)
        {
            LogMessage(message, null, Level.Fatal,
                new KeyValuePair<string, object>("CorrelationID", CorrelationID));
        }

        public void Fatal(object message, Exception e, string CorrelationID)
        {
            LogMessage(message, e, Level.Fatal,
                new KeyValuePair<string, object>("CorrelationID", CorrelationID));
        }

        #endregion Routine Logging

        #endregion Implementation of ILogExt

        #region Protected Methods

        /// <summary>
        /// Generate a log record with event properties.
        /// </summary>
        /// <param name="message">Text message</param>
        /// <param name="t">Exception</param>
        /// <param name="level">Severity level</param>
        /// <param name="properties">Log event properties</param>
        protected void LogMessage(object message, Exception t, Level level, params KeyValuePair<string, object>[] properties)
        {
            if (IsLogEnabled(level))
            {
                LoggingEvent loggingEvent = new LoggingEvent(ThisDeclaringType, Logger.Repository, Logger.Name, level, message, t);
                foreach (KeyValuePair<string, Object> property in properties)
                {
                    string key = property.Key.Trim();
                    if ((key != "") && (property.Value!=null))
                        if (property.Value.ToString().Length>0)
                            loggingEvent.Properties[key] = property.Value;
                }
                Logger.Log(loggingEvent);
            }
        }

        /// <summary>
        /// Check if a logging severity level is enabled.
        /// </summary>
        /// <param name="level">Logging severity level</param>
        /// <returns>True if the severity level is enabled; otherwise False.</returns>
        protected bool IsLogEnabled(Level level)
        {
            if (level == Level.Debug)
                return this.IsDebugEnabled;
            else if (level == Level.Info)
                return this.IsInfoEnabled;
            else if (level == Level.Warn)
                return this.IsWarnEnabled;
            else if (level == Level.Error)
                return this.IsErrorEnabled;
            else if (level == Level.Fatal)
                return this.IsFatalEnabled;
            else if (level == Audit)
                return true;
            else
                return this.IsInfoEnabled;
        }

        #endregion Protected Methods
    }
}
