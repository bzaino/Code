using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASA.Web.Services.Proxies.SALTService;

namespace ASA.Web.Services.Proxies
{
    public class SALTServiceAgent
    {
        /// <summary>
        /// Executes the specified proxy.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="proxy">The proxy.</param>
        /// <returns></returns>
        public static TResult Execute<TResult>(Func<SALTServiceClient, TResult> proxy)
        {
            using (var client = new SALTServiceClient())
            {
                return proxy(client);
            }
        }
    }
}
