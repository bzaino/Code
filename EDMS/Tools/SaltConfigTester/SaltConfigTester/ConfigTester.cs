using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Text;
using System.Xml;

namespace SaltConfigTester
{
    public class ConfigTester
    {
        #region Private Members
        private Arguments _args;
        private readonly IList<Report> _reportList = new List<Report>();
        #endregion

        #region Public Properties
        public string LogFile { get { return LogPath + "SaltConfigTesterLog.csv"; } }
        public string LogPath
        {
            get
            {
                if (_args["f"] != null)
                {
                    return _args["f"].TrimEnd('\\') + '\\';
                }
                return null;
            }
        }
        public string SalConfigFile { get { return SalRootPath + "Web.Config"; } }
        public string SalRootPath { get { return _args["SAL"].TrimEnd('\\') + "\\"; } }
        public string SaltConfigFile { get { return SaltRootPath + "Web.Config"; } }
        public string SaltRootPath { get { return _args["SALT"].TrimEnd('\\') + "\\"; } }
        #endregion

        #region Constructor(s)
        public ConfigTester(Arguments args)
        {
            _args = args;
            if (SalRootPath != null && !Directory.Exists(SalRootPath))
            {
                throw new Exception(SalRootPath + " folder not found. Please check the path and try again.");
            }
            if (SalConfigFile != null && !File.Exists(SalConfigFile))
            {
                throw new Exception(SalConfigFile + " not found. Please check the path and try again.");
            }
            if (SaltRootPath != null && !Directory.Exists(SaltRootPath))
            {
                throw new Exception(SaltRootPath + " folder not found. Please check the path and try again.");
            }
            if (SaltConfigFile != null && !File.Exists(SaltConfigFile))
            {
                throw new Exception(SaltConfigFile + " not found. Please check the path and try again.");
            }
            if (LogPath != null && !Directory.Exists(LogPath))
            {
                throw new Exception(LogPath + " folder not found. Please check the path and try again.");
            }
        }
        #endregion

        #region Internal Members
        internal void RunTests()
        {
            FindAttributes();
        }
        #endregion

        #region Private Members
        private void FindAttributes()
        {
            _reportList.Add(new ReportMessageOnly(SalConfigFile, "SALT Config Tester", 2));
            TestSalConfiguration(SalConfigFile);
            TestSaltConfiguration(SaltConfigFile);
            ReportOutput.Run(_reportList, LogFile);
        }

        private void TestSalConfiguration(string configFile)
        {
            if (SalConfigFile == null)
                return;

            _reportList.Add(new ReportMessageOnly(configFile, "     SAL web.config values:", 1));
            ParseXml(configFile, "//appSettings/add", "SearchHost", "key", "value", true);
            ParseXml(configFile, "//appSettings/add", "SearchPort", "key", "value", true);
            ParseXml(configFile, "//appSettings/add", "xWebLogin", "key", "value", true);
            ParseXml(configFile, "//appSettings/add", "xWebPassword", "key", "value", true);
            _reportList.Add(new ReportMessageOnly(configFile, "", 1));
            ExtractUrl(configFile, "//system.serviceModel/client/endpoint", "|LMS", "name", "address", new string[] { "http://", "net.tcp://" });
            ExtractUrl(configFile, "//system.serviceModel/client/endpoint", "|PMS", "name", "address", new string[] { "http://", "net.tcp://" });
            ExtractUrl(configFile, "//system.serviceModel/client/endpoint", "|ILoanManagement", "name", "address", new string[] { "http://", "net.tcp://" });
            ExtractUrl(configFile, "//system.serviceModel/client/endpoint", "|IPersonManagement", "contract", "address", new string[] { "http://", "net.tcp://" });
            ExtractUrl(configFile, "//system.serviceModel/client/endpoint", "QAPortType", "name", "address", new string[] { "http://", "net.tcp://" });
            ExtractUrl(configFile, "//system.serviceModel/client/endpoint", "netForumXMLSoap", "name", "address", new string[] { "http://", "net.tcp://" });
            _reportList.Add(new ReportMessageOnly(configFile, "", 1));
        }

