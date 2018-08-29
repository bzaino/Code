using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.DirectoryServices;
using System.Configuration;
using System.Diagnostics;
using System.Security.Principal;
using System.Collections;


namespace ASA.Web.GUIDGenerator
{
    class GUIDGenerator
    {
        private const string CLASSNAME = " ASA.Web.Utility.GUIDGenerator";
        //static ASA.Log.ServiceLogger.IASALog _log = ASA.Log.ServiceLogger.ASALogManager.GetLogger(CLASSNAME);

        static Hashtable adConnection = (Hashtable)ConfigurationManager.GetSection("adConnection");
        static string domain = adConnection["domain"].ToString();
        static string uId = adConnection["userID"].ToString();
        static string password = adConnection["password"].ToString();
        static string userContainer = adConnection["userContainer"].ToString();
        static int sleepTime = Convert.ToInt32(adConnection["sleepTime"]);
        static int retries = Convert.ToInt32(adConnection["retries"]);

        public static Dictionary<string, Guid> GetObjectGUIDs(List<string> userNameList)
        {
            String logMethodName = ".GetObjectGUIDs(string userName)";
           // _log.Debug(logMethodName + "Method Begin");
           // _log.Debug(logMethodName + "Looking up ActiveDirectory Object ID using ADSI : " + userNameList);


            List<Guid> objectGuids = new List<Guid>(); // = (Guid)string.Empty;
            Dictionary<string, Guid> EmailGuidPair = new Dictionary<string, Guid>();
            Guid userGuid = new Guid(); // = (Guid)string.Empty;

            if (userNameList.Count == 0)
            {
               // _log.Warn(logMethodName + "Error: UsernameList is empty");

                return EmailGuidPair;
            }

            //_log.Debug(logMethodName + "Provided username is valid, looking user up in ActiveDirectory");
            SearchResult result = null;
            //_log.Debug(logMethodName + "Attempting directory entry search for " + userName + " Attempt#: " + i);


            string path = string.Format("LDAP://{0}/{1}", domain, userContainer);
            using (DirectoryEntry directoryEntry = new DirectoryEntry(path, uId, password, AuthenticationTypes.Secure))
            {
                using (DirectorySearcher search = new DirectorySearcher(directoryEntry))
                {
                    foreach (string userName in userNameList)
                    {
                        try
                        {
                            search.Filter = String.Format("(cn={0})", userName);
                            search.SearchScope = SearchScope.Subtree;
                            result = search.FindOne();
                            //_log.Debug(logMethodName + "Finished searching active directory");


                            if (result != null)
                            {
                                //_log.Debug(logMethodName + "User found...getting user ObjectId");

                                userGuid = new Guid(result.GetDirectoryEntry().NativeGuid); //.ToString();
                                //String guidString = objectGuids != null ? usrGuid.ToString() : "NULL";
                                //objectGuids.Add(userGuid);

                                EmailGuidPair.Add(userName, userGuid);
                                //break;
                            }
                            else
                            {
                                EmailGuidPair.Add(userName, System.Guid.Empty);
                                //_log.Debug(logMethodName + "No user found in active directory");
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("Error attempting to get the AD directory entry for the user", ex);
                        } 
                    }

                }

            }
            // _log.Debug(logMethodName + "There was a problem accessing active directory, retrying...");

            System.Threading.Thread.Sleep(sleepTime);              

            //_log.Debug(logMethodName + "User objectid has been retrieved the id is: " + guidString);

            return EmailGuidPair;
        }

        public static void dumpToFile(string fileName, Dictionary<string, Guid> EmailGuidPair)
        {
            string GuidFile = fileName.Substring(0, (fileName.Length - 4)) + "-GUID.csv";
            
            File.WriteAllLines(GuidFile, EmailGuidPair.Select(x => x.Key + "," + x.Value));
        }

        public static List<string> readUserNames(String filePath)
        {
            List<string> usrnames = new List<string>(); //= string.Empty;
            string filename = Path.GetFileName(filePath);
            try
            {
                using (StreamReader sr = new StreamReader(filename))
                {
                    while (!sr.EndOfStream)
                    {
                        String line = sr.ReadLine(); // .ReadToEnd();
                        usrnames.Add(line);
                        Console.WriteLine(line);
                    }
                    
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }

            return usrnames;
        }

        /***
         * Usage GUIDGenerato.exe filepath\filename1.txt filename2.txt
         * the filepath part of the parameter is optional and needn't be specified if the file
         * is located in the same folder as the GUIDGenerator.exe.
         * you can specify multiple files at the same time and all get processed and corresponding
         * result files with corresponding file names, with '-GUID' suffix appended.
         * */
        static void Main(string[] args)
        {
            //System.Diagnostics.Debugger.Break();
            string path, filenameOnly;
            foreach (String argString in args)
            {
                // Split into path and wildcard
                int lastBackslashPos = argString.LastIndexOf('\\') + 1;
                path = argString.Substring(0, lastBackslashPos);
                filenameOnly = argString.Substring(lastBackslashPos,
                                           argString.Length - lastBackslashPos);

                String[] fileList;

                if (string.IsNullOrEmpty(path))
                {
                    fileList = System.IO.Directory.GetFiles(System.IO.Directory.GetCurrentDirectory(), filenameOnly);
                }
                else
                {
                    fileList = System.IO.Directory.GetFiles(path, filenameOnly); 
                }
                
                foreach (String fileName in fileList)
                {
                    //do things for each file
                    List<string> usrNames = new List<string>(readUserNames(fileName));
                    Dictionary<string, Guid> emlGuid = new Dictionary<string, Guid>(GetObjectGUIDs(usrNames));
                    dumpToFile(fileName, emlGuid);
                }
            }

        }
    }
}
