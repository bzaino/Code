using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;

namespace ViewValidator
{
    public class Validator
    {
        /// <summary>
        /// I use this error counter so that when there is an exception thrown anywhere, I just move the line "ErrorCounter++" to the method where I can
        /// see the counter increasing, before the failure. Then I put a breakpoint in ViewValidator.cs at line 40 and inspect the value. Once I know 
        /// how many iterations are successful, I can use a conditional breakpoint to stop just before the program crashes.  So this is a handy public 
        /// value to have.
        /// </summary>
        public int ErrorCounter = 0;

        #region Private Members
        private ControllerActionValidator _caValidator = null;
        private Log _logErrors=null, _logUrls=null;
        private string _rootContentPath = null;
        private readonly ReportList _reportList = new ReportList();
        private string _viewPath = null;
        #endregion

        #region Properties
        // ReportList contains the running set of report objects
        public ReportList ReportList { get { return _reportList; } }
        
        // The arguments passed into the Console app
        public string[] Args { get; set; }

        // The root path, above the PublishedContent folder, for the comparison
        public string RootContentPath 
        { 
            get
            {
                if (_rootContentPath == null)
                {
                    AssertNumParams();
                    _rootContentPath = AssertPathValid(Args[0], "First parameter must be a valid root path to the RootContent");
                }
                return _rootContentPath;
            }
        }

        // The PublishedContent folder for the comparison
        public string PublishedContentPath
        {
            get
            {
                return RootContentPath + "\\publishedcontent\\";
            }
        }

        // The path to the Razor files for the comparison
        public string ViewPath
        {
            get
            {
                if (_viewPath == null)
                {
                    AssertNumParams();
                    _viewPath = AssertPathValid(Args[1], "Second parameter must be a valid root path to the Views");
                }
                return _viewPath;
            }
        }

        /// <summary>
        /// This command line argument (the third one) can be an * for all controllers, of comma delimited, such as "Account,Loans"
        /// </summary>
        public string ControllerNamesSelected
        {
            get { return Args[2].ToString().ToLower(); }
        }

        /// <summary>
        /// This command line argument (the fourth one) which contains the path to the primary logfile
        /// </summary>
        public string LogFile
        {
            get
            {
                if (Args.Length >= 4)
                {
                    return Args[3];
                }
                return String.Empty;
            }
        }

        /// <summary>
        /// This command line argument (the fourth one) which contains the path to the logfile for the URL's
        /// </summary>
        public string URLFile
        {
            get
            {
                if (Args.Length == 5)
                {
                    return Args[4];
                }
                return String.Empty;
            }
        }

        /// <summary>
        /// This returns the number of errors found
        /// </summary>
        public int NumErrors
        {
            get
            {
                return _reportList.Count;
            }
        }
        #endregion

        #region Private Helper Methods
        /// <summary>
        /// AssertNumParams() asserts that the number of input params is correct: 3, 4 or 5 and displays an error to the user if an 
        /// unacceptable number is added.
        /// </summary>
        private void AssertNumParams()
        {
            if (Args.Length < 3 || Args.Length > 5)
            {
                throw new Exception("View Validator:\r\n\r\n" +
                                    "Enter three, four or five arguments:\r\n" +
                                    "1) RootContent folder\r\n" +
                                    "2) Views folder\r\n" +
                                    "3) ControllerNames (comma separated, * for All Controllers)\r\n" +
                                    "4) Logfile path (optional)\r\n" +
                                    "5) URL Logfile path (optional)");
            }
        }

        /// <summary>
        /// AssertPathValid() asserts that the path is valid. In the case that it is not, AssertPathValid() logs the error 
        /// with the errorMessage input param, and returns null so that the path is not used.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        private string AssertPathValid(string path, string errorMessage)
        {
            if (!String.IsNullOrEmpty(path))
            {
                // replaces the UNIX / with a DOS \
                path = path.Replace('/', '\\');
                if (System.IO.Directory.Exists(path))
                {
                    return path.ToLower().TrimEnd('\\') + '\\';
                }
            }
            _reportList.Add(new ReportMissingFile(ReportType.MissingFolder, path, errorMessage, true));
            return null;
        }

        /// <summary>
        /// GetFolder() returns the folder (or folders) beyond the root, in the case that path is inside root.  In the case 
        /// that it is not, AssertPathValid() logs the error with the errorMessage input param, and returns null so 
        /// that the path is not used.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="root"></param>
        /// <returns></returns>
        private string GetFolder(string path, string root)
        {
            // returns "abc" from .../RootContent/abc
            if (path.IndexOf(root) > -1)
            {
                return TestSubString(path, path.IndexOf(root) + root.Length);
            }
            _reportList.Add(new ReportMissingFile(ReportType.MissingFolder, path, "Path does not contain " + root, true));
            return null;
        }

