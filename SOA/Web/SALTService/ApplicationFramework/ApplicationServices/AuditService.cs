using System;
using System.Collections.Generic;
using System.Linq;
using Asa.Salt.Web.Services.BusinessServices.Interfaces;
using Asa.Salt.Web.Services.Data.Infrastructure;
using Asa.Salt.Web.Services.Data.Model.Database;
using Asa.Salt.Web.Services.Data.Repositories;
using Asa.Salt.Web.Services.Domain;
using Asa.Salt.Web.Services.Logging;
using ActivityType = Asa.Salt.Web.Services.Domain.ActivityType;
using MemberAlert = Asa.Salt.Web.Services.Domain.MemberAlert;

namespace Asa.Salt.Web.Services.BusinessServices
{
    /// <summary>
    /// The audit service.
    /// </summary>
    public class AuditService : IAuditService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuditService"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public AuditService(ILog logger)
        {
            
        }

        /// <summary>
        /// Logs the activity.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="activity">The activity.</param>
        public void LogActivity(int userId, ActivityType activity)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Logs the user activation.
        /// </summary>
        /// <param name="userId">The user id.</param>
        public void LogUserActivation(int userId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Logs the user deactivation.
        /// </summary>
        /// <param name="userId">The user id.</param>
        public void LogUserDeactivation(int userId)
        {
            throw new NotImplementedException();
        }
    }
}
