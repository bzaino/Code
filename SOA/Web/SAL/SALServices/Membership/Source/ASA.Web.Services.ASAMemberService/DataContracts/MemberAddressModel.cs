using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ASA.Web.Services.Common;

namespace ASA.Web.Services.ASAMemberService.DataContracts
{
    public class MemberAddressModel : BaseWebModel
    {
        public MemberAddressModel() : base() { }
        public MemberAddressModel(Boolean newRecord = false) : base(newRecord) { }

        public bool  IsValidated { get; set; }
        public string AddressKey { get; set; }
        public bool IsPrimary { get; set; }

        [DisplayName("Address")]
        public string AddressLine1 { get; set; }

        public string AddressLine2 { get; set; }

        [DisplayName("City")]
        public string City { get; set; }

        [DisplayName("State")]
        public string State { get; set; }

        [DisplayName("State ID")]
        public string StateID{ get; set; }

        [DisplayName("Zip")]
        public string Zip { get; set; }

        [DisplayName("Foreign Postal Code")]
        public string ForeignPostalCode { get; set; }

        [DisplayName("Country")]
        public string Country { get; set; }

        [DisplayName("Country ID")]
        public string CountryID { get; set; }

        [DisplayName("Type")]
        public string Type { get; set; }

        [DisplayName("Type ID")]
        public string TypeID { get; set; }

    }
}
