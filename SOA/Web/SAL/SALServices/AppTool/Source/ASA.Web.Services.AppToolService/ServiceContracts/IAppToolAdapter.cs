using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASA.Web.Services.AppToolService.Proxy.DataContracts;
using ASA.Web.Services.Common;

namespace ASA.Web.Services.AppToolService.ServiceContracts
{
    public interface IAppToolAdapter
    {
        AppToolModel GetAppTool(int searchId, int toolTypeId);
        ResultCodeModel SaveAppTool(AppToolModel p);
    }
}
