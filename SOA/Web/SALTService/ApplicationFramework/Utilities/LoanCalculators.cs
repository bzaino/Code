using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MemberReportedLoan = Asa.Salt.Web.Services.Domain.MemberReportedLoan;

namespace Asa.Salt.Web.Services.BusinessServices.Utilities
{
    public class LoanCalculators : ILoanCalc
    {
        /// <summary>
        /// calculates monthly payment for loans.
        /// </summary>
        /// <param name="memberReportedLoans">The member reported loan.</param>
        /// <returns>
        /// decimal? Monthly loan payment.
        /// </returns>
        public decimal? calculateMonthlyPayment(MemberReportedLoan loan)
        {
            double total = 0,
            interestRate = (double)loan.InterestRate,
            loanTerm = (double)loan.LoanTerm,
            loanAmount = (double)loan.OriginalLoanAmount;

            if (loan.InterestRate == 0)
            {
                total += (loanAmount) / (loanTerm * 12);
            }
            else
            {
                total += (loanAmount * ((interestRate / 100) / 12)) / (1 - (Math.Pow((1 + ((interestRate / 100) / 12)), -(loanTerm * 12))));
            }

            total = Math.Floor(total);
            return (decimal?)total;
        }
    }
}
