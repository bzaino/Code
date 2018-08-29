using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

using ASA.Web.Services.Common;
using ASA.Web.Common.Validation;
using ASA.Web.Sites.SALT.Models;

namespace UnitTestsValidations
{
    
    
    /// <summary>
    ///This is a test class for ChangePasswordModelTest and is intended
    ///to contain all ChangePasswordModelTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ChangePasswordModelTest
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

        #region Additional test attributes
        #endregion

        [TestMethod()]
        public void PasswordTest_New_changePassword_check_for_required_ValidValue()
        {
            ChangePasswordModel passingValue = new ChangePasswordModel() { NewPassword = "passingPW!" };

            List<ErrorModel> errorList = new List<ErrorModel>();
            ASAModelValidator mv = new ASAModelValidator();

            bool valid = mv.Validate(passingValue, "NewPassword", errorList);
            Assert.IsTrue(valid, "Assertion of positive case being true failed");
        }

        [TestMethod()]
        public void PasswordTest_New_changePassword_check_for_required_InvalidValue()
        {
            ChangePasswordModel invalidValue = new ChangePasswordModel() { };

            List<ErrorModel> errorList = new List<ErrorModel>();
            ASAModelValidator mv = new ASAModelValidator();

            bool valid = mv.Validate(invalidValue, "NewPassword", errorList);
            Assert.IsFalse(valid, "Assertion of negative case being false failed");
            Assert.AreEqual("Password is required!", errorList[0].BusinessMessage);
        }

        [TestMethod()]
        public void PasswordTest_New_changePassword_check_for_string_length_ValidLowerValue()
        {
            ChangePasswordModel passingValueLowerBound = new ChangePasswordModel() { NewPassword = "8charact" };

            List<ErrorModel> errorList = new List<ErrorModel>();
            ASAModelValidator mv = new ASAModelValidator();

            bool valid = mv.Validate(passingValueLowerBound, "NewPassword", errorList);
            Assert.IsTrue(valid, "Assertion of positive case (lower bound) being true failed");
        }

        [TestMethod()]
        public void PasswordTest_New_changePassword_check_for_string_length_ValidUpperBoundValue()
        {
            ChangePasswordModel passingValueUpperBound = new ChangePasswordModel() { NewPassword = "32CharactersCharactersCharacters" };

            List<ErrorModel> errorList = new List<ErrorModel>();
            ASAModelValidator mv = new ASAModelValidator();

            bool valid = mv.Validate(passingValueUpperBound, "NewPassword", errorList);
            Assert.IsTrue(valid, "Assertion of positive case (upper bound) being true failed");
        }

        [TestMethod()]
        public void PasswordTest_New_changePassword_check_for_string_length_InvalidUnderMinValue()
        {
            ChangePasswordModel failingValueUnderMin = new ChangePasswordModel() { NewPassword = "7charac" };

            List<ErrorModel> errorList = new List<ErrorModel>();
            ASAModelValidator mv = new ASAModelValidator();

            bool valid = mv.Validate(failingValueUnderMin, "NewPassword", errorList);
            Assert.IsFalse(valid, "Assertion of negative case (under min) being false failed");
            Assert.AreEqual(2, errorList.Count, "Expected 2 errors");
            String verifyMessage = errorList[0].BusinessMessage + errorList[1].BusinessMessage;
            Assert.IsTrue(verifyMessage.Contains("Invalid Password"), "message should contain - Invalid Password");
            Assert.IsTrue(verifyMessage.Contains("The New password must be between 8 and 32 characters long."), "message should contain - The New password must be between 8 and 32 characters long.");
        }

        [TestMethod()]
        public void PasswordTest_New_changePassword_check_for_string_lengthInvalidOverMaxValue()
        {
            ChangePasswordModel failingValueOverMax = new ChangePasswordModel() { NewPassword = "33CharactersCharactersCharactersC" };

            List<ErrorModel> errorList = new List<ErrorModel>();
            ASAModelValidator mv = new ASAModelValidator();

            bool valid = mv.Validate(failingValueOverMax, "NewPassword", errorList);
            Assert.IsFalse(valid, "Assertion of negative case (over max) being false failed");
            Assert.AreEqual(2, errorList.Count, "Expected 2 errors");
            String verifyMessage = errorList[0].BusinessMessage + errorList[1].BusinessMessage;
            Assert.IsTrue(verifyMessage.Contains("Invalid Password"), "message should contain - Invalid Password");
            Assert.IsTrue(verifyMessage.Contains("The New password must be between 8 and 32 characters long."), "message should contain - The New password must be between 8 and 32 characters long.");
        }

