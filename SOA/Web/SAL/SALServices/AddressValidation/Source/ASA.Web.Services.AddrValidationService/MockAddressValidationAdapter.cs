using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASA.Web.Services.AddrValidationService.ServiceContracts;
using ASA.Web.Services.AddrValidationService.DataContracts;
using ASA.Web.Services.Common;

namespace ASA.Web.Services.AddrValidationService
{
    public class MockAddressValidationAdapter : IAddressValidationAdapter
    {
        public AddrValidationResponseModel ValidateAddress(AddressModel addr)
        {
            return MockJsonLoader.GetJsonObjectFromFile<AddrValidationResponseModel>("AddrValidationService", @"ValidateAddress");
        }
    }
}
