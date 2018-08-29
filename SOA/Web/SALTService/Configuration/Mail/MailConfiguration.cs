using System.Collections.Generic;

namespace Asa.Salt.Web.Services.Configuration.Mail
{
    public class MailConfiguration
    {
        public bool MailingEnabled { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public string DefaultMailingAddress { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public List<MailTemplate> Templates { get; set; }
    }

    public class MailTemplate
    {
        public string Type { get; set; }
        public string Path { get; set; }
        public string Subject { get; set; }
    }
}
