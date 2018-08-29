using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASA.Web.Services.SelfReportedService.Proxy.LoanManagement;
using ASA.Web.Services.Common;

namespace ASA.Web.Services.SelfReportedService.Proxy
{
    public interface IInvokeSelfReportedService
    {
        GetLoanSelfReportedEntryResponse GetSelfReported(GetLoanSelfReportedEntryRequest getRequest);
        ResultCodeModel SaveSelfReported(LoanCanonicalType SelfReported);
    }
}
