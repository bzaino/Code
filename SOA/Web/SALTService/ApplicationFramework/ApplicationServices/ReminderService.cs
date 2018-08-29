using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Asa.Salt.Web.Common.Types.Constants;
using Asa.Salt.Web.Common.Types.Enums;
using Asa.Salt.Web.Services.BusinessServices.Interfaces;
using Asa.Salt.Web.Services.Data.Infrastructure;
using Asa.Salt.Web.Services.Data.Model.Database;
using Asa.Salt.Web.Services.Data.Repositories;
using Asa.Salt.Web.Services.Logging;
using Asa.Salt.Web.Services.SaltSecurity.Utilities;
using PaymentReminder = Asa.Salt.Web.Services.Domain.PaymentReminder;
using MemberAlert = Asa.Salt.Web.Services.Domain.MemberAlert;
using MemberOrganization = Asa.Salt.Web.Services.Domain.MemberOrganization;

namespace Asa.Salt.Web.Services.BusinessServices
{
    public class ReminderService : IReminderService
    {

        /// <summary>
        /// The db context
        /// </summary>
        private readonly SALTEntities _dbContext;

        /// <summary>
        /// The payment reminder repository
        /// </summary>
        private readonly IRepository<PaymentReminder, int> _paymentReminderRepository;

        /// <summary>
        /// The member alert repository
        /// </summary>
        private readonly IRepository<MemberAlert, int> _memberAlertRepository;

        /// <summary>
        /// The member organization repository
        /// </summary>
        private readonly IRepository<MemberOrganization, int> _memberOrganizationRepository;

        /// <summary>
        /// The email service.
        /// </summary>
        private readonly IEmailService _emailService;

        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILog _log;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReminderService" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="mailService">The mail service.</param>
        /// <param name="dbContext">The db context.</param>
        /// <param name="paymentReminderRepository">The payment reminder repository.</param>
        /// <param name="memberAlertRepository">The member alert repository.</param>
        /// <param name="memberOrganizationRepository">The member organization repository.</param>
        public ReminderService(ILog logger, IEmailService mailService, SALTEntities dbContext, IRepository<PaymentReminder, int> paymentReminderRepository, IRepository<MemberAlert, int> memberAlertRepository, IRepository<MemberOrganization, int> memberOrganizationRepository)
        {
            _log = logger;
            _dbContext = dbContext;
            _paymentReminderRepository = paymentReminderRepository;
            _memberAlertRepository = memberAlertRepository;
            _memberOrganizationRepository = memberOrganizationRepository;
            _emailService = mailService;
        }

        /// <summary>
        /// Gets the reminders.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns></returns>
        public IList<PaymentReminder> GetUserPaymentReminders(int userId)
        {
            return _paymentReminderRepository.Get(p => p.MemberId == userId).ToList();
        }

