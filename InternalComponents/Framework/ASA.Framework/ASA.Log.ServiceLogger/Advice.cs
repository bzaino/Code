///////////////////////////////////////////////////////////////////////////////
//
//  WorkFile Name:	Advice.cs
//
//  ASA Proprietary Information
//
///////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Spring.Aop;
using AopAlliance.Intercept;

namespace ASA.Log.ServiceLogger.Advice
{
    /// <summary>
    /// This advice creates a log record for method call. Arguments, return value, and execution time are logged.
    /// </summary>
    public class ASALoggingAdvice : IMethodInterceptor
    {
        /// <summary>
        /// Adds logging to the method invocation.
        /// </summary>
        /// <param name="invocation"></param>
        /// <returns> return value of the targetd method</returns>
        public object Invoke(IMethodInvocation invocation)
        {
            IASALog log = ASALogManager.GetLogger(invocation.TargetType);

            string methodName = invocation.TargetType.ToString() + "." + invocation.Method.Name;
            StringBuilder arguments = new StringBuilder();
            ParameterInfo[] parameterInfos = invocation.Method.GetParameters();
            object[] argValues = invocation.Arguments;
            for (int i = 0; i < parameterInfos.Length; i++)
            {
                arguments.Append(parameterInfos[i].Name).Append("=").Append(argValues[i]);
                if (i < (parameterInfos.Length - 1)) arguments.Append("; ");
            }
            if (LogEntry)
                log.LogMethodEntry(methodName, arguments.ToString());

            object returnValue = null;
            bool exitThroughException = false;
            DateTime startTime = DateTime.Now;

            try
            {
                returnValue = invocation.Proceed();
                return returnValue;
            }
            catch(Exception e)
            {
                if (logException)
                    log.Error("Exception occured while calling method " + methodName, e);
                exitThroughException = true;
                throw;
            }
            finally
            {
                if (!exitThroughException && logExit)
                {
                    TimeSpan executionTime = DateTime.Now - startTime;
                    if (returnValue == null)
                        log.LogMethodExit(methodName, "", arguments.ToString(), executionTime);
                    else
                        log.LogMethodExit(methodName, returnValue.ToString(), arguments.ToString(), executionTime);
                }
            }

        }

        #region Fields

        /// <summary>
        /// Log method entry when set to true.
        /// </summary>
        protected bool logEntry = true;

        /// <summary>
        /// Log method exit when set to true.
        /// </summary>
        protected bool logExit = true;

        /// <summary>
        /// Log exception during method call when set to true.
        /// </summary>
        protected bool logException = false;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Indicates whether or not method entry will be logged.
        /// </summary>
        protected bool LogEntry 
        {
            set { logEntry = value; }
            get { return logEntry; }
        }

        /// <summary>
        /// Indicates whether or not method exit will be logged.
        /// </summary>
        protected bool LogExit
        {
            set { logExit = value; }
            get { return logExit; }
        }

        /// <summary>
        /// Indicates whether or not exception during method call will be logged.
        /// </summary>
        protected bool LogException
        {
            set { logException = value; }
            get { return logException; }
        }

        #endregion Properties
    }
}
