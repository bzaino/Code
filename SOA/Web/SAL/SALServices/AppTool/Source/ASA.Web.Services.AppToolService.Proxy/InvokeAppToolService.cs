using System;
using System.Collections.Generic;
using System.ServiceModel;
using ASA.Log.ServiceLogger;
using ASA.Web.Services.AppToolService.Proxy.AppTool;
using ASA.Web.Services.Common;

namespace ASA.Web.Services.AppToolService.Proxy
{
    public class InvokeAppToolService : IInvokeAppToolService
    {
        static readonly IASALog _log = ASALogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public GetAppToolResponse GetAppTool(GetAppToolRequest getRequest)
        {
            _log.Info("InvokeAppToolService.GetAppTool() starting ...");
            AppToolClient client = null;  
            GetAppToolResponse response = null;
            try
            {
                client = new AppToolClient();
                IAppTool pm = (IAppTool)client;
                response = pm.GetAppTool(getRequest);
            }
            catch (TimeoutException timeout)
            {
                _log.Error("InvokeAppToolService.GetAppTool() Timeout Exception:" + timeout.Message);
                ProxyHelper.HandleServiceException(client);
            }
            catch (CommunicationException comm)
            {
                _log.Error("InvokeAppToolService.GetAppTool() Communication Exception:" + comm.Message);
                ProxyHelper.HandleServiceException(client);
            }
            catch(Exception e)
            {
                _log.Error("InvokeAppToolService.GetAppTool() Exception:" + e.Message);
            }
            finally
            {
                if (client!= null && client.State != CommunicationState.Closed)
                {
                    ProxyHelper.CloseChannel(client);
                }
            }


            _log.Info("InvokeAppToolService.GetAppTool() ending ...");
            return response;
        }

        public ResultCodeModel SaveAppTool(AppToolCanonicalType AppTool)
        {

            _log.Info("InvokeAppToolService.SaveAppTool() starting ...");
            AppToolClient client = null;
            Dictionary<string, string> paramList = null;
            ResponseMessageList messageList = new ResponseMessageList();

            //try/catch here, and handle results/errors appropriately...
            ResultCodeModel result = new ResultCodeModel();
            try
            {
                client = new AppToolClient();
                result.ResultCode = client.SaveAppTool(paramList, ref AppTool, out messageList);
            }
            catch (TimeoutException timeout)
            {
                ErrorModel error = new ErrorModel("There was a timeout problem calling the AppTool Management Service", "InvokeAppToolMangementService");
                result.ErrorList.Add(error);
                ProxyHelper.HandleServiceException(client);
                _log.Error("InvokeAppToolService.SaveAppTool() Timeout Exception:" + timeout.Message);
            }
            catch (CommunicationException comm)
            {
                ErrorModel error = new ErrorModel("There was a communication problem calling the AppTool Management Service", "InvokeAppToolMangementService");
                result.ErrorList.Add(error);
                ProxyHelper.HandleServiceException(client);
                _log.Error("InvokeAppToolService.SaveAppTool() Communication Exception:" + comm.Message);
            }
            catch (Exception e)
            {
                _log.Error("InvokeAppToolService.SaveAppTool() Exception:" + e.Message);
            }
            finally
            {
                if (client != null && client.State != CommunicationState.Closed)
                {
                    ProxyHelper.CloseChannel(client);
                }
            }

            _log.Info("InvokeAppToolService.SaveAppTool() ending ...");
            return result;
        }

    }
}
