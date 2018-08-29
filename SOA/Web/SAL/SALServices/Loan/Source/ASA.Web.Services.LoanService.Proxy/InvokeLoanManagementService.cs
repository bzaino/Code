using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using ASA.Web.Services.LoanService.Proxy.LoanManagement;
using ASA.Web.Services.Common;
using ASA.Log.ServiceLogger;
using Common.Logging;

namespace ASA.Web.Services.LoanService.Proxy
{
    public class InvokeLoanManagementService : IInvokeLoanManagementService
    {
        private const string CLASSNAME = "ASA.Web.Services.LoanService.Proxy.InvokeLoanManagementService";
        static readonly ILog _log = LogManager.GetLogger(CLASSNAME);

        public GetLoanResponse GetLoan(GetLoanRequest getRequest)
        {
            _log.Debug("InvokeLoanManagementService.GetLoan() starting ...");
            LoanManagementClient client = null;

            GetLoanResponse response = null;
            try
            {
                client = new LoanManagementClient();
                _log.Debug("client created successfully");
                ILoanManagement lm = (ILoanManagement)client;
                response = lm.GetLoan(getRequest);
                _log.Debug("response was received from ODS LoanManagement service");
            }
            catch (TimeoutException timeout)
            {
                _log.Error("InvokeLoanManagementService.GetLoan() Timeout Exception:" + timeout.Message);
                ProxyHelper.HandleServiceException(client);
            }
            catch (CommunicationException comm)
            {
                _log.Error("InvokeLoanManagementService.GetLoan() Communication Exception:" + comm.Message);
                ProxyHelper.HandleServiceException(client);
            }
            catch (Exception e)
            {
                _log.Error("InvokeLoanManagementService.GetLoan() Exception:" + e.Message);
            }
            finally
            {
                if (client !=null && client.State != CommunicationState.Closed)
                {
                    ProxyHelper.CloseChannel(client);
                }
            }

            _log.Debug("InvokeLoanManagementService.GetLoan() ending ...");
            return response;
        }

    }
}