        /// <summary>
        /// GetXmlFilename() returns the XML file that matches the View file, but also logs the error and returns null when the file is not found 
        /// </summary>
        /// <param name="viewFileNameAndPath"></param>
        /// <returns></returns>
        private string GetXmlFilename(string viewFileNameAndPath)
        {
            if (viewFileNameAndPath == "_ViewStart.cshtml".ToLower())
            {
                return null;
            }

            // Get corresponding XML file but first remove "overlay\\" from XML path
            string xmlPath1 = viewFileNameAndPath;
            if (xmlPath1.IndexOf("overlay\\") > -1)
            {
                xmlPath1 = xmlPath1.Remove(xmlPath1.IndexOf("overlay\\"), "overlay\\".Length);
            }
            // Replaces the Razor file extension with an XML file extension
            xmlPath1 = xmlPath1.Replace(".cshtml", ".xml");

            // This is the case when there's a perfect match between XML and Razor filenames
            if (File.Exists(PublishedContentPath + xmlPath1))
            {
                return xmlPath1;
            }

            // No point in trying the following replacement test, so log the error and ditch
            if ( xmlPath1.ToLower().EndsWith("index.xml"))
            {
                _reportList.Add(new ReportMissingFile(ReportType.MissingXmlFile, xmlPath1, "XML file not found", true));
                return null;
            }

            // Test with Index.xml instead, when the Controller name matches the Raxor filename
            string xmlPath2 = xmlPath1;
            string xmlFileName = null;
            string revisedPath = null;
            try
            {
                // Get the Razor File name
                xmlFileName = xmlPath1.Substring(xmlPath1.LastIndexOf('\\') + 1);

                // And the shortened path, in order get the Controller name
                revisedPath = xmlPath1.Substring(0, xmlPath1.LastIndexOf('\\')).TrimEnd('\\');
            }
            catch (Exception ex)
            {
                throw ex;
            }

            // Use Index.xml when the Razor filename matches the Controller name
            string controllerName = revisedPath.Substring(0, xmlPath1.LastIndexOf('\\'));
            if (xmlFileName.StartsWith(controllerName) || controllerName == "loans")
            {
                xmlPath2 = revisedPath + "\\Index.xml";
            }
            else
            {
                xmlPath2 = xmlPath1;
            }

            if (File.Exists(PublishedContentPath + xmlPath2))
            {
                return xmlPath2;
            }

            // Log the errors and return null
            _reportList.Add(new ReportMissingFile(ReportType.MissingXmlFile, xmlPath1, "XML file not found", true));
            if (xmlPath1 != xmlPath2)
            {
                _reportList.Add(new ReportMissingFile(ReportType.MissingXmlFile, xmlPath2, "XML file not found", true));
            }

            return null;
        }

        /// <summary>
        /// This file return the Razor file that matches the XML file, but also logs the error and return null when not found 
        /// </summary>
        /// <param name="xmlFileNameAndPath"></param>
        /// <returns></returns>
        private string GetViewFilename(string xmlFileNameAndPath)
        {
            xmlFileNameAndPath = xmlFileNameAndPath.ToLower();

            // strip "-overlay" out of the name, since the Razor files do not use this format
            if (xmlFileNameAndPath.IndexOf("-overlay") > -1)
            {
                xmlFileNameAndPath.Replace("-overlay", "");
            }

            // strip "GlobalBriefs, repay and advertisingmodules" out of xmlFileNameAndPath, since the Razor files do not use this format
            if (xmlFileNameAndPath.IndexOf('-') > -1 || // the dash denotes an XML file for the General Content Razor page
                xmlFileNameAndPath.IndexOf("GlobalBriefs".ToLower()) > -1 ||
                xmlFileNameAndPath.IndexOf(@"repay\\") > -1 ||
                xmlFileNameAndPath.IndexOf(@"advertisingmodules\\") > -1 )
            {
                return null;
            }

            // Get corresponding View file by inserting "overlay\\" when needed
            string viewPath = xmlFileNameAndPath.Replace(".xml", ".cshtml");
            string pathWithOverlay = null;

            // First check for the Razor file in the Controller's root folder
            if (File.Exists(ViewPath + viewPath))
            {
                return viewPath;
            }

            // Then try inserting "overlay" for three folders which contain Overlays
            if (viewPath.ToLower().IndexOf("account") > -1 ||
                viewPath.ToLower().IndexOf("loans") > -1 ||
                viewPath.ToLower().IndexOf("paymentreminder") > -1)
            {
                // Insert overlay and check if file exists
                pathWithOverlay = viewPath.Insert(viewPath.LastIndexOf('\\'), "\\overlay");
                if (File.Exists(ViewPath + pathWithOverlay))
                {
                    return pathWithOverlay;
                }
                else
                {
                    if (pathWithOverlay.IndexOf("paymentreminder") > -1 && pathWithOverlay.EndsWith("index.cshtml"))
                    {
                        pathWithOverlay = pathWithOverlay.Replace("index", "paymentreminder");
                        if (File.Exists(ViewPath + pathWithOverlay))
                        {
                            return pathWithOverlay;
                        }
                    }
                }
            }

            // Log the errors and return null
            _reportList.Add(new ReportMissingFile(ReportType.MissingRazorFile, viewPath, "Razor View file not found", true));
            if (pathWithOverlay != null)
            {
                _reportList.Add(new ReportMissingFile(ReportType.MissingRazorFile, pathWithOverlay, "Razor View file not found", true));
            }

            return null;
        }

        /// <summary>
        /// FoldersSelected() checks if the third param is a * and returns true since that means use all Controller. Otherwise it parses the 
        /// third parm to see if the Conntroller defined by folderName is in the comma separated list.
        /// </summary>
        /// <param name="folderName"></param>
        /// <returns></returns>
        private bool FoldersSelected(string folderName)
        {
            // The user has entered to search on all controllers
            if (ControllerNamesSelected == "*")
            {
                return true;
            }

            var isFolderFound = false;
            var controllerNames = ControllerNamesSelected.Split(',');
            foreach (string controllerName in controllerNames)
            {
                isFolderFound |= folderName.ToLower().IndexOf(controllerName) > -1;
            }
            return isFolderFound;
        }
        #endregion

        #region Constructor(s)
        public Validator()
        {
        }

        /// <summary>
        /// SetParams handles construction of the Validator object. At the time, it was easier than using the Constructor for this. But at the moment I forget why?
        /// </summary>
        /// <param name="args"></param>
        public void SetParams(string[] args)
        {
            Args = args;

            var file1 = new StreamWriter("c:\\tmp\\XmlTags.txt", false);
            file1.Close();

            var file2 = new StreamWriter("c:\\tmp\\ViewTags.txt", false);
            file2.Close();

            _logErrors = new Log(LogFile);
            if (!String.IsNullOrEmpty(URLFile))
            {
                _logUrls = new Log(URLFile);
            }
        }
        #endregion

