using System;
using System.Configuration;

namespace Asa.Salt.Web.Services.Configuration.MemberReportedLoan
{
    public class TestApplicationMemberReportedLoanConfiguration : IApplicationMemberReportedLoanConfiguration
    {

        /// <summary>
        /// Gets the MemberReportedLoan configuration.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public MemberReportedLoanConfiguration GetConfiguration()
        {
            var configuration = new MemberReportedLoanConfiguration();

            configuration.LoanTerm = 10;
            configuration.FixedRate = 6.8;
            configuration.VariableRate = 2.4;


            return configuration;
        }
    }
}
