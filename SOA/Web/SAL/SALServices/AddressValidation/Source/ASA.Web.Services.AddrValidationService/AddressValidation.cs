using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Activation;
using ASA.Web.Services.AddrValidationService.DataContracts;
using System.ServiceModel.Web;
using Common.Logging;
using ASA.Web.Services.AddrValidationService.ServiceContracts;
using ASA.Web.Services.Common;


namespace ASA.Web.Services.AddrValidationService
{
    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class AddressValidation
    {
        private const string CLASSNAME = "ASA.Web.Services.AddrValidationService.AddressValidation";
        static readonly ILog _log = LogManager.GetLogger(CLASSNAME);

        private const string _addrValidationAdapterExceptionMessage = "Unable to create a AddressValidationAdapter object from the ASA.Web.Services.AddrValidationService library. ";
        private IAddressValidationAdapter _addressValidation = null;

        public AddressValidation()
        {
            _log.Info("ASA.Web.Services.AddrValidationService.AddressValidation() object being created ...");
            if (ASA.Web.Services.Common.Config.Testing)
                _addressValidation = new MockAddressValidationAdapter(); 
            else
                _addressValidation = new AddressValidationAdapter();
        }

        [OperationContract]
        [WebGet(UriTemplate = "ValidateAddress/?addressLine1={addr1}&addressLine2={addr2}&city={city}&state={state}&zip={zip}&country={country}"
            , ResponseFormat = WebMessageFormat.Json)]
        [AspNetCacheProfile("DoNotCache")]
        public AddrValidationResponseModel ValidateAddress(string addr1, string addr2, string city, string state, string zip, string country)
        {
            AddrValidationResponseModel avrModel = null;

            AddressModel addr = new AddressModel(addr1, addr2, city, state, zip, country);
            try
            {
                _log.Info("ASA.Web.Services.AddrValidationService.ValidateAddress() starting ...");

                if (addr!=null && addr.IsValid() && (Config.QasOn == true))
                {
                    // only send US addrs to QAS
                    if (addr.CountryID.ToUpper() == "USA" || addr.CountryID.ToUpper() == "US" || addr.CountryID.ToUpper() == "UNITED STATES")
                        avrModel = _addressValidation.ValidateAddress(addr);
                    else //dont send foreign addrs to QAS.
                    {
                        //avrModel = new AddrValidationResponseModel(QasProWeb.VerifyLevelType.None);
                        avrModel = new AddrValidationResponseModel();
                        ErrorModel error = new ErrorModel("Foreign addresses are not validated by QAS.", "Web AddressValidation Service");
                        avrModel.ErrorList.Add(error);
                        _log.Info("Foreign addresses are not validated by QAS.");
                    }
                }
                else
                {
                    avrModel = new AddrValidationResponseModel();
                    ErrorModel error = new ErrorModel("Invalid search criteria", "Web AddressValidation Service");
                    if (Config.QasOn != true)
                    {
                        error = new ErrorModel("Qas is configured to bypass address validation.", "Web AddressValidation Service");
                    }
                    avrModel.ErrorList.Add(error);
                    _log.Info("ASA.Web.Services.AddrValidationService.ValidateAddress(): Invalid search criteria or QAS is currently disabled.");
                }

                _log.Info("ASA.Web.Services.AddrValidationService.ValidateAddress() ending ...");
            }
            catch (Exception ex)
            {
                avrModel = new AddrValidationResponseModel();
                ErrorModel error = new ErrorModel(ex.ToString(), "Web AddressValidation Service");
                avrModel.ErrorList.Add(error);
                _log.Error("ASA.Web.Services.AddrValidationService.ValidateAddress(): Exception => " + ex.ToString());
            }

            return avrModel;
        }
    }
}
