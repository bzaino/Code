using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using ASA.Web.Services.SelfReportedService.Proxy.DataContracts;

namespace ASA.Web.Services.LoanService.ServiceContracts
{
    [ServiceContract]
    public interface ILoan
    {
        [OperationContract]
        SelfReportedLoanListModel GetLoans(string ssn);

    }
}
