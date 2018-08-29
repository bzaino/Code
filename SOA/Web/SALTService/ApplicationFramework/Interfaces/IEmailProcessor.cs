using Asa.Salt.Web.Common.Types.Enums;

namespace Asa.Salt.Web.Services.BusinessServices.Interfaces
{
    public interface IEmailProcessor
    {
        void SendUserEmail(MemberEmailType emailType, int userId);
    }

}
