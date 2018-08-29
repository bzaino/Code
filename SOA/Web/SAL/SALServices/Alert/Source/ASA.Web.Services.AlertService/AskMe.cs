using System;
using System.Net.Mail;
using System.Text;
using Common.Logging.Log4Net;
using Common.Logging;
using ASA.Web.Services.Common;
using ASA.Web.Services.AlertService.DataContracts;

namespace ASA.Web.Services.AlertService
{
    public class AskMe : IAskMe
    {

        private const string Classname = "ASA.Web.Services.AlertService.AskMe";
        private static readonly Log.ServiceLogger.IASALog _log = Log.ServiceLogger.ASALogManager.GetLogger(Classname);

        public void BuildAskMeEmail(AskMeRequestModel requestModel, string emailRecipient)
        {
            String logMethodName = ".BuildAskMeEmail(AskMeRequestModel requestModel) - ";
            _log.Debug(logMethodName + "Begin Method");

            String body = String.Format("First Name: {0}<br>Last Name: {1}<br>Email: {2}<br>Membership Id: {3}<br><br>Question: {4}", requestModel.FirstName, requestModel.LastName, requestModel.FromEmailAddress, requestModel.MembershipId, requestModel.YourQuestion);
            ComposeAndSendEmail(requestModel.FromEmailAddress, emailRecipient, body, requestModel.Subject, requestModel);

            _log.Debug(logMethodName + "End Method");
        }
        public void BuildContentFeedBackEmail(ContentFeedBackModel requestModel, string emailRecipient)
        {
            String logMethodName = ".BuildContentFeedBackEmail(ContentFeedBackModel requestModel) - ";
            _log.Debug(logMethodName + "Begin Method");

            String body = String.Format("Membership ID: {0}<br>Rating: {1}<br>Content ID: {2}<br>FeedBack: {3}", requestModel.memberID, requestModel.ratingVal, requestModel.contentID, requestModel.emailBody);
            ComposeAndSendEmail(requestModel.FromEmailAddress, emailRecipient, body, requestModel.emailSubject, requestModel);

            _log.Debug(logMethodName + "End Method");
        }
        private void ComposeAndSendEmail(string fromEmail, string emailRecipient, string emailBody, string emailSubject, BaseWebModel requestModel)
        {
            String logMethodName = ".ComposeAndSendEmail(string fromEmail, string emailRecipient, string emailBody, string emailSubject) - ";
            _log.Debug(logMethodName + "Begin Method");
            // COV 10392
            using (MailMessage message = new MailMessage(fromEmail, emailRecipient, emailSubject, emailBody))
            {
                message.SubjectEncoding = Encoding.Default;
                message.IsBodyHtml = true;
                message.BodyEncoding = Encoding.Default;

                _log.Debug(logMethodName + string.Format("Sending email to {0}, SMTPServer ={1}:{2}, subject={3}", emailRecipient, Config.SMTPServer, Config.SMTPServerPort, emailSubject));
                // COV 10344

                try
                {
                    UtilityFunctions.SendEmail(message);
                }
                catch (Exception ex)
                {
                    _log.Error(String.Format("Error sending email: {0}\n\n{1}", ex.InnerException.StackTrace));
                    requestModel.ErrorList.Add(new Common.ErrorModel(String.Format("Error sending email: {0} - {1}", ex.InnerException.StackTrace)));
                }

            }
            _log.Debug(logMethodName + "End Method");
        }
    }
}
