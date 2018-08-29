using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Linq.Expressions;
using ASA.Web.Services.Proxies.SALTService;
using log4net;

namespace ASA.Web.Services.Proxies
{
    public class SaltServiceProxy : IDisposable
    {
        /// <summary>
        /// The logger
        /// </summary>
        private static readonly ILog Log = LogManager.GetLogger("SaltServiceProxy");

        /// <summary>
        /// The salt service proxy.
        /// </summary>
        private SaltServiceClient _client;

        /// <summary>
        /// Gets the proxy.
        /// </summary>
        /// <value>
        /// The proxy.
        /// </value>
        public SaltServiceClient Proxy { get { return _client; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="SaltServiceAgent" /> class.
        /// </summary>
        public SaltServiceProxy()
        {
            _client = new SaltServiceClient();
        }

        /// <summary>
        /// Executes the specified proxy.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="proxy">The proxy.</param>
        /// <returns></returns>
        public TResult Execute<TResult>(Expression<Func<SaltServiceClient, TResult>> proxy)
        {
            var methodName = GetMethodName(proxy);

            Log.Debug(string.Format("Salt Service Agent: Executing {0}.", methodName));
            var toReturn = proxy.Compile().Invoke(_client);
            Log.Debug(string.Format("Salt Service Agent: Execution of {0} complete.", methodName));

            return toReturn;
        }

        /// <summary>
        /// Executes the specified proxy asynchronously.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="proxy">The proxy.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="parameter">The parameter.</param>
        /// <returns></returns>
        public TResult ExecuteAsync<TResult>(Expression<Func<SaltServiceClient, TResult>> proxy, AsyncCallback callback, object parameter)
        {
            var methodName = GetMethodName(proxy);

            Log.Debug(string.Format("Salt Service Agent: Executing {0} Asynchronously.", methodName));
            var method = proxy.Compile();
            Func<TResult> action = () => method(_client);
            var result = action.BeginInvoke(callback, parameter);
            var toReturn = action.EndInvoke(result);
            Log.Debug(string.Format("Salt Service Agent: Asynchronous Execution of {0} complete.", methodName));

            return toReturn;
        }


        /// <summary>
        /// Gets the name of the method for proxy delegate.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="proxy">The proxy.</param>
        /// <returns></returns>
        private string GetMethodName<TResult>(Expression<Func<SaltServiceClient, TResult>> proxy)
        {
            var method = proxy.Body as MethodCallExpression;
            var methodName = method != null ? method.Method.Name : string.Empty;

            return methodName;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            try
            {
                _client.Close();
            }
            catch (Exception ex)
            {
                Log.Debug(string.Format("SaltServiceProxy:Dispose error {0}", ex.Message));
                _client.Abort();
            }

            this._client = null;
            GC.SuppressFinalize(this);

        }
    }
}
