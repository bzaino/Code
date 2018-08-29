using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using ASA.Web.Services.AppToolService.Proxy.DataContracts;
using ASA.Web.Services.Common;

namespace ASA.Web.Services.AppToolService.ServiceContracts
{
    [ServiceContract]
    public interface IAppTool
    {
        [OperationContract]
        AppToolModel GetAppTool(string personId, string toolTypeId);

        [OperationContract]
        ResultCodeModel InsertAppTool(AppToolModel appTool);

        [OperationContract]
        ResultCodeModel UpdateAppTool(AppToolModel appTool);
    }
}
