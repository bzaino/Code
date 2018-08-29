using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using ASA.Web.Services.SelfReportedService.Proxy.DataContracts;


namespace ASA.Web.Services.SelfReportedService.Validation
{
    public class SelfReportedValidation
    {
        public static bool ValidateSelfReportedLoanId(string id)
        {
            bool bValid = false;
            SelfReportedLoanModel srl = new SelfReportedLoanModel();
            srl.LoanSelfReportedEntryId = id; 
            if (id != null && srl.IsValid("ID"))
            {
                bValid = true;
            }

            return bValid;
        }

        public static bool ValidateInputSelfReportedLoanList(SelfReportedLoanListModel sr)
        {
            bool bValid = false;
            if(sr!=null)
            {
                bValid = sr.IsValid();
                if (sr.Loans != null)
                {
                    foreach (SelfReportedLoanModel srl in sr.Loans)
                    {
                        bValid &= ValidateInputSelfReportedLoan(srl);
                        if (!bValid)
                            break;
                    }
                }
            }
            return bValid;
        }

        public static bool ValidateInputSelfReportedLoan(SelfReportedLoanModel sr)
        {
            bool bValid = false;
            if (sr != null)
                bValid = sr.IsValid();
            return bValid;
        }

    }
}
