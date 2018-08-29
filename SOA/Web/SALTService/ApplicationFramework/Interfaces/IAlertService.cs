using System.Collections.Generic;
using Asa.Salt.Web.Services.Domain;

namespace Asa.Salt.Web.Services.BusinessServices.Interfaces
{
    public interface IAlertService
    {

        /// <summary>
        /// Gets the user's alerts.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns></returns>
        IList<MemberAlert> GetUserAlerts(int userId);

        /// <summary>
        /// Deletes the alert.
        /// </summary>
        /// <param name="alertId">The alert id.</param>
        /// <returns></returns>
        bool DeleteAlert(int alertId);
    }
}
