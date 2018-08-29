using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASA.Web.Services.AppToolService.Proxy.AppTool;
using ASA.Web.Services.Common;

namespace ASA.Web.Services.AppToolService.Proxy
{
    public interface IInvokeAppToolService
    {
        GetAppToolResponse GetAppTool(GetAppToolRequest getRequest);
        ResultCodeModel SaveAppTool(AppToolCanonicalType appTool);
    }
}
