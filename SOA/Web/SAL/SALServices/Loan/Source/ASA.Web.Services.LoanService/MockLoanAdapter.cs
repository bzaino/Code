using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASA.Web.Services.LoanService.ServiceContracts;
using ASA.Log.ServiceLogger;
using ASA.Web.Services.LoanService.Proxy;
using System.Reflection;
using ASA.Web.Services.SelfReportedService.Proxy.DataContracts;
using ASA.Web.Services.Common;
using ASA.Web.Services.ASAMemberService.DataContracts;

namespace ASA.Web.Services.LoanService
{
    public class MockLoanAdapter : ILoanAdapter
    {
        public MockLoanAdapter()
        {
        }

        public SelfReportedLoanListModel GetLoans(string ssn, ASAMemberModel member)
        {
            return MockJsonLoader.GetJsonObjectFromFile<SelfReportedLoanListModel>("LoanService", "{SSN}");
        }
    }
}
