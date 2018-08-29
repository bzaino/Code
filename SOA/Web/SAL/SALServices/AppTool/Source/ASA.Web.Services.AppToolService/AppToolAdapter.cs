using ASA.Log.ServiceLogger;
using ASA.Web.Services.AppToolService.Proxy;
using ASA.Web.Services.AppToolService.Proxy.AppTool;
using ASA.Web.Services.AppToolService.Proxy.DataContracts;
using ASA.Web.Services.AppToolService.ServiceContracts;
using ASA.Web.Services.Common;
using System.Reflection;

namespace ASA.Web.Services.AppToolService
{
    public class AppToolAdapter : IAppToolAdapter
    {
        private IInvokeAppToolService _proxy;
        public IASALog _log = ASALogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public AppToolAdapter()
        {
            _log.Debug("ASA.Web.Services.AppToolService.AppToolAdapter() object being created ...");
            _proxy = new InvokeAppToolService();
        }

        public AppToolModel GetAppTool(int personId, int toolTypeId)
        {
            GetAppToolRequest getRequest = TranslateAppToolModel.MapAppToolIdToGetRequest(personId, toolTypeId);
            GetAppToolResponse response = _proxy.GetAppTool(getRequest);
            return TranslateAppToolModel.MapGetResponseToModel(response);
        }

        public ResultCodeModel SaveAppTool(AppToolModel appTool)
        {
            AppToolCanonicalType canonicalAppTool = TranslateAppToolModel.MapModelToCanonical(appTool);
            return _proxy.SaveAppTool(canonicalAppTool);
        }
    }
}
