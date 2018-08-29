using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SALTCoursesWSClient;

namespace SALTCoursesWSClient.Tests
{
    /// <summary>
    /// Summary description for UtilityMethodsTests
    /// </summary>
    [TestClass]
    public class UtilityMethodsTests
    {
        public UtilityMethodsTests()
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
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void DeserializeResponseMemberResponseShouldTranslateToSiteMemberModel()
        {
            string memberResponse = "{\"ErrorList\":[],\"RedirectURL\":\"\",\"ActivationStatusFlag\":true,\"ActiveDirectoryKey\":\"e7242ca3-54ab-7d40-90c4-47a52e3f09b9\",\"Addresses\":[],\"CommunityDisplayName\":null,\"ContactFrequency\":true,\"ContactFrequencyKey\":null,\"CustomerId\":0,\"DOB\":null,\"DisplayName\":\"ssoflow\",\"Emails\":[{\"ErrorList\":[],\"RedirectURL\":\"\",\"EmailAddress\":\"ssoflow.testone@asa.org\",\"EmailKey\":null,\"IndividualId\":null,\"IsPrimary\":true,\"Type\":null,\"TypeID\":null}],\"EnrollmentStatus\":\"F\",\"EnrollmentStatusEffective\":null,\"ExpectedGraduationYear\":null,\"ExternalId\":null,\"FirstName\":\"ssoflow\",\"GradeLevel\":\"\",\"IndividualId\":\"e7242ca3-54ab-7d40-90c4-47a52e3f09b9\",\"InvitationToken\":\"\",\"IsCommunityActive\":false,\"LastName\":\"ibrahim\",\"LegalFirstName\":null,\"MemberShipFlag\":true,\"MemberToken\":null,\"MembershipId\":\"3873\",\"MembershipStartDate\":null,\"MiddleInitial\":null,\"NewPassword\":null,\"OrganizationProducts\":[{\"IsOrgProductActive\":true,\"ProductID\":1},{\"IsOrgProductActive\":true,\"ProductID\":2},{\"IsOrgProductActive\":true,\"ProductID\":4},{\"IsOrgProductActive\":true,\"ProductID\":3}],\"Organizations\":[{\"ErrorList\":[],\"RedirectURL\":\"\",\"BranchCode\":\"00\",\"Brand\":\"asa-university-sample\",\"EffectiveEndDate\":null,\"EffectiveStartDate\":\"\\/Date(1454935259630-0500)\\/\",\"ExpectedGraduationYear\":2026,\"IsContracted\":true,\"IsLookupAllowed\":true,\"IsOrganizationDeleted\":false,\"IsPrimary\":true,\"IsReadyToLaunchAlumni\":true,\"IsReadyToLaunchInSchool\":true,\"MemberId\":3873,\"OECode\":\"888888\",\"OrganizationAliases\":null,\"OrganizationId\":7167,\"OrganizationLogoName\":\"asa-university-sample\",\"OrganizationName\":\"ASA University\",\"OrganizationTypeExternalId\":\"SCHL\",\"RefOrganizationTypeId\":4,\"RefSALTSchoolTypeID\":null,\"ReportingId\":null,\"SchoolType\":null}],\"PartTimeDate\":null,\"Password\":null,\"PersonId\":0,\"Phones\":[],\"PrimaryEmailKey\":\"ssoflow.testone@asa.org\",\"PrimaryOrganizationKey\":\"7167\",\"Products\":[{\"CreatedBy\":null,\"CreatedDate\":\"\\/Date(-62135578800000-0500)\\/\",\"IsMemberProductActive\":true,\"MemberID\":3873,\"MemberProductValue\":\"123\",\"ModifiedBy\":null,\"ModifiedDate\":null,\"RefProductID\":1}],\"ProfileQAndAs\":[],\"Roles\":[],\"SALTSchoolTypeID\":null,\"Schools\":[],\"Source\":\"1\",\"USPostalCode\":null,\"WithdrawalDate\":null,\"YearOfBirth\":1915}";
            var deserializedObject = UtilityMethods.DeserializeResponse<SiteMemberModel>(memberResponse);

            Assert.AreEqual(new SiteMemberModel().GetType(), deserializedObject.GetType(), "Deserializer error - Object type mismatch");
            Assert.AreEqual("3873", deserializedObject.MembershipId, "Deserializer error - value mismatch");
        }
    }
}
