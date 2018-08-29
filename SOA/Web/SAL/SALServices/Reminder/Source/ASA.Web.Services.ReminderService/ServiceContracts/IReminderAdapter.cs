using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASA.Web.Services.Common;
using ASA.Web.Services.ReminderService.DataContracts;

namespace ASA.Web.Services.ReminderService.ServiceContracts
{
    public interface IReminderAdapter
    {
        ReminderListModel GetReminders(int memberId);
        ResultCodeModel SaveReminders(ReminderListModel reminders);
    }
}
