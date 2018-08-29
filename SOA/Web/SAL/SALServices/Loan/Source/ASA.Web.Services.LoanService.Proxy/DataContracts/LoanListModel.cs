using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using ASA.Web.Services.Common;

namespace ASA.Web.Services.LoanService.Proxy.DataContracts
{
    public class LoanListModel : BaseWebModel //, IList<LoanModel>  //code is simpler to maintain if I DONT implement IList, and just access _list througha property instead.
    {
        private List<LoanModel> _list;

        public LoanListModel()
        {
            _list = new List<LoanModel>();
        }

        public LoanListModel(List<LoanModel> list)
        {
            _list = list;
        }

        [DisplayName("Loans")]
        public List<LoanModel> Loans
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
