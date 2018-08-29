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
    ///This is a test class for LogOnModelTest and is intended
    ///to contain all LogOnModelTest Unit Tests
    ///</summary>
    [TestClass()]
    public class LogOnModelTest
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
        public void PasswordTest_logon_check_for_required_ValidValue()
        {
            LogOnModel passingValue = new LogOnModel() { Password = "passingPW!" };

            List<ErrorModel> errorList = new List<ErrorModel>();
            ASAModelValidator mv = new ASAModelValidator();

            bool valid = mv.Validate(passingValue, "Password", errorList);
            Assert.IsTrue(valid, "Assertion of positive case being true failed");
        }

        [TestMethod()]
        public void PasswordTest_logon_check_for_required_InvalidValue()
        {
            LogOnModel invalidValue = new LogOnModel() { };

            List<ErrorModel> errorList = new List<ErrorModel>();
            ASAModelValidator mv = new ASAModelValidator();

            bool valid = mv.Validate(invalidValue, "Password", errorList);
            Assert.IsFalse(valid, "Assertion of negative case being false failed");
            Assert.AreEqual("Password is required!", errorList[0].BusinessMessage);
        }

        [TestMethod()]
        public void PasswordTest_logon_check_for_string_length_ValidLowerBoundValue()
        {
            LogOnModel passingValueLowerBound = new LogOnModel() { Password = "8charact" };

            List<ErrorModel> errorList = new List<ErrorModel>();
            ASAModelValidator mv = new ASAModelValidator();

            bool valid = mv.Validate(passingValueLowerBound, "Password", errorList);
            Assert.IsTrue(valid, "Assertion of positive case (lower bound) being true failed");
        }

        [TestMethod()]
        public void PasswordTest_logon_check_for_string_length_ValidUpperBoundValue()
        {
            LogOnModel passingValueUpperBound = new LogOnModel() { Password = "32CharactersCharactersCharacters" };

            List<ErrorModel> errorList = new List<ErrorModel>();
            ASAModelValidator mv = new ASAModelValidator();

            bool valid = mv.Validate(passingValueUpperBound, "Password", errorList);
            Assert.IsTrue(valid, "Assertion of positive case (upper bound) being true failed");
        }

        [TestMethod()]
        public void PasswordTest_logon_check_for_string_length_InvalidUnderMinValue()
        {
            LogOnModel failingValueUnderMin = new LogOnModel() { Password = "7charac" };

            List<ErrorModel> errorList = new List<ErrorModel>();
            ASAModelValidator mv = new ASAModelValidator();

            bool valid = mv.Validate(failingValueUnderMin, "Password", errorList);
            Assert.IsFalse(valid, "Assertion of negative case (under min) being false failed");
            Assert.AreEqual("The Password must be between 8 and 32 characters long.", errorList[0].BusinessMessage);
        }

        [TestMethod()]
        public void PasswordTest_logon_check_for_string_length_InvalidUpperMaxValue()
        {
            LogOnModel failingValueOverMax = new LogOnModel() { Password = "33CharactersCharactersCharactersC" };

            List<ErrorModel> errorList = new List<ErrorModel>();
            ASAModelValidator mv = new ASAModelValidator();

            bool valid = mv.Validate(failingValueOverMax, "Password", errorList);
            Assert.IsFalse(valid, "Assertion of negative case (over max) being false failed");
            Assert.AreEqual("The Password must be between 8 and 32 characters long.", errorList[0].BusinessMessage);
        }

        [TestMethod()]
        public void PasswordTest_logon_check_for_password_standards_ASA()
        {
            LogOnModel passingValue = new LogOnModel() { Password = "passingValue1!" };

            List<ErrorModel> errorList = new List<ErrorModel>();
            ASAModelValidator mv = new ASAModelValidator();

            bool valid = mv.Validate(passingValue, "Password");
            Assert.IsTrue(valid, "Assertion of positive case being true failed");
        }
     
        [TestMethod()]
        public void UserName_logon_check_for_required_ValidValue()
        {
            LogOnModel passingValue = new LogOnModel() { UserName = "validEmalAddress@asa.org" };

            List<ErrorModel> errorList = new List<ErrorModel>();
            ASAModelValidator mv = new ASAModelValidator();

            bool valid = mv.Validate(passingValue, "UserName", errorList);
            Assert.IsTrue(valid, "Assertion of positive case being true failed");
        }

        [TestMethod()]
        public void UserName_logon_check_for_required_InvalidValue()
        {
            LogOnModel invalidValue = new LogOnModel() { };

            List<ErrorModel> errorList = new List<ErrorModel>();
            ASAModelValidator mv = new ASAModelValidator();

            bool valid = mv.Validate(invalidValue, "UserName", errorList);
            Assert.IsFalse(valid, "Assertion of negative case being false failed");
            Assert.AreEqual("User name is required!", errorList[0].BusinessMessage);
        }

        [TestMethod()]
        public void UserName_logon_check_for_string_length_ValidLowerBoundValue()
        {
            LogOnModel passingValueLowerBound = new LogOnModel() { UserName = "xy@z.com" };

            List<ErrorModel> errorList = new List<ErrorModel>();
            ASAModelValidator mv = new ASAModelValidator();

            bool valid = mv.Validate(passingValueLowerBound, "UserName", errorList);
            Assert.IsTrue(valid, "Assertion of positive case (lower bound) being true failed");
        }

        [TestMethod()]
        public void UserName_logon_check_for_string_length_ValidUpperBoundValue()
        {
            LogOnModel passingValueUpperBound = new LogOnModel() { UserName = "64CharactersCharactersCharactersCharactersCharactersChar.y@z.com" };

            List<ErrorModel> errorList = new List<ErrorModel>();
            ASAModelValidator mv = new ASAModelValidator();

            bool valid = mv.Validate(passingValueUpperBound, "UserName", errorList);
            Assert.IsTrue(valid, "Assertion of positive case (upper bound) being true failed");
        }

        [TestMethod()]
        public void UserName_logon_check_for_string_length_InvalidUnderMinValue()
        {
            LogOnModel failingValueUnderMin = new LogOnModel() { UserName = "x@z.ru" };

            List<ErrorModel> errorList = new List<ErrorModel>();
            ASAModelValidator mv = new ASAModelValidator();

            bool valid = mv.Validate(failingValueUnderMin, "UserName", errorList);
            Assert.IsFalse(valid, "Assertion of negative case (under min) being false failed");
            Assert.AreEqual("The User name must be between 8 and 64 characters long.", errorList[0].BusinessMessage);
        }

        [TestMethod()]
        public void UserName_logon_check_for_string_length_InvalidOverMaxValue()
        {
            LogOnModel failingValueOverMax = new LogOnModel() { UserName = "65CharactersCharactersCharactersCharactersCharactersChara.y@z.com" };

            List<ErrorModel> errorList = new List<ErrorModel>();
            ASAModelValidator mv = new ASAModelValidator();
            
            bool valid = mv.Validate(failingValueOverMax, "UserName", errorList);
            Assert.IsFalse(valid, "Assertion of negative case (over max) being false failed");
            Assert.AreEqual(2, errorList.Count, "Expected 2 errors");
            String verifyMessage = errorList[0].BusinessMessage + errorList[1].BusinessMessage;
            Assert.IsTrue(verifyMessage.Contains("Please enter a valid user name."), "message should contain - Please enter a valid user name.");
            Assert.IsTrue(verifyMessage.Contains("The User name must be between 8 and 64 characters long."), "message should contain - The User name must be between 8 and 64 characters long.");
        }

        [TestMethod()]
        public void UserName_logon_check_for_valid_username_ValidValue()
        {
            LogOnModel passingValue = new LogOnModel() { UserName = "person@yahoo.com" };

            List<ErrorModel> errorList = new List<ErrorModel>();
            ASAModelValidator mv = new ASAModelValidator();

            bool valid = mv.Validate(passingValue, "UserName");
            Assert.IsTrue(valid, "Assertion of positive case being true failed");
        }

        [TestMethod()]
        public void UserName_logon_check_for_valid_username_InvalidValue()
        {
            LogOnModel invalidValue = new LogOnModel() { UserName = "failString" };

            List<ErrorModel> errorList = new List<ErrorModel>();
            ASAModelValidator mv = new ASAModelValidator();

            bool valid = mv.Validate(invalidValue, "UserName", errorList);
            Assert.IsFalse(valid, "Assertion of negative case being false failed");
            Assert.AreEqual("Please enter a valid user name.", errorList[0].BusinessMessage);
        }
    }
}
