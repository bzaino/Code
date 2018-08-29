using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using ASA.Web.Services.Common;

namespace ASA.Web.Services.ReminderService.DataContracts
{
    public class ReminderListModel : BaseWebModel 
    {
        private List<ReminderModel> _list;

        public ReminderListModel()
        {
            _list = new List<ReminderModel>();
        }

        public ReminderListModel(List<ReminderModel> list)
        {
            _list = list;
        }

        [DisplayName("Reminders")]
        public List<ReminderModel> Reminders
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
