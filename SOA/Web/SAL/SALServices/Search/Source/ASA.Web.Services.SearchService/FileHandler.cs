using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Script.Serialization;
using System.Threading;
using ASA.Web.Services.SearchService.DataContracts;
using System.Text.RegularExpressions;
using ASA.Web.Services.SearchService.Validation;
namespace ASA.Web.Services.SearchService
{
    public class FileHandler : IDisposable
    {
        /// <summary>
        /// The class name
        /// </summary>
        private const string Classname = "ASA.Web.Services.ASAMemberService.FileHandler";

        /// <summary>
        /// The logger
        /// </summary>
        private static readonly Log.ServiceLogger.IASALog Log = ASA.Log.ServiceLogger.ASALogManager.GetLogger(Classname);

        /// <summary>
        /// The member adapter
        /// </summary>
        private static readonly Object Locker = new Object();
        private static readonly TimeSpan WaitTime = new TimeSpan(0, 0, 1);
        private const int MaxTries = 4;
        private delegate string DelegateUpdateLocations(string filepath, Location newlocation, out int threadId);
        private static readonly JavaScriptSerializer JsonConvert = new JavaScriptSerializer();
        public string QueUpdate(Location newlocation)
        {
            const string logMethodName = "- QueUpdate(Location location) - ";
            string retVal = "Queued location:" + newlocation.key;
            Log.Debug(logMethodName + "Begin Method");
            try
            {
                string filepath = System.Configuration.ConfigurationManager.AppSettings["geolocations"];
                if (!CrossSiteScriptValidator.IsValidJson(JsonConvert.Serialize(newlocation)))
                {
                    Log.Error("Failed CrossSiteScriptValidator: Possible cross site attack intercepted and save for inspection [ ", JsonConvert.Serialize(newlocation) + " ]");
                    return "Failed Validation: the object does not conform to json standards.";
                }
                else
                {
                    if (File.Exists(filepath))
                    {
                        DelegateUpdateLocations callBack = new DelegateUpdateLocations(UpdateLocations);
                        int threadId;
                        Thread t = new Thread(() => retVal = callBack(filepath, newlocation, out threadId));
                        t.Start();
                        t.Join(WaitTime + WaitTime);
                    }
                    else
                    {
                        retVal = "Failed: file not found";
                    }

                } 

            }
            catch (Exception ex)
            {
                Log.Error("Exception executing your request in QueUpdate:", ex);
                retVal = "Failed: with an exception executing your request in QueUpdate";
            }

            Log.Debug(logMethodName + "End Method");
            return retVal;
        }

        public string UpdateLocations(string filepath, Location newlocation, out int threadId)
        {
            string retStatus = "Failed";
            int counter = 0;
            try
            {
                while (counter < MaxTries)
                {
                    Thread.Sleep(2000);
                    counter = counter + 1;
                    lock (Locker)
                    {
                        JsonConvert.MaxJsonLength = Int32.MaxValue;
                        string json = string.Empty;
                        using (StreamReader sr = new StreamReader(filepath))
                        {
                            while (sr.Peek() > 0)
                                json = sr.ReadToEnd();
                            sr.Close();
                            var locations = JsonConvert.Deserialize<dynamic>(json);
                            if (!locations.ContainsKey(newlocation.key))
                            {
                                using (StreamWriter sw = new StreamWriter(filepath, false))
                                {
                                    locations.Add(newlocation.key, new Dictionary<string, object>()
                                    {
                                        {"results", newlocation.results}
                                    });
                                    var newJson = JsonConvert.Serialize(locations);
                                    sw.Write(newJson);
                                    sw.Flush();
                                    retStatus = "Success";
                                }
                            }
                            else
                            {
                                retStatus = "Failed: can't add duplicate key value : '" + newlocation.key + "'";
                            }
                            counter = MaxTries + 1; //exit thread
                        }
                    }
                }
            }
            catch (IOException ex)
            {
                Log.Error("IOExceptio executing your request in UpdateLocations:", ex);
                retStatus = "Failed: with an IOExceptio executing your request in UpdateLocations";
            }
            catch (ObjectDisposedException disposedex)
            {
                Log.Error("DisposedException executing your request in UpdateLocations:", disposedex);
            }
            catch (Exception ex)
            {
                Log.Error("Unexpected executing your request in UpdateLocations:", ex);
                retStatus = "Failed: with an IOExceptio executing your request in UpdateLocations";
            }
            threadId = Thread.CurrentThread.ManagedThreadId;
            return retStatus;
        }

        ~FileHandler()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                lock (Locker)
                {
                    for (int i = 0; i <= GC.MaxGeneration; i++)
                    {
                        GC.CollectionCount(i);
                    }
                }
            }
        }
    }
}