        [TestMethod()]
        public void PasswordTest_New_changePassword_check_for_password_standards_ASA()
        {
            ChangePasswordModel passingValue = new ChangePasswordModel() { NewPassword = "passingValue1!" };

            List<ErrorModel> errorList = new List<ErrorModel>();
            ASAModelValidator mv = new ASAModelValidator();

            bool valid = mv.Validate(passingValue, "NewPassword");
            Assert.IsTrue(valid, "Assertion of positive case being true failed");
        }

        [TestMethod()]
        public void PasswordTest_Old_changePassword_check_for_required_ValidValue()
        {
            ChangePasswordModel passingValue = new ChangePasswordModel() { OldPassword = "passingPW!" };

            List<ErrorModel> errorList = new List<ErrorModel>();
            ASAModelValidator mv = new ASAModelValidator();

            bool valid = mv.Validate(passingValue, "OldPassword", errorList);
            Assert.IsTrue(valid, "Assertion of positive case being true failed");
        }

        [TestMethod()]
        public void PasswordTest_Old_changePassword_check_for_required_InvalidValue()
        {
            ChangePasswordModel invalidValue = new ChangePasswordModel() { };

            List<ErrorModel> errorList = new List<ErrorModel>();
            ASAModelValidator mv = new ASAModelValidator();

            bool valid = mv.Validate(invalidValue, "OldPassword", errorList);
            Assert.IsFalse(valid, "Assertion of negative case being false failed");
            Assert.AreEqual("Password is required!", errorList[0].BusinessMessage);
        }
        
        [TestMethod()]
        public void PasswordTest_Old_changePassword_check_for_string_length_ValidLowerBoundValue()
        {
            ChangePasswordModel passingValueLowerBound = new ChangePasswordModel() { OldPassword = "8charact" };

            List<ErrorModel> errorList = new List<ErrorModel>();
            ASAModelValidator mv = new ASAModelValidator();

            bool valid = mv.Validate(passingValueLowerBound, "OldPassword", errorList);
            Assert.IsTrue(valid, "Assertion of positive case (lower bound) being true failed");
        }

        [TestMethod()]
        public void PasswordTest_Old_changePassword_check_for_string_length_ValidUpperBoundValue()
        {
            ChangePasswordModel passingValueUpperBound = new ChangePasswordModel() { OldPassword = "32CharactersCharactersCharacters" };

            List<ErrorModel> errorList = new List<ErrorModel>();
            ASAModelValidator mv = new ASAModelValidator();

            bool valid = mv.Validate(passingValueUpperBound, "OldPassword", errorList);
            Assert.IsTrue(valid, "Assertion of positive case (upper bound) being true failed");
        }

        [TestMethod()]
        public void PasswordTest_Old_changePassword_check_for_string_length_InvalidUnderMinValue()
        {
            ChangePasswordModel failingValueUnderMin = new ChangePasswordModel() { OldPassword = "7charac" };

            List<ErrorModel> errorList = new List<ErrorModel>();
            ASAModelValidator mv = new ASAModelValidator();

            bool valid = mv.Validate(failingValueUnderMin, "OldPassword", errorList);
            Assert.IsFalse(valid, "Assertion of negative case (under min) being false failed");
            Assert.AreEqual(2, errorList.Count, "Expected 2 errors");
            String verifyMessage = errorList[0].BusinessMessage + errorList[1].BusinessMessage;
            Assert.IsTrue(verifyMessage.Contains("Invalid Password"), "message should contain - Invalid Password");
            Assert.IsTrue(verifyMessage.Contains("The Current password must be between 8 and 32 characters long."), "message should contain - The Current password must be between 8 and 32 characters long.");
        }

        [TestMethod()]
        public void PasswordTest_Old_changePassword_check_for_string_length_InvalidOverMaxValue()
        {
            ChangePasswordModel failingValueOverMax = new ChangePasswordModel() { OldPassword = "33CharactersCharactersCharactersC" };

            List<ErrorModel> errorList = new List<ErrorModel>();
            ASAModelValidator mv = new ASAModelValidator();

            bool valid = mv.Validate(failingValueOverMax, "OldPassword", errorList);
            Assert.IsFalse(valid, "Assertion of negative case (over max) being false failed");
            Assert.AreEqual(2, errorList.Count, "Expected 2 errors");
            String verifyMessage = errorList[0].BusinessMessage + errorList[1].BusinessMessage;
            Assert.IsTrue(verifyMessage.Contains("Invalid Password"), "message should contain - Invalid Password");
            Assert.IsTrue(verifyMessage.Contains("The Current password must be between 8 and 32 characters long."), "message should contain - The Current password must be between 8 and 32 characters long.");
        }

