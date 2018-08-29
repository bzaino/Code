using System;
using System.Collections.Generic;
using System.ServiceModel;
using ASA.Log.ServiceLogger;
using ASA.Web.Services.Common;
using ASA.Web.Services.LoanService.Proxy.PersonManagement;
using AASA.Web.Services.LoanService.Proxy;
using Common.Logging;

namespace ASA.Web.Services.LoanService.Proxy
{
    public class InvokePersonManagementService : IInvokePersonManagementService
    {
        private const string CLASSNAME = "ASA.Web.Services.LoanService.Proxy.InvokePersonManagementService";
        static readonly ILog _log = LogManager.GetLogger(CLASSNAME);

        public GetPersonResponse GetPerson(GetPersonRequest getRequest)
        {
            _log.Debug("InvokePersonManagementService.GetPerson() starting ...");
            PersonManagementClient client = null;  
            GetPersonResponse response = null;

            try
            {
                client = new PersonManagementClient();
                _log.Debug("client created successfully");
                IPersonManagement pm = (IPersonManagement)client;
                response = pm.GetPerson(getRequest);
                _log.Debug("response was received from ODS PersonManagement service");
            }
            catch (TimeoutException timeout)
            {
                _log.Error("InvokePersonManagementService.GetPerson() Timeout Exception:" + timeout.Message);
                ProxyHelper.HandleServiceException(client);
            }
            catch (CommunicationException comm)
            {
                _log.Error("InvokePersonManagementService.GetPerson() Communication Exception:" + comm.Message);
                ProxyHelper.HandleServiceException(client);
            }
            catch (Exception e)
            {
                _log.Error("InvokePersonManagementService.GetPerson() Exception:" + e.Message);
            }
            finally
            {
                if (client != null && client.State != CommunicationState.Closed)
                {
                    ProxyHelper.CloseChannel(client);
                }
            }

            _log.Debug("InvokePersonManagementService.GetPerson() ending ...");
            return response;
        }

    }
}
