using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASA.Web.Services.LoanService.Proxy.PersonManagement;
using ASA.Web.Services.Common;

namespace AASA.Web.Services.LoanService.Proxy
{
    public interface IInvokePersonManagementService
    {
        GetPersonResponse GetPerson(GetPersonRequest getRequest);
    }
}
