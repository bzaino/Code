using System.Threading;
using Asa.Salt.Web.Services.BusinessServices;
using Asa.Salt.Web.Services.BusinessServices.Interfaces;
using Asa.Salt.Web.Services.Configuration.Mail;
using Asa.Salt.Web.Services.Data.Model.Database;
using Asa.Salt.Web.Services.Data.Repositories;
using Asa.Salt.Web.Services.Email.Processors;
using Asa.Salt.Web.Services.Jobs.Interfaces;
using Asa.Salt.Web.Services.Logging;

namespace Asa.Salt.Web.Services.Jobs.Processors
{
    
    public class PaymentReminderProcessor : ISupportTimerService
    {

        /// <summary>
        /// The payment reminder service
        /// </summary>
        private readonly IReminderService _reminderService;

        /// <summary>
        /// A lock obj
        /// </summary>
        private static readonly object _lockObj = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentReminderProcessor"/> class.
        /// </summary>
        public PaymentReminderProcessor(IReminderService reminderService)
        {
            _reminderService = reminderService;
        }

        /// <summary>
        /// Processes the payment reminders.
        /// </summary>
        /// <returns></returns>
        public bool ProcessPaymentReminders()
        {
            _reminderService.SendPaymentReminders();
            return true;
        }

        /// <summary>
        /// ISupportTimerService interface
        /// </summary>
        public void OnTimerElapsed()
        {
            lock (_lockObj)
            {
                ProcessPaymentReminders();
            }
        }

       
    }
}
