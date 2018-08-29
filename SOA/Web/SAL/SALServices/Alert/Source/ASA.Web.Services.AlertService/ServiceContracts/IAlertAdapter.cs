using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASA.Web.Services.AlertService.DataContracts;
using ASA.Web.Services.Common;

namespace ASA.Web.Services.AlertService.ServiceContracts
{
    public interface IAlertAdapter
    {
        /// <summary>
        /// Gets the alerts.
        /// </summary>
        /// <param name="memberId">The member id.</param>
        /// <returns></returns>
        AlertListModel GetAlerts(int memberId);

        /// <summary>
        /// Deletes the alert.
        /// </summary>
        /// <param name="alertId">The alert id.</param>
        /// <returns></returns>
        ResultCodeModel DeleteAlert(string alertId);

        /// <summary>
        /// Gets the alert info.
        /// </summary>
        /// <param name="memberId">The member id.</param>
        /// <returns></returns>
        AlertInfoModel GetAlertInfo(int memberId);
    }
}