        #region Exposed Functions
        /// <summary>
        /// Validate() is the primary function in the program and executes the entire comparison between the XML and Views folders selected.
        /// </summary>
        /// <returns></returns>
        internal int Validate()
        {
            if (RootContentPath == null)
            {
                throw new Exception("First parameter must be a valid root path to the RootContent");
            }
            if (ViewPath == null)
            {
                throw new Exception("Second parameter must be a valid root path to the Views");
            }
            AssertFolderContents();
            CheckForOrphanedXmlTagsInFolder(PublishedContentPath);
            CheckForOrphanedViewTagsInFolder(ViewPath);
            VerifyUrlsResolve();
            OutputExcelReports();
            if (_logUrls != null)
            {
                OutputUrlReports();
            }
            return _reportList.Count();
        }
        #endregion

        #region Private Members
        /// <summary>
        /// AssertFolderContents() asserts that all folders contained in PublishedContent_ActiveFolders.txt are found and logs an error when not found.
        /// It also asserts that no folder found in PublishedContent_DeletedFolders.txt exists and logs an error if found.
        /// </summary>
        private void AssertFolderContents()
        {
            // PublishedContent_ActiveFolders.txt
            var publishedContentActiveTree = new FolderValidator(FolderValidator.FolderStatus.Exists, "PublishedContent_ActiveFolders.txt", PublishedContentPath, _reportList);
            publishedContentActiveTree.Validate();

            // TODO: Replace this list of known deleted folders with a broad search for any folder (or file) found unexpectedly and log those as errors.
            // Assert that folder in PublishedContent_DeletedFolders.txt are deleted
            var publishedContentInactiveTree = new FolderValidator(FolderValidator.FolderStatus.DoesNotExist, "PublishedContent_DeletedFolders.txt", PublishedContentPath, _reportList);
            publishedContentInactiveTree.Validate();

            // RazorViews.txt
            var razorViewsTree = new FolderValidator(FolderValidator.FolderStatus.Exists, "RazorViews.txt", ViewPath, _reportList);
            razorViewsTree.Validate();

            _caValidator = new ControllerActionValidator(razorViewsTree.FolderTreePathAndFile, ViewPath, _reportList);
        }

        /// <summary>
        /// CheckForOrphanedViewTagsInFolder() is a recursive function which checks for Orphaned View Tags in the rootFolder and
        /// every folder beneath it.
        /// </summary>
        /// <param name="rootFolder"></param>
        private void CheckForOrphanedViewTagsInFolder(string rootFolder)
        {
            // Get all folders inside rootFolder and call this function recursively for each sub-folder
            string[] subdirEntries = Directory.GetDirectories(rootFolder);
            foreach (string subdir in subdirEntries)
            {
                // this is the call to itself for a nested subdirectory
                CheckForOrphanedViewTagsInFolder(subdir);
            }

            // Then process each Razor file found in the folder
            string[] fileEntries = Directory.GetFiles(rootFolder, "*.cshtml");
            foreach (string fileName in fileEntries)
            {
                if (FoldersSelected(rootFolder))
                {
                    CheckForOrphanedViewTagsInView(fileName.ToLower());
                }
            }
        }

        /// <summary>
        /// This creates the primary report
        /// </summary>
        private void OutputExcelReports()
        {
            foreach (Report report in _reportList)
            {
                if (report.ReportType != ReportType.UrlRazor && report.ReportType != ReportType.UrlXml)
                {
                    _logErrors.LogReport(report);
                }
            }
        }

        /// <summary>
        /// This creates the URL report
        /// </summary>
        private void OutputUrlReports()
        {            
            foreach (Report report in _reportList)
            {
                if (report.ReportType == ReportType.UrlRazor || report.ReportType == ReportType.UrlXml)
                {
                    _logUrls.LogReport(report);
                }
            }
        }

        /// <summary>
        /// This uses the ReportType and ReportMessage properties to test if the URL's resolve
        /// </summary>
        private void VerifyUrlsResolve()
        {
            int index = 0;
            ErrorCounter = 0;
            while (index < _reportList.Count)
            {
                ErrorCounter++;
                Report report = _reportList[index++];
                if (report.ReportType == ReportType.UrlRazor || report.ReportType == ReportType.UrlXml )
                {
                    report.PathResolves = FilterAndResolveUrl(report.ReportMessage, report.Path);
                }
            }
        }

        /// <summary>
        /// FilterAndResolveUrl() gets the URL to test (inUrl) and based on the type of URL, may attempt to resolve the URL.
        /// </summary>
        /// <param name="inUrl"></param>
        /// <param name="sourceFile"></param>
        /// <returns></returns>
        private PathResolves FilterAndResolveUrl(string inUrl, string sourceFile)
        {
            string url = inUrl.ToLower();

            // This can resolve:
            // ../PublishedContent/images/global/ajax-loader-2.gif
            // depending on the current directory. But I chose to not test it
            if (url.StartsWith("../P"))
            {
                return PathResolves.Untested;
            }

            if ( url.EndsWith(".php"))
            {
                return PathResolves.Fails;
            }

            // TODO: Deal with this kind of link
            if (url[0] == '#')
            {
                // verify the url is found [only once] in the page pointed to 
                return PathResolves.Untested;
            }
                 
            // This tests for JavaScript URL's
            if (url.IndexOf("javascript") > -1)
            {
                return PathResolves.Untested;
            }

            // This tests if the URL points to genuine web URL
            if (IsWebUrl(url))
            {
                if (WebUrlResolves(url))
                {
                    return PathResolves.Resolves;
                }
                return PathResolves.Fails;
            }

            // This tests if the URL points to an email address
            if (IsEmailAddress(url))
            {
                return PathResolves.Untested;
            }

            // This tests if the URL points to a Razor View
            if (_caValidator.IsControllerActionValid(url, sourceFile) == PathResolves.Resolves)
            {
                return PathResolves.Resolves;
            }

            // Otherwise see if the file exists.
            // replaces the UNIX / with a DOS \
            url = url.Replace('/', '\\').TrimStart('~');
            if (File.Exists(RootContentPath+url))
            {
                return PathResolves.Resolves;
            }

            return PathResolves.Fails;
        }

