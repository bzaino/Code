using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Resources;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using ASA.Web.Services.ReminderService.DataContracts;
using ASA.Web.Services.Common;
using ASA.Web.Services.ReminderService.ServiceContracts;
using ASA.Web.Services.ReminderService.Validation;
using ASA.Web.Services.ASAMemberService;
using ASA.Web.Services.ReminderService.Exceptions;

namespace ASA.Web.Services.ReminderService
{
    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class Reminder
    {
        private const string Classname = "ASA.Web.Services.ReminderService.Reminder";
        private static readonly Log.ServiceLogger.IASALog _log = Log.ServiceLogger.ASALogManager.GetLogger(Classname);

        private const string _reminderAdapterExceptionMessage = "Unable to create a ReminderAdapter object from the ASA.Web.Services.ReminderService library. ";
        private IReminderAdapter _reminderAdapter = null;

        public Reminder()
        {
            _log.Info("ASA.Web.Services.ReminderService.Reminder() object being created ...");
            _reminderAdapter = new ReminderAdapter();
        }

        //[OperationContract]
        //[WebInvoke(UriTemplate = "SetReminder", Method = "POST")]
        //[Obsolete]
        //public ResultCodeModel InsertReminder(ReminderModel reminder)
        //{
        //   return new ResultCodeModel(0);
        //}

        //[OperationContract]
        //[WebInvoke(UriTemplate = "SetReminder", Method = "PUT")]
        //[Obsolete]
        //public ResultCodeModel UpdateReminder(ReminderModel reminder)
        //{
        //   return new ResultCodeModel(0);
        //}

        //[OperationContract]
        //[WebInvoke(UriTemplate = "DeleteReminder/{reminderId}", Method = "DELETE")]
        //[Obsolete]
        //public ResultCodeModel DeleteReminder(string reminderId)
        //{
        //   return new ResultCodeModel(0);
        //}

        /// <summary>
        /// Gets the reminders.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ReminderBadDataException">Null adapter in ASA.Web.Services.ReminderService.GetReminders()</exception>
        /// <exception cref="ReminderOperationException">Web Reminder Service - Exception in ASA.Web.Services.ReminderService.GetReminders()</exception>
        [WebGet(UriTemplate = "GetReminders", ResponseFormat = WebMessageFormat.Json)]
        [AspNetCacheProfile("DoNotCache")]
        public ReminderListModel GetReminders()
        {
            ReminderListModel rList = null;
            var memberAdapter = new AsaMemberAdapter();

            try
            {
                _log.Info("ASA.Web.Services.ReminderService.GetReminders() starting ...");
                int memberId = memberAdapter.GetMemberIdFromContext();

                if (_reminderAdapter == null)
                {
                    _log.Error("ASA.Web.Services.ReminderService.GetReminders(): " + _reminderAdapterExceptionMessage);
                    throw new ReminderBadDataException("Null adapter in ASA.Web.Services.ReminderService.GetReminders()");
                }
                else if (memberId <= 0)
                {
                    _log.Info("A user who is anonymous is trying to access GetReminders");
                    rList = new ReminderListModel();
                    ErrorModel error = new ErrorModel("A user who is anonymous is trying to access GetReminders", "Web Reminder Service");
                    rList.ErrorList.Add(error);
                }
                else
                {
                    rList = _reminderAdapter.GetReminders(memberId);
                }
            }
            catch (Exception ex)
            {
                _log.Error("ASA.Web.Services.ReminderService.GetReminders(): Exception => " + ex.ToString());
                throw new ReminderOperationException("Web Reminder Service - Exception in ASA.Web.Services.ReminderService.GetReminders()", ex);
            }

            _log.Info("ASA.Web.Services.ReminderService.GetReminders() ending ...");
            return rList;
        }

        /// <summary>
        /// Saves the reminders.
        /// </summary>
        /// <param name="rList">The r list.</param>
        /// <returns></returns>
        /// <exception cref="ReminderBadDataException">Null adapter in ASA.Web.Services.ReminderService.GetReminders()</exception>
        /// <exception cref="ReminderOperationException">Web Reminder Service - Exception in ASA.Web.Services.ReminderService.SaveReminders()</exception>
        [OperationContract]
        [WebInvoke(UriTemplate = "SetReminders", Method = "PUT")]
        public ResultCodeModel SaveReminders(ReminderListModel rList)
        {
            _log.Info("ASA.Web.Services.ReminderService.SaveReminders() starting ...");
            ResultCodeModel result = null;

            try
            {
                if (ReminderValidation.ValidateInputReminderList(rList))
                {
                    if (_reminderAdapter == null)
                    {
                        _log.Error("ASA.Web.Services.ReminderService.GetReminders(): " + _reminderAdapterExceptionMessage);
                        throw new ReminderBadDataException("Null adapter in ASA.Web.Services.ReminderService.GetReminders()");
                    }
                    else
                        result = _reminderAdapter.SaveReminders(rList);
                }
                else
                {
                    result = new ResultCodeModel();
                    ErrorModel error = new ErrorModel("Invalid information input for this reminder", "Web reminder Service");
                    _log.Error("ASA.Web.Services.ReminderService.SaveReminders(): Invalid information input for these reminders");
                    result.ErrorList.Add(error);
                }

            }

            catch (Exception ex)
            {
                _log.Error("ASA.Web.Services.ReminderService.SaveReminders(): Exception => " + ex.ToString());
                throw new ReminderOperationException("Web Reminder Service - Exception in ASA.Web.Services.ReminderService.SaveReminders()", ex);
            }

            _log.Info("ASA.Web.Services.ReminderService.SaveReminders() ending ...");
            return result;
        }
    }
}
