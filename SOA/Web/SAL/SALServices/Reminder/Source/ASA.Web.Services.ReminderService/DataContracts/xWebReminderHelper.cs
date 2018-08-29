using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ASA.Web.Services.ReminderService.DataContracts
{
    public static class xWebReminderHelper
    {
        public static string IndividualKeyName { get { return "a08_ind_cst_key"; } }
        public static string ActiveFlagName { get { return "a08_active_flag"; } }

        public static string[] GetFieldNames()
        {
            string[] fieldNames = new string[]{
            "a08_key",
            "a08_ind_cst_key",
            "a08_message",
            "a08_number_of_loans",
            "a08_servicer_name",
            "a08_day_of_month",
            "a08_active_flag"
           };

           return fieldNames;
        }
    }
}
