using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Logging;
using ASA.Web.Services.AddrValidationService.DataContracts;
using ASA.Web.Services.AddrValidationService.QasProWeb;
using ASA.Web.Services.Common;
using System.ServiceModel;

namespace ASA.Web.Services.AddrValidationService.Proxy
{
    public class InvokeQasService : IInvokeQasService
    {
        private const string CLASSNAME = "ASA.Web.Services.AddrValidationService.Proxy.InvokeQasService";
        static ASA.Log.ServiceLogger.IASALog _log = ASA.Log.ServiceLogger.ASALogManager.GetLogger(CLASSNAME);

        public QASearchResult ValidateAddress(QASearch search)
        {
            _log.Info("InvokeQasService.ValidateAddress() starting ...");
            QAPortTypeClient client = null;
            QASearchResult result = null;

            try
            {
                client = new QAPortTypeClient();
                _log.Info("InvokeQasService.ValidateAddress() client created ...");
                int retryCount = 0;
                result = client.DoSearch(search);
                if (result == null && retryCount == 0)
                {
                    // retry once
                    result = client.DoSearch(search);
                    retryCount++;
                }
                _log.Info("InvokeQasService.ValidateAddress() result was returned from service ...");
            }
            catch (TimeoutException timeout)
            {
                _log.Error("InvokeQasService.ValidateAddress() Timeout Exception:" + timeout.Message);
                ProxyHelper.HandleServiceException(client);
            }
            catch (CommunicationException comm)
            {
                _log.Error("InvokeQasService.ValidateAddress() Communication Exception:" + comm.Message);
                ProxyHelper.HandleServiceException(client);
            }
            catch (Exception e)
            {
                _log.Error("InvokeQasService.ValidateAddress() Exception:" + e.Message);
            }
            finally
            {
                if (client != null && client.State != CommunicationState.Closed)
                {
                    ProxyHelper.CloseChannel(client);
                }
            }

            _log.Info("InvokeQasService.ValidateAddress() ending ...");
            return result;
        }
    }
}
