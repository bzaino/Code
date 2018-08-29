using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASA.Web.Services.AlertService.ServiceContracts;
using ASA.Web.Services.Common;
using ASA.Web.Services.AlertService.DataContracts;

namespace ASA.Web.Services.AlertService
{
    public class MockAlertAdapter : IAlertAdapter
    {
        public MockAlertAdapter()
        {
        }

        public DataContracts.AlertListModel GetAlerts(string individualId)
        {
            return MockJsonLoader.GetJsonObjectFromFile<AlertListModel>("AlertService", @"GetAlerts");
        }

        public Common.ResultCodeModel DeleteAlert(string alertId)
        {
            return MockJsonLoader.GetJsonObjectFromFile<ResultCodeModel>("AlertService", @"DeleteAlert.{alertId}");
        }

        public DataContracts.AlertInfoModel GetAlertInfo(string individualId)
        {
            return MockJsonLoader.GetJsonObjectFromFile<AlertInfoModel>("AlertService", @"GetAlertInfo");
        }
    }
}