        /// <summary>
        /// IsWebUrl() uses the RegEx to find is the url parameter is a valid web URL
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        protected bool IsWebUrl(string url)
        {
            string urlToTest = url;
            return Regex.IsMatch(urlToTest, @"^(https?://)?([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?$");
        }

        /// <summary>
        /// IsEmailAddress() uses the RegEx to find is the url parameter is a valid email address
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        protected bool IsEmailAddress(string url)
        {
            string urlToTest = url;
            if (url.ToLower().StartsWith("mailto:"))
            {
                urlToTest = url.Substring("mailto:".Length);
            }

            return Regex.IsMatch(urlToTest, @"^(([a-zA-Z0-9_\-\.]+)@([a-zA-Z0-9_\-\.]+)\.([a-zA-Z]{2,5}){1,25})+([;.](([a-zA-Z0-9_\-\.]+)@([a-zA-Z0-9_\-\.]+)\.([a-zA-Z]{2,5}){1,25})+)*$");
        }

        /// <summary>
        /// WebUrlResolves() attempts to connect to the validated Web URL and returns success for a response.StatusCode of 400 or less.
        /// Otherwise, it logs errors and exceptions, but does not throw exceptions.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private bool WebUrlResolves(string url)
        {
            try
            {
                HttpWebRequest request = HttpWebRequest.Create(url) as HttpWebRequest;
                request.Timeout = 5000; //set the timeout to 5 seconds to keep the user from waiting too long for the page to load
                request.Method = "HEAD"; //Get only the header information -- no need to download any content

                HttpWebResponse response = request.GetResponse() as HttpWebResponse;

                int statusCode = (int)response.StatusCode;
                if (statusCode >= 100 && statusCode < 400) //Good requests
                {
                    return true;
                }
                if (statusCode >= 500 && statusCode <= 510) //Server Errors
                {
                    _reportList.Add(new ReportTagError("Url test failed", ReportType.Exception, url, String.Format("The remote server has thrown an internal error. Url is not valid: {0}", url), true));
                }
            }
            catch (WebException ex)
            {
                _reportList.Add(new ReportTagError("Url test failed - " + ex.Status.ToString(), ReportType.Exception, url, String.Format("Unhandled status [{0}] returned for url: {1}", ex.Status, url), true));
            }
            catch (Exception ex)
            {
                _reportList.Add(new ReportTagError("Url test failed", ReportType.Exception, url, String.Format("Could not test url {0}.", url), true));
            }
            return false;
        }

        /// <summary>
        /// NumMatches() uses LINQ to compare the contents of the WordListXml and WordListRazor input parameters and returns 
        /// the number of distinct matches
        /// </summary>
        /// <param name="xmlWords"></param>
        /// <param name="viewWords"></param>
        /// <returns></returns>
        protected int NumMatches(WordListXml xmlWords, WordListRazor viewWords)
        {
            return (from l1 in xmlWords.Words
                    join l2 in viewWords.Words
                    on l1 equals l2
                    select l1).Distinct().Count();
        }

