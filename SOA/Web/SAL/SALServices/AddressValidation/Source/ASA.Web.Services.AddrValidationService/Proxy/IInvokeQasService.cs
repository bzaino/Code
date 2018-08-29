using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASA.Web.Services.AddrValidationService.DataContracts;
using ASA.Web.Services.AddrValidationService.QasProWeb;

namespace ASA.Web.Services.AddrValidationService.Proxy
{
    public interface IInvokeQasService
    {
        QASearchResult ValidateAddress(QASearch validationRequest);
    }
}
