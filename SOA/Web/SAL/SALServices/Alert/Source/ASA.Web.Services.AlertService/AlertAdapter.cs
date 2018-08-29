using System;
using ASA.Web.Services.AlertService.DataContracts;
using ASA.Web.Services.AlertService.Exceptions;
using ASA.Web.Services.AlertService.ServiceContracts;
using ASA.Web.Services.ASAMemberService;
using ASA.Web.Services.Common;
using ASA.Web.Services.Proxies;

namespace ASA.Web.Services.AlertService
{
    public class AlertAdapter : IAlertAdapter
    {
        private const string Classname = "ASA.Web.Services.AlertService.AlertAdapter";
        private static readonly Log.ServiceLogger.IASALog _log = Log.ServiceLogger.ASALogManager.GetLogger(Classname);

        public AlertListModel GetAlerts(int memberId)
        {
            const string logMethodName = ".GetUserAlerts(string individualId) - ";
            _log.Debug(logMethodName + "Begin Method");

            var aList = new AlertListModel();

            try
            {
               aList.Alerts= SaltServiceAgent.GetUserAlerts(memberId).ToDomainObject();
            }

            catch (Exception ex)
            {
                _log.Error(logMethodName + "ASA.Web.Services.AlertService.AlertAdapter.GetUserAlerts(): Exception =>" + ex.ToString());
                throw new AlertOperationException("Web Alert Service - ASA.Web.Services.AlertService.AlertAdapter.GetUserAlerts()", ex);
            }

            _log.Debug(logMethodName + "End Method");
            return aList;
        }

        public Common.ResultCodeModel DeleteAlert(string alertId)
        {
            var logMethodName = ".DeleteAlert(string alertId) - ";
            _log.Debug(logMethodName + "Begin Method");

           var result = new Common.ResultCodeModel(0);
            if (IsAlertForPersonLoggedIn(alertId))
            {

                try
                {
                    SaltServiceAgent.DeleteAlert(Convert.ToInt32(alertId));
                    result.ResultCode = 1;
                }

                catch (Exception ex)
                {
                    _log.Error(logMethodName + "ASA.Web.Services.AlertService.AlertAdapter.DeleteAlert(): Exception =>" + ex.ToString());
                    throw new AlertOperationException("Web Alert Service - ASA.Web.Services.AlertService.AlertAdapter.DeleteAlert()", ex);
                }
            }
            else
            {
                _log.Warn(logMethodName + "ASA.Web.Services.AlertService.AlertAdapter.DeleteAlert(): Someone is trying to delete alerts that don't belong to them!");
                result.ErrorList.Add(new ErrorModel("Alert could not be deleted"));
            }

            _log.Debug(logMethodName + "End Method");
            return result;
        }

        public DataContracts.AlertInfoModel GetAlertInfo(int memberId)
        {
            String logMethodName = ".GetAlertInfo(string individualId) - ";
            _log.Debug(logMethodName + "Begin Method");

            var aInfo = new AlertInfoModel();
            aInfo.AlertCount = 0;

            try
            {
              
                aInfo.AlertCount= SaltServiceAgent.GetUserAlerts(memberId).ToDomainObject().Count;
            }
            catch (Exception ex)
            {
                _log.Error(logMethodName + "ASA.Web.Services.AlertService.AlertAdapter.GetAlertInfo(): Exception =>" + ex.ToString());
                throw new AlertOperationException("Web Alert Service - ASA.Web.Services.AlertService.AlertAdapter.GetAlertInfo()", ex);
            }

            _log.Debug(logMethodName + "End Method");
            return aInfo;
        }

        private bool IsAlertForPersonLoggedIn(string alertId)
        {
            bool alertBelongsToPersonLoggedIn = false;
            //get Id of the member currently logged-in
            int memberId = new AsaMemberAdapter().GetMemberIdFromContext();

            
            //get list of current Alerts's for the member logged-in
            if (memberId>0)
            {
                AlertListModel alertList = GetAlerts(memberId);
                foreach (AlertModel a in alertList.Alerts)
                {
                    if (a.ID == alertId)
                        alertBelongsToPersonLoggedIn = true;
                }
            }

            return alertBelongsToPersonLoggedIn;
        }
    }
}
