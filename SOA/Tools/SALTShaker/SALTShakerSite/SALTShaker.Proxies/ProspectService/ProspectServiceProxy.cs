using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.ServiceModel;
using SALTShaker.Proxies.PROSPECTService;
using log4net;

namespace SALTShaker.Proxies
{
    class ProspectServiceProxy : IDisposable
    {
        /// <summary>
        /// The logger
        /// </summary>
        private static readonly ILog Log = LogManager.GetLogger("ProspectServiceProxy");

        /// <summary>
        /// The Prospect service proxy.
        /// </summary>
        private ProspectServiceClient _client;

        /// <summary>
        /// Gets the proxy.
        /// </summary>
        /// <value>
        /// The proxy.
        /// </value>
        public ProspectServiceClient Proxy { get { return _client; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProspectServiceAgent" /> class.
        /// </summary>
        public ProspectServiceProxy()
        {
            _client = new ProspectServiceClient("BasicHttpBinding_IPROSPECTService");
        }

        /// <summary>
        /// Executes the specified proxy.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="proxy">The proxy.</param>
        /// <returns></returns>
        public TResult Execute<TResult>(Expression<Func<ProspectServiceClient, TResult>> proxy)
        {
            var methodName = GetMethodName(proxy);

            Log.Debug(string.Format("Prospect Service Agent: Executing {0}.", methodName));
            TResult toReturn = default(TResult);

            try
            {
                toReturn = proxy.Compile().Invoke(_client);
            }
            catch (Exception ex)
            {
                Log.Debug(string.Format("Error accessing Prospect Service: {0}.", ex.Message));
                return toReturn;
            }
            
            Log.Debug(string.Format("Prospect Service Agent: Execution of {0} complete.", methodName));

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
        public TResult ExecuteAsync<TResult>(Expression<Func<ProspectServiceClient, TResult>> proxy, AsyncCallback callback, object parameter)
        {
            var methodName = GetMethodName(proxy);

            Log.Debug(string.Format("Prospect Service Agent: Executing {0} Asynchronously.", methodName));
            var method = proxy.Compile();
            Func<TResult> action = () => method(_client);
            TResult toReturn = default(TResult);
            try
            {
                var result = action.BeginInvoke(callback, parameter);
                toReturn = action.EndInvoke(result);
            }
            catch (Exception ex)
            {
                Log.Debug(string.Format("Error accessing Prospect Service: {0}.", ex.Message));
                return toReturn;
            }
            Log.Debug(string.Format("Prospect Service Agent: Asynchronous Execution of {0} complete.", methodName));

            return toReturn;
        }


        /// <summary>
        /// Gets the name of the method for proxy delegate.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="proxy">The proxy.</param>
        /// <returns></returns>
        private string GetMethodName<TResult>(Expression<Func<ProspectServiceClient, TResult>> proxy)
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
                Log.Debug( ex.Message);
                _client.Abort();
            }

            this._client = null;
            GC.SuppressFinalize(this);

        }
    }
}