        private void TestSaltConfiguration(string configFile)
        {
            if (SaltConfigFile == null)
                return;

            _reportList.Add(new ReportMessageOnly(configFile, "     SALT web.config values:", 1));
            ParseXml(configFile, "//appSettings/add", "SearchHost", "key", "value", true);
            ParseXml(configFile, "//appSettings/add", "SearchPort", "key", "value", true);
            ParseXml(configFile, "//appSettings/add", "xWebLogin", "key", "value", true);
            ParseXml(configFile, "//appSettings/add", "xWebPassword", "key", "value", true);
            ParseXml(configFile, "//appSettings/add", "SMTPServer", "key", "value", true);
            ParseXml(configFile, "//appSettings/add", "SMTPServerPort", "key", "value", true);
            _reportList.Add(new ReportMessageOnly(configFile, "", 0));
            ExtractUrl(configFile, "//appSettings/add", "MemberService", "key", "value", new string[] { "http://", "net.tcp://" });
            ExtractUrl(configFile, "//appSettings/add", "LoanServiceEndpoint", "key", "value", new string[] { "http://", "net.tcp://" });
            ExtractUrl(configFile, "//appSettings/add", "SelfReportedServiceEndpoint", "key", "value", new string[] { "http://", "net.tcp://" });
            ExtractUrl(configFile, "//appSettings/add", "AddrValidationServiceEndpoint", "key", "value", new string[] { "http://", "net.tcp://" });
            ExtractUrl(configFile, "//appSettings/add", "SearchServiceEndpoint", "key", "value", new string[] { "http://", "net.tcp://" });
            ExtractUrl(configFile, "//appSettings/add", "AlertServiceEndpoint", "key", "value", new string[] { "http://", "net.tcp://" });
            ExtractUrl(configFile, "//appSettings/add", "ReminderService", "key", "value", new string[] { "http://", "net.tcp://" });
            ExtractUrl(configFile, "//appSettings/add", "SchoolLookup", "key", "value", new string[] { "http://", "net.tcp://" });
            ExtractUrl(configFile, "//appSettings/add", "SchoolLookupIneligibleEmail", "key", "value", new string[] { "http://", "net.tcp://" });
            _reportList.Add(new ReportMessageOnly(configFile, "", 0));
            ExtractUrl(configFile, "//system.serviceModel/client/endpoint", "netForumXMLSoap", "name", "address", new string[] { "http://", "net.tcp://" });
            ExtractUrl(configFile, "//connectionStrings/add", "LdapConnection", "name", "connectionString", new string[] { "LDAP://", "http://", "net.tcp://" }, '"');
            ParseXml(configFile, "//log4net/appender", "RollingFileAppender", "name", "value", true);
            ParseXml(configFile, "//system.web/membership/providers/add", "LdapProvider", "name", "connectionStringName", true);
            ParseXml(configFile, "//system.web/membership/providers/add", "LdapProvider", "name", "connectionUsername", true);
            ParseXml(configFile, "//system.web/membership/providers/add", "LdapProvider", "name", "connectionPassword", true);
            FindASAContent(configFile, "ASAContent", "storeLocation", "path=");
            FindASAActiveDir(configFile, "ASAActiveDirectory", "userID", "value=");
            FindASAActiveDir(configFile, "ASAActiveDirectory", "password", "value=");
            FindASAActiveDir(configFile, "ASAActiveDirectory", "domain", "value=");
            FindASAActiveDir(configFile, "ASAActiveDirectory", "userContainer", "value=");
            _reportList.Add(new ReportMessageOnly(configFile, "", 1));
        }
        #endregion

        private void FindASAActiveDir(string configFile, string regEx, string key, string pattern)
        {
            // For some odd reason ASAContent does not behave like the others, and needs this special behavior
            var config = new XmlDocument();
            config.Load(configFile);
            XmlNode rootNode = config.SelectSingleNode("//configuration");
            foreach (XmlNode node in rootNode.ChildNodes)
            {
                if (node.Name == regEx)
                {
                    string property = ExtractValue(node.InnerXml, regEx, key, pattern);
                    //string property = node.Attributes[key].ToString();
                    _reportList.Add(new ReportAttributeStatus(ReportType.AttributeFound, configFile,
                                          key,
                                          property, "found", 0));
                }
            }
        }

        private void FindASAContent(string configFile, string regEx, string key, string pattern)
        {
            // For some odd reason ASAContent does not behave like the others, and needs this special behavior
            var config = new XmlDocument();
            config.Load(configFile);
            XmlNode rootNode = config.SelectSingleNode("//configuration");
            foreach (XmlNode node in rootNode.ChildNodes)
            {
                if (node.Name == regEx)
                {
                    string trimmedUrl = ExtractValue(node.InnerXml, regEx, key, pattern);
                    trimmedUrl = "\\\\" + trimmedUrl.TrimStart('\\');
                    _reportList.Add(new ReportAttributeStatus(ReportType.AttributeFound, configFile,
                                          key,
                                          trimmedUrl, "found", 0));

                }
            }
        }

