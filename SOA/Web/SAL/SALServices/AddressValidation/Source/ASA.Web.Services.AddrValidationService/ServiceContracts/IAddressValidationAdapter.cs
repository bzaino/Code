using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASA.Web.Services.AddrValidationService.DataContracts;

namespace ASA.Web.Services.AddrValidationService.ServiceContracts
{
    public interface IAddressValidationAdapter
    {
        AddrValidationResponseModel ValidateAddress(AddressModel addr);
    }
}
