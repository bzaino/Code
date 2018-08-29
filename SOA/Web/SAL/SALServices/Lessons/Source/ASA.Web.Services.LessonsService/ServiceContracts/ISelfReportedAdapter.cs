using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASA.Web.Services.SelfReportedService.Proxy.DataContracts;
using ASA.Web.Services.Common;

namespace ASA.Web.Services.SelfReportedService.ServiceContracts
{
    public interface ISelfReportedAdapter
    {
        SelfReportedLoanListModel GetSelfReportedLoans(string searchId);
        ResultCodeModel SaveSelfReportedLoans(SelfReportedLoanListModel srLoans);
        ResultCodeModel SaveSelfReportedLoan(SelfReportedLoanModel srLoans);


        void LogSRLData(string msg, SelfReportedLoanListModel srLoans);
    }
}
