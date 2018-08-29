using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace Asa.Salt.Web.Services.Logging
{
    public interface ILog
    {

        /// <summary>
        /// Configures the logger.
        /// </summary>
        /// <param name="className">Name of the class.</param>
        void ConfigureLogger(string className);

        /// <summary>
        /// Traces the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        void Trace(string message);

        /// <summary>
        /// Traces the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        void Trace(string message, Exception exception);

        /// <summary>
        /// Log a message string with the Debug level.
        /// </summary>
        /// <param name="message">The message string to log.</param>
        void Debug(string message);


        /// <summary>
        /// Log a message string with the Debug level.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        void Debug(string message, Exception exception);


        /// <summary>
        /// Log a message string with the Info level.
        /// </summary>
        /// <param name="message">The message.</param>
        void Info(string message);


        /// <summary>
        /// Log a message string with the Info level.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        void Info(string message, Exception exception);

        /// <summary>
        /// Log a message string with the Warn level.
        /// </summary>
        /// <param name="message">The message.</param>
        void Warn(string message);

        /// <summary>
        /// Log a message string with the Warn level.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        void Warn(string message, Exception exception);

        /// <summary>
        /// Log a message string with the Error level.
        /// </summary>
        /// <param name="message">The message.</param>
        void Error(string message);

        /// <summary>
        /// Errors the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        void Error(string message, Exception exception);

        /// <summary>
        /// Log a message string with the Fatal level.
        /// </summary>
        /// <param name="message">The message.</param>
        void Fatal(string message);

        /// <summary>
        /// Log a message string with the Fatal level.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        void Fatal(string message, Exception exception);

        /// <summary>
        /// Gets a value indicating whether this instance is trace enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is trace enabled; otherwise, <c>false</c>.
        /// </value>
        bool IsTraceEnabled
        {
            get;
        }


        /// <summary>
        /// Gets a value indicating whether this instance is debug enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is debug enabled; otherwise, <c>false</c>.
        /// </value>
        bool IsDebugEnabled
        {
            get;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is error enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is error enabled; otherwise, <c>false</c>.
        /// </value>
        bool IsErrorEnabled
        {
            get;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is fatal enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is fatal enabled; otherwise, <c>false</c>.
        /// </value>
        bool IsFatalEnabled
        {
            get;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is info enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is info enabled; otherwise, <c>false</c>.
        /// </value>
        bool IsInfoEnabled
        {
            get;
        }


        /// <summary>
        /// Gets a value indicating whether this instance is warn enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is warn enabled; otherwise, <c>false</c>.
        /// </value>
        bool IsWarnEnabled
        {
            get;
        }

        /// <summary>
        /// Logs the specified query.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="query">The query.</param>
        /// <param name="context">The context.</param>
        void Query<TEntity>(Func<IQueryable<TEntity>> query, DbContext context) where TEntity : class;
    }
}
