using System;
using System.Data.Entity;
using System.Data.Objects;
using System.Linq;
using System.Reflection;
using Common.Logging;

namespace Asa.Salt.Web.Services.Logging
{
    public class Log : ILog
    {
        /// <summary>
        /// The logger
        /// </summary>
        private Common.Logging.ILog _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="Log"/> class.
        /// </summary>
        public Log()
        {
            _logger = LogManager.GetLogger("Logger");
        }

        /// <summary>
        /// Configures the logger.
        /// </summary>
        /// <param name="className">Name of the class.</param>
        public void ConfigureLogger(string className)
        {

        }

        /// <summary>
        /// Traces the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Trace(string message)
        {
            _logger.Trace(message);
        }

        /// <summary>
        /// Traces the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        public void Trace(string message, Exception exception)
        {
            _logger.Trace(message, exception);
        }

        /// <summary>
        /// Log a message string with the Debug level.
        /// </summary>
        /// <param name="message">The message string to log.</param>
        public void Debug(string message)
        {
            _logger.Debug(message);
        }

        /// <summary>
        /// Log a message string with the Debug level.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        public void Debug(string message, Exception exception)
        {
            _logger.Debug(message, exception);
        }

        /// <summary>
        /// Log a message string with the Info level.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Info(string message)
        {
            _logger.Info(message);
        }

        /// <summary>
        /// Log a message string with the Info level.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        public void Info(string message, Exception exception)
        {
            _logger.Info(message, exception);
        }

        /// <summary>
        /// Log a message string with the Warn level.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Warn(string message)
        {
            _logger.Warn(message);
        }

        /// <summary>
        /// Log a message string with the Warn level.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        public void Warn(string message, Exception exception)
        {
            _logger.Warn(message, exception);
        }

        /// <summary>
        /// Log a message string with the Error level.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Error(string message)
        {
            _logger.Error(message);
        }

        /// <summary>
        /// Log a message string with the Error level.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        public void Error(string message, Exception exception)
        {
            _logger.Error(message, exception);
        }

        /// <summary>
        /// Log a message string with the Fatal level.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Fatal(string message)
        {
            _logger.Fatal(message);
        }

        /// <summary>
        /// Log a message string with the Fatal level.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        public void Fatal(string message, Exception exception)
        {
            _logger.Fatal(message, exception);
        }

        /// <summary>
        /// Gets a value indicating whether this instance is trace enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is trace enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsTraceEnabled { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance is debug enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is debug enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsDebugEnabled { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance is error enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is error enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsErrorEnabled { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance is fatal enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is fatal enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsFatalEnabled { get; private set; }
        /// <summary>
        /// Gets a value indicating whether this instance is info enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is info enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsInfoEnabled { get; private set; }
        /// <summary>
        /// Gets a value indicating whether this instance is warn enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is warn enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsWarnEnabled { get; private set; }

        /// <summary>
        /// Logs the specified query.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="query">The query.</param>
        /// <param name="context">The context.</param>
        public void Query<TEntity>(Func<IQueryable<TEntity>> query, DbContext context) where TEntity : class
        {
            if (IsDebugEnabled)
            {
                _logger.Debug(string.Format("Query Trace: Time {0} Query {1}",
                                            DateTime.Now.ToShortDateString(),
                                            GetTraceStringWithParameters<TEntity>(query.Invoke())));
            }
        }


        /// <summary>
        /// Gets the trace string with parameters.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query">The query.</param>
        /// <returns></returns>
        private string GetTraceStringWithParameters<T>(IQueryable<T> query)
        {
            var internalQuery = query.GetType()
                .GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(field => field.Name == "_internalQuery")
                .Select(field => field.GetValue(query))
                .First();

            var stringQuery = internalQuery.GetType()
                .GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(field => field.Name == "_objectQuery")
                .Select(field => field.GetValue(internalQuery))
                .Cast<ObjectQuery<T>>()
                .First();


            var traceString = stringQuery.ToTraceString().Replace("\n", "") + " ";

            return stringQuery.Parameters.Aggregate(traceString, (current, parameter) => current + (parameter.Name + " [" + parameter.ParameterType.FullName + "] = " + parameter.Value + "\n"));
        }
    }
}
