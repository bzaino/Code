using System.Linq;
using Asa.Salt.Web.Common.Types.Enums;
using Asa.Salt.Web.Services.BusinessServices.Interfaces;
using Asa.Salt.Web.Services.Data.Model.Database;
using Asa.Salt.Web.Services.Data.Repositories;
using Asa.Salt.Web.Services.Email.Processors.Interfaces;
using Asa.Salt.Web.Services.Logging;
using Member = Asa.Salt.Web.Services.Domain.Member;
using PaymentReminder = Asa.Salt.Web.Services.Domain.PaymentReminder;

namespace Asa.Salt.Web.Services.BusinessServices
{
    /// <summary>
    /// Provides a framework for email processsing using templates 
    /// from the database
    /// </summary>
    public class EmailService : IEmailService
    {
        
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILog _log;

        /// <summary>
        /// The mail processor
        /// </summary>
        private readonly IEmailProcessor _mailProcessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="MemberService"/> class.
        /// </summary>
        public EmailService(IEmailProcessor mailProcessor, ILog logger)
        {
            _log = logger;
            _mailProcessor = mailProcessor;
        }

        /// <summary>
        /// Sends member emails by type.
        /// </summary>
        /// <param name="emailType">Type of the email.</param>
        /// <param name="user">The user.</param>
        public void SendUserEmail(MemberEmailType emailType, Member user)
        {
            _mailProcessor.SendUserEmail(emailType, user);
        }

        /// <summary>
        /// Sends the payment reminder.
        /// </summary>
        /// <param name="reminder">The reminder.</param>
        public void SendPaymentReminder(PaymentReminder reminder)
        {
            _mailProcessor.SendPaymentReminder(reminder);
        }
    }
}
