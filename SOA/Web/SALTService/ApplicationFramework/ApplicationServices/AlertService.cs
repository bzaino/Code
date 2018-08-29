using System;
using System.Collections.Generic;
using System.Linq;
using Asa.Salt.Web.Services.BusinessServices.Interfaces;
using Asa.Salt.Web.Services.Data.Infrastructure;
using Asa.Salt.Web.Services.Data.Model.Database;
using Asa.Salt.Web.Services.Data.Repositories;
using Asa.Salt.Web.Services.Domain;
using Asa.Salt.Web.Services.Logging;
using MemberAlert = Asa.Salt.Web.Services.Domain.MemberAlert;

namespace Asa.Salt.Web.Services.BusinessServices
{
    /// <summary>
    /// The member service.
    /// </summary>
    public class AlertService : IAlertService
    {
        /// <summary>
        /// The user alert repository
        /// </summary>
        private readonly IRepository<MemberAlert, int> _memberAlertRepository;

        /// <summary>
        /// The database context
        /// </summary>
        private readonly SALTEntities _dbContext;

        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILog _log;


        /// <summary>
        /// Initializes a new instance of the <see cref="MemberService" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="dbContext">The db context.</param>
        /// <param name="memberAlertRepository">The member alert repository.</param>
        public AlertService(ILog logger, SALTEntities dbContext, IRepository<MemberAlert, int> memberAlertRepository)
        {
            _log = logger;
            _dbContext = dbContext;
            _memberAlertRepository = memberAlertRepository;
        }
 
        /// <summary>
        /// Gets the user's alerts.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns></returns>
        public IList<MemberAlert> GetUserAlerts(int userId)
        {
            return _memberAlertRepository.Get(ma => ma.MemberId == userId,null,"AlertType,Member").ToList();
        }

        /// <summary>
        /// Deletes the alert.
        /// </summary>
        /// <param name="alertId">The alert id.</param>
        /// <returns></returns>
        public bool DeleteAlert(int alertId)
        {
            try
            {
                var alert = _memberAlertRepository.Get(a => a.MemberAlertId == alertId).FirstOrDefault();

                if (alert != null)
                {
                    IUnitOfWork unitOfWork = new UnitOfWork<SALTEntities>(_dbContext);
                    _memberAlertRepository.Delete(alert);
                    unitOfWork.Commit();
                }

                return true;
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message,ex);
                return false;
            }
        }
    }
}
