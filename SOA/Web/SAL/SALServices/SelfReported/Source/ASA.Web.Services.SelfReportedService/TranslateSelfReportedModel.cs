using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ASA.Web.Services.ASAMemberService;
using ASA.Web.Services.SelfReportedService.Proxy.DataContracts;
using ASA.Web.Services.Proxies.SALTService;

namespace ASA.Web.Services.SelfReportedService
{

    public static class TranslateSelfReportedModel
    {

        /// <summary>
        /// Converts the member reported loan contract list to the domain model.
        /// </summary>
        /// <param name="loans">The loans.</param>
        /// <returns></returns>
        public static SelfReportedLoanListModel FromMemberReportedLoanContractList(this List<MemberReportedLoanContract> loans)
        {
           var list = new SelfReportedLoanListModel();
           foreach (var loan in loans)
           {
              list.Loans.Add(loan.FromMemberReportedLoanContract());
           }
           return list;
        }


        /// <summary>
        /// Froms the member reported loan contract.
        /// </summary>
        /// <param name="loan">The loan.</param>
        /// <returns></returns>
        public static SelfReportedLoanModel FromMemberReportedLoanContract(this MemberReportedLoanContract loan)
        {
           return new SelfReportedLoanModel 
           {
              LoanTypeId = loan.LoanType,
              DateAdded = loan.CreatedDate,
              LoanStatusId = loan.LoanStatus,
              PrincipalBalanceOutstandingAmount = loan.PrincipalOutstandingAmount,
              MemberId = loan.MemberId,
              IndividualId = new AsaMemberAdapter().GetActiveDirectoryKeyFromContext(),
              InterestRate = loan.InterestRate.HasValue? Convert.ToDouble(loan.InterestRate.Value): null as double?,
              ReceivedYear = loan.ReceivedYear.HasValue?loan.ReceivedYear.Value:0,
              OriginalLoanAmount = loan.OriginalLoanAmount,
              LoanSelfReportedEntryId = loan.MemberReportedLoanId.ToString(CultureInfo.InvariantCulture),
              LoanTerm = loan.LoanTerm.HasValue?loan.LoanTerm.Value:0,
              LoanSource = loan.RecordSource.RecordSourceName,
              RecordSourceId = loan.RecordSourceId,
              InterestRateType = loan.InterestRateType,
              LoanName = loan.LoanName,
              OriginalLoanDate = loan.OriginalLoanDate,
              MonthlyPaymentAmount = loan.MonthlyPaymentAmount,
              ServicingOrganizationName = loan.ServicingOrganizationName,
              LastModified = loan.ModifiedDate.HasValue ? loan.ModifiedDate : loan.CreatedDate
           };

        }

        /// <summary>
        /// To the member reported loan contract list.
        /// </summary>
        /// <param name="loans">The loans.</param>
        /// <returns></returns>
        public static MemberReportedLoanContract[] ToMemberReportedLoanContractList(this SelfReportedLoanListModel loans)
        {
            return loans.Loans.Select(loan => loan.ToMemberReportedLoanContract()).ToArray();
        }

        /// <summary>
        /// To the member reported loan contract.
        /// </summary>
        /// <param name="loan">The loan.</param>
        /// <returns></returns>
        public static MemberReportedLoanContract ToMemberReportedLoanContract(this SelfReportedLoanModel loan)
        {
           return new MemberReportedLoanContract 
           {
              LoanType = loan.LoanTypeId,
              LoanStatus = loan.LoanStatusId,
              PrincipalOutstandingAmount = loan.PrincipalBalanceOutstandingAmount,
              MemberId = new AsaMemberAdapter().GetMemberIdFromContext(),
              InterestRate = loan.InterestRate.HasValue?Convert.ToDecimal(loan.InterestRate):null as decimal?,
              ReceivedYear = loan.ReceivedYear,
              OriginalLoanAmount = loan.OriginalLoanAmount,
              MemberReportedLoanId = loan.LoanSelfReportedEntryId == null || string.IsNullOrEmpty(loan.LoanSelfReportedEntryId) ? 0 : Int32.Parse(loan.LoanSelfReportedEntryId),
              LoanTerm = loan.LoanTerm,
              RecordSourceId = loan.RecordSourceId,
              InterestRateType = loan.InterestRateType,
              LoanName = loan.LoanName,
              OriginalLoanDate = loan.OriginalLoanDate,
              MonthlyPaymentAmount = loan.MonthlyPaymentAmount,
              ServicingOrganizationName = loan.ServicingOrganizationName,
              ModifiedDate = DateTime.Now
           };

        }

    }
}