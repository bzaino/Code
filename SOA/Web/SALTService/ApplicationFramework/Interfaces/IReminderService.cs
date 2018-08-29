using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Asa.Salt.Web.Common.Types.Enums;
using Asa.Salt.Web.Services.Domain;

namespace Asa.Salt.Web.Services.BusinessServices.Interfaces
{
    public interface IReminderService
    {

        /// <summary>
        /// Gets the reminders.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns></returns>
        IList<PaymentReminder> GetUserPaymentReminders(int userId);

        /// <summary>
        /// Saves the reminders.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="reminders">The reminders.</param>
        /// <returns></returns>
        RemindersUpdateStatus SavePaymentReminders(int userId, IList<PaymentReminder> reminders);

        /// <summary>
        /// Sends payment reminders to users 15 days prior to the payment due date.
        /// </summary>
        bool SendPaymentReminders();

        /// <summary>
        /// Deletes the payment reminders.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="reminders">The reminders.</param>
        /// <returns></returns>
        bool DeleteUserPaymentReminders(int userId, IList< PaymentReminder> reminders  = null );
    }
}
