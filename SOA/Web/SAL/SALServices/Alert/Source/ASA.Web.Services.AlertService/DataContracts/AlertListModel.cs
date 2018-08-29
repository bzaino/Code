using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASA.Web.Services.Common;
using System.ComponentModel;

namespace ASA.Web.Services.AlertService.DataContracts
{
    public class AlertListModel : BaseWebModel
    {
        private List<AlertModel> _list;

        public AlertListModel()
        {
            _list = new List<AlertModel>();
        }

        public AlertListModel(List<AlertModel> list)
        {
            _list = list;
        }

        [DisplayName("Alerts")]
        public List<AlertModel> Alerts
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
