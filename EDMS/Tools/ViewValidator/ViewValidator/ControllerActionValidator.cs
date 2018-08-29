using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ViewValidator
{
    /// <summary>
    /// This object verifies that a URL to a Controller Action is valid, in the IsControllerActionValid() function.
    /// </summary>
    public class ControllerActionValidator
    {
        private List<string> _razorFolders = new List<string>();
        private string _razorPath = null;
        private ReportList _reportList;

        /// <summary>
        ///  The Constructor sets the three private member variables used by ControllerActionValidator and
        ///  fills _razorFolders with each Razor folder contained in the text file nameed folderFilename
        /// </summary>
        /// <param name="folderFilename"></param>
        /// <param name="razorPath"></param>
        /// <param name="reportList"></param>
        public ControllerActionValidator(string folderFilename, string razorPath, ReportList reportList)
        {
            // replaces the UNIX / with a DOS \
            _razorPath = razorPath.Replace('/', '\\');
            _reportList = reportList;
            var file = new System.IO.StreamReader(folderFilename);
            string controllerPath = null;
            while ((controllerPath = file.ReadLine()) != null)
            {
                _razorFolders.Add(controllerPath);
            }
            file.Close();
        }

        
        /// <summary>
        /// IsControllerActionValid() return Resolves, Fails and Untested to describe whether the controller action, defined by href
        /// (example: ~/PaymentReminder/LoanPaymentDate), can actually be matched up with a Razor page.
        /// </summary>
        /// <param name="href"></param>
        /// <param name="sourceFile"></param>
        /// <returns></returns>
        public PathResolves IsControllerActionValid(string href, string sourceFile)
        {            
            // TODO: Eventually, use sourceFile to see if the link is to an Overlay or not. 
            // TODO: What about errors when the file's found in both the Controller folder AND the Controller's Overlay folder ?

            // Direct calls to the Shared folder should never happen, so flag it as an error
            if (href.ToLower().IndexOf("shared") > -1)
            {
                _reportList.Add(new ReportTagError("BAD URL", ReportType.UrlRazor, sourceFile, "Faulty URL pointing to " + href + " found in " + sourceFile, false));
                return PathResolves.Fails;
            }

            string razorFilename = CreateRazorFilename(href);
            if (razorFilename != null)
            {
                if ( File.Exists(razorFilename))
                {
                    return PathResolves.Resolves;
                }

                // The Razor file was  found in the Controller's root folder
                // So look for the Razor file in the Controller's Overlay folder
                string razorFilenameWithOverlay = razorFilename.Substring(0, razorFilename.LastIndexOf('\\')) + "\\Overlay\\" + razorFilename.Substring(razorFilename.LastIndexOf('\\') + 1);
                if ( File.Exists(razorFilenameWithOverlay))
                {
                    return PathResolves.Resolves;
                }
                return PathResolves.Fails;
            }

            return PathResolves.Untested;
        }

        /// <summary>
        /// This method creates the Razor filename by trimming certain leading characters, replaces the UNIX / with a DOS \,
        /// checks if the path is solely a Controller Name, or if the path points to a class Controller Name / Controller Action combo.
        /// </summary>
        /// <param name="inHref"></param>
        /// <returns></returns>
        private string CreateRazorFilename(string inHref)
        {
            string cleanedInHref = inHref;
            if (cleanedInHref.StartsWith("~/"))
            {
                cleanedInHref = cleanedInHref.Substring(1);
            }
            // replaces the UNIX / with a DOS \
            cleanedInHref = cleanedInHref.Replace('/', '\\');
            string controllerName_inputted = null;
            string actionName_inputted = null;
            if (cleanedInHref.IndexOf('\\') > -1)
            {
                cleanedInHref = cleanedInHref.TrimStart('\\');
                if (cleanedInHref.IndexOf('\\') > -1)
                {
                    // There is a Controller Action in the URL
                    controllerName_inputted = cleanedInHref.Substring(0, cleanedInHref.IndexOf('\\'));
                    if (cleanedInHref.IndexOf(controllerName_inputted + "\\") > -1)
                    {
                        actionName_inputted = cleanedInHref.Substring(cleanedInHref.IndexOf(controllerName_inputted + "\\") + controllerName_inputted.Length + 1);
                    }
                }
                else
                {
                    controllerName_inputted = cleanedInHref;
                }
                cleanedInHref = '\\' + cleanedInHref;
            }

            // the path is solely a Controller Nam
            if (cleanedInHref == '\\' + controllerName_inputted)
            {
                return _razorPath + '\\' + controllerName_inputted + "\\Index.cshtml";
            }

            // This means the URL is a normal Controller Action, like ~/PaymentReminder/LoanPaymentDate
            if (cleanedInHref.TrimStart('\\').StartsWith(controllerName_inputted) && cleanedInHref.EndsWith(actionName_inputted))
            {
                return _razorPath + cleanedInHref + ".cshtml";
            }

            return null;
        }
    }
}
