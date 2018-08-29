using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using ASA.Web.Services.SelfReportedService.Proxy.DataContracts;
using ASA.Web.Services.Common;

namespace ASA.Web.Services.SelfReportedService.ServiceContracts
{
    [ServiceContract]
    public interface ISelfReported
    {
        [OperationContract]
        SelfReportedLoanListModel GetSelfReportedLoans();

        [OperationContract]
        SelfReportedLoanListModel SaveSelfReportedLoans(SelfReportedLoanListModel SelfReported);

        [OperationContract]
        ResultCodeModel SaveSelfReportedLoan(SelfReportedLoanModel SelfReported);
    }
}
