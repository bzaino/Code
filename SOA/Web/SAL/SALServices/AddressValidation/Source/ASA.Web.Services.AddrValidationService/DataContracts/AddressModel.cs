using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASA.Web.Services.Common;
using System.ComponentModel;

namespace ASA.Web.Services.AddrValidationService.DataContracts
{
    // NOTE:  This model class was basically just a copy of what was in 
    // the Person SAL for PersonAddressModel.  Once we know better how these
    // addresses will be used, we should make an effort to have only ONE address model
    // that is used across any SAL's that require some sort of Address Model.
    public class AddressModel : BaseWebModel
    {
        public AddressModel()
        {
        }

        public AddressModel(string addr1, string addr2, string city, string stateId, string zip, string country)
        {
            this.AddressLine1 = addr1;
            this.AddressLine2 = addr2;
            this.City = city;
            this.StateID = stateId;
            this.Zip = zip;
            this.CountryID = country;
        }

        [DisplayName("Address")]
        public string AddressLine1 { get; set; }

        public string AddressLine2 { get; set; }

        [DisplayName("City")]
        public string City { get; set; }

        [DisplayName("State ID")]
        public string StateID { get; set; }

        [DisplayName("Postal Code")]
        public string Zip { get; set; }

        [DisplayName("Country ID")]
        public string CountryID { get; set; } 
    }
}

