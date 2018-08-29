using System.ServiceModel;

namespace Asa.Salt.Web.Services.Contracts.Operations
{
    [ServiceContract()]
    public interface ISaltJobService
    {
        /// <summary>
        /// Sends the payment reminders.
        /// </summary>
        /// <returns></returns>
        [OperationContract()]
        bool ProcessPaymentReminders();
    }
}
