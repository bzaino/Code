using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ASA.Web.Services.SelfReportedService.Proxy.DataContracts;
using System.ComponentModel.DataAnnotations;
using ASA.Web.Common.Validation;

namespace ASA.Web.Services.SelfReportedService.Tests
{
    
    [TestClass]
    public class ValidationTests
    {

        #region Test setup

        public ValidationTests()
        {
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        private SelfReportedLoanModel GetValidSelfReportedLoanData()
        {
            SelfReportedLoanModel srlModel = new SelfReportedLoanModel();
            //srlModel.AccountNickname = "Test Account";
            //srlModel.HolderName = "Test Holder";
            srlModel.InterestRate = 6.2;
            srlModel.LoanSelfReportedEntryId = "1";
            srlModel.LoanStatusId = "DF";
            srlModel.LoanTypeId = "SF";
            //srlModel.NextPaymentDueAmount = 200.02M;
            //srlModel.NextPaymentDueDate = DateTime.Now;
            //srlModel.PaymentDueAmount = 100.02M;
            srlModel.PrincipalBalanceOutstandingAmount = 20000.00M;
            //srlModel.SchoolName = "Test School";
            //srlModel.ServicerName = "Test Servicer";
            //srlModel.ServicerWebAddress = "http://test.servicer.web.address.com";

            return srlModel;
        }
       
        #endregion

        #region Valid

        public void Validation_ValidSelfReportedLoanModel()
        {
            SelfReportedLoanModel srl = GetValidSelfReportedLoanData();

            bool bValid = srl.IsValid();
            Assert.IsTrue(bValid, "A SelfReportedLoanModel with all good information validated incorrectly.");
        }

        public void Validation_ValidSelfReportedLoanListModel()
        {
            SelfReportedLoanModel srl = GetValidSelfReportedLoanData();
            SelfReportedLoanListModel srlList = new SelfReportedLoanListModel();
            srlList.Loans.Add(srl);

            bool bValid = srlList.IsValid();
            Assert.IsTrue(bValid, "A SelfReportedLoanListModel with all good information validated incorrectly.");
        }

        #endregion

        #region Lists<>

        [TestMethod]
        public void Validation_ValidSelfReportedLoanNullLists()
        {
            SelfReportedLoanListModel srlList= new SelfReportedLoanListModel();
            srlList.Loans = null;

            bool bValid = srlList.IsValid();
            Assert.IsTrue(bValid, "This SelfReportedLoanList failed validation when it has a null Loan list.");
        }


        [TestMethod]
        public void Validation_ValidSelfReportedLoanEmptyLists()
        {
            SelfReportedLoanListModel srlList = new SelfReportedLoanListModel();
            srlList.Loans.Clear();

            bool bValid = srlList.IsValid();
            Assert.IsTrue(bValid, "This SelfReportedLoanList failed validation when it has an empty Loan list.");
        }

        #endregion

        #region Required

        #endregion
       
        #region Length
            //<!-- LENGTH STUFF-->
            //<v:condition id="LoanStatusIdLengthValidator" test="LoanStatusId.Length &lt;= 2">
            //  <v:message id="error_loanstatusid_length_invalid" providers="SelfReportedLoanModelErrorProvider"/>
            //</v:condition>
            //<v:condition id="LoanTypeIdLengthValidator" test="LoanTypeId.Length &lt;= 2">
            //  <v:message id="error_loantypeid_length_invalid" providers="SelfReportedLoanModelErrorProvider"/>
            //</v:condition>
            //<v:condition id="AccountNicknameLengthValidator" test="AccountNickname.Length &lt;= 50">
            //  <v:message id="error_accountnickname_length_invalid" providers="SelfReportedLoanModelErrorProvider"/>
            //</v:condition>
            //<v:condition id="HolderNameLengthValidator" test="HolderName.Length &lt;= 80">
            //  <v:message id="error_holdername_length_invalid" providers="SelfReportedLoanModelErrorProvider"/>
            //</v:condition>
            //<v:condition id="SchoolNameLengthValidator" test="SchoolName.Length &lt;= 80">
            //  <v:message id="error_schoolname_length_invalid" providers="SelfReportedLoanModelErrorProvider"/>
            //</v:condition>
            //<v:condition id="ServicerNameLengthValidator" test="ServicerName.Length &lt;= 80">
            //  <v:message id="error_servicername_length_invalid" providers="SelfReportedLoanModelErrorProvider"/>
            //</v:condition>
            //<v:condition id="ServicerWebAddressLengthValidator" test="ServicerWebAddress.Length &lt;= 120">
            //  <v:message id="error_servicerwebaddress_length_invalid" providers="SelfReportedLoanModelErrorProvider"/>
            //</v:condition>
            //<v:condition id="ServicerWebAddressLengthValidator" test="ServicerWebAddress.Length &lt;= 120">
            //  <v:message id="error_servicerwebaddress_length_invalid" providers="SelfReportedLoanModelErrorProvider"/>
            //</v:condition>
        #endregion
       
        #region Regex   
          //  <!-- REGEX STUFF -->
          //  <v:regex id="InterestRateRegexValidator" test="InterestRate">
          //    <v:property name="Expression" ref="MoneyExpression" />
          //    <v:message id="error_interestrate_invalid" providers="SelfReportedLoanModelErrorProvider" />
          //  </v:regex>
          //  <v:regex id="PrincipalBalanceOutstandingAmountRegexValidator" test="PrincipalBalanceOutstandingAmount">
          //    <v:property name="Expression" ref="MoneyExpression" />
          //    <v:message id="error_principalbalanceoutstandingamount_invalid" providers="SelfReportedLoanModelErrorProvider" />
          //  </v:regex>
          //  <v:regex id="PaymentDueAmountRegexValidator" test="PaymentDueAmount">
          //    <v:property name="Expression" ref="MoneyExpression" />
          //    <v:message id="error_paymentdueamount_invalid" providers="SelfReportedLoanModelErrorProvider" />
          //  </v:regex>
          //  <v:regex id="NextPaymentDueAmountRegexValidator" test="NextPaymentDueAmount">
          //    <v:property name="Expression" ref="MoneyExpression" />
          //    <v:message id="error_nextpaymentdueamount_invalid" providers="SelfReportedLoanModelErrorProvider" />
          //  </v:regex>
          //</v:group>







        #endregion

        [TestMethod]
        public void AccountNicknameTest_SelfReportedLoanModel_check_for_string_length()
        {

            //SelfReportedLoanModel passingValueLowerBound = new SelfReportedLoanModel() { AccountNickname = "1" };
            //SelfReportedLoanModel passingValueUpperBound = new SelfReportedLoanModel() { AccountNickname = "50CharactersCharactersCharactersCharactersCharacte" };
            //SelfReportedLoanModel failingValueUnderMin = new SelfReportedLoanModel() { AccountNickname = "" };
            //SelfReportedLoanModel failingValueAboveMax = new SelfReportedLoanModel() { AccountNickname = "51CharactersCharactersCharactersCharactersCharacter" };


            //StringLengthAttribute stringLengthCheck = new StringLengthAttribute(50);
            //stringLengthCheck.MinimumLength = 1;


            //Assert.IsTrue(stringLengthCheck.IsValid(passingValueLowerBound.AccountNickname), "Assertion of positive case (lower bound) being true failed");
            //Assert.IsTrue(stringLengthCheck.IsValid(passingValueUpperBound.AccountNickname), "Assertion of positive case (upper bound) being true failed");
            //Assert.IsFalse(stringLengthCheck.IsValid(failingValueUnderMin.AccountNickname), "Assertion of negative case (under min) case being false failed");
            //Assert.IsFalse(stringLengthCheck.IsValid(failingValueAboveMax.AccountNickname), "Assertion of negative case (above max) case being false failed");

        }


        [TestMethod]
        public void HolderNameTest_SelfReportedLoanModel_check_for_string_length()
        {

            //SelfReportedLoanModel passingValueLowerBound = new SelfReportedLoanModel() { HolderName = "1" };
            //SelfReportedLoanModel passingValueUpperBound = new SelfReportedLoanModel() { HolderName = "80CharactersCharactersCharactersCharactersCharactersCharactersCharactersCharacte" };
            //SelfReportedLoanModel failingValueUnderMin = new SelfReportedLoanModel() { HolderName = "" };
            //SelfReportedLoanModel failingValueAboveMax = new SelfReportedLoanModel() { HolderName = "81CharactersCharactersCharactersCharactersCharactersCharactersCharactersCharacter" };


            //StringLengthAttribute stringLengthCheck = new StringLengthAttribute(80);
            //stringLengthCheck.MinimumLength = 1;


            //Assert.IsTrue(stringLengthCheck.IsValid(passingValueLowerBound.HolderName), "Assertion of positive case (lower bound) being true failed");
            //Assert.IsTrue(stringLengthCheck.IsValid(passingValueUpperBound.HolderName), "Assertion of positive case (upper bound) being true failed");
            //Assert.IsFalse(stringLengthCheck.IsValid(failingValueUnderMin.HolderName), "Assertion of negative case (under min) case being false failed");
            //Assert.IsFalse(stringLengthCheck.IsValid(failingValueAboveMax.HolderName), "Assertion of negative case (above max) case being false failed");
        
        }


        [TestMethod]
        public void IndividualIdTest_SelfReportedLoanModel_check_for_required()
        {

            SelfReportedLoanModel passingValue = new SelfReportedLoanModel() { IndividualId = "Non- null string for required check" };
            SelfReportedLoanModel failingValueEmptyString = new SelfReportedLoanModel() { IndividualId = "" };
            SelfReportedLoanModel failingValueNull = new SelfReportedLoanModel() { };


            RequiredAttribute requiredCheck = new RequiredAttribute();


            Assert.IsTrue(requiredCheck.IsValid(passingValue.IndividualId), "Assertion of positive case being true failed");
            Assert.IsFalse(requiredCheck.IsValid(failingValueEmptyString.IndividualId), "Assertion of negative case (empty string) case being false failed");
            Assert.IsFalse(requiredCheck.IsValid(failingValueNull.IndividualId), "Assertion of negative case (null) being false failed");

        }


        [TestMethod]
        public void IndividualIdTest_SelfReportedLoanModel_check_for_regex()
        {

            SelfReportedLoanModel passingValue = new SelfReportedLoanModel() { IndividualId = "B93F10EB-2BC0-47A2-B5C5-082A064E3099" };
            SelfReportedLoanModel failingValueImproperNumberOfDigits = new SelfReportedLoanModel() { IndividualId = "B93F10EB-2BC0-47A2-B5C5-082A064E309" };
            SelfReportedLoanModel failingValueImproperDigits = new SelfReportedLoanModel() { IndividualId = "!!3F10EB-2BC0-47A2-B5C5-082A064E309" };


            RegularExpressionAttribute regexCheck = new RegularExpressionAttribute(@"^(\{){0,1}[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}(\}){0,1}$");


            Assert.IsTrue(regexCheck.IsValid(passingValue.IndividualId), "Assertion of positive case being true failed");
            Assert.IsFalse(regexCheck.IsValid(failingValueImproperNumberOfDigits.IndividualId), "Assertion of negative case (improper # of digits) case being false failed");
            Assert.IsFalse(regexCheck.IsValid(failingValueImproperDigits.IndividualId), "Assertion of negative case (improper characters) being false failed");
       
        }


        [TestMethod]
        public void InterestRateTest_SelfReportedLoanModel_check_for_required()
        {

            SelfReportedLoanModel passingValue = new SelfReportedLoanModel() { InterestRate = 50 };
            SelfReportedLoanModel failingValueNull = new SelfReportedLoanModel() { InterestRate = null };


            RequiredAttribute requiredCheck = new RequiredAttribute();


            Assert.IsTrue(requiredCheck.IsValid(passingValue.InterestRate), "Assertion of positive case being true failed");
            Assert.IsFalse(requiredCheck.IsValid(failingValueNull.InterestRate), "Assertion of negative case (null) being false failed");

        }


        [TestMethod]
        public void InterestRateTest_SelfReportedLoanModel_check_for_range()
        {

            SelfReportedLoanModel passingValueLowerBound = new SelfReportedLoanModel() { InterestRate = 0.0 };
            SelfReportedLoanModel passingValueUpperBound = new SelfReportedLoanModel() { InterestRate = 100.0 };
            SelfReportedLoanModel failingValueUnderMin = new SelfReportedLoanModel() { InterestRate = (-.01) };
            SelfReportedLoanModel failingValueAboveMax = new SelfReportedLoanModel() { InterestRate = 100.01 };

            RangeAttribute rangeCheck = new RangeAttribute(0.0, 100.0);

            Assert.IsTrue(rangeCheck.IsValid(passingValueLowerBound.InterestRate), "Assertion of positive case (lower bound) being true failed");
            Assert.IsTrue(rangeCheck.IsValid(passingValueUpperBound.InterestRate), "Assertion of positive case (upper bound) being true failed");
            Assert.IsFalse(rangeCheck.IsValid(failingValueUnderMin.InterestRate), "Assertion of negative case (under min) case being false failed");
            Assert.IsFalse(rangeCheck.IsValid(failingValueAboveMax.InterestRate), "Assertion of negative case (above max) case being false failed");

        }


        [TestMethod]
        public void LoanSelfReportedEntryIdTest_SelfReportedLoanModel_check_for_regex()
        {
            SelfReportedLoanModel passingValue = new SelfReportedLoanModel() { LoanSelfReportedEntryId = "B93F10EB-2BC0-47A2-B5C5-082A064E3099" };
            SelfReportedLoanModel failingValueImproperNumberOfDigits = new SelfReportedLoanModel() { LoanSelfReportedEntryId = "B93F10EB-2BC0-47A2-B5C5-082A064E309" };
            SelfReportedLoanModel failingValueImproperDigits = new SelfReportedLoanModel() { LoanSelfReportedEntryId = "!!3F10EB-2BC0-47A2-B5C5-082A064E309" };

            RegularExpressionAttribute regexCheck = new RegularExpressionAttribute(@"^(\{){0,1}[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}(\}){0,1}$");


            Assert.IsTrue(regexCheck.IsValid(passingValue.LoanSelfReportedEntryId), "Assertion of positive case being true failed");
            Assert.IsFalse(regexCheck.IsValid(failingValueImproperNumberOfDigits.LoanSelfReportedEntryId), "Assertion of negative case (improper # of digits) case being false failed");
            Assert.IsFalse(regexCheck.IsValid(failingValueImproperDigits.LoanSelfReportedEntryId), "Assertion of negative case (improper characters) being false failed");
        }


        [TestMethod]
        public void LoanStatusIdTest_SelfReportedLoanModel_check_for_string_length()
        {

            SelfReportedLoanModel passingValueLowerBound = new SelfReportedLoanModel() { LoanStatusId = "1" };
            SelfReportedLoanModel passingValueUpperBound = new SelfReportedLoanModel() { LoanStatusId = "12" };
            SelfReportedLoanModel failingValueUnderMin = new SelfReportedLoanModel() { LoanStatusId = "" };
            SelfReportedLoanModel failingValueAboveMax = new SelfReportedLoanModel() { LoanStatusId = "123" };


            StringLengthAttribute stringLengthCheck = new StringLengthAttribute(2);
            stringLengthCheck.MinimumLength = 1;


            Assert.IsTrue(stringLengthCheck.IsValid(passingValueLowerBound.LoanStatusId), "Assertion of positive case (lower bound) being true failed");
            Assert.IsTrue(stringLengthCheck.IsValid(passingValueUpperBound.LoanStatusId), "Assertion of positive case (upper bound) being true failed");
            Assert.IsFalse(stringLengthCheck.IsValid(failingValueUnderMin.LoanStatusId), "Assertion of negative case (under min) case being false failed");
            Assert.IsFalse(stringLengthCheck.IsValid(failingValueAboveMax.LoanStatusId), "Assertion of negative case (above max) case being false failed");

        }


        [TestMethod]
        public void LoanTermTest_SelfReportedLoanModel_check_for_required()
        {

            // Non-Nullable
            SelfReportedLoanModel passingValue = new SelfReportedLoanModel() { LoanTerm = 2005 };


            RequiredAttribute requiredCheck = new RequiredAttribute();


            Assert.IsTrue(requiredCheck.IsValid(passingValue.LoanTerm), "Assertion of positive case being true failed");

        }


        [TestMethod]
        public void LoanTermTest_SelfReportedLoanModel_check_for_range()
        {

            SelfReportedLoanModel passingValueLowerBound = new SelfReportedLoanModel() { LoanTerm = 1 };
            SelfReportedLoanModel passingValueUpperBound = new SelfReportedLoanModel() { LoanTerm = 100 };
            SelfReportedLoanModel failingValueUnderMin = new SelfReportedLoanModel() { LoanTerm = 0 };
            SelfReportedLoanModel failingValueOverMax = new SelfReportedLoanModel() { LoanTerm = 101 };


            RangeAttribute rangeCheck = new RangeAttribute(1, 100);


            Assert.IsTrue(rangeCheck.IsValid(passingValueLowerBound.LoanTerm), "Assertion of positive case (lower bound) being true failed");
            Assert.IsTrue(rangeCheck.IsValid(passingValueUpperBound.LoanTerm), "Assertion of positive case (upper bound) being true failed");
            Assert.IsFalse(rangeCheck.IsValid(failingValueUnderMin.LoanTerm), "Assertion of negative case (under min) being true failed");
            Assert.IsFalse(rangeCheck.IsValid(failingValueOverMax.LoanTerm), "Assertion of negative case (over max) being true failed");

        }


        [TestMethod]
        public void LoanTypeIdTest_SelfReportedLoanModel_check_for_required()
        {

            SelfReportedLoanModel passingValue = new SelfReportedLoanModel() { LoanTypeId = "Non- null string for required check" };
            SelfReportedLoanModel failingValueEmptyString = new SelfReportedLoanModel() { LoanTypeId = "" };
            SelfReportedLoanModel failingValueNull = new SelfReportedLoanModel() {};


            RequiredAttribute requiredCheck = new RequiredAttribute();


            Assert.IsTrue(requiredCheck.IsValid(passingValue.LoanTypeId), "Assertion of positive case being true failed");
            Assert.IsFalse(requiredCheck.IsValid(failingValueEmptyString.LoanTypeId), "Assertion of negative case (empty string) case being false failed");
            Assert.IsFalse(requiredCheck.IsValid(failingValueNull.LoanTypeId), "Assertion of negative case (null) being false failed");
       
        }


        [TestMethod]
        public void LoanTypeIdTest_SelfReportedLoanModel_check_for_string_length()
        {

            SelfReportedLoanModel passingValueLowerBound = new SelfReportedLoanModel() { LoanTypeId = "1" };
            SelfReportedLoanModel passingValueUpperBound = new SelfReportedLoanModel() { LoanTypeId = "50CharactersCharactersCharactersCharactersCharacte" };
            SelfReportedLoanModel failingValueUnderMin = new SelfReportedLoanModel() { LoanTypeId = "" };
            SelfReportedLoanModel failingValueAboveMax = new SelfReportedLoanModel() { LoanTypeId = "51CharactersCharactersCharactersCharactersCharacter" };


            StringLengthAttribute stringLengthCheck = new StringLengthAttribute(50);
            stringLengthCheck.MinimumLength = 1;


            Assert.IsTrue(stringLengthCheck.IsValid(passingValueLowerBound.LoanTypeId), "Assertion of positive case (lower bound) being true failed");
            Assert.IsTrue(stringLengthCheck.IsValid(passingValueUpperBound.LoanTypeId), "Assertion of positive case (upper bound) being true failed");
            Assert.IsFalse(stringLengthCheck.IsValid(failingValueUnderMin.LoanTypeId), "Assertion of negative case (under min) case being false failed");
            Assert.IsFalse(stringLengthCheck.IsValid(failingValueAboveMax.LoanTypeId), "Assertion of negative case (above max) case being false failed");
        
        }


        [TestMethod]
        public void NextPaymentDueAmountTest_SelfReportedLoanModel_check_for_regex()
        {

            //SelfReportedLoanModel passingValueTwoDecimalPlaces = new SelfReportedLoanModel() { NextPaymentDueAmount = 100.01m };
            //SelfReportedLoanModel passingValueOneDecimalPlace = new SelfReportedLoanModel() { NextPaymentDueAmount = 100.1m };
            //SelfReportedLoanModel passingValueNoDecimalPlaces = new SelfReportedLoanModel() { NextPaymentDueAmount = 100m };
            //SelfReportedLoanModel failingValueImproperNumberOfDigits = new SelfReportedLoanModel() { NextPaymentDueAmount = 100.001m };


            //RegularExpressionAttribute regexCheck = new RegularExpressionAttribute(RegexStrings.CURRENCY);


            //Assert.IsTrue(regexCheck.IsValid(passingValueTwoDecimalPlaces.NextPaymentDueAmount), "Assertion of positive case (2 decimal places) being true failed");
            //Assert.IsTrue(regexCheck.IsValid(passingValueNoDecimalPlaces.NextPaymentDueAmount), "Assertion of positive case (0 decimal places) being true failed");
            //Assert.IsTrue(regexCheck.IsValid(passingValueOneDecimalPlace.NextPaymentDueAmount), "Assertion of positive case (1 decimal place) being true failed");
            //Assert.IsFalse(regexCheck.IsValid(failingValueImproperNumberOfDigits.NextPaymentDueAmount), "Assertion of negative case (improper # of digits) case being false failed");

        }


        [TestMethod]
        public void OriginalLoanAmountTest_SelfReportedLoanModel_check_for_required()
        {

            SelfReportedLoanModel passingValue = new SelfReportedLoanModel() { OriginalLoanAmount = 0.0m };
            SelfReportedLoanModel failingValueNull = new SelfReportedLoanModel() { };


            RequiredAttribute requiredCheck = new RequiredAttribute();


            Assert.IsTrue(requiredCheck.IsValid(passingValue.OriginalLoanAmount), "Assertion of positive case being true failed");
            Assert.IsFalse(requiredCheck.IsValid(failingValueNull.OriginalLoanAmount), "Assertion of negative case (null) case being false failed");

        }


        [TestMethod]
        public void OriginalLoanAmountTest_SelfReportedLoanModel_check_for_regex()
        {

            SelfReportedLoanModel passingValueTwoDecimalPlaces = new SelfReportedLoanModel() { OriginalLoanAmount = 100.01m };
            SelfReportedLoanModel passingValueOneDecimalPlace = new SelfReportedLoanModel() { OriginalLoanAmount = 100.1m };
            SelfReportedLoanModel passingValueNoDecimalPlaces = new SelfReportedLoanModel() { OriginalLoanAmount = 100m };
            SelfReportedLoanModel failingValueImproperNumberOfDigits = new SelfReportedLoanModel() { OriginalLoanAmount = 100.001m };


            RegularExpressionAttribute regexCheck = new RegularExpressionAttribute(RegexStrings.CURRENCY);


            Assert.IsTrue(regexCheck.IsValid(passingValueTwoDecimalPlaces.OriginalLoanAmount), "Assertion of positive case (2 decimal places) being true failed");
            Assert.IsTrue(regexCheck.IsValid(passingValueNoDecimalPlaces.OriginalLoanAmount), "Assertion of positive case (0 decimal places) being true failed");
            Assert.IsTrue(regexCheck.IsValid(passingValueOneDecimalPlace.OriginalLoanAmount), "Assertion of positive case (1 decimal place) being true failed");
            Assert.IsFalse(regexCheck.IsValid(failingValueImproperNumberOfDigits.OriginalLoanAmount), "Assertion of negative case (improper # of digits) case being false failed");

        }


        [TestMethod]
        public void PaymentDueAmountTest_SelfReportedLoanModel_check_for_regex()
        {

            //SelfReportedLoanModel passingValueTwoDecimalPlaces = new SelfReportedLoanModel() { PaymentDueAmount = 100.01m };
            //SelfReportedLoanModel passingValueOneDecimalPlace = new SelfReportedLoanModel() { PaymentDueAmount = 100.1m };
            //SelfReportedLoanModel passingValueNoDecimalPlaces = new SelfReportedLoanModel() { PaymentDueAmount = 100m };
            //SelfReportedLoanModel failingValueImproperNumberOfDigits = new SelfReportedLoanModel() { PaymentDueAmount = 100.001m };


            //RegularExpressionAttribute regexCheck = new RegularExpressionAttribute(RegexStrings.CURRENCY);


            //Assert.IsTrue(regexCheck.IsValid(passingValueTwoDecimalPlaces.PaymentDueAmount), "Assertion of positive case (2 decimal places) being true failed");
            //Assert.IsTrue(regexCheck.IsValid(passingValueNoDecimalPlaces.PaymentDueAmount), "Assertion of positive case (0 decimal places) being true failed");
            //Assert.IsTrue(regexCheck.IsValid(passingValueOneDecimalPlace.PaymentDueAmount), "Assertion of positive case (1 decimal place) being true failed");
            //Assert.IsFalse(regexCheck.IsValid(failingValueImproperNumberOfDigits.PaymentDueAmount), "Assertion of negative case (improper # of digits) case being false failed");

        }


        [TestMethod]
        public void PrincipalBalanceOutstandingAmountTest_SelfReportedLoanModel_check_for_required()
        {

            SelfReportedLoanModel passingValue = new SelfReportedLoanModel() { PrincipalBalanceOutstandingAmount = 0.0m };
            SelfReportedLoanModel failingValueNull = new SelfReportedLoanModel() {  };
            

            RequiredAttribute requiredCheck = new RequiredAttribute();


            Assert.IsTrue(requiredCheck.IsValid(passingValue.PrincipalBalanceOutstandingAmount), "Assertion of positive case being true failed");
            Assert.IsFalse(requiredCheck.IsValid(failingValueNull.PrincipalBalanceOutstandingAmount), "Assertion of negative case (null) case being false failed");
           
        }


        [TestMethod]
        public void PrincipalBalanceOutstandingAmountTest_SelfReportedLoanModel_check_for_regex()
        {

            SelfReportedLoanModel passingValueTwoDecimalPlaces = new SelfReportedLoanModel() { PrincipalBalanceOutstandingAmount = 100.01m };
            SelfReportedLoanModel passingValueOneDecimalPlace = new SelfReportedLoanModel() { PrincipalBalanceOutstandingAmount = 100.1m };
            SelfReportedLoanModel passingValueNoDecimalPlaces = new SelfReportedLoanModel() { PrincipalBalanceOutstandingAmount = 100m };
            SelfReportedLoanModel failingValueImproperNumberOfDigits = new SelfReportedLoanModel() { PrincipalBalanceOutstandingAmount = 100.001m };
       

            RegularExpressionAttribute regexCheck = new RegularExpressionAttribute(RegexStrings.CURRENCY);


            Assert.IsTrue(regexCheck.IsValid(passingValueTwoDecimalPlaces.PrincipalBalanceOutstandingAmount), "Assertion of positive case (2 decimal places) being true failed");
            Assert.IsTrue(regexCheck.IsValid(passingValueNoDecimalPlaces.PrincipalBalanceOutstandingAmount), "Assertion of positive case (0 decimal places) being true failed");
            Assert.IsTrue(regexCheck.IsValid(passingValueOneDecimalPlace.PrincipalBalanceOutstandingAmount), "Assertion of positive case (1 decimal place) being true failed");
            Assert.IsFalse(regexCheck.IsValid(failingValueImproperNumberOfDigits.PrincipalBalanceOutstandingAmount), "Assertion of negative case (improper # of digits) case being false failed");
            
        }

        
        [TestMethod]
        public void ReceivedYearTest_SelfReportedLoanModel_check_for_required()
        {

            SelfReportedLoanModel passingValue = new SelfReportedLoanModel() { ReceivedYear = 2005 };


            RequiredAttribute requiredCheck = new RequiredAttribute();


            Assert.IsTrue(requiredCheck.IsValid(passingValue.ReceivedYear), "Assertion of positive case being true failed");

        }


        [TestMethod]
        public void ReceivedYearTest_SelfReportedLoanModel_check_for_regex()
        {

            SelfReportedLoanModel passingValueLowerBound = new SelfReportedLoanModel() { ReceivedYear = 1900 };
            SelfReportedLoanModel passingValueUpperBound = new SelfReportedLoanModel() { ReceivedYear = 2099 };
            SelfReportedLoanModel failingValueYearTooLow = new SelfReportedLoanModel() { ReceivedYear = 1899 };
            SelfReportedLoanModel failingValueYearTooHigh = new SelfReportedLoanModel() { ReceivedYear = 2100 };
            SelfReportedLoanModel failingValueNull = new SelfReportedLoanModel() { };
            SelfReportedLoanModel failingValueImproperNumberOfDigitsTooFew = new SelfReportedLoanModel() { ReceivedYear = 213 };
            SelfReportedLoanModel failingValueImproperNumberOfDigitsTooMany = new SelfReportedLoanModel() { ReceivedYear = 20013 };
            SelfReportedLoanModel failingValueNegative = new SelfReportedLoanModel() { ReceivedYear = -2013 };


            RegularExpressionAttribute regexCheck = new RegularExpressionAttribute(@"^(19|20)\d{2}$");


            Assert.IsTrue(regexCheck.IsValid(passingValueLowerBound.ReceivedYear), "Assertion of positive case lower bound being true failed");
            Assert.IsTrue(regexCheck.IsValid(passingValueUpperBound.ReceivedYear), "Assertion of positive case upper bound  being true failed");
            Assert.IsFalse(regexCheck.IsValid(failingValueYearTooLow.ReceivedYear), "Assertion of negative case lower bound  being true failed");
            Assert.IsFalse(regexCheck.IsValid(failingValueYearTooHigh.ReceivedYear), "Assertion of negative case upper bound  being true failed");
            Assert.IsFalse(regexCheck.IsValid(failingValueNull.ReceivedYear), "Assertion of negative case null case being false failed");
            Assert.IsFalse(regexCheck.IsValid(failingValueImproperNumberOfDigitsTooFew.ReceivedYear), "Assertion of negative case too few digits case being false failed");
            Assert.IsFalse(regexCheck.IsValid(failingValueImproperNumberOfDigitsTooMany.ReceivedYear), "Assertion of negative case to many digits case being false failed");
            Assert.IsFalse(regexCheck.IsValid(failingValueNegative.ReceivedYear), "Assertion of negative case (negative year) case being false failed");

        }


        [TestMethod]
        public void SchoolNameTest_SelfReportedLoanModel_check_for_string_length()
        {

            //SelfReportedLoanModel passingValueLowerBound = new SelfReportedLoanModel() { SchoolName = "1" };
            //SelfReportedLoanModel passingValueUpperBound = new SelfReportedLoanModel() { SchoolName = "80CharactersCharactersCharactersCharactersCharactersCharactersCharactersCharacte" };
            //SelfReportedLoanModel failingValueUnderMin = new SelfReportedLoanModel() { SchoolName = "" };
            //SelfReportedLoanModel failingValueAboveMax = new SelfReportedLoanModel() { SchoolName = "81CharactersCharactersCharactersCharactersCharactersCharactersCharactersCharacter" };


            //StringLengthAttribute stringLengthCheck = new StringLengthAttribute(80);
            //stringLengthCheck.MinimumLength = 1;


            //Assert.IsTrue(stringLengthCheck.IsValid(passingValueLowerBound.SchoolName), "Assertion of positive case (lower bound) being true failed");
            //Assert.IsTrue(stringLengthCheck.IsValid(passingValueUpperBound.SchoolName), "Assertion of positive case (upper bound) being true failed");
            //Assert.IsFalse(stringLengthCheck.IsValid(failingValueUnderMin.SchoolName), "Assertion of negative case (under min) case being false failed");
            //Assert.IsFalse(stringLengthCheck.IsValid(failingValueAboveMax.SchoolName), "Assertion of negative case (above max) case being false failed");

        }


        [TestMethod]
        public void ServicerNameTest_SelfReportedLoanModel_check_for_string_length()
        {

            //SelfReportedLoanModel passingValueLowerBound = new SelfReportedLoanModel() { ServicerName = "1" };
            //SelfReportedLoanModel passingValueUpperBound = new SelfReportedLoanModel() { ServicerName = "80CharactersCharactersCharactersCharactersCharactersCharactersCharactersCharacte" };
            //SelfReportedLoanModel failingValueUnderMin = new SelfReportedLoanModel() { ServicerName = "" };
            //SelfReportedLoanModel failingValueAboveMax = new SelfReportedLoanModel() { ServicerName = "81CharactersCharactersCharactersCharactersCharactersCharactersCharactersCharacter" };


            //StringLengthAttribute stringLengthCheck = new StringLengthAttribute(80);
            //stringLengthCheck.MinimumLength = 1;


            //Assert.IsTrue(stringLengthCheck.IsValid(passingValueLowerBound.ServicerName), "Assertion of positive case (lower bound) being true failed");
            //Assert.IsTrue(stringLengthCheck.IsValid(passingValueUpperBound.ServicerName), "Assertion of positive case (upper bound) being true failed");
            //Assert.IsFalse(stringLengthCheck.IsValid(failingValueUnderMin.ServicerName), "Assertion of negative case (under min) case being false failed");
            //Assert.IsFalse(stringLengthCheck.IsValid(failingValueAboveMax.ServicerName), "Assertion of negative case (above max) case being false failed");

        }


        [TestMethod]
        public void ServicerWebAddressTest_SelfReportedLoanModel_check_for_string_length()
        {

            //SelfReportedLoanModel passingValueLowerBound = new SelfReportedLoanModel() { ServicerWebAddress = "1" };
            //SelfReportedLoanModel passingValueUpperBound = new SelfReportedLoanModel() { ServicerWebAddress = "120CharactersCharactersCharactersCharactersCharactersCharactersCharactersCharactersCharactersCharactersCharactersCharact" };
            //SelfReportedLoanModel failingValueUnderMin = new SelfReportedLoanModel() { ServicerWebAddress = "" };
            //SelfReportedLoanModel failingValueAboveMax = new SelfReportedLoanModel() { ServicerWebAddress = "121CharactersCharactersCharactersCharactersCharactersCharactersCharactersCharactersCharactersCharactersCharactersCharacte" };


            //StringLengthAttribute stringLengthCheck = new StringLengthAttribute(120);
            //stringLengthCheck.MinimumLength = 1;


            //Assert.IsTrue(stringLengthCheck.IsValid(passingValueLowerBound.ServicerWebAddress), "Assertion of positive case (lower bound) being true failed");
            //Assert.IsTrue(stringLengthCheck.IsValid(passingValueUpperBound.ServicerWebAddress), "Assertion of positive case (upper bound) being true failed");
            //Assert.IsFalse(stringLengthCheck.IsValid(failingValueUnderMin.ServicerWebAddress), "Assertion of negative case (under min) case being false failed");
            //Assert.IsFalse(stringLengthCheck.IsValid(failingValueAboveMax.ServicerWebAddress), "Assertion of negative case (above max) case being false failed");

        }
   
    }
}
