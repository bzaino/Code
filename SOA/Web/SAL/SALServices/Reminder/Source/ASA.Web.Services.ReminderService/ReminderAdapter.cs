using System;
using System.Linq;
using Asa.Salt.Web.Common.Types.Enums;
using ASA.Web.Services.ASAMemberService;
using ASA.Web.Services.Common;
using ASA.Web.Services.Proxies;
using ASA.Web.Services.ReminderService.DataContracts;
using ASA.Web.Services.ReminderService.Exceptions;
using ASA.Web.Services.ReminderService.ServiceContracts;
using log4net;

namespace ASA.Web.Services.ReminderService
{
    public class ReminderAdapter : IReminderAdapter
    {
        private const string CLASSNAME = "Reminder";
        private static readonly ILog _log = LogManager.GetLogger(CLASSNAME);

        public ReminderAdapter()
        {
            _log.Debug("ASA.Web.Services.ReminderService.ReminderAdapter() object being created ...");
        }

        #region Main Functions

        /// <summary>
        /// Gets the reminders.
        /// </summary>
        /// <param name="memberId">The member id.</param>
        /// <returns></returns>
        /// <exception cref="ReminderOperationException">Web Reminder Service - ASA.Web.Services.ReminderService.ReminderAdapter.GetReminders()</exception>
        public ReminderListModel GetReminders(int  memberId)
        {
            var rList = new ReminderListModel();
            
            try
            {
                rList = SaltServiceAgent.GetUserPaymentReminders(memberId).ToDomainObject();
            }
            catch (Exception ex)
            {
                _log.Error("ASA.Web.Services.ReminderService.ReminderAdapter.GetReminders(): Exception =>" + ex.ToString());
                throw new ReminderOperationException("Web Reminder Service - ASA.Web.Services.ReminderService.ReminderAdapter.GetReminders()", ex);
            }       

            return rList;
        }

        /// <summary>
        /// Saves the reminders.
        /// </summary>
        /// <param name="rList">The r list.</param>
        /// <returns></returns>
        /// <exception cref="ReminderOperationException">Web Reminder Service - ASA.Web.Services.ReminderService.ReminderAdapter.SaveReminders()</exception>
        public ResultCodeModel SaveReminders(ReminderListModel rList)
        {

            var toReturn = new ResultCodeModel(1);

            try
            {
                var memberAdapter = new AsaMemberAdapter();
                int memberId = memberAdapter.GetMemberIdFromContext();
                foreach (var r in rList.Reminders)
                {
                    r.MemberId = memberId;
                }

                rList.Reminders = rList.Reminders.Where(r => r.IsActive).ToList();

                var result = SaltServiceAgent.SaveUserPaymentReminders(memberId,rList.ToDataContract());

                switch (result)
                {
                    case RemindersUpdateStatus.Failure:
                        toReturn.ResultCode = 0;
                        break;
                    case RemindersUpdateStatus.Success:
                        toReturn.ResultCode = 1;
                        break;
                    case RemindersUpdateStatus.PartialSuccess:
                        toReturn.ResultCode = 2;
                        break;
                }
            }

            catch (Exception ex)
            {
                _log.Error("ASA.Web.Services.ReminderService.ReminderAdapter.SaveReminders(): Exception =>" + ex.ToString());
                throw new ReminderOperationException("Web Reminder Service - ASA.Web.Services.ReminderService.ReminderAdapter.SaveReminders()", ex);
            }
            
            return toReturn;
        }

        #endregion

    }
}
