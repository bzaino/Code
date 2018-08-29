using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using ASA.Web.Services.ASAMemberService;
using ASA.Web.Services.ASAMemberService.ServiceContracts;
using Common.Logging;

using ASA.Web.Services.Common;
using ASA.Web.Services.AlertService.DataContracts;
using ASA.Web.Services.AlertService.ServiceContracts;
using ASA.Web.Services.AlertService.Validation;
using ASA.Web.Services.AlertService.Exceptions;


namespace ASA.Web.Services.AlertService
{
    // Note that the pattern here is a little different from other services in this namespace.
    // This is due to a shift in desgin mid-way through the project. 
    // Orignally Alerts was meant to be an external service called via Ajax. 
    // This is still the case when getting the alert list however getting the alert 
    // count is done by this class but within the MVC application. 
    // As a result rather than use the member adapter we are using the WTF adapter for
    // user data. This type of re-factoring will be done through all services eventually.


    /// <summary>
    /// Member alerts from avectra.
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceContract]
    public class Alert
    {
        private const string Classname = "ASA.Web.Services.AlertService.Alert";
        private static readonly Log.ServiceLogger.IASALog _log = Log.ServiceLogger.ASALogManager.GetLogger(Classname);

        private const string AlertAdapterExceptionMessage = "Unable to create a AlertAdapter object from the ASA.Web.Services.AlertService library. ";
        private readonly IAlertAdapter _alertAdapter = null;
        public Alert()
        {
            const string logMethodName = ".ctor() - ";
            _log.Debug(logMethodName + "Begin Method");

            _alertAdapter = new AlertAdapter();
            _log.Debug(logMethodName + "End Method");

        }

        /// <summary>
        /// Gets all alerts for the user.  User is figured out from the Context.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="AlertBadDataException">Null adapter in ASA.Web.Services.AlertService.GetUserAlerts()</exception>
        [OperationContract]
        [WebGet(UriTemplate = "GetAlerts", ResponseFormat = WebMessageFormat.Json)]
        [AspNetCacheProfile("DoNotCache")]
        public AlertListModel GetAlerts()
        {
            const string logMethodName = ".GetUserAlerts() - ";
            _log.Debug(logMethodName + "Begin Method");

            AlertListModel aList = null;
            try
            {
                var adapter = new AsaMemberAdapter();
                if (adapter.GetMemberIdFromContext()>0)
                {
                    if (_alertAdapter == null)
                    {
                        _log.Error(logMethodName + "ASA.Web.Services.AlertService.GetUserAlerts(): " + AlertAdapterExceptionMessage);
                        throw new AlertBadDataException("Null adapter in ASA.Web.Services.AlertService.GetUserAlerts()");
                    }
                    
                    else
                    {
                        aList = _alertAdapter.GetAlerts(adapter.GetMemberIdFromContext());
                    }
                }
                else 
                {
                    _log.Warn(logMethodName + "A user who is anonymous is trying to access GetUserAlerts");
                    aList = new AlertListModel();
                    var error = new ErrorModel("A user who is anonymous is trying to access GetUserAlerts", "Web Alert Service");
                    aList.ErrorList.Add(error);
                }


            }
            catch (Exception ex)
            {
                //Alerts is non-critical functionality so if there is some kind of problem getting alerts we simply log it and move on my returning a null set.
                _log.Error(logMethodName + "ASA.Web.Services.AlertService.GetUserAlerts(): Exception => " + ex.ToString());
                //throw new AlertOperationException("Web Alert Service - Exception in ASA.Web.Services.AlertService.GetUserAlerts()", ex);
            }

            _log.Debug(logMethodName + "End Method");

            return aList ?? (aList = new AlertListModel());
        }

        /// <summary>
        /// Deletes the alert.
        /// </summary>
        /// <param name="alertId">The alert id.</param>
        /// <returns></returns>
        /// <exception cref="AlertBadDataException">Null, empty, or invalid format alert ID in ASA.Web.Services.AlertService.AlertAdapter.DeleteAlert().</exception>
        /// <exception cref="AlertOperationException">Web Alert Service - Exception in ASA.Web.Services.AlertService.DeleteAlert()</exception>
        [OperationContract]
        [WebInvoke(UriTemplate = "DeleteAlert/{alertId}", Method = "DELETE")]
        public ResultCodeModel DeleteAlert(string alertId)
        {
            const string logMethodName = ".DeleteAlert(string alertId) - ";
            _log.Debug(logMethodName + "Begin Method");

            var result = new ResultCodeModel();

            if (!AlertValidation.ValidateAlertId(alertId))
            {
                _log.Info(logMethodName + "Null or empty alertId in ASA.Web.Services.AlertService.DeleteAlert().");
                throw new AlertBadDataException("Null, empty, or invalid format alert ID in ASA.Web.Services.AlertService.AlertAdapter.DeleteAlert()."); 
            }

            else
            {
                try
                {
                    result = _alertAdapter.DeleteAlert(alertId);
                }
                catch (Exception ex)
                {
                    _log.Error(logMethodName + "ASA.Web.Services.AlertService.DeleteAlert(): Exception => " + ex.ToString());
                    throw new AlertOperationException("Web Alert Service - Exception in ASA.Web.Services.AlertService.DeleteAlert()", ex);  
                }
            }

            _log.Debug(logMethodName + "End Method");
            return result;
        }

        /// <summary>
        /// Get the number of alerts for the user.  User is figured out from the Context.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="AlertBadDataException">Null adapter in ASA.Web.Services.AlertService.GetAlertInfo()</exception>
        [OperationContract]
        [WebGet(UriTemplate = "GetAlertInfo", ResponseFormat = WebMessageFormat.Json)]
        [AspNetCacheProfile("DoNotCache")]
        public AlertInfoModel GetAlertInfo()
        {
            const string logMethodName = ".GetAlertInfo() - ";
            _log.Debug(logMethodName + "Begin Method");

            AlertInfoModel aInfo = null;
            int memberId = new AsaMemberAdapter().GetMemberIdFromContext();
            try
            {

                if (memberId>0)
                {
                    if (_alertAdapter == null)
                    {
                        _log.Error(logMethodName + "ASA.Web.Services.AlertService.GetAlertInfo(): " + AlertAdapterExceptionMessage);
                        throw new AlertBadDataException("Null adapter in ASA.Web.Services.AlertService.GetAlertInfo()");
                    }
                   
                    else
                    {
                        aInfo = _alertAdapter.GetAlertInfo(memberId);
                    }
                }
                else 
                {
                    _log.Warn(logMethodName + "A user who is anonymous is trying to access GetAlertInfo");
                    aInfo = new AlertInfoModel();
                    var error = new ErrorModel("A user who is anonymous is trying to access GetAlertInfo", "Web Alert Service");
                    aInfo.ErrorList.Add(error);
                }


            }
            catch (Exception ex)
            {
                //Alerts is non-critical functionality so if there is some kind of problem getting alerts we simply log it and move on my returning a null set.
                _log.Error(logMethodName + "ASA.Web.Services.AlertService.GetAlertInfo(): Exception => " + ex.ToString());
                //throw new AlertOperationException("Web Alert Service - Exception in ASA.Web.Services.AlertService.GetAlertInfo()", ex);
            }

            _log.Debug(logMethodName + "End Method");

            return aInfo ?? (aInfo = new AlertInfoModel());
        }

        #region AskMe Calls

        [OperationContract]
        [WebInvoke(UriTemplate = "JustAsk", Method = "POST")]
        public AskMeRequestModel AskMe(AskMeRequestModel requestModel)
        {
            if (requestModel.IsValid())
            {
                //Create AskMe object
                IAskMe askMeAdapter = new AskMe();
                //Pass request model to email builder function. Email will be sent from within
                askMeAdapter.BuildAskMeEmail(requestModel, Config.AskMeEmailRecipient);

                return requestModel;
            }

            //request model invalid, handle gracefully
            return requestModel ?? (requestModel = new AskMeRequestModel());
        }

        #endregion

        #region SALT Live Email Us Calls

        [OperationContract]
        [WebInvoke(UriTemplate = "SaltLiveEmailUs", Method = "POST")]
        public AskMeRequestModel SaltLiveEmailUs(AskMeRequestModel requestModel)
        {
            if (requestModel.IsValid())
            {
                //Create AskMe object
                IAskMe askMeAdapter = new AskMe();
                //Pass request model to email builder function. Email will be sent from within
                askMeAdapter.BuildAskMeEmail(requestModel, Config.SaltLiveEmailRecipient);

                return requestModel;
            }

            //request model invalid, handle gracefully
            return requestModel ?? (requestModel = new AskMeRequestModel());
        }

        #endregion


        #region Content Feedback Email Calls

        [OperationContract]
        [WebInvoke(UriTemplate = "ContentFeedbackEmail", Method = "POST")]
        public ContentFeedBackModel ContentFeedbackEmail(ContentFeedBackModel requestModel)
        {
            if (requestModel.IsValid())
            {
                //Create AskMe object
                IAskMe askMeAdapter = new AskMe();
                //Pass request model to email builder function. Email will be sent from within
                askMeAdapter.BuildContentFeedBackEmail(requestModel, Config.ContentFeedbackEmail);

                return requestModel;
            }

            //request model invalid, handle gracefully
            return requestModel ?? (requestModel = new ContentFeedBackModel());
        }

        #endregion
    }
}
