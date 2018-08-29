using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASA.Web.Services.SelfReportedService.Proxy.DataContracts;
using ASA.Web.Services.ASAMemberService.DataContracts;

namespace ASA.Web.Services.LoanService.ServiceContracts
{
    public interface ILoanAdapter
    {
        SelfReportedLoanListModel GetLoans(string ssn, ASAMemberModel member);
    }
}
