using Asa.Salt.Web.Common.Types.Unity;
using Asa.Salt.Web.Services.BusinessServices.Interfaces;
using Asa.Salt.Web.Services.Contracts.Operations;

namespace Asa.Salt.Web.Services.Application.Implementation.Services
{

    public class SaltJobService : ISaltJobService
    {
        /// <summary>
        /// The reminder service
        /// </summary>
        private readonly ILazyResolver<IReminderService> _reminderService;

        /// <summary>
        /// Initializes a new instance of the <see cref="SaltService"/> class.
        /// </summary>
        public SaltJobService(ILazyResolver<IReminderService> reminderService)
        {
            _reminderService = reminderService;
        }

        /// <summary>
        /// Sends the payment reminders.
        /// </summary>
        /// <returns></returns>
        public bool ProcessPaymentReminders()
        {
            return _reminderService.Resolve().SendPaymentReminders();
        }
    }
}
