using System;
using System.Configuration;

namespace Asa.Salt.Web.Services.Configuration.MemberReportedLoan
{
   
   public class MemberReportedLoanConfigurationSection : ConfigurationSection
   {

      [ConfigurationProperty("loanTerm", IsRequired = true)]
      public int LoanTerm
      {
         get { return (int)this["loanTerm"]; }
         set { this["loanTerm"] = value; }
      }

      [ConfigurationProperty("fixedRate", IsRequired = true)]
      public double FixedRate
      {
         get { return (double)this["fixedRate"]; }
         set { this["fixedRate"] = value; }
      }

      [ConfigurationProperty("variableRate", IsRequired = true)]
      public double VariableRate
      {
         get { return (double)this["variableRate"]; }
         set { this["variableRate"] = value; }
      }



   }

}
