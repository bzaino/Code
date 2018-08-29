using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using Common.Logging;

namespace ASA.Web.Services.Common
{
    public static class ProxyHelper
    {
        private const string CLASSNAME = "ASA.Web.Services.Common.ProxyHelper";
        private static ASA.Log.ServiceLogger.IASALog _log = ASA.Log.ServiceLogger.ASALogManager.GetLogger(CLASSNAME);

        public static void HandleServiceException<T>(T client) where T : ICommunicationObject
        {
            _log.Debug("HandleServiceException<T> start");
            if (client != null)
            {
                if (client.State == CommunicationState.Created)
                {
                    CloseChannel(client);
                }
                else if (client.State == CommunicationState.Faulted)
                {
                    _log.Debug("HandleServiceException<T> client.Abort();");
                    client.Abort();
                }
            }
            _log.Debug("HandleServiceException<T> end");
        }

        public static void CloseChannel<T>(T client) where T : ICommunicationObject
        {
            _log.Debug("CloseChannel<T> start");
            try
            {
                client.Close();
            }
            catch (TimeoutException timeout)
            {
                _log.Info("Caught a timeout exception", timeout);
                if (client != null)
                {
                    _log.Debug("client.Abort");
                    client.Abort();
                }
            }
            catch(Exception e)
            {
                _log.Info("Caught an exception that wasn't a Timeout", e);
            }
            _log.Debug("CloseChannel<T> end");
        }
    }
}

