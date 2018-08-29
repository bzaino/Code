using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using ASA.Web.Services.Common;
using ASA.Web.Services.AlertService.DataContracts;

namespace ASA.Web.Services.AlertService.ServiceContracts
{
    [ServiceContract]
    public interface IAlert
    {
        [OperationContract]
        AlertListModel GetAlerts();

        [OperationContract]
        ResultCodeModel DeleteAlert(string alertId);

        [OperationContract]
        AlertInfoModel GetAlertInfo();
    }
}