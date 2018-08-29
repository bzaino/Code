using System;
using System.Linq;
using Asa.Salt.Web.Common.Types.Enums;
using Asa.Salt.Web.Services.BusinessServices.Interfaces;
using Asa.Salt.Web.Services.Configuration.Mail;
using Asa.Salt.Web.Services.Data.Model.Database;
using Asa.Salt.Web.Services.Data.PropertyBags;
using Asa.Salt.Web.Services.Data.Repositories;
using System.Net.Mail;
using Member = Asa.Salt.Web.Services.Domain.Member;
using RefEmailTemplate = Asa.Salt.Web.Services.Domain.EmailTemplate;

namespace Asa.Salt.Web.Services.BusinessServices
{
    /// <summary>
    /// Provides a framework for email processsing using templates 
    /// from the database
    /// </summary>
    public class EmailProcessor : IEmailProcessor
    {
        /// <summary>
        /// The email repository
        /// </summary>
        private readonly IRepository<RefEmailTemplate, int> _emailTemplateRepository;

        /// <summary>
        /// The user repository
        /// </summary>
        private readonly IRepository<Member, int> _memberRepository;

        /// <summary>
        /// The database context
        /// </summary>
        private readonly SALTEntities _dbContext;

        /// <summary>
        /// The mail configuration
        /// </summary>
        private readonly IApplicationMailConfiguration _mailConfiguration;

        /// <summary>
        /// Initializes a new instance of the <see cref="MemberService"/> class.
        /// </summary>
        public EmailProcessor(IApplicationMailConfiguration mailConfiguration)
        {
            _dbContext = new SALTEntities();
            _emailTemplateRepository = new Repository<RefEmailTemplate>(_dbContext);
            _memberRepository = new Repository<Member>(_dbContext);
            _mailConfiguration = mailConfiguration;
        }

        /// <summary>
        /// Sends member emails by type.
        /// </summary>
        /// <param name="emailType">Type of the email.</param>
        /// <param name="userId">The user id.</param>
        public void SendUserEmail(MemberEmailType emailType, int userId)
        {
            var configuration = _mailConfiguration.GetConfiguration();

            if (configuration.MailingEnabled)
            {
                var mailType = int.Parse(emailType.ToString("d"));
                var mailTemplate = _emailTemplateRepository.Get(e => e.EmailTemplateId == mailType).FirstOrDefault();
                if (mailTemplate == null) return;

                var user = _memberRepository.Get(m => m.MemberId == userId).FirstOrDefault();
                var subject = mailTemplate.EmailSubject;
                var body = mailTemplate.EmailBody;
                var from = configuration.DefaultMailingAddress;
                var to = user.EmailAddress;

                var valuesPropertyBag = new MemberEmailPropertyBag(userId);

                body = valuesPropertyBag.Keys.Aggregate(body, (current, token) => current.Replace(string.Format("{0}", token), valuesPropertyBag[token]).Replace("{~", "").Replace("}", ""));
                subject = valuesPropertyBag.Keys.Aggregate(subject, (current, token) => current.Replace(string.Format("{0}", token), valuesPropertyBag[token]).Replace("{~", "").Replace("}", ""));

                var message = new MailMessage(from, to, subject, body){IsBodyHtml = true};
                var mailClient = new SmtpClient(configuration.Host, configuration.Port.Value);

                try
                {
                    mailClient.Send(message);
                }
                catch (Exception ex)
                {
                }
            }
        }


    }
}
