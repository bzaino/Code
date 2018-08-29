using System;
using System.Configuration;

namespace Asa.Salt.Web.Services.Configuration.MemberReportedLoan
{
   public class ApplicationMemberReportedLoanConfiguration : IApplicationMemberReportedLoanConfiguration
   {
      /// <summary>
      /// Gets the MemberReportedLoan configuration.
      /// </summary>
      /// <returns></returns>
      /// <exception cref="System.NotImplementedException"></exception>
      public MemberReportedLoanConfiguration GetConfiguration()
      {
         var configuration = new MemberReportedLoanConfiguration();
         var section = (MemberReportedLoanConfigurationSection)ConfigurationManager.GetSection("importedLoanConfiguration");
         if (section != null)
         {
            configuration.LoanTerm = section.LoanTerm;
            configuration.FixedRate = section.FixedRate;
            configuration.VariableRate = section.VariableRate;
         }

         return configuration;
      }
   }
}
