using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using ASA.Web.Services.Common;

namespace ASA.Web.Services.SelfReportedService.Proxy.DataContracts
{
    public class SelfReportedLoanListModel : BaseWebModel
    {
        private List<SelfReportedLoanModel> _list;

        public SelfReportedLoanListModel()
        {
            _list = new List<SelfReportedLoanModel>();
        }

        public SelfReportedLoanListModel(List<SelfReportedLoanModel> list)
        {
            _list = list;
        }

        [DisplayName("Self Reported Loans")]
        public List<SelfReportedLoanModel> Loans
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
