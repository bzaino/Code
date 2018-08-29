using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASA.Web.Services.Common;
using System.ComponentModel;

namespace ASA.Web.Services.AddrValidationService.DataContracts
{
    public class AddressListModel : BaseWebModel
    {
        private List<AddressModel> _list;

        public AddressListModel()
        {
            _list = new List<AddressModel>();
        }

        public AddressListModel(List<AddressModel> addressList)
        {
            _list = addressList;
        }

        [DisplayName("Addresses")]
        public List<AddressModel> Addresses
        {
            get
            {
                return _list;
            }
            set
            {
                _list = value;
            }
        }
    }
}