        [TestMethod()]
        public void PasswordTest_Old_changePassword_check_for_password_standards_ASA()
        {
            ChangePasswordModel passingValue = new ChangePasswordModel() { OldPassword = "passingValue1!" };

            List<ErrorModel> errorList = new List<ErrorModel>();
            ASAModelValidator mv = new ASAModelValidator();

            bool valid = mv.Validate(passingValue, "OldPassword");
            Assert.IsTrue(valid, "Assertion of positive case being true failed");
        }

        [TestMethod()]
        public void PasswordTest_Confirm_changePassword_check_for_string_length_ValidLowerBoundValue()
        {
            ChangePasswordModel passingValueLowerBound = new ChangePasswordModel() { ConfirmPassword = "8charact" };

            List<ErrorModel> errorList = new List<ErrorModel>();
            ASAModelValidator mv = new ASAModelValidator();

            bool valid = mv.Validate(passingValueLowerBound, "ConfirmPassword", errorList);
            Assert.IsTrue(valid, "Assertion of positive case (lower bound) being true failed");
        }

        [TestMethod()]
        public void PasswordTest_Confirm_changePassword_check_for_string_length_ValidUpperBoundValue()
        {
            ChangePasswordModel passingValueUpperBound = new ChangePasswordModel() { ConfirmPassword = "32CharactersCharactersCharacters" };

            List<ErrorModel> errorList = new List<ErrorModel>();
            ASAModelValidator mv = new ASAModelValidator();

            bool valid = mv.Validate(passingValueUpperBound, "ConfirmPassword", errorList);
            Assert.IsTrue(valid, "Assertion of positive case (upper bound) being true failed");
        }

        [TestMethod()]
        public void PasswordTest_Confirm_changePassword_check_for_string_length_InvalidUnderMinValue()
        {
            ChangePasswordModel failingValueUnderMin = new ChangePasswordModel() { ConfirmPassword = "7charac" };

            List<ErrorModel> errorList = new List<ErrorModel>();
            ASAModelValidator mv = new ASAModelValidator();

            bool valid = mv.Validate(failingValueUnderMin, "ConfirmPassword", errorList);
            Assert.IsFalse(valid, "Assertion of negative case (under min) being false failed");
            Assert.AreEqual(2, errorList.Count, "Expected 2 errors");
            String verifyMessage = errorList[0].BusinessMessage + errorList[1].BusinessMessage;
            Assert.IsTrue(verifyMessage.Contains("Invalid Password"), "message should contain - Invalid Password");
            Assert.IsTrue(verifyMessage.Contains("The Confirm new password must be between 8 and 32 characters long."), "message should contain - The Confirm new password must be between 8 and 32 characters long.");
        }

        [TestMethod()]
        public void PasswordTest_Confirm_changePassword_check_for_string_length_InvalidOverMaxValue()
        {
            ChangePasswordModel failingValueOverMax = new ChangePasswordModel() { ConfirmPassword = "33CharactersCharactersCharactersC" };

            List<ErrorModel> errorList = new List<ErrorModel>();
            ASAModelValidator mv = new ASAModelValidator();

            bool valid = mv.Validate(failingValueOverMax, "ConfirmPassword", errorList);
            Assert.IsFalse(valid, "Assertion of negative case (over max) being false failed");
            Assert.AreEqual(2, errorList.Count, "Expected 2 errors");
            String verifyMessage = errorList[0].BusinessMessage + errorList[1].BusinessMessage;
            Assert.IsTrue(verifyMessage.Contains("Invalid Password"), "message should contain - Invalid Password");
            Assert.IsTrue(verifyMessage.Contains("The Confirm new password must be between 8 and 32 characters long."), "message should contain - The Confirm new password must be between 8 and 32 characters long.");
        }

        [TestMethod()]
        public void PasswordTest_Confirm_changePassword_check_for_password_standards_ASA()
        {
            ChangePasswordModel passingValue = new ChangePasswordModel() { ConfirmPassword = "passingValue1!" };

            List<ErrorModel> errorList = new List<ErrorModel>();
            ASAModelValidator mv = new ASAModelValidator();

            bool valid = mv.Validate(passingValue, "Confirm");
            Assert.IsTrue(valid, "Assertion of positive case being true failed");
        }
    }
}
