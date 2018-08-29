using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using ASA.Web.Sites.SALT.Models;
using ASA.Web.Services.ASAMemberService.DataContracts;
using ASA.Web.Sites.SALT.Controllers;

namespace UnitTestsValidations
{
    /// <summary>
    ///This is a test class for ASA.Web.Sites.SALT.Controllers namespace and is intended
    ///to contain Unit Tests for AccountController
    /// </summary>
    [TestClass]
    public class AccountControllerTest
    {
        public AccountControllerTest()
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

        #region  AccountController Unit Tests
        //not able to test this until we can mock out the HttpContext for the controller.
        //[TestMethod()]
        //public void AccountController_ManageAccount_Test()
        //{
        //    AccountController ac = new AccountController();

        //    ASA.Web.WTF.MemberAccountData accountData = new ASA.Web.WTF.MemberAccountData {
        //        //Created = user.CreationDate,
        //        //Id = user.ProviderUserKey,
        //        //IsApproved = user.IsApproved,
        //        //IsLockedOut = user.IsLockedOut,
        //        //IsOnline = user.IsOnline,
        //        //LastActivity = user.LastActivityDate,
        //        //LastLockout = user.LastLockoutDate,
        //        //LastLogin = user.LastLoginDate,
        //        //LastPasswordChange = user.LastPasswordChangedDate,
        //        //MemberId = user.ProviderUserKey,
        //        //PasswordQuestion = user.PasswordQuestion,
        //        //RegistrationDate = user.CreationDate,
        //        Username = "UnitTestUserName"
        //    };

        //    ASA.Web.WTF.MemberProfileData profileData = new ASA.Web.WTF.MemberProfileData {
        //        //ContactFrequency = memberIn.ContactFrequency,
        //        //FirstName = memberIn.FirstName,
        //        //LastName = memberIn.LastName,
        //        //EmailAddress = memberIn.Emails[0].EmailAddress,
        //        //Source = memberIn.Source,
        //        //InvitationToken = memberIn.InvitationToken,
        //        //ExpectedGraduationYear = memberIn.ExpectedGraduationYear,
        //        //YearOfBirth = memberIn.YearOfBirth
        //    };

            
        //    ac.SiteMember = new ASA.Web.WTF.SiteMember(accountData, profileData);
            
        //    //AccountController ac = new AccountController(new ASA.Web.WTF.SiteMember(accountData, profileData));

        //    //ac.HttpContext.Items["SiteMember"] = new ASA.Web.WTF.SiteMember(accountData, profileData);

        //    //ac.HttpContext = CreateHttpContext //Controller.HttpContext();
 
        //    //var httpRequest = new HttpRequest("", "http://mySomething/", "");
        //    //var stringWriter = new StringWriter();
        //    //var httpResponse = new HttpResponse(stringWriter);
        //    //var httpContext = new  HttpContext(httpRequest, httpResponse);
        //    //var controllerHttpContext = new HttpContext(httpRequest, httpResponse);

        //    //ac.ControllerContext.HttpContext.Items = httpContext.Items;
        //    //ac.HttpContext.Items = httpContext;

            
        //    //ac.HttpContext.Items.Add("SiteMember", null);
        //    //ac.HttpContext.Items.Add("SiteMember", new ASA.Web.WTF.SiteMember(accountData, profileData));
        //    //ac.HttpContext.Items["SiteMember"] = null;
            
        //    //ActionResult result = ac.ManageAccount(amModel);
        //}

        [TestMethod()]
        public void AccountController_GetManageAccountModel_ValidateValues_Test()
        {
            AccountController ac = new AccountController();

            //set test values into ASAMemberModel
            ASAMemberModel amModel = new ASAMemberModel();
            SetASAMemberModelValues(amModel);

            //values that should be set on Model
            string emailAddressPrimary = "emailAddressPrimary@asa.org";

            string firstName = "FirstName";
            string lastName = "LastName";
            string newPassword = "NewPassword";
            string password = "Password";
            short yearOfBirth = 1975;

            string phoneNumber = "1-619-555-1212";
            string phoneNumberType = "Mobile";

            string oeCode = "123456";
            string branchCode = "12";
            string organizationName = "ASA University";
            int expectedGraduationYearDefault = 2001;

            // Wrap an already existing instance
            PrivateObject accessor = new PrivateObject(ac);

            // Call a private method
            ManageAccountModel maModel = (ManageAccountModel)accessor.Invoke("GetManageAccountModel", amModel);

            Assert.AreEqual(emailAddressPrimary, maModel.EmailAddress);
            Assert.AreEqual(emailAddressPrimary, maModel.UserName);

            Assert.AreEqual(phoneNumber, maModel.PhoneNumber);
            Assert.AreEqual(phoneNumberType, maModel.PhoneNumberType);

            Assert.AreEqual(oeCode, maModel.OECode);
            Assert.AreEqual(branchCode, maModel.BranchCode);
            Assert.AreEqual(organizationName, maModel.OrganizationName);

            Assert.AreEqual(newPassword, maModel.ConfirmPassword);
            Assert.AreEqual(newPassword, maModel.NewPassword);
            Assert.AreEqual(password, maModel.Password);

            Assert.AreEqual(firstName, maModel.FirstName);
            Assert.AreEqual(lastName, maModel.LastName);
            Assert.IsTrue(maModel.IsCommunityActive);
            Assert.AreEqual(expectedGraduationYearDefault, maModel.ExpectedGraduationYear);
            Assert.AreEqual(null, maModel.USPostalCode);
            Assert.AreEqual(null, maModel.SALTSchoolTypeID);
            Assert.AreEqual(yearOfBirth, maModel.YOB);

            Assert.IsFalse(maModel.AddressValidated);
        }

        private void SetASAMemberModelValues(ASAMemberModel amModel)
        {
            //values to be set on Model
            string emailAddressPrimary = "emailAddressPrimary@asa.org";
            string emailAddressSecondary = "secondaryEmailAddress";

            bool isCommunityActive = true;
            string firstName = "FirstName";
            string lastName = "LastName";
            string newPassword = "NewPassword";
            string password = "Password";
            short yearOfBirth = 1975;

            string phoneNumber = "1-619-555-1212";
            string phoneNumberType = "Mobile";

            string oeCode = "123456";
            string branchCode = "12";
            string organizationName = "ASA University";
            int expectedGraduationYear = 2001;

            amModel.Emails = new List<MemberEmailModel> { 
                new MemberEmailModel { 
                    EmailAddress = emailAddressPrimary,
                    IsPrimary = true
                },
                new MemberEmailModel { 
                    EmailAddress = emailAddressSecondary,
                    IsPrimary = false
                }
            };

            amModel.Phones = new List<MemberPhoneModel> { 
                new MemberPhoneModel { 
                    PhoneNumber = phoneNumber,
                    Type = phoneNumberType
                }
            };

            amModel.Organizations = new List<MemberOrganizationModel> { 
                new MemberOrganizationModel { 
                    OECode = oeCode,
                    BranchCode = branchCode,
                    OrganizationName = organizationName,
                    ExpectedGraduationYear = expectedGraduationYear
                }
            };

            amModel.IsCommunityActive = isCommunityActive;
            amModel.FirstName = firstName;
            amModel.LastName = lastName;
            amModel.NewPassword = newPassword;
            amModel.Password = password;
            amModel.YearOfBirth = yearOfBirth;
        }
        #endregion
    }
}
