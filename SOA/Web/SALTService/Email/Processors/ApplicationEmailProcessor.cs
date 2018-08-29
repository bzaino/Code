using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using Asa.Salt.Web.Common.Types.Enums;
using Asa.Salt.Web.Services.Configuration.Mail;
using Asa.Salt.Web.Services.Domain;
using Asa.Salt.Web.Services.Email.Processors.Interfaces;
using Asa.Salt.Web.Services.Email.PropertyBags;
using Asa.Salt.Web.Services.Logging;
using RazorEngine;

namespace Asa.Salt.Web.Services.Email.Processors
{
    /// <summary>
    /// Provides a framework for email processsing using templates 
    /// from the database or Razor
    /// </summary>
    public class ApplicationEmailProcessor : IEmailProcessor
    {
        /// <summary>
        /// The mail configuration
        /// </summary>
        private readonly IApplicationMailConfiguration _mailConfiguration;

        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILog _log;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationEmailProcessor" /> class.
        /// </summary>
        /// <param name="mailConfiguration">The mail configuration.</param>
        /// <param name="logger">The logger.</param>
        public ApplicationEmailProcessor(IApplicationMailConfiguration mailConfiguration, ILog logger)
        {
            _mailConfiguration = mailConfiguration;
            _log = logger;
        }

        /// <summary>
        /// Initializes the <see cref="ApplicationEmailProcessor"/> class.
        /// </summary>
        static ApplicationEmailProcessor()
        {
            try
            {
                foreach (var templateConfiguration in new ApplicationMailConfiguration().GetConfiguration().Templates)
                {
                    Razor.Compile(File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,templateConfiguration.Path)), templateConfiguration.Type);
                }
            }
            catch (Exception ex)
            {
                new Log().Error(ex.Message, ex);
            }
        }

        /// <summary>
        /// Sends member emails by type.
        /// </summary>
        /// <param name="emailType">Type of the email.</param>
        /// <param name="member">The member.</param>
        public  void SendUserEmail(MemberEmailType emailType, Member member)
        {
            var configuration = _mailConfiguration.GetConfiguration();

            if (configuration.MailingEnabled)
            {
                try
                {
                    var templateConfiguration = configuration.Templates.First(t => t.Type == emailType.ToString());
                    var html = Razor.Run(emailType.ToString(), member);
                    var subject = templateConfiguration.Subject;
                    var from = configuration.DefaultMailingAddress;
                    var to = member.EmailAddress;

                    var message = new MailMessage(from, to, subject, html) {IsBodyHtml = true};
                    message = ReplaceMemberValueTokens(message, member);

                    SendMail(message);
                }
                catch (Exception ex)
                {
                    _log.Error(ex.Message, ex);
                }
            }
        }

        /// <summary>
        /// Sends the payment reminder.
        /// </summary>
        /// <param name="reminder">The reminder.</param>
        public void SendPaymentReminder(PaymentReminder reminder)
        {
            var configuration = _mailConfiguration.GetConfiguration();

            if (configuration.MailingEnabled)
            {
                try
                {
                    var templateConfiguration = configuration.Templates.First(t => t.Type == MemberEmailType.PaymentReminderEmail.ToString());
                    var html = Razor.Run(MemberEmailType.PaymentReminderEmail.ToString(), reminder);
                    var subject = templateConfiguration.Subject;
                    var from = configuration.DefaultMailingAddress;
                    var to =reminder.Member.EmailAddress;

                    var message = new MailMessage(from, to, subject, html) { IsBodyHtml = true };
                    message = ReplaceMemberValueTokens(message, reminder.Member);
                    SendMail(message);
                }
                catch (Exception ex)
                {
                    _log.Error(ex.Message, ex);
                }
            }
        }

        /// <summary>
        /// Replaces the member value tokens.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="member">The member.</param>
        /// <returns></returns>
        private MailMessage ReplaceMemberValueTokens(MailMessage message, Member member)
        {
            var valuesPropertyBag = new MemberEmailPropertyBag(member);

            message.Subject = valuesPropertyBag.Keys.Aggregate(message.Subject,
                                                       (current, token) =>
                                                       current.Replace(string.Format("{0}", token),
                                                                       valuesPropertyBag[token]));
            message.Body = valuesPropertyBag.Keys.Aggregate(message.Body,
                                                      (current, token) =>
                                                      current.Replace(string.Format("{0}", token),
                                                                      valuesPropertyBag[token]));

            return message;
        }

        /// <summary>
        /// Sends the mail.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        private bool SendMail(MailMessage message)
        {
            var configuration = _mailConfiguration.GetConfiguration();

            if (configuration.MailingEnabled)
            {
                using (var mailClient = new SmtpClient(configuration.Host, configuration.Port))
                {
                    try
                    {
                        mailClient.Send(message);
                        message.Dispose();
                        message = null;
                    }
                    catch (SmtpFailedRecipientException ex)
                    {
                        _log.Info(ex.Message, ex);
                    }
                    catch (Exception ex)
                    {
                        _log.Error(ex.Message, ex);
                    }
                }
            }

            return true;
        }

    }
}
