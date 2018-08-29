using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASA.Web.Services.SelfReportedService.Proxy.LoanManagement;
using ASA.Web.Services.Common;
using ASA.ErrorHandling;
using System.ServiceModel;
using ASA.Log.ServiceLogger;

namespace ASA.Web.Services.SelfReportedService.Proxy
{
  
    public class InvokeSelfReportedService : IInvokeSelfReportedService
    {
        static readonly IASALog _log = ASALogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public GetLoanSelfReportedEntryResponse GetSelfReported(GetLoanSelfReportedEntryRequest getRequest)
        { 
           
            _log.Info("InvokeSelfReportedService.GetSelfReported() starting ...");
            LoanManagementClient client = null;  
            GetLoanSelfReportedEntryResponse response = null; 
            
            
            try
            {
                client = new LoanManagementClient();
                ILoanManagement lm = (ILoanManagement)client;
                response = lm.GetLoanSelfReportedEntry(getRequest);
            }
            catch (TimeoutException timeout)
            {
                _log.Error("InvokeSelfReportedService.GetSelfReported() Timeout Exception:" + timeout.Message);
                ProxyHelper.HandleServiceException(client);
            }
            catch (CommunicationException comm)
            {
                _log.Error("InvokeSelfReportedService.GetSelfReported() Communication Exception:" + comm.Message);
                ProxyHelper.HandleServiceException(client);
            }
            catch(Exception e)
            {
                _log.Error("InvokeSelfReportedService.GetSelfReported() Exception:" + e.Message);
            }
            finally
            {
                if (client!= null && client.State != CommunicationState.Closed)
                {
                    ProxyHelper.CloseChannel(client);
                }
            }

            
            _log.Info("InvokeSelfReportedService.GetSelfReported() ending ...");
            return response;
        }

        public ResultCodeModel SaveSelfReported(LoanCanonicalType SelfReported)
        {
            
            _log.Info("InvokeSelfReportedService.SaveSelfReported() starting ...");
            LoanManagementClient client = null;
            Dictionary<string, string> paramList = null;
            ResponseMessageList messageList = new ResponseMessageList();

            //try/catch here, and handle results/errors appropriately...
            ResultCodeModel result = new ResultCodeModel();
            
            
            try
            {
                client = new LoanManagementClient();
                result.ResultCode = client.SaveLoanSelfReportedEntry(paramList, ref SelfReported, out messageList);
            }
            catch (TimeoutException timeout)
            {
                ErrorModel error = new ErrorModel("There was a timeout problem calling the SelfReported Management Service", "InvokeSelfReportedMangementService");
                result.ErrorList.Add(error);
                result.ResultCode = 2;//FAIL;
                ProxyHelper.HandleServiceException(client);
                _log.Error("InvokeSelfReportedService.SaveSelfReported() Timeout Exception:" + timeout.Message);
            }
            catch (CommunicationException comm)
            {
                ErrorModel error = new ErrorModel("There was a communication problem calling the SelfReported Management Service", "InvokeSelfReportedMangementService");
                result.ErrorList.Add(error);
                result.ResultCode = 2;//FAIL;
                ProxyHelper.HandleServiceException(client);
                _log.Error("InvokeSelfReportedService.SaveSelfReported() Communication Exception:" + comm.Message);
            }
            catch (Exception e)
            {
                ErrorModel error = new ErrorModel("There was an exception thrown calling the SelfReported Management Service", "InvokeSelfReportedMangementService");
                result.ErrorList.Add(error);
                result.ResultCode = 2;//FAIL;
                _log.Error("InvokeSelfReportedService.SaveSelfReported() Exception:" + e.Message);
            }
            finally
            {
                if (client != null && client.State != CommunicationState.Closed)
                {
                    ProxyHelper.CloseChannel(client);
                }
            }

            
            _log.Info("InvokeSelfReportedService.SaveSelfReported() ending ...");
            return result;
        }

    }
    
}
