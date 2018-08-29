using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASA.Web.Services.AppToolService.Proxy.DataContracts;
using ASA.Web.Services.Common;
using ASA.Web.Services.AppToolService.ServiceContracts;

namespace ASA.Web.Services.AppToolService
{
    public class MockAppToolAdapter : IAppToolAdapter
    {
        public MockAppToolAdapter()
        {
        }

        public AppToolModel GetAppTool(int personId, int toolTypeId)
        {
            return MockJsonLoader.GetJsonObjectFromFile<AppToolModel>("AppToolService", "{PERSONID}.{TOOLTYPE}");
        }

        public ResultCodeModel SaveAppTool(AppToolModel appTool)
        {
            return MockJsonLoader.GetJsonObjectFromFile<ResultCodeModel>("AppToolService", "AppToolObject");
        }
    }
}
