using System;
using System.Collections.Generic;
using Asa.Salt.Web.Services.Domain;

namespace Asa.Salt.Web.Services.BusinessServices.Interfaces
{
    public interface IAuditService
    {
        /// <summary>
        /// Logs the activity.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="activity">The activity.</param>
        void LogActivity(int userId, ActivityType activity);

        /// <summary>
        /// Logs the user activation.
        /// </summary>
        /// <param name="userId">The user id.</param>
        void LogUserActivation(int userId);

        /// <summary>
        /// Logs the user deactivation.
        /// </summary>
        /// <param name="userId">The user id.</param>
        void LogUserDeactivation(int userId);
    }
}
