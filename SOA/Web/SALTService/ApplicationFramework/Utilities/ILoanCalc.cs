using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Asa.Salt.Web.Services.Domain;

namespace Asa.Salt.Web.Services.BusinessServices.Utilities
{
    public interface ILoanCalc
    {
        /// <summary>
        /// calculates monthly payment for a single loan.
        /// </summary>
        /// /// <param name="loan">The loan</param>
        /// <returns>decimal? monthly payment</returns>
        decimal? calculateMonthlyPayment(MemberReportedLoan loan);
    }
}
