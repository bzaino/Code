using ASA.Web.WTF.Integration.MVC3;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Web;

namespace UnitTestsValidations
{
    
    
    /// <summary>
    ///This is a test class for Mvc3HelperTest and is intended
    ///to contain all Mvc3HelperTest Unit Tests
    ///</summary>
    [TestClass()]
    public class Mvc3HelperTest
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
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        [TestMethod()]
        [TestProperty("jira", "SWD-5581")]
        [TestProperty("author", "cmak")]
        public void IsRequestPathException_RequestPathExceptionTest()
        {
            String exceptionMessage = "A potentially dangerous Request.Path value was detected from the client (*)";
            Mvc3Helper target = new Mvc3Helper();
            HttpException e = new HttpException(exceptionMessage);

            bool actual = target.IsRequestPathException(e);

            Assert.AreEqual(true, actual, "This is a valid RequestPathException and should return true.");
        }

        [TestMethod()]
        [TestProperty("jira", "SWD-5581")]
        [TestProperty("author", "cmak")]
        public void IsRequestPathException_HttpExceptionWithoutMessageTest()
        {
            String exceptionMessage = "Random Exception Message"; 
            Mvc3Helper target = new Mvc3Helper();
            HttpException e = new HttpException(exceptionMessage);

            bool actual = target.IsRequestPathException(e);

            Assert.AreEqual(false, actual, "This is not a RequestPathException and should return false.");
        }

        [TestMethod()]
        [TestProperty("jira", "SWD-5581")]
        [TestProperty("author", "cmak")]
        public void IsRequestPathException_NonHttpExceptionTest()
        {
            String exceptionMessage = "A potentially dangerous Request.Path value was detected from the client (*)";
            Mvc3Helper target = new Mvc3Helper();
            Exception e = new Exception(exceptionMessage);

            bool actual = target.IsRequestPathException(e);

            Assert.AreEqual(false, actual, "This is not a HttpException and should return false.");
        }
    }
}
