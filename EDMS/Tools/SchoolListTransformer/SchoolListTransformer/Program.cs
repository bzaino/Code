using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Security;
using System.Threading;
using System.Configuration;

namespace SchoolListTransformer
{
    class Program
    {
        private static string _logFilePath;
        private static string _outputFilePath;
        private static bool bChangedOutStream = false;
        private static StringBuilder sbUsage = new StringBuilder();

        static void Main(string[] args)
        {

            bool gotFileNames = true;
            System.Xml.XmlNode result = null;

            //get file from args
            if (args.Length > 0)
            {
                //get args from command line
                GetParamsFromCommandLine(args);
            }
           
            //Get args from config file if not specified on the command-line
            if(String.IsNullOrEmpty(_outputFilePath))
            {
                _outputFilePath = ConfigurationManager.AppSettings.Get("OutputPath");
                if(!String.IsNullOrEmpty(_outputFilePath))
                {
                    Array.Resize<string>(ref args,(args.Length + 1));
                    if(!_outputFilePath.EndsWith("\\"))
                        _outputFilePath += '\\';
                    _outputFilePath += ConfigurationManager.AppSettings.Get("OutputFileName");
                    args[0] = _outputFilePath;
                }
                
            }

            if(String.IsNullOrEmpty(_logFilePath) && args.Length == 1)
            {
                Array.Resize<string>(ref args, (args.Length + 1));
                _logFilePath = ConfigurationManager.AppSettings.Get("LogFilePath");
                if(!_logFilePath.EndsWith("\\"))
                    _logFilePath += '\\';
                _logFilePath += ConfigurationManager.AppSettings.Get("LogFileName");
                args[1] = _logFilePath;
            }

            sbUsage.Append("Generates a School list xml file to be loaded into Endeca.\n");
            sbUsage.Append("The tool loads the school list from the configured MRM application server using XWeb:\n");
            sbUsage.Append("\n");
            sbUsage.Append("USAGE:\n\nSchooListTransformer outputFile logFile\n\n");
            sbUsage.Append("  Example:\n\n");
            sbUsage.Append(@"    SchooListTransformer D:\\Files\FolderB\myXmlSchoolListOutput.xml D:\\Files\FolderB\SchoolListUtility.log" + "\n\n");

            //FileStream fs = new FileStream(_outputFileName, FileMode.Create);
            StreamWriter sw = null;

            if (args.Length > 1)
            {
                try
                {
                    sw = new StreamWriter(_logFilePath, false);
                    Console.SetOut(sw);
                    bChangedOutStream = true;
                }
                catch (System.IO.IOException e)
                {
                    TextWriter errorWriter = Console.Error;
                    errorWriter.WriteLine(e.Message);
                    errorWriter.WriteLine(sbUsage.ToString());
                    return;
                }
            }

            if(args.Length==0)
            {
                //ask for input/output files
//                Console.Out.Write("\nEnter tab delimited school list file path here:  ");
//                _inputFileName = Console.In.ReadLine();
                Console.Out.Write("\nEnter path for xml school list output file here:  ");
                _outputFilePath = Console.In.ReadLine();

                if (_outputFilePath != null && _outputFilePath.Length > 0) //_inputFileName != null && _inputFileName.Length > 0 && 
                    gotFileNames = true;
                else
                    gotFileNames = false;
            }
            else if((args.Length < 0 || args.Length > 2))
            {
                gotFileNames = false;
            }

            if(gotFileNames)
            {


                Console.WriteLine("Getting XWeb Token...");
                string token = GetXWebToken();
                Console.WriteLine("Attempting to retrieve school list...");
                Thread.Sleep(500);

                result = GetSchoolList(token);

                if (!bChangedOutStream)
                    Console.Clear();

                if (result != null)
                {
                    try
                    {

                        StringBuilder sb = new StringBuilder();
                        List<SchoolEntry> schoolList = new List<SchoolEntry>();
                        System.Xml.XmlNode node = result.FirstChild;
                        while (node != null)
                        {
                            SchoolEntry school = new SchoolEntry();
                            //iterate through child nodes by name to get innertext values
                            if (node.ChildNodes != null && node.ChildNodes.Count > 0)
                            {
                                int index = 0;
                                System.Xml.XmlNode child = node.ChildNodes.Item(index);
                                String OECode = String.Empty, branchCode = String.Empty;

                                while (child != null)
                                {
                                    switch (child.Name)
                                    {
                                        case "org_name":
                                            school.School = child.InnerText;
                                            break;
                                        case "org_school_oe_code_ext":
                                            OECode = child.InnerText;
                                            break;
                                        case "org_school_oe_branch_code_ext":
                                            branchCode = child.InnerText;
                                            break;
                                        case "org_acronym":
                                            school.Alias = child.InnerText;
                                            break;
                                    }

                                    index++;
                                    child = node.ChildNodes.Item(index);
                                }

                                school.SchoolID = OECode + branchCode;
                            }
                            if (!String.IsNullOrWhiteSpace(school.School) && !String.IsNullOrWhiteSpace(school.SchoolID) && school.SchoolID.Length > 5)
                                schoolList.Add(school);
                            node = node.NextSibling;
                        }

                        // create header string
                        string header = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n<!DOCTYPE RECORDS SYSTEM \"records.dtd\">\n<RECORDS>\n";

                        string recordsList = getRecordsListString(schoolList);

                        // create footer string
                        string footer = "</RECORDS>";

                        sb.Append(header);
                        sb.Append(recordsList);
                        sb.Append(footer);

                        // save generated string to output file with UTF-8 encoding
                        UTF8Encoding encoding = new UTF8Encoding();
                        //StreamWriter writer = new StreamWriter(@"C:\DDrive Data\WebIntegration_2.5.1\School_list_4-11-2012.TRANSFORMED.xml", false, encoding);
                        StreamWriter writer = new StreamWriter(_outputFilePath, false, encoding);
                        writer.Write(sb.ToString());
                        writer.Close();
                        Console.WriteLine(_outputFilePath + " generated successfully");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error trying to retrieve school list: {0}", e.Message);
                        if (!bChangedOutStream)
                            System.Console.Read();
                    }
                }
            }
            else
            {
                showUsage(sbUsage.ToString(), !bChangedOutStream);
            }
            if (bChangedOutStream)
                sw.Close();
                
        }

