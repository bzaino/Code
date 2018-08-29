using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASA.Web.Services.Common;
using System.Xml.Linq;
using System.Xml;

namespace ASA.Web.Services.SelfReportedService.Proxy.DataContracts
{
    public static class xWebSelfReportedLoanHelper
    {
        public static string IndividualKeyName { get { return "a07_ind_cst_key"; } }

        public static string ActiveFlagName { get { return "a07_active_flag"; } }

        public static string[] GetFieldNames()
        {
            string[] fieldNames = new string[]{
            "a07_key",
            "a07_ind_cst_key",
            "a07_loan_type",
            "a07_loan_status",
            "a07_account_nickname",
            "a07_holder_name",
            "a07_school_name",
            "a07_servicer_name",
            "a07_servicer_url",
            "a07_principal_balance_outstanding_amount",
            "a07_payment_due_amount",
            "a07_next_payment_due_amount",
            "a07_next_payment_due_date",
            "a07_active_flag",
            "a07_interest_rate",
            "a07_received_year",
            "a07_original_loan_amount",
            "a07_loan_term",
            "a07_delete_flag",
            "a07_add_date",
            "a07_loan_record_source"
           };

            return fieldNames;
        }
    }
}
