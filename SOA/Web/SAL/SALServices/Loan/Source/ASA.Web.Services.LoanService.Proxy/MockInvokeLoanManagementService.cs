using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASA.Web.Services.LoanService.Proxy.LoanManagement;
using ASA.Web.Services.Common;

namespace ASA.Web.Services.LoanService.Proxy
{
    public class MockInvokeLoanManagementService : IInvokeLoanManagementService
    {

        #region Get

        public GetLoanResponse GetLoan(GetLoanRequest getRequest)
        {
            //TODO: Populate fake GetLoanResponse object here...
            GetLoanResponse response = new GetLoanResponse();
            response.LoanCanonical = new LoanCanonicalType[4];
            response.LoanCanonical[0] = GetNewLoanCanonicalType(7, new DateTime(2006, 5, 28), 4000.00M, 400.00M, "School of Good Times", "Fake Money Lender");
            response.LoanCanonical[1] = GetNewLoanCanonicalType(8, new DateTime(2006, 6, 13), 3000.00M, 300.00M, "Univ of Rock n'Roll", "Bags O' Cash Lending");
            response.LoanCanonical[2] = GetNewLoanCanonicalType(9, new DateTime(2008, 9, 10), 2225.99M, 19.95M, "The Mock School of Test Data", "Bank of Fake Currency");
            response.LoanCanonical[3] = GetNewLoanCanonicalType(10, new DateTime(2009, 9, 6),  1000.00M, 19.95M, "Graduate School of Basket Weavers", "Bank of Dad's Wallet");
            return response;
        }


        private LoanCanonicalType GetNewLoanCanonicalType(int loanId, DateTime repayDt, decimal balance, decimal nextPmt, string schoolName, string lenderName)
        {
            LoanCanonicalType loanCanonical = new LoanCanonicalType();

            loanCanonical.LoanTier1 = new LoanTier1Type();
            loanCanonical.LoanTier2 = new LoanTier2Type();
            loanCanonical.LoanTier2.LoanInfoType = new LoanTier2Type.LoanInfoTypeType();

            loanCanonical.LoanTier2.LoanInfoType.LoanId = loanId;
            loanCanonical.LoanTier1.CustomerId = 2;
            loanCanonical.LoanTier2.LoanInfoType.EnteredRepaymentDate = repayDt;
            loanCanonical.LoanTier2.LoanInfoType.LoanStatusId = "DF";
            loanCanonical.LoanTier2.LoanInfoType.LoanStatusDate = new DateTime(2011, 5, 15);
            loanCanonical.LoanTier2.LoanInfoType.LoanTypeId = "SF";
            loanCanonical.LoanTier2.LoanInfoType.OutstandingPrincipalBalance = balance;
            loanCanonical.LoanTier2.LoanInfoType.OutstandingPrincipalBalanceDate = new DateTime(2011, 6, 5); ;
            loanCanonical.LoanTier2.LoanInfoType.InterestRate = 6.2;
            loanCanonical.LoanTier2.RepaymentInfoType = new LoanTier2Type.RepaymentInfoTypeType();
            loanCanonical.LoanTier2.RepaymentInfoType.NextPaymentDueAmount = nextPmt;
            loanCanonical.LoanTier2.RepaymentInfoType.NextPaymentDueDate = new DateTime(2011, 8, 15);

            loanCanonical.LoanTier2.OrganizationArray = new LoanTier2Type.OrganizationArrayType();
            OrganizationType org1 = new OrganizationType();
            org1.OrganizationId = 7;
            org1.OrganizationRoleId = "SCHL";
            org1.ReadOnlyOrganizationName = schoolName;
            loanCanonical.LoanTier2.OrganizationArray.Add(org1);

            OrganizationType org2 = new OrganizationType();
            org2.OrganizationId = 8;
            org2.OrganizationRoleId = "LEND";
            org2.ReadOnlyOrganizationName = lenderName;
            loanCanonical.LoanTier2.OrganizationArray.Add(org2);

            return loanCanonical;
        }
        #endregion

    }
}
