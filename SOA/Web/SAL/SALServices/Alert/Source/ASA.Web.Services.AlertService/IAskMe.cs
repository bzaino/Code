using ASA.Web.Services.AlertService.DataContracts;

namespace ASA.Web.Services.AlertService
{
    public interface IAskMe
    {
        /// <summary>
        ///Accepts an AskMeRequestModel, parses it and creates an email
        ///</summary>
        ///
        void BuildAskMeEmail(AskMeRequestModel requestModel, string emailRecipient);
        void BuildContentFeedBackEmail(ContentFeedBackModel requestModel, string emailRecipient);
    }
}