        private string ExtractUrl(string configFile, string nodePattern, string regEx, string keyPattern, string valuePattern, string[] leftPattern, char rightDelimitingChar='/')
        {
            string extractedUrl = ParseXml(configFile, nodePattern, regEx, keyPattern, valuePattern, false);
            if (extractedUrl != null )
            {
                foreach (string leftPat in leftPattern)
                {
                    string trimmedUrl = SplitString(extractedUrl, leftPat, rightDelimitingChar);
                    if (trimmedUrl != null)
                    {
                        _reportList.Add(new ReportAttributeStatus(ReportType.AttributeFound, configFile,
                                                                  regEx,
                                                                  trimmedUrl, "found", 0));
                        return extractedUrl;
                    }
                }
                _reportList.Add(new ReportAttributeStatus(ReportType.AttributeFound, configFile,
                                                          regEx,
                                                          null, "not found", 0));
            }
            return null;
        }

        private string SplitString(string line, string leftStr, char rightDelimitingChar)
        {
            if (line.IndexOf(leftStr) > -1 )
            {
                int leftIndex = line.IndexOf(leftStr) + leftStr.Length;
                string searchStr = line.Substring(leftIndex);
                if (rightDelimitingChar == '/')
                {
                    int rightDelimitingIndex = searchStr.IndexOf(rightDelimitingChar);
                    return searchStr.Substring(0, rightDelimitingIndex);
                }
                else
                {
                    return searchStr;
                }
            }
            return null;
        }

        private string ParseXml(string configFile, string nodePattern, string regExpression, string keyPattern, string valuePattern, bool updateReportList)
        {
            var config = new XmlDocument();
            config.Load(configFile);
            XmlNodeList list = config.SelectNodes("//comment()");
            if (list != null)
            {
                foreach (XmlNode commentNode in list)
                {
                    if (commentNode.ParentNode != null)
                    {
                        commentNode.ParentNode.RemoveChild(commentNode);
                    }
                }
            }

            string searchStr = null;
            if (regExpression[0] == '|')
            {
                // Creates this string: "//system.serviceModel/client/endpoint[@name='netForumXMLSoap']"
                //searchStr = String.Format("{0}[ends-with(@{1}='{2}')]", nodePattern, keyPattern, regExpression); // ends-with is not supported with Xpath engine

                // Creates this string: "system.serviceModel/client/endpoint[substring(@name, string-length(@name) - string-length('netForumXMLSoap')) = 'netForumXMLSoap']"
                searchStr = String.Format("{0}[substring(@{1}, string-length(@{1}) - string-length('{2}') +1) = '{2}']", nodePattern, keyPattern, regExpression.Trim('|'));
            }
            else
            {
                searchStr = String.Format("{0}[@{1}='{2}']", nodePattern, keyPattern, regExpression);
               
            }
            XmlNode node = config.SelectSingleNode(searchStr);
            string value = ExtractValue(node, regExpression, keyPattern, valuePattern);
            if (value == null && node != null)
            {
                value = ExtractValue(node.FirstChild, regExpression, keyPattern, valuePattern);
            }
            if (value != null)
            {
                if (updateReportList)
                {
                    _reportList.Add(new ReportAttributeStatus(ReportType.AttributeFound, configFile, regExpression,
                                                              value, "found", 0));
                }
                return value;
            }
            _reportList.Add(new ReportAttributeStatus(ReportType.AttributeFound, configFile, regExpression,
                                                      null, "not found", 0));
            return null;
        }

        private string ExtractValue(XmlNode node, string regExpression, string keyPattern, string valuePattern)
        {
            if (node != null && node.Attributes[valuePattern] != null)
            {
                return node.Attributes[valuePattern].Value;
            }
            return null;
        }

        private string ExtractValue(string innerXml, string regExpression, string keyPattern, string valuePattern)
        {
            string searchStr = keyPattern; 
            var regex = new Regex(searchStr);
            if (regex.IsMatch(innerXml))
            {
                if (innerXml.IndexOf(keyPattern) > -1 && innerXml.IndexOf(valuePattern) > -1)
                {
                    //int valuePatternIndex = innerXml.IndexOf(valuePattern);
                    //string valueStr = innerXml.Substring(valuePatternIndex + valuePattern.Length).TrimStart('\"');
                    int keyPatternIndex = innerXml.IndexOf(keyPattern);
                    string valueStr = innerXml.Substring(keyPatternIndex + keyPattern.Length);
                    int openQuoteIndex = valueStr.IndexOf('\"');
                    valueStr = valueStr.Substring(openQuoteIndex+1);
                    int closedQuoteIndex = valueStr.IndexOf('\"');
                    if (closedQuoteIndex > -1)
                    {
                        return valueStr.Substring(0, closedQuoteIndex);
                    }
                }
            }
            return null;
        }
    }
}

