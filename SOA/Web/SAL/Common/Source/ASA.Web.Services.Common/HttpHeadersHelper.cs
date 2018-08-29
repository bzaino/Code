using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Web;
using System.Net;

namespace ASA.Web.Services.Common
{
    public static class HttpHeadersHelper
    {
        private const string CLASSNAME = "ASA.Web.Services.Common.HttpHeadersHelper";
        private static ASA.Log.ServiceLogger.IASALog _log = ASA.Log.ServiceLogger.ASALogManager.GetLogger(CLASSNAME);

        public static void SetNoCacheResponseHeaders(WebOperationContext context)
        {
            //LogRequestHeaders(context);

            // set CacheControl header
            HttpResponseHeader cacheHeader = HttpResponseHeader.CacheControl;
            String cacheControlValue = String.Format("no-cache, no-store");
            context.OutgoingResponse.Headers.Add(cacheHeader, cacheControlValue);

            HttpResponseHeader pragmaHeader = HttpResponseHeader.Pragma;
            String pragmaValue = String.Format("no-cache");
            context.OutgoingResponse.Headers.Add(pragmaHeader, pragmaValue);
        }

        public static void LogRequestHeaders(WebOperationContext context)
        {
            StringBuilder sb = new StringBuilder("\n");
            try
            {
                if (context != null && context.IncomingRequest != null && context.IncomingRequest.Headers != null)
                {
                    for (int i = 0; i < context.IncomingRequest.Headers.Count; i++)
                    {
                        String header = context.IncomingRequest.Headers.GetKey(i);
                        String[] values = context.IncomingRequest.Headers.GetValues(header);
                        if (values.Length > 0)
                        {
                            sb.AppendLine(string.Format("The values of {0} header are : ", header));
                            for (int j = 0; j < values.Length; j++)
                                sb.Append(string.Format("\t{0}", values[j]));
                        }
                        else
                            sb.AppendLine("There is no value associated with the header");
                    }
                }
                _log.Debug(sb.ToString());
            }
            catch
            {
                _log.Debug("Problem logging Http headers.");
            }
        }
    }
}
