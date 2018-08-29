using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASA.Web.Services.LoanService.Proxy.LoanManagement;
using ASA.Web.Services.Common;

namespace ASA.Web.Services.LoanService.Proxy
{
    public interface IInvokeLoanManagementService
    {
        GetLoanResponse GetLoan(GetLoanRequest getRequest);
    }
}
