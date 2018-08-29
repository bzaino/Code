using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASA.Web.Services.LoanService.Proxy.DataContracts;
using System.Text.RegularExpressions;


namespace ASA.Web.Services.LoanService.Validation
{
    public class LoanValidation
    {
        public static bool ValidateInputSsn(string ssn)
        {
            bool valid = false;

            if (ssn != null && ssn.Length == 9)
            {
                try
                {
                    Regex re = new Regex(@"^\d{9}$");
                    valid = re.IsMatch(ssn);
                }
                catch
                {
                    valid = false;
                }
            }

            return valid;
        }

        public static bool ValidateInputLoan(LoanModel p)
        {
            //ValidateInputLoan() --> do this after once we start implementing stuff that allows Loan Add/Updates from the UI.
            // ...nothing is needed here yet.  
            return true;
        }
    }
}
