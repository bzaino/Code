using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASA.Web.Services.ReminderService.DataContracts;


namespace ASA.Web.Services.ReminderService.Validation
{
    public class ReminderValidation
    {
        public static bool ValidateReminderId(string id)
        {
	            bool bValid = false;
	            ReminderModel reminder = new ReminderModel();
                reminder.ID = id;
	            if (id != null && reminder.IsValid("ID"))
	            {
	                bValid = true;
	            }
	
	            return bValid;
        }

        public static bool ValidateInputReminderList(ReminderListModel rList)
        {
            bool bValid = false;
            if (rList != null && rList.Reminders != null)
            {
                bValid = true;
                foreach (ReminderModel reminder in rList.Reminders)
                {
                    bValid &= reminder.IsValid();
                    if (!bValid)
                        break;
                }
            }

            return bValid;
        }

        public static bool ValidateReminder(ReminderModel reminder)
        {
            bool bValid = false;
            if (reminder != null)
                bValid = reminder.IsValid();
            return bValid;
        }
    }
}