        private static System.Xml.XmlNode GetSchoolList(string sztoken)
        {
            SchoolListTransformer.NetForumXML.netForumXMLSoapClient client = new NetForumXML.netForumXMLSoapClient();
            SchoolListTransformer.NetForumXML.AuthorizationToken token = new NetForumXML.AuthorizationToken();
            String szObjectName = "Organization @TOP -1";
            String szColumnList = "org_name,org_school_oe_code_ext,org_school_oe_branch_code_ext,org_acronym";
            String szWhereClause = "org_ogt_code = 'School' and org_delete_flag = 0";
            String szOrderBy = "org_name ASC";
            System.Xml.XmlNode node = null;

            try
            {
                token.Token = sztoken;

                node = client.GetQuery(ref token, szObjectName, szColumnList, szWhereClause, szOrderBy);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to retrieve school list from Netforum: {0}", e.Message);
                if(!bChangedOutStream)
                    System.Console.Read();
            }
            finally
            {
                if (client.State == System.ServiceModel.CommunicationState.Opened)
                    client.Close();
            }
            return node;
        }

        private static string GetXWebToken()
        {

            SchoolListTransformer.NetForumXML.netForumXMLSoapClient client = new NetForumXML.netForumXMLSoapClient();
            SchoolListTransformer.NetForumXML.AuthorizationToken token = new NetForumXML.AuthorizationToken();
            String AuthResult = String.Empty;

            try
            {
                String xwebUser = ConfigurationManager.AppSettings.Get("XWebUser");
                String xwebPwd = ConfigurationManager.AppSettings.Get("XWebPwd");
                token = client.Authenticate(xwebUser,xwebPwd, out AuthResult);
                return token.Token;
            }
            catch (Exception e)
            {
                Console.Out.Write("Error connecting to XWeb: {0}",e.Message);
                if(!bChangedOutStream)
                    System.Console.Read();
            }
            finally
            {
                if (client.State == System.ServiceModel.CommunicationState.Opened)
                    client.Close();
            }
            return token.Token;
        }
        private static string getRecordsListString(List<SchoolEntry> schoolList)
        {
            //<RECORD>
            //    <PROP NAME="SCHOOL NAME">
            //        <PVAL>Massachusetts Institute of Technology (MIT)</PVAL>
            //    </PROP>
            //    <PROP NAME="SCHOOL ID">
            //        <PVAL>0217800</PVAL>
            //    </PROP>
            //    <PROP NAME="ALIAS">
            //        <PVAL>MIT</PVAL>
            //    </PROP>
            //</RECORD>
            StringBuilder sb = new StringBuilder();
            foreach (SchoolEntry s in schoolList)
            {
                sb.AppendLine("\t<RECORD>");
                GetPropteryString(sb, s.School, "SCHOOL");
                GetPropteryString(sb, s.SchoolID, "SCHOOL ID");

                if (!string.IsNullOrEmpty(s.Alias))
                {
                    GetPropteryString(sb, s.Alias, "ALIAS");
                }

                sb.AppendLine("\t</RECORD>");

            }
            return sb.ToString();
        }

        private static void GetPropteryString(StringBuilder sb, string propteryValue, string propertyName)
        {
            sb.AppendLine("\t\t<PROP NAME=\"" + propertyName + "\">");
            sb.Append("\t\t\t<PVAL>");
            //sb.Append(propteryValue.Replace("&", "and"));
            //sb.Append(SecurityElement.Escape(propteryValue));
            if(propertyName != "SCHOOL ID")
                sb.Append("<![CDATA[");
            sb.Append(propteryValue);
            if (propertyName != "SCHOOL ID")
                sb.Append("]]>");
            sb.Append("</PVAL>\n");
            sb.AppendLine("\t\t</PROP>");
        }

        public static void GetParamsFromCommandLine(string[] args)
        {
            //look to see if user needs help
            foreach (string str in args)
            {
                if (str.Contains("/?") || str.Contains("/help") || str.Contains("-help") || str.Contains("-usage"))
                {
                    //output usage, then exit.
                    showUsage(sbUsage.ToString(), !bChangedOutStream);
                    return;
                }
            }

            if (args.Length > 2)
            {
                System.Console.Out.WriteLine("ERROR: Invalid number of arguements passed in. Number = " + args.Length + " See the following usage:\n\n\n");
                showUsage(sbUsage.ToString(), !bChangedOutStream);
                return;
            }

            //get params from command line
            if(args.Length == 2)
                   _logFilePath = args[1];
            _outputFilePath = args[0];

            return;
        }

        private static void showUsage(string usageText, bool bwaitForResponse)
        {
            if (bwaitForResponse)
                Console.Clear();
            else
                System.Console.WriteLine("");
            System.Console.Out.Write(usageText);
            if(bwaitForResponse)
                System.Console.Read();
        }
    }
}
