using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ASA.Web.Services.AlertService.DataContracts
{
    class xWebAlertHelper
    {
        public static string IndividualKeyName { get { return "a09_ind_cst_key"; } }
        public static string ActiveFlagName { get { return "a09_active_flag"; } }
        public static string IssuedDateName { get { return "a09_issued_date"; } }

        public static string[] GetFieldNames()
        {
            string[] fieldNames = new string[]{
            "a09_key",
            "a09_ind_cst_key",
            "a09_issued_date",
            "a09_message",
            "a09_title",
            "a09_logo",
            "a09_link",
            "a09_active_flag",
            "a25_alert_type"
           };

            return fieldNames;
        }
    }
}
