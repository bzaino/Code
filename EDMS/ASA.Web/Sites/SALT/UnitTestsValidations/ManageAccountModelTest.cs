using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Web.Mvc;

using ASA.Web.Services.Common;
using ASA.Web.Common.Validation;
using ASA.Web.Sites.SALT.Models;

namespace UnitTestsValidations
{
    /// <summary>
    ///This is a test class for ManageAccountModelTest and is intended
    ///to contain all ManageAccountModelTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ManageAccountModelTest
    {
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

        [TestMethod()]
        public void YOBTest_manageAccount_check_for_valid_YOB_ValidValue()
        {
            ManageAccountModel passingValue = new ManageAccountModel() { YOB = (short)(DateTime.Today.Year - 14) };

            ASAModelValidator mv = new ASAModelValidator();
            
            bool valid = mv.Validate(passingValue, "YOB");
            Assert.IsTrue(valid, "Assertion of positive case being true failed");
        }

        [TestMethod()]
        public void YOBTest_manageAccount_check_for_valid_YOB_InvalildValue()
        {
            ManageAccountModel invalildValue = new ManageAccountModel() { YOB = (short)(DateTime.Today.Year - 32767) };

            ASAModelValidator mv = new ASAModelValidator();

            bool valid = mv.Validate(invalildValue, "YOB");
            Assert.IsFalse(valid, "Assertion of negative case being false failed");
            Assert.AreEqual("Year of birth is invalid.", invalildValue.ErrorList[0].BusinessMessage);
        }

        [TestMethod()]
        public void EmailAddressTest_manageAccount_check_for_required_ValidValue()
        {
            ManageAccountModel passingValue = new ManageAccountModel() { EmailAddress = "validEmalAddress@asa.org" };

            ASAModelValidator mv = new ASAModelValidator();

            bool valid = mv.Validate(passingValue, "EmailAddress");
            Assert.IsTrue(valid, "Assertion of positive case being true failed");
        }

        [TestMethod()]
        public void EmailAddressTest_manageAccount_check_for_required_InvalidValue()
        {
            ManageAccountModel invalidValue = new ManageAccountModel() { };

            ASAModelValidator mv = new ASAModelValidator();

            bool valid = mv.Validate(invalidValue, "EmailAddress");
            Assert.IsFalse(valid, "Assertion of negative case being false failed");
            Assert.AreEqual("Email Address is required!", invalidValue.ErrorList[0].BusinessMessage);
        }

        [TestMethod()]
        public void EmailAddressTest_manageAccount_check_for_string_length_ValidLowerBoundValue()
        {
            ManageAccountModel passingValueLowerBound = new ManageAccountModel() { EmailAddress = "xy@z.com" };

            ASAModelValidator mv = new ASAModelValidator();

            bool valid = mv.Validate(passingValueLowerBound, "EmailAddress");
            Assert.IsTrue(valid, "Assertion of positive case (lower bound) being true failed");
        }

        [TestMethod()]
        public void EmailAddressTest_manageAccount_check_for_string_length_ValidUpperBoundValue()
        {
            ManageAccountModel passingValueUpperBound = new ManageAccountModel() { EmailAddress = "64CharactersCharactersCharactersCharactersCharactersChar.y@z.com" };

            ASAModelValidator mv = new ASAModelValidator();

            bool valid = mv.Validate(passingValueUpperBound, "EmailAddress");
            Assert.IsTrue(valid, "Assertion of positive case (upper bound) being true failed");
        }

        [TestMethod()]
        public void EmailAddressTest_manageAccount_check_for_string_length_InvalidUnderMinValue()
        {
            ManageAccountModel failingValueUnderMin = new ManageAccountModel() { EmailAddress = "x@z.ru" };

            ASAModelValidator mv = new ASAModelValidator();

            bool valid = mv.Validate(failingValueUnderMin, "EmailAddress");
            Assert.IsFalse(valid, "Assertion of negative case (under min) being false failed");
            Assert.AreEqual(1, failingValueUnderMin.ErrorList.Count, "Expected 1 error");
            Assert.AreEqual(failingValueUnderMin.ErrorList[0].BusinessMessage,"The EmailAddress address must be between 8 and 64 characters long.");
        }

        [TestMethod()]
        public void EmailAddressTest_manageAccount_check_for_string_length_InvalidOverMaxValue()
        {
            ManageAccountModel failingValueOverMax = new ManageAccountModel() { EmailAddress = "65CharactersCharactersCharactersCharactersCharactersChara.y@z.com" };

            ASAModelValidator mv = new ASAModelValidator();

            bool valid = mv.Validate(failingValueOverMax, "EmailAddress");
            Assert.IsFalse(valid, "Assertion of negative case (over max) being false failed");
            Assert.AreEqual(2, failingValueOverMax.ErrorList.Count, "Expected 2 errors");
            String verifyMessage = failingValueOverMax.ErrorList[0].BusinessMessage + failingValueOverMax.ErrorList[1].BusinessMessage;
            Assert.IsTrue(verifyMessage.Contains("Please enter a valid email."), "message should contain - Please enter a valid email.");
            Assert.IsTrue(verifyMessage.Contains("The EmailAddress address must be between 8 and 64 characters long."), "message should contain - The EmailAddress address must be between 8 and 64 characters long.");
        }

        [TestMethod()]
        public void EmailAddressTest_manageAccount_check_for_valid_email_ValidValue()
        {
            ManageAccountModel passingValue = new ManageAccountModel() { EmailAddress = "person@yahoo.com" };

            ASAModelValidator mv = new ASAModelValidator();

            bool valid = mv.Validate(passingValue, "EmailAddress");
            Assert.IsTrue(valid, "Assertion of positive case being true failed");
        }


        [TestMethod()]
        public void EmailAddressTest_manageAccount_check_for_valid_email_InvalidValue()
        {
            ManageAccountModel invalidValue = new ManageAccountModel() { EmailAddress = "failString" };

            ASAModelValidator mv = new ASAModelValidator();

            bool valid = mv.Validate(invalidValue, "EmailAddress");
            Assert.IsFalse(valid, "Assertion of negative case being false failed");
            Assert.AreEqual("Please enter a valid email.", invalidValue.ErrorList[0].BusinessMessage);
        }

        [TestMethod()]
        public void EnrollmentStatus_manageAccount_Test_check_for_string_length_ValidValue()
        {
            ManageAccountModel passingValue = new ManageAccountModel() { EnrollmentStatus = "P" };

            ASAModelValidator mv = new ASAModelValidator();

            bool valid = mv.Validate(passingValue, "EnrollmentStatus");
            Assert.IsTrue(valid, "Assertion of positive case being true failed");
        }


        [TestMethod()]
        public void EnrollmentStatus_manageAccount_Test_check_for_string_length_InvalidUnderMinValue()
        {
            ManageAccountModel failingValueUnderMin = new ManageAccountModel() { EnrollmentStatus = "" };

            ASAModelValidator mv = new ASAModelValidator();

            bool valid = mv.Validate(failingValueUnderMin, "EnrollmentStatus");
            Assert.IsFalse(valid, "Assertion of negative case (over max) being false failed");
            Assert.AreEqual("The field Enrollment Status must be a string with a minimum length of 1 and a maximum length of 1.", failingValueUnderMin.ErrorList[0].BusinessMessage);
        }

        [TestMethod()]
        public void EnrollmentStatus_manageAccount_Test_check_for_string_length_InvalidOverMaxValue()
        {
            ManageAccountModel failingValueOverMax = new ManageAccountModel() { EnrollmentStatus = "2C" };

            ASAModelValidator mv = new ASAModelValidator();

            bool valid = mv.Validate(failingValueOverMax, "EnrollmentStatus");
            Assert.IsFalse(valid, "Assertion of negative case (over max) being false failed");
            Assert.AreEqual("The field Enrollment Status must be a string with a minimum length of 1 and a maximum length of 1.", failingValueOverMax.ErrorList[0].BusinessMessage);
        }

        //[TestMethod()]
        //public void GraduationDateTest_manageAccount_check_for_valid_date_ValidValue()
        //{
        //    ManageAccountModel passingValue = new ManageAccountModel() { GraduationDate = DateTime.Now.ToString() };

        //    ASAModelValidator mv = new ASAModelValidator();

        //    bool valid = mv.Validate(passingValue, "GraduationDate");
        //    Assert.IsTrue(valid, "Assertion of positive case being true failed");
        //}

        //[TestMethod()]
        //public void GraduationDateTest_manageAccount_check_for_valid_date_InvalidValue()
        //{
        //    ManageAccountModel passingValue = new ManageAccountModel() { GraduationDate = DateTime.Now.ToString() };
        //    ManageAccountModel invalidValue = new ManageAccountModel() { GraduationDate = "test" };

        //    ASAModelValidator mv = new ASAModelValidator();

        //    bool valid = mv.Validate(invalidValue, "GraduationDate");
        //    Assert.IsFalse(valid, "Assertion of negative case being false failed");
        //    Assert.AreEqual("Improper date input", invalidValue.ErrorList[0].BusinessMessage);
        //}

        [TestMethod()]
        public void PasswordTest_New_manageAccount_check_for_password_standards_ASA()
        {
            ManageAccountModel passingValue = new ManageAccountModel() { NewPassword = "passingValue1!" };
            
            ASAModelValidator mv = new ASAModelValidator();

            bool valid = mv.Validate(passingValue, "NewPassword");
            Assert.IsTrue(valid, "Assertion of positive case being true failed");
        }

        [TestMethod()]
        public void PasswordTest_New_manageAccount_check_for_string_length_ValidLowerBoundValue()
        {
            ManageAccountModel passingValueLowerBound = new ManageAccountModel() { NewPassword = "8Charact" };

            ASAModelValidator mv = new ASAModelValidator();

            bool valid = mv.Validate(passingValueLowerBound, "NewPassword");
            Assert.IsTrue(valid, "Assertion of positive case (lower bound) being true failed");
        }

        [TestMethod()]
        public void PasswordTest_New_manageAccount_check_for_string_length_ValidUpperBoundValue()
        {
            ManageAccountModel passingValueUpperBound = new ManageAccountModel() { NewPassword = "32CharactersCharactersCharacters" };

            ASAModelValidator mv = new ASAModelValidator();

            bool valid = mv.Validate(passingValueUpperBound, "NewPassword");
            Assert.IsTrue(valid, "Assertion of positive case (upper bound) being true failed");
        }

        [TestMethod()]
        public void PasswordTest_New_manageAccount_check_for_string_length_InvalidUnderMinValue()
        {
            ManageAccountModel failingValueUnderMin = new ManageAccountModel() { NewPassword = "7seven" };

            ASAModelValidator mv = new ASAModelValidator();

            bool valid = mv.Validate(failingValueUnderMin, "NewPassword");
            Assert.IsFalse(valid, "Assertion of negative case (under min) being false failed");
            Assert.AreEqual(2, failingValueUnderMin.ErrorList.Count, "Expected 2 errors");
            String verifyMessage = failingValueUnderMin.ErrorList[0].BusinessMessage + failingValueUnderMin.ErrorList[1].BusinessMessage;
            Assert.IsTrue(verifyMessage.Contains("Invalid Password"), "message should contain - Invalid Password");
            Assert.IsTrue(verifyMessage.Contains("The New Password must be between 8 and 32 characters long."), "message should contain - The New Password must be between 8 and 32 characters long.");
        }

        [TestMethod()]
        public void PasswordTest_New_manageAccount_check_for_string_length_InvalidOverMaxValue()
        {
            ManageAccountModel failingValueOverMax = new ManageAccountModel() { NewPassword = "33CharactersCharactersCharactersC" };

            ASAModelValidator mv = new ASAModelValidator();

            bool valid = mv.Validate(failingValueOverMax, "NewPassword");
            Assert.IsFalse(valid, "Assertion of negative case (over max) being false failed");
            Assert.AreEqual(2, failingValueOverMax.ErrorList.Count, "Expected 2 errors");
            String verifyMessage = failingValueOverMax.ErrorList[0].BusinessMessage + failingValueOverMax.ErrorList[1].BusinessMessage;
            Assert.IsTrue(verifyMessage.Contains("Invalid Password"), "message should contain - Invalid Password");
            Assert.IsTrue(verifyMessage.Contains("The New Password must be between 8 and 32 characters long."), "message should contain - The New Password must be between 8 and 32 characters long.");
        }

        [TestMethod()]
        public void PasswordTest_Confirm_manageAccount_check_for_password_standards_ASA()
        {
            ManageAccountModel passingValue = new ManageAccountModel() { ConfirmPassword = "passingValue1!" };

            ASAModelValidator mv = new ASAModelValidator();

            bool valid = mv.Validate(passingValue, "ConfirmPassword");
            Assert.IsTrue(valid, "Assertion of positive case (2 of 2 attributes) being true failed");
        }

        [TestMethod()]
        public void PasswordTest_Confirm_manageAccount_check_for_string_length_ValidLowerBoundValue()
        {
            ManageAccountModel passingValueLowerBound = new ManageAccountModel() { ConfirmPassword = "8Charact" };

            ASAModelValidator mv = new ASAModelValidator();

            bool valid = mv.Validate(passingValueLowerBound, "ConfirmPassword");
            Assert.IsTrue(valid, "Assertion of positive case (lower bound) being true failed");
        }

        [TestMethod()]
        public void PasswordTest_Confirm_manageAccount_check_for_string_length_ValidUpperBoundValue()
        {
            ManageAccountModel passingValueUpperBound = new ManageAccountModel() { ConfirmPassword = "32CharactersCharactersCharacters" };

            ASAModelValidator mv = new ASAModelValidator();

            bool valid = mv.Validate(passingValueUpperBound, "ConfirmPassword");
            Assert.IsTrue(valid, "Assertion of positive case (upper bound) being true failed");
        }

        [TestMethod()]
        public void PasswordTest_Confirm_manageAccount_check_for_string_length_InvalidUnderMinValue()
        {
            ManageAccountModel failingValueUnderMin = new ManageAccountModel() { ConfirmPassword = "7seven" };

            ASAModelValidator mv = new ASAModelValidator();

            bool valid = mv.Validate(failingValueUnderMin, "ConfirmPassword");
            Assert.IsFalse(valid, "Assertion of negative case (under min) being false failed");
            Assert.AreEqual(2, failingValueUnderMin.ErrorList.Count, "Expected 2 errors");
            String verifyMessage = failingValueUnderMin.ErrorList[0].BusinessMessage + failingValueUnderMin.ErrorList[1].BusinessMessage;
            Assert.IsTrue(verifyMessage.Contains("Invalid Password"), "message should contain - Invalid Password");
            Assert.IsTrue(verifyMessage.Contains("The Confirm Password must be between 8 and 32 characters long."), "message should contain - The Confirm Password must be between 8 and 32 characters long.");
        }

        [TestMethod()]
        public void PasswordTest_Confirm_manageAccount_check_for_string_length_InvalidOverMaxValue()
        {
            ManageAccountModel failingValueOverMax = new ManageAccountModel() { ConfirmPassword = "33CharactersCharactersCharactersC" };

            ASAModelValidator mv = new ASAModelValidator();

            bool valid = mv.Validate(failingValueOverMax, "ConfirmPassword");
            Assert.IsFalse(valid, "Assertion of negative case (over max) being false failed");
            Assert.AreEqual(2, failingValueOverMax.ErrorList.Count, "Expected 2 errors");
            String verifyMessage = failingValueOverMax.ErrorList[0].BusinessMessage + failingValueOverMax.ErrorList[1].BusinessMessage;
            Assert.IsTrue(verifyMessage.Contains("Invalid Password"), "message should contain - Invalid Password");
            Assert.IsTrue(verifyMessage.Contains("The Confirm Password must be between 8 and 32 characters long."), "message should contain - The Confirm Password must be between 8 and 32 characters long.");
        }

        [TestMethod()]
        public void PasswordTest_manageAccount_check_for_string_length_ValidLowerBoundValue()
        {
            ManageAccountModel passingValueLowerBound = new ManageAccountModel() { Password = "8Charact" };

            ASAModelValidator mv = new ASAModelValidator();

            bool valid = mv.Validate(passingValueLowerBound, "Password");
            Assert.IsTrue(valid, "Assertion of positive case (lower bound) being true failed");
        }

        [TestMethod()]
        public void PasswordTest_manageAccount_check_for_string_length_ValidUpperBoundValue()
        {
            ManageAccountModel passingValueUpperBound = new ManageAccountModel() { Password = "32CharactersCharactersCharacters" };

            ASAModelValidator mv = new ASAModelValidator();

            bool valid = mv.Validate(passingValueUpperBound, "Password");
            Assert.IsTrue(valid, "Assertion of positive case (upper bound) being true failed");
        }

        [TestMethod()]
        public void PasswordTest_manageAccount_check_for_string_length_InvalidUnderMinValue()
        {
            ManageAccountModel failingValueUnderMin = new ManageAccountModel() { Password = "7seven" };

            ASAModelValidator mv = new ASAModelValidator();

            bool valid = mv.Validate(failingValueUnderMin, "Password");
            Assert.IsFalse(valid, "Assertion of negative case (under min) being false failed");
            Assert.AreEqual(2, failingValueUnderMin.ErrorList.Count, "Expected 2 errors");
            String verifyMessage = failingValueUnderMin.ErrorList[0].BusinessMessage + failingValueUnderMin.ErrorList[1].BusinessMessage;
            Assert.IsTrue(verifyMessage.Contains("Invalid Password"), "message should contain - Invalid Password");
            Assert.IsTrue(verifyMessage.Contains("The Password must be between 8 and 32 characters long."), "message should contain - The Password must be between 8 and 32 characters long.");
        }

        [TestMethod()]
        public void PasswordTest_manageAccount_check_for_string_length_InvalidOverMaxValue()
        {
            ManageAccountModel failingValueOverMax = new ManageAccountModel() { Password = "33CharactersCharactersCharactersC" };

            ASAModelValidator mv = new ASAModelValidator();

            bool valid = mv.Validate(failingValueOverMax, "Password");
            Assert.IsFalse(valid, "Assertion of negative case (over max) being false failed");
            Assert.AreEqual(2, failingValueOverMax.ErrorList.Count, "Expected 2 errors");
            String verifyMessage = failingValueOverMax.ErrorList[0].BusinessMessage + failingValueOverMax.ErrorList[1].BusinessMessage;
            Assert.IsTrue(verifyMessage.Contains("Invalid Password"), "message should contain - Invalid Password");
            Assert.IsTrue(verifyMessage.Contains("The Password must be between 8 and 32 characters long."), "message should contain - The Password must be between 8 and 32 characters long.");
        }

        [TestMethod()]
        public void PasswordTest_manageAccount_check_for_password_standards_ASA()
        {
            ManageAccountModel passingValue = new ManageAccountModel() { Password = "passingValue1!" };

            ASAModelValidator mv = new ASAModelValidator();

            bool valid = mv.Validate(passingValue, "Password");
            Assert.IsTrue(valid, "Assertion of positive case (1 of 4 attributes) being true failed");
        }

        [TestMethod()]
        public void UserName_manageAccount_check_for_string_length_ValidLowerBoundValue()
        {
            ManageAccountModel passingValueLowerBound = new ManageAccountModel() { UserName = "xy@z.com" };

            ASAModelValidator mv = new ASAModelValidator();

            bool valid = mv.Validate(passingValueLowerBound, "UserName");
            Assert.IsTrue(valid, "Assertion of positive case (lower bound) being true failed");
        }

        [TestMethod()]
        public void UserName_manageAccount_check_for_string_length_ValidUpperBoundValue()
        {
            ManageAccountModel passingValueUpperBound = new ManageAccountModel() { UserName = "64CharactersCharactersCharactersCharactersCharactersChar.y@z.com" };

            ASAModelValidator mv = new ASAModelValidator();

            bool valid = mv.Validate(passingValueUpperBound, "UserName");
            Assert.IsTrue(valid, "Assertion of positive case (upper bound) being true failed");
        }

        [TestMethod()]
        public void UserName_manageAccount_check_for_string_length_InvalidUnderMinValue()
        {
            ManageAccountModel failingValueUnderMin = new ManageAccountModel() { UserName = "x@z.ru" };

            ASAModelValidator mv = new ASAModelValidator();

            bool valid = mv.Validate(failingValueUnderMin, "UserName");
            Assert.IsFalse(valid, "Assertion of negative case (under min) being false failed");
            Assert.AreEqual("The UserName must be between 8 and 64 characters long.", failingValueUnderMin.ErrorList[0].BusinessMessage);
        }

        [TestMethod()]
        public void UserName_manageAccount_check_for_string_length_InvalidOverMaxValue()
        {
            ManageAccountModel failingValueOverMax = new ManageAccountModel() { UserName = "65CharactersCharactersCharactersCharactersCharactersChara.y@z.com" };

            ASAModelValidator mv = new ASAModelValidator();

            bool valid = mv.Validate(failingValueOverMax, "UserName");
            Assert.IsFalse(valid, "Assertion of negative case (over max) being false failed");
            Assert.AreEqual(2, failingValueOverMax.ErrorList.Count, "Expected 2 errors");
            String verifyMessage = failingValueOverMax.ErrorList[0].BusinessMessage + failingValueOverMax.ErrorList[1].BusinessMessage;
            Assert.IsTrue(verifyMessage.Contains("Please enter a valid user name."), "message should contain - Please enter a valid user name.");
            Assert.IsTrue(verifyMessage.Contains("The UserName must be between 8 and 64 characters long."), "message should contain - The UserName must be between 8 and 64 characters long.");
        }

        [TestMethod()]
        public void UserName_manageAccountModel_check_for_valid_username_ValidValue()
        {
            ManageAccountModel passingValue = new ManageAccountModel() { UserName = "person@yahoo.com" };

            ASAModelValidator mv = new ASAModelValidator();

            bool valid = mv.Validate(passingValue, "UserName");
            Assert.IsTrue(valid, "Assertion of positive case being true failed");
        }

        [TestMethod()]
        public void UserName_manageAccountModel_check_for_valid_username_InvalidValue()
        {
            ManageAccountModel invalidValue = new ManageAccountModel() { UserName = "failString" };

            ASAModelValidator mv = new ASAModelValidator();

            bool valid = mv.Validate(invalidValue, "UserName");
            Assert.IsFalse(valid, "Assertion of negative case being false failed");
            Assert.AreEqual("Please enter a valid user name.", invalidValue.ErrorList[0].BusinessMessage);
        }

        [TestMethod()]
        public void PhoneNumberTest_manageAccount_check_for_regex()
        {
            ASAModelValidator mv = new ASAModelValidator();

            ManageAccountModel passingValueWithHyphens = new ManageAccountModel() { PhoneNumber = "617-555-4545" };
            bool valid = mv.Validate(passingValueWithHyphens, "PhoneNumber");
            Assert.IsTrue(valid, "Positive assertion for (passingValueWithHyphens) Failed");

            ManageAccountModel passingValueWithLeadingOne = new ManageAccountModel() { PhoneNumber = "1-617-555-4545" };
            valid = mv.Validate(passingValueWithLeadingOne, "PhoneNumber");
            Assert.IsTrue(valid, "Positive assertion for (passingValueWithLeadingOne) Failed");

            ManageAccountModel passingValueNoHyphens = new ManageAccountModel() { PhoneNumber = "6175554545" };
            valid = mv.Validate(passingValueNoHyphens, "PhoneNumber");
            Assert.IsTrue(valid, "Positive assertion for (passingValueNoHyphens) Failed");

            ManageAccountModel passingValueWithLeadingSpace = new ManageAccountModel() { PhoneNumber = " 617-555-4545" };
            valid = mv.Validate(passingValueWithLeadingSpace, "PhoneNumber");
            Assert.IsTrue(valid, "Positive assertion for (passingValueWithLeadingSpace) Failed");

            ManageAccountModel passingValueWithMultipleLeadingSpaces = new ManageAccountModel() { PhoneNumber = "    617-555-4545" };
            valid = mv.Validate(passingValueWithMultipleLeadingSpaces, "PhoneNumber");
            Assert.IsTrue(valid, "Positive assertion for (passingValueWithMultipleLeadingSpaces) Failed");

            ManageAccountModel passingValueWithParenth = new ManageAccountModel() { PhoneNumber = "(617)-555-4545" };
            valid = mv.Validate(passingValueWithParenth, "PhoneNumber");
            Assert.IsTrue(valid, "Positive assertion for (passingValueWithParenth) Failed");

            ManageAccountModel passingValueWithSpaceAfterAreaCode = new ManageAccountModel() { PhoneNumber = "(617) 555-4545" };
            valid = mv.Validate(passingValueWithSpaceAfterAreaCode, "PhoneNumber");
            Assert.IsTrue(valid, "Positive assertion for (passingValueWithSpaceAfterAreaCode) Failed");

            ManageAccountModel failingValueTooManyDigits = new ManageAccountModel() { PhoneNumber = "617-123-45678" };
            valid = mv.Validate(failingValueTooManyDigits, "PhoneNumber");
            Assert.IsFalse(valid, "Negative assertion for (failingValuetTooManyDigits) Failed");

            ManageAccountModel failingValueTooFewDigits = new ManageAccountModel() { PhoneNumber = "617-123-456" };
            valid = mv.Validate(failingValueTooFewDigits, "PhoneNumber");
            Assert.IsFalse(valid, "Negative assertion for (failingValuetTooFewDigits) Failed");

            ManageAccountModel failingValueSpecialCharacter = new ManageAccountModel() { PhoneNumber = "617-123-456!" };
            valid = mv.Validate(failingValueSpecialCharacter, "PhoneNumber");
            Assert.IsFalse(valid, "Negative assertion for (failingValueSpecialCharacter) Failed");

            ManageAccountModel failingValueTooManyMiddleDigits = new ManageAccountModel() { PhoneNumber = "617-1234-4567" };
            valid = mv.Validate(failingValueTooManyMiddleDigits, "PhoneNumber");
            Assert.IsFalse(valid, "Negative assertion for (failingValueTooManyMiddleDigits) Failed");
        }
    }
}
