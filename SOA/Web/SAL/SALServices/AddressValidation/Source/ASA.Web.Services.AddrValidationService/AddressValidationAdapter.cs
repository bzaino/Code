using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASA.Web.Services.AddrValidationService.ServiceContracts;
using ASA.Web.Services.AddrValidationService.DataContracts;
using ASA.Web.Services.AddrValidationService.Proxy;
using Common.Logging;
using ASA.Web.Services.AddrValidationService.QasProWeb;

namespace ASA.Web.Services.AddrValidationService
{
    public class AddressValidationAdapter : IAddressValidationAdapter
    {
        private IInvokeQasService _proxy;

        private const string CLASSNAME = "ASA.Web.Services.AddrValidationService.AddressValidationAdapter";
        static readonly ILog _log = LogManager.GetLogger(CLASSNAME);

        public AddressValidationAdapter()
        {
            _proxy = new InvokeQasService();
        }

        public AddrValidationResponseModel ValidateAddress(AddressModel addr)
        {
            _log.Debug("START ValidateAddress");
            _log.Debug(addr.ToString());
            QASearch search = TranslateAddrValidationModel.MapAddressToQasSearch(addr);
            QASearchResult result = _proxy.ValidateAddress(search);
            AddrValidationResponseModel response = TranslateAddrValidationModel.MapGetResponseToModel(result);
            _log.Debug("END ValidateAddress");
            return response;
        }
    }
}
