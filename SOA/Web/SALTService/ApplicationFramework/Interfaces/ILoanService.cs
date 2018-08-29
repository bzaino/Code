using System.Collections.Generic;
using Asa.Salt.Web.Services.Domain;
using System.IO;
using System;

namespace Asa.Salt.Web.Services.BusinessServices.Interfaces
{
   public interface ILoanService
    {
      /// <summary>
      /// Saves the member reported loan.
      /// </summary>
      /// <param name="userId">The member id.</param>
      /// <param name="memberReportedLoans">The member reported loans.</param>
      /// <param name="recordSourceId">The record source id.</param>
      /// <returns>IList{MemberReportedLoan}.</returns>
      IList<MemberReportedLoan> SaveImportList(int userId, IList<MemberReportedLoan> memberReportedLoans);

       /// <summary>
      /// Gets the member reported loans.
      /// </summary>
      /// <param name="userId">The member id.</param>
      /// <param name="recordSourceId">The record source id.</param>
      /// <returns>IList{MemberReportedLoan}.</returns>
      IList<MemberReportedLoan> GetUserLoans(int userId, int? recordSourceId = null);

      /// <summary>
      /// Gets the user loans by user ID and array of record source names.
      /// </summary>
      /// <param name="userId">The member id.</param>
      /// <param name="recordSourceNames">an array of record source names.</param>
      /// <returns>IList{MemberReportedLoan}.</returns>
      IList<MemberReportedLoan> GetUserLoansRecordSourceList(int userId, string[] recordSourceNames);

      /// <summary>
      /// Gets the member reported loans.
      /// </summary>
      /// <param name="userId">The member id.</param>>
      /// <returns>IList{vMemberReportedLoan}.</returns>
      IList<vMemberReportedLoans> GetReportedLoans(int userId);

      /// <summary>
      /// Imports the loan file.
      /// </summary>
      /// <param name="userId">The member id.</param>
      /// <param name="file">The file.</param>
      /// <param name="sourceName">The source name e.g. 'KWYO'</param>
      /// <returns>IList{MemberReportedLoan}.</returns>
      IList<MemberReportedLoan> ImportUserLoans(int userId, byte[] file, string sourceName);

      /// <summary>
      /// Deletes a single loan.
      /// </summary>
      /// <param name="userId">The member id.</param>
      /// <param name="loanId">The loan id.</param>
      /// <returns>bool</returns>
      bool RemoveLoan(int userId, int loanId);

      /// <summary>
      /// Insert a single loan.
      /// </summary>
      /// <param name="userId">The member id.</param>
      /// <param name="loan">The loan</param>
      /// <returns>MemberReportedLoan</returns>
      MemberReportedLoan CreateLoan(int userId, MemberReportedLoan loan);

      /// <summary>
      /// Updates a single loan.
      /// </summary>
      /// <param name="userId">The member id.</param>
      /// <param name="loan">The loan</param>
      /// <returns>MemberReportedLoan</returns>
      MemberReportedLoan UpdateLoan(int userId, MemberReportedLoan updatedLoan);


      /// <summary>
      /// Updates loan attributes based on lines from a LoanFile
      /// </summary>
      /// <param name="loan">The loan being updated.</param>
      /// <param name="fileLine">The line from the loan file</param>
      /// <param name="servicerName">List of servicer names</param>
      void AddLoanAttribute(MemberReportedLoan loan, string fileLine, List<string> servicerName);
    }
}
