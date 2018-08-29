using System;

namespace Asa.Salt.Web.Services.Domain
{
    public class ActiveDirectoryUser
    {
        public Guid ActiveDirectoryKey { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string PasswordReminderQuestion { get; set; }
        public string PasswordReminderQuestionAnswer { get; set; }
    }
}
