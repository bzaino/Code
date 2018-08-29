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
    ///This is a test class for ASA.Web.Comon.Validation namespace and is intended
    ///to contain all Validation folder Unit Tests
    /// </summary>
    [TestClass]
    public class ValidatorTest
    {
        public ValidatorTest()
        {
            //
            // TODO: Add constructor logic here
            //
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

        #region Additional test attributes
        #endregion

        #region Generic data annotation attributes tests
        [TestMethod()]
        public void RequiredIfAttribute_Test()
        {
            object passingValueBothPopulated = new { NewPassword = "legalValue!", Password = "passingValue!" };
            object failingValue = new { NewPassword = "failingValue" };
            object passingValueNewPasswordNull = new { Password = "passingValue!5" };
            object passingValueBothNull = new { };

            RequiredIfAttribute attribute = new RequiredIfAttribute("Password", "NewPassword");

            //this does not test the model just the RequiredIfAttribute object's logic.
            Assert.IsTrue(attribute.IsValid(passingValueBothPopulated), "Assertion of positive case being true failed");
            Assert.IsFalse(attribute.IsValid(failingValue), "Assertion of negative case being false failed");
            Assert.IsTrue(attribute.IsValid(passingValueNewPasswordNull), "Assertion of positive case (Field  being false failed");
            Assert.IsTrue(attribute.IsValid(passingValueBothNull), "Assertion of negative case being false failed");
        }
        #endregion


        #region EmailValidator Unit Tests
        [TestMethod()]
        public void EmailValidator_Test()
        {
            String passingValue = "person@yahoo.com";
            String failingValue = "failString";

            EmailValidator emailCheck = new EmailValidator();

            Assert.IsTrue(emailCheck.IsValid(passingValue), "Assertion of positive case being true failed");
            Assert.IsFalse(emailCheck.IsValid(failingValue), "Assertion of negative case being false failed");
        }
        #endregion

        #region PasswordStandardsASAValidator Unit Tests
        [TestMethod]
        public void PasswordStandardsASAValidator_Test()
        {
            String passingValueThreeAttributes = "passingValue!";
            String passingValueFourAttributes = "passingValue1!";
            String failingValueTwoAttributes = "failingValue";
            String failingValueOneAttribute = "failingvalue";

            PasswordStandardsASAValidator passwordLogicAttribute = new PasswordStandardsASAValidator();

            Assert.IsTrue(passwordLogicAttribute.IsValid(passingValueThreeAttributes), "Assertion of positive case (3 of 4 attributes) being true failed");
            Assert.IsTrue(passwordLogicAttribute.IsValid(passingValueFourAttributes), "Assertion of positive case (4 of 4 attributes) being true failed");
            Assert.IsTrue(passwordLogicAttribute.IsValid(failingValueTwoAttributes), "Assertion of positive case (2 of 4 attributes) being false failed");
            Assert.IsTrue(passwordLogicAttribute.IsValid(failingValueOneAttribute), "Assertion of positive case (1 of 4 attributes) being false failed");
        }
        #endregion

        #region YOBValidator Unit Tests
        [TestMethod()]
        public void YOBValidator_Test()
        {
            Nullable<short> passingValue = (short)(DateTime.Today.Year - 14);
            Nullable<short> failingValue = (short)(DateTime.Today.Year - 32767);
            Nullable<short> nullValue = null;

            YOBValidator yobValidator = new YOBValidator();

            Assert.IsTrue(yobValidator.IsValid(passingValue), "Assertion of positive case being true failed");
            Assert.IsFalse(yobValidator.IsValid(failingValue), "Assertion of negative case being false failed");
            Assert.IsTrue(yobValidator.IsValid(nullValue), "Assertion of positive case being true failed");
        }
        #endregion
    }
}
