using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using ASA.Web.Services.Common;
using ASA.Web.Services.ReminderService.DataContracts;

namespace ASA.Web.Services.ReminderService.ServiceContracts
{
    [ServiceContract]
    interface IReminder
    {
        [OperationContract]
        ResultCodeModel InsertReminder(ReminderModel reminder);

        [OperationContract]
        ResultCodeModel UpdateReminder(ReminderModel reminder);

        [OperationContract]
        ResultCodeModel DeleteReminder(string reminderId);

        [OperationContract]
        ReminderListModel GetReminders();  
    }
}
