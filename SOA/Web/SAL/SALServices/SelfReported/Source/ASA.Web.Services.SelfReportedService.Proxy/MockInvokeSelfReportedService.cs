using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASA.Web.Services.SelfReportedService.Proxy.LoanManagement;
using ASA.Web.Services.Common;

namespace ASA.Web.Services.SelfReportedService.Proxy
{
    public class MockInvokeSelfReportedService : IInvokeSelfReportedService
    {

        #region Get

        public GetLoanSelfReportedEntryResponse GetSelfReported(GetLoanSelfReportedEntryRequest getRequest)
        {
            GetLoanSelfReportedEntryResponse response = new GetLoanSelfReportedEntryResponse();

            response.LoanCanonical = new LoanCanonicalType[1];
            response.LoanCanonical[0] = new LoanCanonicalType();
            response.LoanCanonical[0].LoanTier3 = new LoanTier3Type();
            response.LoanCanonical[0].LoanTier3.LoanSelfReportedEntryArray = new LoanTier3Type.LoanSelfReportedEntryArrayType();

            LoanSelfReportedEntryType sre1 = GetNewLoanSelfReportedEntry(
                "My first loan", "My Loan Holder", "Y", 1, "DF", "SF", 102.00M, new DateTime(2011,6,24), 160.56M, 
                10001, 1234.11M, "My School", "My Servicer", "www.myservicer.com" 
                );
            LoanSelfReportedEntryType sre2 = GetNewLoanSelfReportedEntry(
                "My second loan", "My Loan Holder2", "N", 1, "DF", "SF", 202.20M, new DateTime(2011,7,6), 260.56M, 
                10001, 1234.22M, "My School", "My Servicer2", "www.myservicer2.com" 
                );
            LoanSelfReportedEntryType sre3 = GetNewLoanSelfReportedEntry(
                "My third loan", "My Loan Holder3", "Y", 1, "DF", "SF", 302.33M, new DateTime(2011,8,8), 360.56M, 
                10001, 1234.33M, "My School", "My Servicer3", "www.myservicer3.com" 
                );
            response.LoanCanonical[0].LoanTier3.LoanSelfReportedEntryArray.Add(sre1);
            response.LoanCanonical[0].LoanTier3.LoanSelfReportedEntryArray.Add(sre2);
            response.LoanCanonical[0].LoanTier3.LoanSelfReportedEntryArray.Add(sre3);

            return response;
        }

        #endregion

        #region Set
        public ResultCodeModel SaveSelfReported(LoanCanonicalType SelfReported)
        {
            ResultCodeModel rcm = new ResultCodeModel();
            rcm.ResultCode = 1;// Success.
            return rcm;
        }

        #endregion

        private LoanSelfReportedEntryType GetNewLoanSelfReportedEntry(
                string nickname, string holdername, string isActive, int sreId, string loanStatusId,
                string loanTypeId, decimal nextPmtDueAmt, DateTime nextPmtDueDt, decimal pmtDueAmt, int personId,
                decimal balance, string schoolName, string servicerName, string webAddress
            )
        {
            LoanSelfReportedEntryType sre = new LoanSelfReportedEntryType();
            sre.AccountNickname = nickname;
            sre.HolderName = holdername;
            sre.IsActive =  isActive;
            sre.LoanSelfReportedEntryId = sreId;
            sre.LoanStatusId = loanStatusId;
            sre.LoanTypeId = loanTypeId;
            sre.NextPaymentDueAmount = nextPmtDueAmt;
            sre.NextPaymentDueDate = nextPmtDueDt;
            sre.PaymentDueAmount = pmtDueAmt ;
            sre.PersonId = personId;
            sre.PrincipalBalanceOutstandingAmount = balance;
            sre.SchoolName = schoolName;
            sre.ServicerName = servicerName;
            sre.ServicerWebAddress = webAddress;

            return sre;
        }

    }
}
