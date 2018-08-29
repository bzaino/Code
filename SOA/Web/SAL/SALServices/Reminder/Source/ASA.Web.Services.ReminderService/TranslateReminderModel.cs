using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ASA.Web.Services.ASAMemberService;
using ASA.Web.Services.Proxies.SALTService;
using ASA.Web.Services.ReminderService.DataContracts;
using log4net;

namespace ASA.Web.Services.ReminderService
{
    public static class MappingExtensions
    {
        /// <summary>
        /// Converts to the domain object.
        /// </summary>
        /// <param name="paymentReminders">The payment reminders.</param>
        /// <returns></returns>
        public static ReminderListModel ToDomainObject(this IList<PaymentReminderContract> paymentReminders)
        {
            var toReturn = new ReminderListModel();

            foreach (var paymentReminderContract in paymentReminders)
            {
                toReturn.Reminders.Add(paymentReminderContract.ToDomainObject());
            }

            return toReturn;
        }

        /// <summary>
        /// Converts to the domain object.
        /// </summary>
        /// <param name="paymentReminder">The payment reminder.</param>
        /// <returns></returns>
        public static ReminderModel ToDomainObject(this PaymentReminderContract paymentReminder)
        {
            var userAdapter = new AsaMemberAdapter();
            return new ReminderModel()
                {
                    DayOfMonth = paymentReminder.DayOfMonth,
                    ID = paymentReminder.PaymentReminderId.ToString(CultureInfo.InvariantCulture),
                    IsActive = true,
                    MemberId = userAdapter.GetMemberIdFromContext(),
                    IndividualId =  userAdapter.GetActiveDirectoryKeyFromContext(),
                    NumberOfLoans = paymentReminder.NumberOfLoans.HasValue?paymentReminder.NumberOfLoans.Value:0,
                    ServicerName = paymentReminder.ServicerName
                };
        }

        /// <summary>
        /// Converts to the data contract.
        /// </summary>
        /// <param name="paymentReminders">The payment reminders.</param>
        /// <returns></returns>
        public static IList<PaymentReminderContract> ToDataContract(this ReminderListModel paymentReminders)
        {
            return paymentReminders.Reminders.Select(reminder => reminder.ToDataContract()).ToList();
        }

        /// <summary>
        /// Converts to the data contract.
        /// </summary>
        /// <param name="paymentReminder">The payment reminder.</param>
        /// <returns></returns>
        public static PaymentReminderContract ToDataContract(this ReminderModel paymentReminder)
        {
            return new PaymentReminderContract()
            {
                DayOfMonth = paymentReminder.DayOfMonth,
                PaymentReminderId = !string.IsNullOrWhiteSpace(paymentReminder.ID)? int.Parse(paymentReminder.ID):0,
                MemberId = paymentReminder.MemberId,
                NumberOfLoans = paymentReminder.NumberOfLoans,
                ServicerName = paymentReminder.ServicerName
            };
        }
    }
}
