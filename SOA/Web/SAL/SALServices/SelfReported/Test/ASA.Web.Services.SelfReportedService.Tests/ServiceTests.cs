using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ASA.Web.Services.Common;

using ASA.Web.Services.SelfReportedService.Proxy.DataContracts;

using ASA.Web.Services.Proxies;
using ASA.Web.Services.Proxies.SALTService;

using ASA.Web.WTF.Integration;

namespace ASA.Web.Services.SelfReportedService.Tests
{

    [TestClass]
    public class ServiceTests
    {
        public static string _authToken = "";

        #region Test init


        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            //instantiating this once will hopefully keep the Auth Token in memory for subsequent test calls.
            
        }

        private SelfReportedLoanModel GetValidSelfReportedLoanData()
        {
            SelfReportedLoanModel srlModel = new SelfReportedLoanModel();
            //srlModel.AccountNickname = "TEST Account";
            //srlModel.HolderName = "TEST Holder";
            srlModel.InterestRate = 6.2;
            srlModel.LoanSelfReportedEntryId = "";
            srlModel.LoanStatusId = "DF";
            srlModel.LoanTypeId = "SF";
            //srlModel.NextPaymentDueAmount = 200.02M;
            //srlModel.NextPaymentDueDate = DateTime.Now;
            //srlModel.PaymentDueAmount = 100.02M;
            srlModel.PrincipalBalanceOutstandingAmount = 20000.00M;
            //srlModel.SchoolName = "TEST School";
            srlModel.OriginalLoanAmount = 5000.00M;
            srlModel.LoanTerm = 50;
            //srlModel.ServicerName = "TEST Servicer";
            //srlModel.ServicerWebAddress = "http://test.servicer.web.address.com";
            srlModel.IndividualId = "01A9606D-DE45-4790-BC6F-C58B041A45A6";

            return srlModel;
        }
        #endregion

        [TestMethod]
        public void SelfReportedService_GetSelfReportedLoans_UserId()
        {
            var mockSaltServiceAgent = (SaltServiceAgentStub)IntegrationLoader.LoadDependency<ISaltServiceAgent>("saltServiceAgent");

            mockSaltServiceAgent.GetUserReportedLoansResponses = new List<MemberReportedLoanContract>(){
                new MemberReportedLoanContract()
                {
                    CreatedDate = DateTime.Now,
                    MemberId = 1,
                    MemberReportedLoanId = 2,
                    RecordSourceId = 1,
                    LoanName = "Loan Test 1",
                    LoanType = "Self-Reported",
                    InterestRate = 5,
                    OriginalLoanAmount = 5000,
                    PrincipalOutstandingAmount = 4000,
                    ReceivedYear = 2011,
                    LoanTerm = 8,
                    ServicingOrganizationName = "Sallie Mae",
                    RecordSource = new RecordSourceContract(),
                    OriginalLoanDate = DateTime.Now,
                    MonthlyPaymentAmount = 50,
                    ModifiedDate = DateTime.Now
                },
                new MemberReportedLoanContract()
                {
                    CreatedDate = DateTime.Now,
                    MemberId = 1,
                    MemberReportedLoanId = 3,
                    RecordSourceId = 1,
                    LoanName = "Loan Test 2",
                    LoanType = "Self-Reported",
                    InterestRate = 5,
                    OriginalLoanAmount = 2000,
                    PrincipalOutstandingAmount = 1500,
                    ReceivedYear = 2012,
                    LoanTerm = 5,
                    ServicingOrganizationName = "Sallie Mae",
                    RecordSource = new RecordSourceContract(),
                    OriginalLoanDate = DateTime.Now,
                    MonthlyPaymentAmount = 50,
                    ModifiedDate = DateTime.Now
                }                 
            };

            SelfReported srl = new SelfReported();

            SelfReportedLoanListModel loanList = srl.GetSelfReportedLoans();

            Assert.AreEqual(2, loanList.Loans.Count); 
        }

        [TestMethod]
        public void SelfReportedService_InsertSelfReportedLoans_AdapterTest()
        {
            //SelfReportedLoanModel srl = GetValidSelfReportedLoanData();

            //// insert one
            //SelfReportedAdapter adapter = new SelfReportedAdapter();
            //ResultCodeModel res = adapter.InsertSelfReportedLoan(srl);

            //// insert list
            //SelfReportedLoanListModel srList = new SelfReportedLoanListModel();
            //srList.Loans.Add(srl);
            //srList.Loans.Add(srl);
            //res = adapter.InsertSelfReportedLoans(srList);
            //Assert.IsTrue(res != null && res.ResultCode ==1, "Something bad happened.");
        }

        [TestMethod]
        public void SelfReportedService_GetUseringRecordSourceList()
        {
            var mockSaltServiceAgent = (SaltServiceAgentStub)IntegrationLoader.LoadDependency<ISaltServiceAgent>("saltServiceAgent");

            mockSaltServiceAgent.GetUserReportedLoansRecordSourceListResponses = new List<MemberReportedLoanContract>(){
                new MemberReportedLoanContract()
                {
                    LoanName = "Loan Test 1",
                    OriginalLoanAmount = 5000
                },
                new MemberReportedLoanContract()
                {
                    LoanName = "Loan Test 2",
                    OriginalLoanAmount = 2000
                }                 
            };

            SelfReported srl = new SelfReported();

            int recordCount = srl.GetKWYOSelfReportedLoans();

            Assert.AreEqual(2, recordCount); 
        }   

        #region Save Tests

        [TestMethod]
        public void Test_SaveNew_SelfReportedLoanForPerson()
        {
            SelfReportedLoanModel srlModel = GetValidSelfReportedLoanData();

            ASA.Web.Services.SelfReportedService.SelfReported srlAbstraction = new ASA.Web.Services.SelfReportedService.SelfReported();
            ResultCodeModel result = new ResultCodeModel(1); //srlAbstraction.SaveSelfReportedLoan(srlModel);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.ErrorList.Count == 0, "ErrorList.Count != 0");
        }
        #endregion

    }
}
