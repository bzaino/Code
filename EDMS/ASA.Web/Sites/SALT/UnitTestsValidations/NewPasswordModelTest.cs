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
    ///This is a test class for NewPasswordModelTest and is intended
    ///to contain all NewPasswordModelTest Unit Tests
    ///</summary>
    [TestClass()]
    public class NewPasswordModelTest
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
        public void PasswordTest_Confirm_newpassword_check_for_required_ValidValue()
        {
            NewPasswordModel passingValue = new NewPasswordModel() { ConfirmPassword = "passingPW!" };

            List<ErrorModel> errorList = new List<ErrorModel>();
            ASAModelValidator mv = new ASAModelValidator();

            bool valid = mv.Validate(passingValue, "ConfirmPassword", errorList);
            Assert.IsTrue(valid, "Assertion of positive case being true failed");
        }


        [TestMethod()]
        public void PasswordTest_Confirm_newpassword_check_for_required_InvalidValue()
        {
            NewPasswordModel invalidValue = new NewPasswordModel() { };

            List<ErrorModel> errorList = new List<ErrorModel>();
            ASAModelValidator mv = new ASAModelValidator();

            bool valid = mv.Validate(invalidValue, "ConfirmPassword", errorList);
            Assert.IsFalse(valid, "Assertion of negative case being false failed");
            Assert.AreEqual("Confirm Password is required!", errorList[0].BusinessMessage);
        }

        [TestMethod()]
        public void PasswordTest_Confirm_newpassword_check_for_string_length_ValidLowerBoundValue()
        {
            NewPasswordModel passingValueLowerBound = new NewPasswordModel() { ConfirmPassword = "8charact" };

            List<ErrorModel> errorList = new List<ErrorModel>();
            ASAModelValidator mv = new ASAModelValidator();

            bool valid = mv.Validate(passingValueLowerBound, "ConfirmPassword", errorList);
            Assert.IsTrue(valid, "Assertion of positive case (lower bound) being true failed");
        }

        [TestMethod()]
        public void PasswordTest_Confirm_newpassword_check_for_string_length_ValidUpperBoundValue()
        {
            NewPasswordModel passingValueUpperBound = new NewPasswordModel() { ConfirmPassword = "32CharactersCharactersCharacters" };

            List<ErrorModel> errorList = new List<ErrorModel>();
            ASAModelValidator mv = new ASAModelValidator();

            bool valid = mv.Validate(passingValueUpperBound, "ConfirmPassword", errorList);
            Assert.IsTrue(valid, "Assertion of positive case (upper bound) being true failed");
        }

        [TestMethod()]
        public void PasswordTest_Confirm_newpassword_check_for_string_length_InvalidUnderMinValue()
        {
            NewPasswordModel failingValueUnderMin = new NewPasswordModel() { ConfirmPassword = "7charac" };

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
        public void PasswordTest_Confirm_newpassword_check_for_string_length_InvalidOverMaxValue()
        {
            NewPasswordModel failingValueOverMax = new NewPasswordModel() { ConfirmPassword = "33CharactersCharactersCharactersC" };

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
        public void PasswordTest_New_newpassword_check_for_required_ValidValue()
        {
            NewPasswordModel passingValue = new NewPasswordModel() { NewPassword = "passingPW!" };

            List<ErrorModel> errorList = new List<ErrorModel>();
            ASAModelValidator mv = new ASAModelValidator();

            bool valid = mv.Validate(passingValue, "NewPassword", errorList);
            Assert.IsTrue(valid, "Assertion of positive case being true failed");
        }

        [TestMethod()]
        public void PasswordTest_New_newpassword_check_for_required_InvalidValue()
        {
            NewPasswordModel invalidValue = new NewPasswordModel() { };

            List<ErrorModel> errorList = new List<ErrorModel>();
            ASAModelValidator mv = new ASAModelValidator();

            bool valid = mv.Validate(invalidValue, "NewPassword", errorList);
            Assert.IsFalse(valid, "Assertion of negative case being false failed");
            Assert.AreEqual("Password is required!", errorList[0].BusinessMessage);
        }

        [TestMethod()]
        public void PasswordTest_New_newpassword_check_for_string_length_ValidLowerBoundValue()
        {
            NewPasswordModel passingValueLowerBound = new NewPasswordModel() { NewPassword = "8Charact" };

            List<ErrorModel> errorList = new List<ErrorModel>();
            ASAModelValidator mv = new ASAModelValidator();

            bool valid = mv.Validate(passingValueLowerBound, "NewPassword", errorList);
            Assert.IsTrue(valid, "Assertion of positive case (lower bound) being true failed");
        }

        [TestMethod()]
        public void PasswordTest_New_newpassword_check_for_string_length_ValidUpperBoundValue()
        {
            NewPasswordModel passingValueUpperBound = new NewPasswordModel() { NewPassword = "32CharactersCharactersCharacters" };

            List<ErrorModel> errorList = new List<ErrorModel>();
            ASAModelValidator mv = new ASAModelValidator();

            bool valid = mv.Validate(passingValueUpperBound, "NewPassword", errorList);
            Assert.IsTrue(valid, "Assertion of positive case (upper bound) being true failed");
        }

        [TestMethod()]
        public void PasswordTest_New_newpassword_check_for_string_length_InvalidUnderMinValue()
        {
            NewPasswordModel failingValueUnderMin = new NewPasswordModel() { NewPassword = "7seven" };

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
        public void PasswordTest_New_newpassword_check_for_string_length_InvalidOverMaxValue()
        {
            NewPasswordModel failingValueOverMax = new NewPasswordModel() { NewPassword = "33CharactersCharactersCharactersC" };

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
        public void PasswordTest_New_newpassword_check_for_password_standards_ASA()
        {
            NewPasswordModel passingValue = new NewPasswordModel() { NewPassword = "passingValue1!" };

            List<ErrorModel> errorList = new List<ErrorModel>();
            ASAModelValidator mv = new ASAModelValidator();

            bool valid = mv.Validate(passingValue, "NewPassword");
            Assert.IsTrue(valid, "Assertion of positive case being true failed");
        }
    }
}
