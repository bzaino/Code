using Asa.Salt.Web.Common.Types.Enums;
using Asa.Salt.Web.Services.Domain;

namespace Asa.Salt.Web.Services.Email.Processors.Interfaces
{
    public interface IEmailProcessor
    {
        void SendUserEmail(MemberEmailType emailType, Member member);
        void SendPaymentReminder(PaymentReminder reminder);
    }

}
