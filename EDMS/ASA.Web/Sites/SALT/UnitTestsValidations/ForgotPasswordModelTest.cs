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
    ///This is a test class for ForgotPasswordModelTest and is intended
    ///to contain all ForgotPasswordModelTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ForgotPasswordModelTest
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
        public void EmailTest_forgotpassword_check_for_string_length_ValidLowerBoundValue()
        {
            ForgotPasswordModel passingValueLowerBound = new ForgotPasswordModel() { Email = "xy@z.com" };

            List<ErrorModel> errorList = new List<ErrorModel>();
            ASAModelValidator mv = new ASAModelValidator();

            bool valid = mv.Validate(passingValueLowerBound, "Email", errorList);
            Assert.IsTrue(valid, "Assertion of positive case (lower bound) being true failed");
        }

        [TestMethod()]
        public void EmailTest_forgotpassword_check_for_string_length_ValidUpperBoundValue()
        {
            ForgotPasswordModel passingValueUpperBound = new ForgotPasswordModel() { Email = "64CharactersCharactersCharactersCharactersCharactersChar.y@z.com" };

            List<ErrorModel> errorList = new List<ErrorModel>();
            ASAModelValidator mv = new ASAModelValidator();

            bool valid = mv.Validate(passingValueUpperBound, "Email", errorList);
            Assert.IsTrue(valid, "Assertion of positive case (upper bound) being true failed");
        }

        [TestMethod()]
        public void EmailTest_forgotpassword_check_for_string_length_InvalidUnderMinValue()
        {
            ForgotPasswordModel failingValueUnderMin = new ForgotPasswordModel() { Email = "x@z.ru" };

            List<ErrorModel> errorList = new List<ErrorModel>();
            ASAModelValidator mv = new ASAModelValidator();

            bool valid = mv.Validate(failingValueUnderMin, "Email", errorList);
            Assert.IsFalse(valid, "Assertion of negative case (under min) being false failed");
            Assert.AreEqual("The Email Address must be between 8 and 64 characters long.", errorList[0].BusinessMessage);
        }

        [TestMethod()]
        public void EmailTest_forgotpassword_check_for_string_length_InvalidOverMaxValue()
        {
            ForgotPasswordModel failingValueOverMax = new ForgotPasswordModel() { Email = "65CharactersCharactersCharactersCharactersCharactersChara.y@z.com" };

            List<ErrorModel> errorList = new List<ErrorModel>();
            ASAModelValidator mv = new ASAModelValidator();

            bool valid = mv.Validate(failingValueOverMax, "Email", errorList);
            Assert.IsFalse(valid, "Assertion of negative case (over max) being false failed");
            Assert.AreEqual(2, errorList.Count, "Expected 2 errors");
            String verifyMessage = errorList[0].BusinessMessage + errorList[1].BusinessMessage;
            Assert.IsTrue(verifyMessage.Contains("Please enter a valid email."), "message should contain - Please enter a valid email.");
            Assert.IsTrue(verifyMessage.Contains("The Email Address must be between 8 and 64 characters long."), "message should contain - The Email Address must be between 8 and 64 characters long.");
        }

        [TestMethod()]
        public void EmailTest_forgotpassword_check_for_valid_email_Valid()
        {
            ForgotPasswordModel passingValue = new ForgotPasswordModel() { Email = "person@yahoo.com" };

            List<ErrorModel> errorList = new List<ErrorModel>();
            ASAModelValidator mv = new ASAModelValidator();

            bool valid = mv.Validate(passingValue, "Email");
            Assert.IsTrue(valid, "Assertion of positive case being true failed");
        }

        [TestMethod()]
        public void EmailTest_forgotpassword_check_for_valid_email_Invalid()
        {
            ForgotPasswordModel failingValue = new ForgotPasswordModel() { Email = "failString" };

            List<ErrorModel> errorList = new List<ErrorModel>();
            ASAModelValidator mv = new ASAModelValidator();

            bool valid = mv.Validate(failingValue, "Email", errorList);
            Assert.IsFalse(valid, "Assertion of negative case being false failed");
            Assert.AreEqual("Please enter a valid email.", errorList[0].BusinessMessage);
        }
    }
}