        /// <summary>
        /// IsMatch() compares the contents of the WordListXml and WordListRazor to find the number of matches,
        /// then uses a hard coded algorithm that finds a match if at least 65% of the words in the WordListRazor
        /// have a match in the WordListXml. This formula was the first draft and never tested rigorously, which is why I
        /// have a TODO: there, to revisit the factor.
        /// </summary>
        /// <param name="xmlWords"></param>
        /// <param name="viewWords"></param>
        /// <returns></returns>
        private bool IsMatch(WordListXml xmlWords, WordListRazor viewWords)
        {
            int numMatches = NumMatches(xmlWords, viewWords);
            // TODO: Refine this algorithm
            if (numMatches > viewWords.Words.Count * 0.65)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// CheckXmlNodeForUrl() checks the InnerXml of the XmlNode input param for URL's following the "href=" and "src=" and 
        /// adds those that are found to the ReportList for later testing, by calling ProcessXmlStr.
        /// </summary>
        /// <param name="xmlFile"></param>
        /// <param name="node"></param>
        private void CheckXmlNodeForUrl(string xmlFile, XmlNode node)
        {
            try
            {
                string textToParse = node.Value;
                if (String.IsNullOrEmpty(node.Value))
                {
                    textToParse = node.InnerXml;
                }
                ProcessXmlStr(xmlFile, textToParse, "href=");
                ProcessXmlStr(xmlFile, textToParse, "src=");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// ProcessXmlStr() checks the innerHtml input param for URL's including the searchPattern "href=" and "src=", then 
        /// calls LogXmlUrl for each which match.
        /// </summary>
        /// <param name="xmlFile">For logging purposes</param>
        /// <param name="innerHtml">contains the entire URL</param>
        /// <param name="searchPattern">contains href= or src=</param>
        private void ProcessXmlStr(string xmlFile, string innerHtml, string searchPattern)
        {
            if (!String.IsNullOrEmpty(innerHtml))
            {
                string modifiedStr = innerHtml;
                int count = 0;
                while (modifiedStr != null && modifiedStr.IndexOf(searchPattern) > -1)
                {
                    modifiedStr = LogXmlUrl(xmlFile, modifiedStr, searchPattern);
                    count++;
                }
            }
        }

        /// <summary>
        /// LogXmlUrl() gets a cleaned version of the URL by calling TakeUrl(). If null, it exists the function. If a URL is found
        /// it cleans it further of the end tags, updating the delimiter field to record whether the URL was properly encapsulated. It adds 
        /// a new ReportUrlValidation object to ReporList set to PathResolves.Untested, since it will be validated later in the processing.
        /// </summary>
        /// <param name="xmlFile"></param>
        /// <param name="searchStr"></param>
        /// <param name="searchPattern">contains href= or src=</param>
        /// <returns>searchStr minus the discoverd URL. This allows the LogXmlUrl to be called repeatedly for a string which has multiple URL's.</returns>
        private string LogXmlUrl(string xmlFile, string searchStr, string searchPattern)
        {
            string returnStr = null;
            string cleanUrlStr = TakeUrl(searchStr, searchPattern);
            if (cleanUrlStr != null)
            {
                string openingTag = "|%";
                string closingTag = "%|";
                int openingDelimiterIndex = cleanUrlStr.IndexOf(openingTag);
                int closingingDelimiterIndex = cleanUrlStr.IndexOf(closingTag);
                string delimitedUrl = cleanUrlStr;
                string delimiterFlag = string.Empty;
                if (cleanUrlStr.ToLower().IndexOf(".php") > -1 )
                {
                    delimiterFlag += "PPH!!!";
                }
                int firstPatternIndex = searchStr.IndexOf(searchPattern);
                if (openingDelimiterIndex < closingingDelimiterIndex && openingDelimiterIndex > -1)
                {
                    cleanUrlStr = cleanUrlStr.Trim('|').Trim('%');
                }
                else
                {
                    delimiterFlag += delimiterFlag.Length > 0 ? " - " : "";
                    delimiterFlag += "|% no delimiters %|";
                }

                _reportList.Add(new ReportUrlValidation(delimiterFlag, ReportType.UrlXml, xmlFile, PathResolves.Untested, cleanUrlStr, false));

                string part1 = searchStr.Substring(0, firstPatternIndex);
                try
                {
                    string part2 = searchStr.Substring(firstPatternIndex + searchPattern.Length + delimitedUrl.Length);
                    returnStr = part1 + part2;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return returnStr;
        }

        /// <summary>
        /// CheckRazorStringForUrl() checks the searchStr input param for URL's following the "href=" and "src=" tags and 
        /// adds those that are found to the ReportList for later testing, by calling ProcessRazorStr.
        /// </summary>
        /// <param name="viewFile"></param>
        /// <param name="searchStr"></param>
        private void CheckRazorStringForUrl(string viewFile, string searchStr)
        {
            try
            {
                ErrorCounter++;

                ProcessRazorStr(viewFile, searchStr, "href=");
                ProcessRazorStr(viewFile, searchStr, "src=");
            }
            catch (Exception ex)
            {               
                throw ex;
            }
        }

        /// <summary>
        /// ProcessRazorStr() checks the searchStr input param for URL's including the searchPattern "href=" and "src=", then 
        /// calls LogRazorUrl for each which match.
        /// </summary>
        /// <param name="viewFile">For logging purposes</param>
        /// <param name="searchStr">contains the entire URL</param>
        /// <param name="searchPattern">contains href= or src=</param>
        private void ProcessRazorStr(string viewFile, string searchStr, string searchPattern)
        {
            if (!String.IsNullOrEmpty(searchStr))
            {
                string modifiedStr = searchStr;
                while (modifiedStr != null && modifiedStr.IndexOf(searchPattern) > -1)
                {
                    modifiedStr = LogRazorUrl(viewFile, modifiedStr, searchPattern);
                }
            }
        }

        /// <summary>
        /// LogRazorUrl() gets a cleaned version of the URL by calling TakeUrl(). If null, it exists the function. If a URL is found
        /// it cleans it further of the end tags, updating the delimiter field to record whether the URL was properly encapsulated. It adds 
        /// a new ReportUrlValidation object to ReporList set to PathResolves.Untested, since it will be validated later in the processing.
        /// </summary>
        /// <param name="viewFile"></param>
        /// <param name="searchStr"></param>
        /// <param name="searchPattern">contains href= or src=</param>
        /// <returns>searchStr minus the discoverd URL. This allows the LogRazorUrl to be called repeatedly for a string which has multiple URL's.</returns>
        private string LogRazorUrl(string viewFile, string searchStr, string searchPattern)
        {
            string returnStr = null;
            int searchPatternIndex = searchStr.IndexOf(searchPattern);
            bool isUrlContentFunctionFound = searchStr.IndexOf(searchPattern + "\"@Url.Content(") == searchPatternIndex;
            if (isUrlContentFunctionFound)
            {
                string fullSearchPatternStr1 = searchPattern + "\"@Url.Content(";
                searchStr = searchStr.Substring(0, searchStr.IndexOf(searchPattern) + searchPattern.Length) + "" + searchStr.Substring(searchStr.IndexOf(fullSearchPatternStr1) + fullSearchPatternStr1.Length);
            }
            string cleanUrlStr = TakeUrl(searchStr, searchPattern);
            if (cleanUrlStr != null)
            {
                string delimiterFlag = string.Empty;
                if (cleanUrlStr.ToLower().IndexOf(".php") > -1)
                {
                    delimiterFlag += "PPH!!!";
                }
                bool javascript = cleanUrlStr.IndexOf("javascript") > -1 || cleanUrlStr.IndexOf(".js") > -1;
                bool logJavascriptUrls = true;
                if ( javascript )
                {
                    if ( !logJavascriptUrls )
                    {
                        cleanUrlStr = null;
                    }
                }
                else
                {
                    if (!isUrlContentFunctionFound)
                    {
                        delimiterFlag += delimiterFlag.Length > 0 ? " - " : "";
                        delimiterFlag += "No @Url.Content() used ";
                    }
                }

                if (cleanUrlStr != null)
                {
                    _reportList.Add(new ReportUrlValidation(delimiterFlag, ReportType.UrlRazor, viewFile, PathResolves.Untested, cleanUrlStr, false));
                }

                string fullSearchPatternStr2 = searchPattern + '\"' + cleanUrlStr;
                returnStr = searchStr.Substring(searchStr.IndexOf(searchPattern) + searchPattern.Length + 1);
                returnStr = returnStr.Substring(returnStr.IndexOf(cleanUrlStr) + cleanUrlStr.Length + 1);
            }
            return returnStr;
        }

        /// <summary>
        /// TakeUrl grabs the URL from the string if it's found after the href= or src= string
        /// </summary>
        /// <param name="searchStr"></param>
        /// <param name="pattern">contains href= or src=</</param>
        /// <returns></returns>
        private string TakeUrl(string searchStr, string pattern)
        {
            string url = String.Empty;
            int patternIndex = searchStr.IndexOf(pattern);
            if (patternIndex > -1)
            {
                string urlStr = searchStr.Substring(patternIndex + pattern.Length + 1);
                int doubleQuoteIndex = urlStr.IndexOf('"');
                int singleQuoteIndex = urlStr.IndexOf('\'');
                char endChar = '\"';
                if (singleQuoteIndex > -1 && ((singleQuoteIndex < doubleQuoteIndex) || doubleQuoteIndex == -1 ))
                {
                    endChar = '\'';
                }
                return urlStr.Substring(0, urlStr.IndexOf(endChar));
            }
            return null;
        }

        private int CheckRazorForXmlHardCoding(string xmlFile, string viewFile)
        {
            int xmlNodeCount = 0;
            int razorLineCount = 0;
            int hardCodedTagsFound = 0;
            try
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.Load(PublishedContentPath + xmlFile);
                XmlNode rootNode = xmlDoc.SelectSingleNode("//content-item");
                List<XmlNode> nodeList = new List<XmlNode>();
                nodeList = FillNodeList(rootNode, nodeList);

                foreach (XmlNode node in nodeList)
                {
                    xmlNodeCount++;
                    var xmlWords = new WordListXml(node, ' ');
                    CheckXmlNodeForUrl(xmlFile, node);
                    var xmlTag = node.LocalName;
                    xmlTag = node.Name;
                    string line;
                    var file = new System.IO.StreamReader(ViewPath + viewFile);
                    while ((line = file.ReadLine()) != null)
                    {
                        razorLineCount++;
                        var viewWords = new WordListRazor(line, ' ');
                        CheckRazorStringForUrl(viewFile, line);
                        if (xmlWords.Words.Count > 0 && viewWords.Words.Count > 0 && IsMatch(xmlWords, viewWords))
                        {
                            hardCodedTagsFound++;
                            _reportList.Add(new ReportTagError(xmlTag, ReportType.HardCodedContent, viewFile, "Xml content for '" + xmlTag + "' found in the Razor file. " + viewFile, false));
                        }
                    }
                    file.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return hardCodedTagsFound;
        }

        /// <summary>
        /// This recursive function gets an XmlNode and an empty list, of the first call. It calls itself for each child node found,
        /// for the node passed in. In the case there are no child nodes it adds the node param to inNodeList. Since it's recursive,
        /// the combined calls to FillNodeList return a List<XmlNode> for every node in that Xml tree topped by the original XmlNode
        /// passed in the first call to FillNodeList.
        /// /// </summary>
        /// <param name="node"></param>
        /// <param name="inNodeList"></param>
        /// <returns></returns>
        protected List<XmlNode> FillNodeList(XmlNode node, List<XmlNode> inNodeList)
        {
            if (node != null)
            {
                if (node.HasChildNodes)
                {
                    foreach (XmlNode childNode in node.ChildNodes)
                    {
                        FillNodeList(childNode, inNodeList);
                    }
                }
                else
                {
                    // I verified that filtering on CDATA was okay, since this function is used only when looking for English words contained in the XML
                    if (node.NodeType == XmlNodeType.CDATA )
                    {
                        inNodeList.Add(node);
                    }                    
                }
            }

            return inNodeList;
        } 

        /// <summary>
        /// CompareLists compares the XML tags found in the XML files and used in the Razor files
        /// </summary>
        /// <param name="xmlFile"></param>
        /// <param name="viewFile"></param>
        /// <param name="xmlList"></param>
        /// <param name="viewList"></param>
        /// <param name="name"></param>
        private void CompareLists(string xmlFile, string viewFile, IList<string> xmlList, IList<string> viewList, string name)
        {
            int defectsFound = -1;
            try
            {
                // This make an additional call to find if there is any XML hard coding in the viewFile
                defectsFound = CheckRazorForXmlHardCoding(xmlFile, viewFile);
                int xmlTagsFound = 0;

                // First, test that each Xml tag is used in the View
                foreach (string xmlTag in xmlList)
                {
                    xmlTagsFound++;
                    // TODO: Remove this hard coded URL or simply remove the 3 lines of code. They write each of the XML tags in xmlList to a textfile
                    var file = new StreamWriter("c:\\tmp\\XmlTags.txt", true);
                    file.WriteLine(xmlTag);
                    file.Close();

                    if (viewList.IndexOf(xmlTag) == -1)
                    {
                        defectsFound++;
                        _reportList.Add(new ReportTagError(xmlTag, ReportType.OrphanedXmlTag, viewFile, "Xml tag '" + xmlTag + "' found in " + xmlFile + " but not found in the View", false));
                    }
                }

                int viewTagsFound = 0;
                // Then, test that each View tag matches one in the Xml list
                foreach (string viewTag in viewList)
                {
                    viewTagsFound++;
                    // TODO: Remove this hard coded URL or simply remove the 3 lines of code. They write each of the XML tags in viewList to a textfile
                    var file = new StreamWriter("c:\\tmp\\ViewTags.txt", true);
                    file.WriteLine(viewTag);
                    file.Close();

                    if (xmlList.IndexOf(viewTag) == -1)
                    {
                        defectsFound++;
                        _reportList.Add(new ReportTagError(viewTag, ReportType.OrphanedRazorTag, xmlFile, "View tag '" + viewTag + "' found in " + viewFile + " but not found in the Xml", false));
                    }
                }
                if (viewTagsFound == 0)
                {
                    _reportList.Add(new ReportTagError("None found", ReportType.NoRazorTagsFound, viewFile, "No Identifiers found in Razor file", true));
                }
                else
                {
                    if (defectsFound == 0)
                    {
                        _reportList.Add(new ReportSuccess(ReportType.SuccessConfirmation,
                                                          "Successful " + name + " integration of " + xmlFile + " into " +
                                                          viewFile, name == "Widgets"));
                    }
                }
            }
            catch (Exception ex)
            {
                defectsFound = CheckRazorForXmlHardCoding(xmlFile, viewFile);
                throw ex;
            }
        }

        #endregion

        #region OrphanedViewTags
        /// <summary>
        /// CheckForOrphanedViewTagsInView() does the heavy lifting in finding and logging Orphaned View Tags In View. These are tags used
        /// in the View that do not have a corresponding match in the XML file with which its associated
        /// </summary>
        /// <param name="fileNameAndPath"></param>
        private void CheckForOrphanedViewTagsInView(string fileNameAndPath)
        {
            string viewFileName = null;
            string xmlFileName = null;
            try
            {
                string viewPathAndFileName = GetFolder(fileNameAndPath, ViewPath);
                string xmlPathAndFileName = GetXmlFilename(viewPathAndFileName);

                if (!String.IsNullOrEmpty(viewPathAndFileName) && !String.IsNullOrEmpty(xmlPathAndFileName))
                {
                    viewFileName = TestSubString(viewPathAndFileName, viewPathAndFileName.LastIndexOf('\\') + 1);
                    xmlFileName = TestSubString(xmlPathAndFileName, xmlPathAndFileName.LastIndexOf('\\') + 1);

                    // Read from the Razor View
                    var viewMessagesList = FillListFromView(ViewPath + viewPathAndFileName, ".Messages");
                    var viewLabelsList = FillListFromView(ViewPath + viewPathAndFileName, ".Labels");
                    var viewWidgetNameList = FillListFromView(ViewPath + viewPathAndFileName, ".Widgets");

                    // Read from the XML file
                    var xmlMessageNameList = FillListFromXml(PublishedContentPath + '\\' + xmlPathAndFileName, "<message name=");
                    var xmlLabelsList = FillListFromXml(PublishedContentPath + '\\' + xmlPathAndFileName, "<label name=");
                    var xmlWidgetNameList = FillListFromXml(PublishedContentPath + '\\' + xmlPathAndFileName, "<widget name=");
                    // TODO: investigate whether "<list name=" needs to be included

                    // Assert
                    if (viewMessagesList != null && viewLabelsList != null && viewWidgetNameList != null &&
                        xmlMessageNameList != null && xmlLabelsList != null && xmlWidgetNameList != null && FoldersSelected(fileNameAndPath))
                    {
                        if (!AreListsEqual(viewWidgetNameList, xmlWidgetNameList))
                        {
                            string tagList = ConcatenateTags(viewWidgetNameList, xmlWidgetNameList);
                            _reportList.Add(new ReportTagError(tagList, ReportType.WidgetNameMismatch, viewFileName, "Widget name error with Xml:" + xmlFileName, false));
                        }

                        CompareLists(xmlPathAndFileName, viewPathAndFileName, xmlMessageNameList, viewMessagesList, "Messages");
                        CompareLists(xmlPathAndFileName, viewPathAndFileName, xmlLabelsList, viewLabelsList, "Labels");
                        CompareLists(xmlPathAndFileName, viewPathAndFileName, xmlWidgetNameList, viewWidgetNameList, "Widget");
                    }
                }
            }
            catch (Exception ex)
            {
                _reportList.Add(new ReportTagError("Exception caught in CheckForOrphanedViewTagsInView()", ReportType.Exception, viewFileName, ex.Message, true));
            }
        }

        /// <summary>
        /// FillListFromView fills the matchList of all lines matching the pattern input parameter
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        protected IList<string> FillListFromView(string filePath, string pattern)
        {
            var matchList = new List<string>();
            try
            {
                string line;
                var file = new System.IO.StreamReader(filePath);
                while ((line = file.ReadLine()) != null)
                {
                    var regex = new Regex(pattern);
                    if (regex.IsMatch(line))
                    {
                        string tag = ExtractViewTagName(line, pattern);
                        if (tag != String.Empty && matchList.IndexOf(tag) == -1)
                        {
                            matchList.Add(tag);
                        }
                    }
                }
                file.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return matchList;
        }

        /// <summary>
        /// ExtractViewTagName gets the View tag contained in line matching pattern
        /// </summary>
        /// <param name="line"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        private string ExtractViewTagName(string line, string pattern)
        {
            string viewTagName = String.Empty;
            int openBracketIndex = line.IndexOf(pattern + '[') + pattern.Length;
            if ( openBracketIndex > -1)
            {
                string subStr = TestSubString(line, openBracketIndex + 2);
                int closeBracketIndex = subStr.IndexOf(']');
                if (closeBracketIndex > -1)
                {
                    viewTagName = TestSubString(subStr, 0, closeBracketIndex - 1);
                }
            }
            return viewTagName;
        }
        #endregion

        #region OrphanedXmlTags
        /// <summary>
        /// CheckForOrphanedXmlTagsInFolder() is a recursive function which checks for Orphaned Xml Tags in the rootFolder and
        /// every folder beneath it.
        /// </summary>
        /// <param name="rootFolder"></param>
        private void CheckForOrphanedXmlTagsInFolder(string rootFolder)
        {
            // Get all folders inside rootFolder and call this function recursively for each sub-folder
            string[] subdirEntries = Directory.GetDirectories(rootFolder);
            foreach (string subdir in subdirEntries)
            {
                CheckForOrphanedXmlTagsInFolder(subdir);
            }

            // Then process each Razor file found in the folder
            string[] fileEntries = Directory.GetFiles(rootFolder, "*.xml");
            foreach (string fileName in fileEntries)
            {
                if (FoldersSelected(rootFolder))
                {
                    CheckForOrphanedXmlTagsInView(fileName.ToLower());
                }
            }

        }

        /// <summary>
        /// CheckForOrphanedXmlTagsInView() does the heavy lifting in finding and logging Orphaned Xml Tags In View. These are tags used
        /// in the Xml that do not have a corresponding match in the View file with which its associated
        /// </summary>
        /// <param name="fileNameAndPath"></param>
        private void CheckForOrphanedXmlTagsInView(string fileNameAndPath)
        {
            string viewFileName = null;
            string xmlFileName = null;
            try
            {
                string xmlPathAndFileName = GetFolder(fileNameAndPath, PublishedContentPath);
                string viewPathAndFileName = GetViewFilename(xmlPathAndFileName);

                if (!String.IsNullOrEmpty(xmlPathAndFileName) && !String.IsNullOrEmpty(viewPathAndFileName))
                {

                    viewFileName = TestSubString(viewPathAndFileName, viewPathAndFileName.LastIndexOf('\\') + 1);
                    xmlFileName = TestSubString(xmlPathAndFileName, xmlPathAndFileName.LastIndexOf('\\') + 1);

                    // Read from the Razor View
                    var viewMessagesList = FillListFromView(ViewPath + viewPathAndFileName, ".Messages");
                    var viewLabelsList = FillListFromView(ViewPath + viewPathAndFileName, ".Labels");
                    var viewWidgetNameList = FillListFromView(ViewPath + viewPathAndFileName, ".Widgets");

                    // Read from the XML file
                    var xmlMessageNameList = FillListFromXml(PublishedContentPath + xmlPathAndFileName, "<message name=");
                    var xmlLabelsList = FillListFromXml(PublishedContentPath + xmlPathAndFileName, "<label name=");
                    var xmlWidgetNameList = FillListFromXml(PublishedContentPath + xmlPathAndFileName, "<widget name=");
                    // TODO: investigate whether "<list name=" needs to be included


                    // Assert
                    if (viewMessagesList != null && viewLabelsList != null && viewWidgetNameList != null &&
                        xmlMessageNameList != null && xmlLabelsList != null && xmlWidgetNameList != null && FoldersSelected(fileNameAndPath))
                    {
                        if (!AreListsEqual(viewWidgetNameList, xmlWidgetNameList))
                        {
                            string tagList = ConcatenateTags(viewWidgetNameList, xmlWidgetNameList);
                            _reportList.Add(new ReportTagError(tagList, ReportType.WidgetNameMismatch, xmlFileName, "Widget name error with View:" + viewFileName, true));
                        }

                        CompareLists(xmlPathAndFileName, viewPathAndFileName, xmlMessageNameList, viewMessagesList, "Messages");
                        CompareLists(xmlPathAndFileName, viewPathAndFileName, xmlLabelsList, viewLabelsList, "Labels");
                        CompareLists(xmlPathAndFileName, viewPathAndFileName, xmlWidgetNameList, viewWidgetNameList, "Widget");
                    }
                }
            }
            catch (Exception ex)
            {
                _reportList.Add(new ReportTagError("Exception caught in CheckForOrphanedXmlTagsInView()", ReportType.Exception, xmlFileName, ex.Message, true));
            }
        }

        /// <summary>
        /// This concatenates the list of tags contained in both viewTagList and xmlTagList, for inclusion in the error report
        /// </summary>
        /// <param name="viewTagList"></param>
        /// <param name="xmlTagList"></param>
        /// <returns></returns>
        private string ConcatenateTags(IList<string> viewTagList, IList<string> xmlTagList)
        {
            string viewWidgets = "VIEW:";
            foreach (string tag in viewTagList)
            {
                viewWidgets += tag + ",";
            }
            string xmlWidgets = "XML:";
            foreach (string tag in xmlTagList)
            {
                xmlWidgets += tag + ",";
            }
            return viewWidgets.TrimEnd(',') + " BUT " + xmlWidgets.TrimEnd(',');
        }

        /// <summary>
        /// AreListsEqual returns true if both list have the same contents. Sequence is not important.
        /// </summary>
        /// <param name="viewTagList"></param>
        /// <param name="xmlTagList"></param>
        /// <returns></returns>
        private bool AreListsEqual(IList<string> viewTagList, IList<string> xmlTagList)
        {
            foreach( string st in viewTagList)
            {
                if (xmlTagList.IndexOf(st) == -1)
                {
                    return false;
                }
            }
            foreach (string st in xmlTagList)
            {
                if (viewTagList.IndexOf(st) == -1)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// FillListFromXml() returnns a list of all XML tags found in the xml file, following a given search pattern. Examples are 
        /// "<message name="
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        private IList<string> FillListFromXml(string filePath, string pattern)
        {
            if (!File.Exists(filePath))
            {
                _reportList.Add(new ReportMissingFile(ReportType.MissingXmlFile, filePath, "XML file not found", true));
            }
            else
            {
                var matchList = new List<string>();
                string line;
                var file = new System.IO.StreamReader(filePath);
                while ((line = file.ReadLine()) != null)
                {
                    var regex = new Regex(pattern);
                    if (regex.IsMatch(line))
                    {
                        string tag = ExtractXmlTagName(line, pattern);
                        matchList.Add(tag);
                    }
                }
                file.Close();
                return matchList;
            }
            return null;
        }

        // TODO: check to see if tags multiple tags can exist on the same line of text, since there may be lost tags in this case.

        /// <summary>
        /// ExtractXmlTagName() extracts a given tag from a line of code
        /// </summary>
        /// <param name="line"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        private string ExtractXmlTagName(string line, string pattern)
        {
            string xmlTagName = String.Empty;
            int quoteStart = line.IndexOf(pattern) + pattern.Length;
            if (quoteStart > -1)
            {
                string subStr = TestSubString(line, quoteStart + 1);
                int quoteEnd = subStr.IndexOf('"');
                xmlTagName = TestSubString(subStr, 0, quoteEnd);
            }
            return xmlTagName;
        }
        #endregion

        // This code helped find a bug in the params I was passing into String.Substring(). Since it's not hurting anything,
        // I left it in case it's needed again.
        private int testSubString1 = 0;
        private int testSubString2 = 0;

        private string TestSubString(string str, int start, int length)
        {
            testSubString1++;
            return str.Substring(start, length);
        }

        private string TestSubString(string str, int start)
        {
            testSubString2++;
            return str.Substring(start);
        }
    }
}
