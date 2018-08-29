using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASA.Web.Services.Common;
using System.ComponentModel;
using ASA.Web.Services.AddrValidationService.QasProWeb;

namespace ASA.Web.Services.AddrValidationService.DataContracts
{
    public class AddrValidationResponseModel : BaseWebModel
    {
        //private VerifyLevelType _verifyLevel = QasProWeb.VerifyLevelType.None;
        //private AddressListModel _addressList = null;

        //public AddrValidationResponseModel()
        //{
        //    _addressList = new AddressListModel();
        //}

        //public AddrValidationResponseModel(VerifyLevelType verifyLevel)
        //{
        //    _addressList = new AddressListModel();
        //    _verifyLevel = verifyLevel;
        //}

        //public AddrValidationResponseModel(AddressListModel addressList)
        //{
        //    _addressList = addressList;
        //}

        //public AddrValidationResponseModel(AddressListModel addressList, VerifyLevelType verifyLevel)
        //{
        //    _addressList = addressList;
        //    _verifyLevel = verifyLevel;
        //}

        [DisplayName("Verification Level")]
        [DefaultValue("None")]
        public string VerifyLevel{get;set;}

        [DisplayName("Verified Addresses")]
        public AddressModel VerifiedAddress { get; set; }

        [DisplayName("Suggested Addresses")]
        public AddressListModel SuggestedAddresses { get; set; }


    }
}