        /// <summary>
        /// Saves the reminders.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="reminders">The reminders.</param>
        /// <returns></returns>
        public RemindersUpdateStatus SavePaymentReminders(int userId, IList<PaymentReminder> reminders)
        {
            var toReturn = RemindersUpdateStatus.Success;

            try
            {
                var originalReminders = GetUserPaymentReminders(userId);

                //purge all reminders
                DeleteUserPaymentReminders(userId, reminders != null && reminders.Any() ? originalReminders.Where(r => !reminders.Contains(r)).ToList() : null);

                if (reminders != null && reminders.Any())
                {
                    foreach (var reminder in reminders.Where(r => !originalReminders.Contains(r)))
                    {
                        try
                        {
                            if (!reminder.Validate().Any())
                            {
                                // the SALT application allows partial success
                                IUnitOfWork unitOfWork = new UnitOfWork<SALTEntities>(_dbContext);
                                reminder.CreatedDate = DateTime.Now;
                                reminder.CreatedBy = Principal.GetIdentity();
                                _paymentReminderRepository.Add(reminder);

                                unitOfWork.Commit();
                            }
                        }
                        catch (Exception ex)
                        {
                            _log.Error(ex.Message, ex);
                            toReturn = RemindersUpdateStatus.PartialSuccess;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
                toReturn = RemindersUpdateStatus.Failure;
            }

            return toReturn;
        }

        /// <summary>
        /// Sends payment reminders to users 15 days prior to the payment due date.
        /// </summary>
        public bool SendPaymentReminders()
        {
            const string logMethodName = "SendPaymentReminders()";
            _log.Debug(logMethodName + "Begin Method");

            var reminderList = _paymentReminderRepository.GetAll(null, "Member");

            reminderList = reminderList.Where(
                pr => ((pr.NextPaymentDueDate - (pr.LastReminderSentDate.HasValue ? pr.LastReminderSentDate.Value : DateTime.MinValue)).TotalDays >= 15
                       && (pr.NextPaymentDueDate - DateTime.Now).TotalDays <= 15)
            ).ToList();

            foreach (var reminder in reminderList)
            {
                reminder.LastReminderSentDate = DateTime.Now;

                var daysToPayment = Convert.ToInt32(reminder.NextPaymentDueDate.Subtract(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day)).TotalDays);
                var paymentDateString = reminder.NextPaymentDueDate.ToString("m");

                if (daysToPayment == 0 && reminder.NextPaymentDueDate.Date != DateTime.Now.Date)
                {
                    daysToPayment++;
                }
                if (reminder.Member.IsContactAllowed && reminder.Member.IsMemberActive)
                {
                    var alert = new MemberAlert
                    {
                        MemberId = reminder.MemberId,
                        AlertTypeId = AlertTypes.Individual,
                        AlertTitle = "Payment Reminder",
                        AlertIssueDate = DateTime.Now,
                        IsAlertViewed = false,
                        AlertMessage =
                            String.Format("{0} PAYMENT REMINDER ALERT {5}" +
                                          "{1}, you have a payment due with {2} in {3} day{4}. {5}" +
                                          "Be sure to submit your payment before the due date to avoid late fees or other charges."
                                          , paymentDateString
                                          , reminder.Member.FirstName
                                          , reminder.ServicerName
                                          , daysToPayment
                                          , daysToPayment == 1 ? string.Empty : "s"
                                          , Environment.NewLine
                                         ),
                        CreatedDate = DateTime.Now,
                        CreatedBy = Principal.GetIdentity()
                    };

                    var organizationList = _memberOrganizationRepository.Get(m => m.MemberID == reminder.MemberId && m.EffectiveEndDate.HasValue == false, null, "RefOrganization");

                    foreach (MemberOrganization organization in organizationList)
                    {
                        reminder.Member.MemberOrganizations.Add(organization);
                    }

                    //create the member alert and update the payment reminder in a single transaction
                    try
                    {
                        _log.Debug(logMethodName + String.Format("updating reminder.Id={0}", reminder.Id));
                        _log.Debug(logMethodName + String.Format("updating reminder.MemberId={0}", reminder.MemberId));

                        var serializedAlert = JsonConvert.SerializeObject(alert);
                        _log.Debug(logMethodName + String.Format("alert={0}", serializedAlert.ToString()));

                        //the payment reminder is already loaded into context
                        IUnitOfWork unitOfWork = new UnitOfWork<SALTEntities>(_dbContext);
                        _memberAlertRepository.Add(alert);
                        _paymentReminderRepository.Update(reminder);
                        unitOfWork.Commit();
                        _emailService.SendPaymentReminder(reminder);
                        System.Threading.Thread.Sleep(100);
                    }
                    catch (Exception ex)
                    {
                        _log.Error(ex.Message, ex);
                    }
                }
            }

            _log.Debug(logMethodName + "End Method");
            return true;
        }


        /// <summary>
        /// Deletes payments reminders for the given user.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="reminders">The reminders.</param>
        /// <returns></returns>
        public bool DeleteUserPaymentReminders(int userId, IList<PaymentReminder> reminders = null)
        {
            IUnitOfWork unitOfWork = new UnitOfWork<SALTEntities>(_dbContext);
            foreach (var reminder in _paymentReminderRepository.Get(p => p.MemberId == userId, null, string.Empty).ToList().Where(reminder => reminders == null || reminders.Any(r => r.PaymentReminderId == reminder.PaymentReminderId)))
            {
                _paymentReminderRepository.Delete(reminder);
            }
            unitOfWork.Commit();

            return true;
        }

    }
}
