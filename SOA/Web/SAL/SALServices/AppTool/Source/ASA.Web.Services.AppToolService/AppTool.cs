using System;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using ASA.Log.ServiceLogger;
using ASA.Web.Services.AppToolService.Validation;
using ASA.Web.Services.AppToolService.Proxy.DataContracts;
using ASA.Web.Services.AppToolService.ServiceContracts;
using ASA.Web.Services.Common;

namespace ASA.Web.Services.AppToolService
{
    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class AppTool
    {
        static readonly IASALog _log = ASALogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private const string _appToolAdapterExceptionMessage = "Unable to create a AppToolAdapter object from the ASA.Web.Services.AppToolService.ServiceImplementation library. ";
        private IAppToolAdapter _appToolAdapter = null;
        public AppTool()
        {
            _log.Info("ASA.Web.Services.AppToolService.ServiceImplementation.AppTool() object being created ...");
            if (Config.Testing)
                _appToolAdapter = new MockAppToolAdapter();
            else
                _appToolAdapter = new AppToolAdapter();
        }

        [OperationContract]
        [WebGet(UriTemplate = "/{toolType}", ResponseFormat = WebMessageFormat.Json)]
        [AspNetCacheProfile("DoNotCache")]
        public AppToolModel GetAppTool(string toolType)//(string personId, string toolType)
        {            
            AppToolModel AppTool = null;

            string personId = ""; // TODO: get "individualId" from security context here, and use that value.
            if (AppToolValidation.ValidateSearchId(personId) && AppToolValidation.ValidateSearchId(toolType))
            {
                int searchId; int toolTypeId;
                if (Int32.TryParse(personId, out searchId) && Int32.TryParse(toolType, out toolTypeId))
                {
                    if (_appToolAdapter == null)
                    {
                        _log.Error(_appToolAdapterExceptionMessage);
                        AppTool = new AppToolModel();
                        ErrorModel error = new ErrorModel(_appToolAdapterExceptionMessage, "Web AppTool Service");
                        AppTool.ErrorList.Add(error);
                    }
                    else
                        AppTool = _appToolAdapter.GetAppTool(searchId, toolTypeId);
                }
            }
            else
            {
                AppTool = new AppToolModel();
                ErrorModel error = new ErrorModel("Invalid search criteria", "Web AppTool Service");
                AppTool.ErrorList.Add(error);
            }

            return AppTool;
        }


        [OperationContract]
        [WebInvoke(UriTemplate = "AppToolObject", Method = "POST")]
        public ResultCodeModel InsertAppTool(AppToolModel appTool)
        {
            _log.Info("ASA.Web.Services.AppToolService.ServiceImplementation.InsertAppTool() starting ...");
            ResultCodeModel result = null;

            if (AppToolValidation.ValidateInputAppTool(appTool))
            {
                if (_appToolAdapter == null)
                {
                    _log.Error(_appToolAdapterExceptionMessage);
                    result = new ResultCodeModel();
                    ErrorModel error = new ErrorModel(_appToolAdapterExceptionMessage, "Web AppTool Service");
                    _log.Error("ASA.Web.Services.AppToolService.ServiceImplementation.InsertAppTool(): " + _appToolAdapterExceptionMessage);
                    result.ErrorList.Add(error);
                }
                else
                    result = _appToolAdapter.SaveAppTool(appTool);
            }
            else
            {
                result = new ResultCodeModel();
                ErrorModel error = new ErrorModel("Invalid information input for this appTool", "Web AppTool Service");
                _log.Error("ASA.Web.Services.AppToolService.ServiceImplementation.InsertAppTool(): Invalid information input for this appTool");
                result.ErrorList.Add(error);
            }


            _log.Info("ASA.Web.Services.AppToolService.ServiceImplementation.InsertAppTool() ending ...");
            return result;
        }

        [OperationContract]
        [WebInvoke(UriTemplate = "AppToolObject", BodyStyle = WebMessageBodyStyle.Bare, Method = "PUT", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public ResultCodeModel UpdateAppTool(AppToolModel appTool)
        {
            _log.Info("ASA.Web.Services.AppToolService.ServiceImplementation.UpdateAppTool() starting ...");
            ResultCodeModel result = null;

            if (AppToolValidation.ValidateInputAppTool(appTool))
            {
                if (_appToolAdapter == null)
                {
                    _log.Error(_appToolAdapterExceptionMessage);
                    result = new ResultCodeModel();
                    ErrorModel error = new ErrorModel(_appToolAdapterExceptionMessage, "Web AppTool Service");
                    _log.Error("ASA.Web.Services.AppToolService.ServiceImplementation.UpdateAppTool(): " + _appToolAdapterExceptionMessage);
                    result.ErrorList.Add(error);
                }
                else
                    result = _appToolAdapter.SaveAppTool(appTool);
            }
            else
            {
                result = new ResultCodeModel();
                ErrorModel error = new ErrorModel("Invalid information input for this appTool", "Web AppTool Service");
                _log.Error("ASA.Web.Services.AppToolService.ServiceImplementation.UpdateAppTool(): Invalid information input for this appTool");
                result.ErrorList.Add(error);
            }

            _log.Info("ASA.Web.Services.AppToolService.ServiceImplementation.UpdateAppTool() ending ...");
            return result;
        }
    }
}
