using Asa.Salt.Web.Common.Types.Enums;
using Asa.Salt.Web.Services.Domain;

namespace Asa.Salt.Web.Services.BusinessServices.Interfaces
{
    public interface IEmailService
    {
        /// <summary>
        /// Sends a user email.
        /// </summary>
        /// <param name="emailType">Type of the email.</param>
        /// <param name="user">The user.</param>
        void SendUserEmail(MemberEmailType emailType, Member user);

        /// <summary>
        /// Sends the payment reminder.
        /// </summary>
        /// <param name="reminder">The reminder.</param>
        void SendPaymentReminder(PaymentReminder reminder);
    }

}
