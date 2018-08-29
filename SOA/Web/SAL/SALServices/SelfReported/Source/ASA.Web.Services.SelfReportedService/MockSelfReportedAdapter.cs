using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
//using ASA.Common;
using ASA.Log.ServiceLogger;
using ASA.Web.Services.SelfReportedService.Proxy.DataContracts;
using ASA.Web.Services.SelfReportedService.Proxy;
using ASA.Web.Services.SelfReportedService.ServiceContracts;
using ASA.Web.Services.Common;
using System.Reflection;

namespace ASA.Web.Services.SelfReportedService
{
    public class MockSelfReportedAdapter : ISelfReportedAdapter
    {
        public MockSelfReportedAdapter()
        {
        }

        public void LogSRLData(string p, SelfReportedLoanListModel srList)
        {
            //throw new NotImplementedException();
        }

        public SelfReportedLoanListModel GetSelfReportedLoans(string individualId)
        {
             return MockJsonLoader.GetJsonObjectFromFile<SelfReportedLoanListModel>("SelfReportedService", "GetSelfReportedLoans");
        }

        public ResultCodeModel SaveSelfReportedLoans(SelfReportedLoanListModel srLoans)
        {
             return MockJsonLoader.GetJsonObjectFromFile<ResultCodeModel>("SelfReportedService", "SelfReportedLoans");
        }

        public ResultCodeModel SaveSelfReportedLoan(SelfReportedLoanModel srLoan)
        {
             return MockJsonLoader.GetJsonObjectFromFile<ResultCodeModel>("SelfReportedService", "SelfReportedLoan");
        }

    }
}
