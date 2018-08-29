using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ViewValidator
{
    // Use this DOS command to create the folder tree: >dir /s /b /ad "c:\temp\folder" > list.txt
    // Use this DOS command to create the folder tree: >dir /s /b /ad "<source folder>" > <target file>
    /// <summary>
    /// FolderValidator validates the entire FolderTree file passed in on construction. It can be used to either assert every folder in the file exists
    /// or that every folder in the list has been deleted. It logs errors to the ReportList.
    /// 
    /// TODO: A better way to handle this is to log an error if any folder found aside from those expected is logged as an error.
    /// TODO: This list should be expanded to include file names and not just folder names
    /// </summary>
    public class FolderValidator
    {
        // This enum should be removed since it makes little sense to give this object two modes. Instead, it should get a list of valid folder name,
        // AND a root folder where anything additional that's found it an error
        public enum FolderStatus
        {
            Exists=0,
            DoesNotExist=1
        }

        private FolderStatus _folderStatus;
        private string _folderTreePathAndFile;
        private ReportList _reportList;
        private string _root;

        public string FolderTreePathAndFile { get { return _folderTreePathAndFile; } }

        // Connstructor that sets the private member variables
        public FolderValidator(FolderStatus folderStatus, string folderTree, string root, ReportList reportList)
        {
            _folderStatus = folderStatus;
            _folderTreePathAndFile = Environment.CurrentDirectory + @"\FolderTrees\" + folderTree;
            _folderTreePathAndFile = _folderTreePathAndFile.Replace(@"\bin", "").Replace(@"\Debug", "").Replace(@"\Release", "");
            _reportList = reportList;
            _root = root;
        }

        /// <summary>
        /// Validate() is called to run the validation. No return type is needed since it logs any errors found, using the _reportList
        /// object set in the constructor.
        /// </summary>
        public void Validate()
        {            
            try
            {
                if (!System.IO.File.Exists(_folderTreePathAndFile))
                {
                    _folderTreePathAndFile = _folderTreePathAndFile.Replace(@"\FolderTrees\", "\\");
                }
                if (!System.IO.File.Exists(_folderTreePathAndFile))
                {
                    _reportList.Add(new ReportMissingFile(ReportType.MissingFolder, _folderTreePathAndFile, "Folder Tree File missing", true));
                    return;
                }

                string path;
                var file = new System.IO.StreamReader(_folderTreePathAndFile);
                while ((path = file.ReadLine()) != null)
                {
                    if (!System.IO.Directory.Exists(_root + '\\' + path) && _folderStatus == FolderStatus.Exists)
                    {
                        _reportList.Add(new ReportMissingFile(ReportType.MissingFolder, path, "Expected Path was not found", true));
                    }
                    if (System.IO.Directory.Exists(_root + '\\' + path) && _folderStatus == FolderStatus.DoesNotExist)
                    {
                        _reportList.Add(new ReportMissingFile(ReportType.UnexpectedFolder, path, "Unexpected Path was found", true));
                    }
                }
                file.Close();
            }
            catch (Exception ex)
            {
                throw new Exception("Could not open Folder Tree File " + _folderTreePathAndFile);
            }
        }
    }
}
