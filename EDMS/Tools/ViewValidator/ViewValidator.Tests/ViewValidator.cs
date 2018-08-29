using System;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ViewValidator.Tests
{
    [TestClass]
    public class ViewValidator : Validator
    {
        #region Private Properties
        private string DataPath
        {
            get
            {
                string rootPath = Environment.CurrentDirectory;
                int vvIndex = rootPath.IndexOf(@"\Tools\ViewValidator\");
                if (vvIndex == -1)
                {
                    Assert.Inconclusive(@"\Tools\ViewValidator not found in Environment.CurrentDirectory.");
                }

                return rootPath.Substring(0, vvIndex + @"\Tools\ViewValidator\".Length) + @"ViewValidator.Tests\TestData\";
            }
        }
        #endregion

        #region Test Methods
        /// <summary>
        /// This test verifies the LINQ expression in the NumMatches() function works for the happy path
        /// </summary>
        [TestMethod]
        public void NumMatches_WordListsHaveValues()
        {
            // Arrange
            string fileName = "NumMatchesTest.xml";
            XmlNode testNode = GetXmlNode(fileName, "/content-item/labels", "label", "TEST-LABEL-FOR-UNIT-TEST");
            WordListXml xmlWords = new WordListXml(testNode, ' ');
            WordListRazor viewWords = new WordListRazor("TEST LABEL UNIT", ' ');
            Assert.IsNotNull(xmlWords, "xmlWords was not expected to be null");
            Assert.IsNotNull(viewWords, "viewWords was not expected to be null");
            Assert.IsNotNull(xmlWords.Words, "xmlWords.Words was not expected to be null");
            Assert.IsNotNull(viewWords.Words, "viewWords.Words was not expected to be null");
            Assert.IsTrue(xmlWords.Words.Count == 5, "xmlWords.Words was expected to have 5 words but had " + xmlWords.Words.Count.ToString());
            Assert.IsTrue(viewWords.Words.Count == 3, "viewWords.Words was expected to have 3 words but had " + viewWords.Words.Count.ToString());


            // Act
            int numMatches = this.NumMatches(xmlWords, viewWords);

            // Assert
            Assert.AreEqual(numMatches, 3, "NumMatches was expected to be three. Check file: " + fileName);          
        }

        /// <summary>
        /// This test verifies the LINQ expression in the NumMatches() function works for the unhappy path where one list is empty
        /// </summary>
        [TestMethod]
        public void NumMatches_WordListIsEmpty()
        {
            // Arrange
            XmlNode testNode = GetXmlNode("NumMatchesTest.xml", "/content-item/labels", "label", "TEST-LABEL-FOR-UNIT-TEST");
            WordListXml xmlWords = new WordListXml(testNode, ' ');
            WordListRazor viewWords = new WordListRazor("TEST LABEL UNIT", ' ');
            xmlWords.Words.RemoveRange(0, xmlWords.Words.Count); // This is to empty the list for this test
            Assert.IsTrue(xmlWords.Words.Count == 0, "xmlWords.Words was expected to be empty");
            Assert.IsTrue(viewWords.Words.Count == 3, "viewWords.Words was expected to have 3 words");

            // Act
            int numMatches = this.NumMatches(xmlWords, viewWords);

            // Assert
            Assert.AreEqual(numMatches, 0, "NumMatches was expected to be zero.");
        }

        /// <summary>
        /// This test verifies the LINQ expression in the NumMatches() function works for the unhappy path where both lists are empty
        /// </summary>
        [TestMethod]
        public void NumMatches_WordListsAreEmpty()
        {
            // Arrange
            XmlNode testNode = GetXmlNode("NumMatchesTest.xml", "/content-item/labels", "label", "TEST-LABEL-FOR-UNIT-TEST");
            WordListXml xmlWords = new WordListXml(testNode, ' ');
            WordListRazor viewWords = new WordListRazor("TEST LABEL UNIT", ' ');
            xmlWords.Words.RemoveRange(0, xmlWords.Words.Count); // This is to empty the list for this test
            viewWords.Words.RemoveRange(0, viewWords.Words.Count); // This is to empty the list for this test
            Assert.IsTrue(xmlWords.Words.Count == 0, "xmlWords.Words was expected to be empty");
            Assert.IsTrue(viewWords.Words.Count == 0, "viewWords.Words was expected to be empty");

            // Act
            int numMatches = this.NumMatches(xmlWords, viewWords);

            // Assert
            Assert.AreEqual(numMatches, 0, "NumMatches was expected to be zero. ");
        }

        /// <summary>
        /// This test verifies the LINQ expression in the NumMatches() function throws an exception when one list is null
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(NullReferenceException), "A null object was inappropriately allowed.")] 
        public void NumMatches_WordListIsNull()
        {
            // Arrange
            WordListXml xmlWords = null;
            WordListRazor viewWords = new WordListRazor("TEST LABEL UNIT", ' ');

            // Act
            int numMatches = this.NumMatches(xmlWords, viewWords);

            // Assert
            // There need be no assertions since a NullReferenceException is the only expected result
        }

        /// <summary>
        /// This test verifies the length of the List<XmlNode> returned from FillNodeList
        /// </summary>
        [TestMethod]      
        public void FillNodeList_VerifyReturnedList()
        {
            // Arrange
            // TODO: Fix these hard coded paths
            string contentPath = @"C:\ASA\www.saltmoney.org";
            string viewPath = @"C:\WS\PAS_WebDEV_Scrum_ra_2\EDMS\ASA.Web\Sites\SALT\Views\";
            string testLogfile = @"C:\Logs\Salt\Log_Test.csv";
            string testUrlgfile = @"C:\Logs\Salt\Log_Test.csv";
            this.Args = new string[5] { contentPath, viewPath, "*", testLogfile, testUrlgfile };
            string xmlFilename = DataPath + "BenefitsIndexTest.xml";
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlFilename);
            XmlNode rootNode = xmlDoc.SelectSingleNode("//content-item");
            List<XmlNode> nodeList = new List<XmlNode>();

            // Act
            nodeList = FillNodeList(rootNode, nodeList);

            // Assert
            Assert.AreEqual(nodeList.Count, 47);
        }
        #endregion

        #region URL Unit Tests
        /// <summary>
        /// Tests the IsWebUrl() function
        /// </summary>
        [TestMethod]
        public void IsWebUrl()
        {
            // Arrange
            string validUrl1 = "http://www.asa.org";
            string validUrl2 = "www.asa.org";
            string validUrl3 = "web.com";
            string validUrl4 = "web.can";
            string validUrl5 = "web.uk";
            string validUrl6 = "aruba.aw";

            string inValidUrl1 = "mailto:membersupport@saltmoney.org";
            string inValidUrl2 = "~/Content/images/repayment/budget-calculator.jpg";
            string inValidUrl3 = "web .com";
            string inValidUrl4 = "!web.com";

            // Act and Assert
            Assert.IsTrue(IsWebUrl(validUrl1), validUrl1 + " was expected to be a valid url");
            Assert.IsTrue(IsWebUrl(validUrl2), validUrl2 + " was expected to be a valid url");
            Assert.IsTrue(IsWebUrl(validUrl3), validUrl3 + " was expected to be a valid url");
            Assert.IsTrue(IsWebUrl(validUrl4), validUrl4 + " was expected to be a valid url");
            Assert.IsTrue(IsWebUrl(validUrl5), validUrl5 + " was expected to be a valid url");
            Assert.IsTrue(IsWebUrl(validUrl6), validUrl6 + " was expected to be a valid url");

            Assert.IsFalse(IsWebUrl(inValidUrl1), inValidUrl1 + " was expected to be a invalid url");
            Assert.IsFalse(IsWebUrl(inValidUrl2), inValidUrl2 + " was expected to be a invalid url");
            Assert.IsFalse(IsWebUrl(inValidUrl3), inValidUrl3 + " was expected to be a invalid url");
            Assert.IsFalse(IsWebUrl(inValidUrl4), inValidUrl4 + " was expected to be a invalid url");
        }

        /// <summary>
        /// Tests the IsEmailAddress() function
        /// </summary>
        [TestMethod]
        public void IsEmailAddress()
        {
            // Arrange
            string validEmail1 = "membersupport@saltmoney.org";
            string validEmail2 = "support@me.org";
            string validEmail3 = "QA@test.edu";
            string validEmail4 = "WTF@testing.de";
            string validEmail5 = "test@bulgaria.bg";
            string validEmail6 = "mailto:membersupport@saltmoney.org";

            string inValidEmail1 = "mail to:membersupport@saltmoney.org";
            string inValidEmail2 = "~/Content/images/repayment/budget-calculator.jpg";
            string inValidEmail3 = "sales@web .com";
            string inValidEmail4 = "sup port@web.com";

            // Act and Assert
            Assert.IsTrue(IsEmailAddress(validEmail1), validEmail1 + " was expected to be in a valid format");
            Assert.IsTrue(IsEmailAddress(validEmail2), validEmail2 + " was expected to be in a valid format");
            Assert.IsTrue(IsEmailAddress(validEmail3), validEmail3 + " was expected to be in a valid format");
            Assert.IsTrue(IsEmailAddress(validEmail4), validEmail4 + " was expected to be in a valid format");
            Assert.IsTrue(IsEmailAddress(validEmail5), validEmail5 + " was expected to be in a valid format");
            Assert.IsTrue(IsEmailAddress(validEmail6), validEmail6 + " was expected to be in a valid format");

            Assert.IsFalse(IsEmailAddress(inValidEmail1), inValidEmail1 + " was expected to be in an invalid format");
            Assert.IsFalse(IsEmailAddress(inValidEmail2), inValidEmail2 + " was expected to be in an invalid format");
            Assert.IsFalse(IsEmailAddress(inValidEmail3), inValidEmail3 + " was expected to be in an invalid format");
            Assert.IsFalse(IsEmailAddress(inValidEmail4), inValidEmail4 + " was expected to be in an invalid format");
        }

        /// <summary>
        /// Tests the WebUrlResolves() function
        /// </summary>
        [TestMethod]
        public void WebUrlResolves()
        {
            Assert.Inconclusive("Not completed");

            // Arrange

            // Act

            // Assert

            // These failed...
            // ../PublishedContent/images/global/ajax-loader-2.gif	paymentreminder\overlay\loanpaymentdate.cshtml
            // |%~/Account/DeactivateAccount%|	                    account\manageaccount.xml
            // |%~/Home/about%|	                                    register\index.xml
            // |%~/Home/Privacy%|	                                loans\loan.xml
            // |%~/LoanInfo/%|	                                    loaninfo\index.xml
            // |%~/PaymentReminder/LoanPaymentDate%|	            paymentreminder\index.xml
            // |%~/PaymentReminder/LoanPaymentDate%|	            paymentreminder\Index.xml
            // ~/css/base.css	                                    home\termsif.cshtml
            // ~/loan	                                            loans\whattodo.xml
                // coded in Razor as ~/Loans/Loan but transformed by mode code to be wrong!!!
            // ~/Scripts/js/modules/homepage-hero.js	            home\index.cshtml
            // http://internships.com	                            loans\planabudgetheader.cshtml
            // http://www.google.com	                            account\overlay\hasalerts.cshtml
            // https://www.facebook.com/pages/SALT/200867293325268	home\index.xml
            // modules/loan-payment-date.php	                    paymentreminder\paymentreminder.xml
        }

        /// <summary>
        /// Tests the ContentUrlResolves() function
        /// </summary>
        [TestMethod]
        public void ContentUrlResolves()
        {
            Assert.Inconclusive("Not completed");

            // Arrange

            // Act

            // Assert

            // These failed...
            //~/PublishedContent/images/benefits/member-benefits-hero-icon-1.png
            //~/PublishedContent/images/contact/contact-piggy-bank.jpg
            //~/PublishedContent/images/fpo/binders.png
            //~/PublishedContent/images/fpo/loan-alert.png
            //~/PublishedContent/images/fpo/sidebar-registration-1.png
            //~/PublishedContent/images/global/secure-site-logo.png
            //~/PublishedContent/images/global/secure-site-logo.png
            //~/PublishedContent/images/loan-info-landing/icon1.png
            //~/PublishedContent/images/repayment/budget-book.jpg
            //~/PublishedContent/images/repayment/budget-calculator.jpg
            //~/PublishedContent/images/repayment/budget-dinnerplate.png
            //~/Scripts/js/modules/homepage-hero.js
            //http://internships.com
            //http://www.asa.org
            //http://www.google.com
            //https://www.facebook.com/pages/SALT/200867293325268

            // ../PublishedContent/images/global/ajax-loader-2.gif	paymentreminder\overlay\loanpaymentdate.cshtml
            // ~/PublishedContent/css/base.css	                                    home\termsif.cshtml
            // ~/Scripts/js/modules/homepage-hero.js	            home\index.cshtml
            // http://internships.com	                            loans\planabudgetheader.cshtml
            // http://www.google.com	                            account\overlay\hasalerts.cshtml
            // https://www.facebook.com/pages/SALT/200867293325268	home\index.xml
            // modules/loan-payment-date.php	                    paymentreminder\paymentreminder.xml
        }

        /// <summary>
        /// Tests the ContentUrlResolves() function
        /// </summary>
        [TestMethod]
        public void JavascriptUrlResolves()
        {
            Assert.Inconclusive("Not completed");

            // Arrange

            // Act

            // Assert

            // These failed...
            // ~/Scripts/js/modules/homepage-hero.js	            home\index.cshtml
        }

        /// <summary>
        /// Tests the CSSUrlResolves() function
        /// </summary>
        [TestMethod]
        public void CSSUrlResolves()
        {
            Assert.Inconclusive("Not completed");

            // Arrange

            // Act

            // Assert
        }

        /// <summary>
        /// Tests the RazorUrlResolves() function
        /// </summary>
        [TestMethod]
        public void RazorUrlResolves()
        {
            Assert.Inconclusive("Not completed");

            // Arrange

            // Act

            // Assert

            //~/Account/ForgotPassword
            //~/Home/Privacy
            //~/Home/Terms
            //~/PaymentReminder/LoanPaymentDate

            // These failed...
            // |%~/Account/DeactivateAccount%|	                    account\manageaccount.xml
            // |%~/Home/about%|	                                    register\index.xml
            // |%~/Home/Privacy%|	                                loans\loan.xml
            // |%~/LoanInfo/%|	                                    loaninfo\index.xml
            // |%~/PaymentReminder/LoanPaymentDate%|	            paymentreminder\index.xml
            // |%~/PaymentReminder/LoanPaymentDate%|	            paymentreminder\Index.xml
            // ~/css/base.css	                                    home\termsif.cshtml
            // ~/loan	                                            loans\whattodo.xml
            // coded in Razor as ~/Loans/Loan but transformed by mode code to be wrong!!!
            // ~/Scripts/js/modules/homepage-hero.js	            home\index.cshtml
            // http://internships.com	                            loans\planabudgetheader.cshtml
            // http://www.google.com	                            account\overlay\hasalerts.cshtml
            // https://www.facebook.com/pages/SALT/200867293325268	home\index.xml
            // modules/loan-payment-date.php	                    paymentreminder\paymentreminder.xml
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// GetXmlNode() is used by most of the unit tests for NumMatches, and makes those unit tests more readable by abstracting out the code 
        /// common to the three.
        /// </summary>
        /// <param name="xmlFile"></param>
        /// <param name="nodeGroup"></param>
        /// <param name="nodeType"></param>
        /// <param name="nodeName"></param>
        /// <returns></returns>
        private XmlNode GetXmlNode(string xmlFile, string nodeGroup, string nodeType, string nodeName)
        {
            if (String.IsNullOrEmpty(xmlFile) || String.IsNullOrEmpty(nodeGroup) ||
                String.IsNullOrEmpty(nodeType) || String.IsNullOrEmpty(nodeName))
            {
                Assert.Inconclusive("One or more input values was null or empty in call to GetXmlNode()");
            }

            string xmlFilename = DataPath + xmlFile;
            if (!File.Exists(xmlFilename))
            {
                Assert.Inconclusive(xmlFilename + " was NOT FOUND so this test could not be run.");
            }

            string searchStr = nodeGroup + '/' + nodeType + "[@name='" + nodeName + "']";
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlFilename);
            XmlNode xmlNode = xmlDoc.SelectSingleNode(searchStr);

            if (xmlNode == null)
            {
                Assert.Inconclusive(xmlFilename + " not in correct format for this test to be run. " + nodeName + " node not found.");
            }

            return xmlNode;
        }
        #endregion
    }
}
