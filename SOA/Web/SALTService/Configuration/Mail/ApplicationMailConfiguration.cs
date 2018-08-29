using System;
using System.Collections.Generic;
using System.Configuration;

namespace Asa.Salt.Web.Services.Configuration.Mail
{
    public class ApplicationMailConfiguration : IApplicationMailConfiguration
    {
        /// <summary>
        /// Gets the mail configuration.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public MailConfiguration GetConfiguration()
        {
            var configuration = new MailConfiguration();
            var section = (MailConfigurationSection)ConfigurationManager.GetSection("mailConfiguration");
            //COV-10357 -- added test for null Templates
            if (section != null && section.Templates != null)
            {
                configuration.MailingEnabled = section.SendMailEnabled;
                configuration.DefaultMailingAddress = section.DefaultFromAddress;
                configuration.Host = section.Host;
                configuration.Port = string.IsNullOrWhiteSpace(section.Port)?0 : Convert.ToInt32(section.Port);
                configuration.Username = section.Username;
                configuration.Password = section.Password;
                configuration.Templates = new List<MailTemplate>();

                foreach (TemplateConfigurationElement template in section.Templates)
                {
                    configuration.Templates.Add(new MailTemplate(){Path = template.Path, Subject = template.Subject, Type = template.Type});
                }
            }

            return configuration;
        }
    }
}
