using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASA.Web.Services.ReminderService.ServiceContracts;
using ASA.Web.Services.ReminderService.DataContracts;
using ASA.Web.Services.Common;

namespace ASA.Web.Services.ReminderService
{
    public class MockReminderAdapter : IReminderAdapter
    {
        public ResultCodeModel InsertReminder(ReminderModel reminder)
        {
            return MockJsonLoader.GetJsonObjectFromFile<ResultCodeModel>("ReminderService", @"SetReminder");
        }

        public ResultCodeModel UpdateReminder(ReminderModel reminder)
        {
            return MockJsonLoader.GetJsonObjectFromFile<ResultCodeModel>("ReminderService", @"SetReminder");
        }

        public ResultCodeModel DeleteReminder(string reminderId)
        {
            return MockJsonLoader.GetJsonObjectFromFile<ResultCodeModel>("ReminderService", @"DeleteReminder.{reminderId}");
        }

        public ReminderListModel GetReminders(string reminderId)  
        {
            return MockJsonLoader.GetJsonObjectFromFile<ReminderListModel>("ReminderService", @"GetReminders");
        }

        public ResultCodeModel SaveReminders(ReminderListModel reminders)
        {
            return MockJsonLoader.GetJsonObjectFromFile<ResultCodeModel>("ReminderService", "Reminders");
        }

    }
}
