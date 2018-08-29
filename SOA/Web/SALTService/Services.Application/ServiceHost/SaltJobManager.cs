using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Asa.Salt.Web.Services.Configuration.Jobs;
using Asa.Salt.Web.Services.Jobs.Interfaces;
using Asa.Salt.Web.Services.Logging;
using Microsoft.Practices.Unity;

namespace Asa.Salt.Web.Services.Application.ServiceHost
{
    /// <summary>
    /// Utility class used for managing Timer related processing jobs.
    /// </summary>
    public static class SaltJobManager
    {

        /// <summary>
        /// Private variable used as a holder for the supplied processor settings.
        /// </summary>
        static readonly JobConfiguration JobConfiguration = null;

        /// <summary>
        /// The logger
        /// </summary>
        private static readonly ILog Log;

        /// <summary>
        /// The unity container
        /// </summary>
        private static UnityContainer _unityContainer;

        /// <summary>
        /// Static dictionary used to store the objects which implement the ISupportTimerService
        /// interface and will be called when the specified Timer Interval elapses.
        /// </summary>
        private static readonly Dictionary<string, ElapsedItem> Timers = new Dictionary<string, ElapsedItem>();

        /// <summary>
        /// Thread synchronization object.
        /// </summary>
        private static readonly object _lockObject = new object();

        /// <summary>
        /// Initializes the <see cref="SaltJobManager"/> class.
        /// </summary>
        static SaltJobManager()
        {
            JobConfiguration = new ApplicationJobConfiguration().GetConfiguration();
            _unityContainer = UnityBootStrapper.GetUnityConfiguration();
            Log = (ILog)_unityContainer.Resolve(typeof(ILog));
        }

        /// <summary>
        /// Starts the Timer Processor objects.
        /// </summary>
        public static void Start()
        {
            RunTimers();
        }

        /// <summary>
        /// Stops the Timer Processor objects.
        /// </summary>
        public static void Stop()
        {
            StopTimers();
        }


        /// <summary>
        /// Stops the timers.
        /// </summary>
        static void StopTimers()
        {
            try
            {
                if ((Timers != null) && (Timers.Count > 0))
                {
                    lock (_lockObject)
                    {
                        foreach (var item in Timers)
                        {
                            if ((item.Value != null) && (item.Value.TimerObject != null))
                            {
                                item.Value.TimerObject.Enabled = false;
                                item.Value.TimerObject.Elapsed -= TimerElapsed;
                            }
                        }
                    }
                }
            }
            catch (Exception error)
            {
                Log.Debug(error.Message, error);
            }
        }

        /// <summary>
        /// Operation which enumerates the ProcessorSetting collection and initializes the
        /// timer objects.
        /// </summary>
        static void RunTimers()
        {

            try
            {
                if (JobConfiguration != null && JobConfiguration.Jobs.Count > 0)
                {
                    // Enumerate through all of the processor objects as supplied in 
                    // the host configuration file.
                    //
                    foreach (var setting in JobConfiguration.Jobs)
                    {
                        // If the Configuration Setting exists, is enabled, contains
                        // a Type and Name string - proceed.
                        //
                        if ((setting != null) && (setting.Enabled) && (false == string.IsNullOrEmpty(setting.Type)) && (false == string.IsNullOrEmpty(setting.Name)))
                        {
                            if (false == Timers.ContainsKey(setting.Name))
                            {
                                lock (_lockObject)
                                {
                                    if (false == Timers.ContainsKey(setting.Name))
                                    {
                                        // Let's load a valid type before crufting up the Timers, etc...
                                        var elapseObject = LoadType(setting.Type);

                                        var maxConcurrentJobs = setting.MaxConcurrentJobs;
                                        if (elapseObject != null)
                                        {
                                            var t = new TimerInternal()
                                            {
                                                Enabled = setting.Enabled,
                                                TimerKey = setting.Name,
                                                DebugEnabled = setting.DebugEnabled,
                                                Interval = setting.Interval.TotalMilliseconds

                                            };

                                            // Hookup handler for elapsed event.
                                            t.Elapsed += TimerElapsed;

                                            // Add to Dictionary.
                                            Timers.Add(setting.Name
                                                , new ElapsedItem()
                                                {
                                                    ElapsedObject = elapseObject,
                                                    TimerObject = t,
                                                    Type = setting.Type,
                                                    MaxConcurrentJobs = maxConcurrentJobs
                                                });
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception error)
            {

                Log.Error(error.Message, error);
            }
        }

        /// <summary>
        /// Loads the System.Type which is specified in configuration and should implement the
        /// ISupportTimerService interface.
        /// </summary>
        /// <param name="typeName">Type name to load.</param>
        /// <returns><see cref="ISupportTimerService"/> object.</returns>
        static ISupportTimerService LoadType(string typeName)
        {
            var jobType = Type.GetType(typeName);
            var elapseObject = (ISupportTimerService)_unityContainer.Resolve(jobType);

            return elapseObject;
        }

        /// <summary>
        /// Is the TimerElapsed event in which the target ISupportTimerService object
        /// OnTimerElapsed operation is called.
        /// </summary>
        /// <remarks>
        /// Elapsed handler is executed on a ThreadPool Thread.
        /// http://msdn.microsoft.com/en-us/library/system.timers.timer.aspx
        /// </remarks>
        static void TimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                var t = sender as TimerInternal;

                if (t != null)
                {
                    Log.Debug(string.Format("TimerElapsed Event Entered for '{0}'.", t.TimerKey));
                    //ISupportTimerService toInvoke = null;
                    ElapsedItem toInvoke = null;

                    // The Dictionary object used to store timer objects should not be modified
                    // continually, only at startup and as a one time event.  Using a thread
                    // synchronization lock here is not necessary.
                    // http://msdn.microsoft.com/en-us/library/xfhwa508(VS.85).aspx
                    //
                    if (Timers.TryGetValue(t.TimerKey, out toInvoke))
                    {
                        if ((toInvoke != null) && (toInvoke.ElapsedObject != null))
                        {
                            Log.Debug("Found TimerWrapper to Invoke.");
                            Log.Debug("Before calling OnTimerElapsed.");

                            // attempt to add a new job. if the job is added successfully, run it. otherwise skip past and 
                            // exit the timer function.

                            if (toInvoke.AddRef())
                            {
                                try
                                {
                                    toInvoke.ElapsedObject.OnTimerElapsed();
                                }
                                finally
                                {
                                    toInvoke.Release();
                                }
                            }

                        }
                    }
                }
            }
            catch (Exception error)
            {
                Log.Debug(error.Message, error);
            }
        }

        /// <summary>
        /// Internal wrapper class for the <see cref="Timer"/>.
        /// </summary>
        private class TimerInternal : System.Timers.Timer
        {
            /// <summary>
            /// Key used to represent this timer object.  Currently this is being tied
            /// to the Name value from the processors element from the host config file.
            /// </summary>
            public string TimerKey { get; set; }
            /// <summary>
            /// Boolean value representing if the output of any debug messages is
            /// enabled for this timer / processor object.  This value is driven by the
            /// setting from the processors element in the host config file.
            /// </summary>
            public bool DebugEnabled { get; set; }


        }

        private class ElapsedItem
        {
            public ISupportTimerService ElapsedObject { get; set; }
            public TimerInternal TimerObject { get; set; }
            public string Type { get; set; }

            public int MaxConcurrentJobs { get; set; }

            private int _refCount = 0;
            private object _refCountSync = new object();

            /// <summary>
            /// increment the number of running jobs. if addref is called and the incremented value would be greater than the max number concurrent jobs 
            /// allowed for this job type the call will return false. 
            /// the caller is responsible for calling release if this function returns true and should not call release if the call returns false.
            /// </summary>
            /// <returns>
            /// true: reference added successfully and the caller should call release to free up the resource
            /// false: reference was not added and the caller should NOT call release 
            /// </returns>
            public bool AddRef()
            {
                bool result = true;


                //
                // if there is a configured MaxConfiguredJobs, syncronize access to the _refCount. if the _refCount is less than the configured 
                // max concurrent jobs, increment the refcount. 
                //
                // returns true if the count was successfully incremented
                //
                if (MaxConcurrentJobs > 0)
                {
                    lock (_refCountSync)
                    {
                        if (_refCount < MaxConcurrentJobs)
                        {
                            Interlocked.Increment(ref _refCount);
                        }
                        else
                        {
                            result = false;
                        }
                    }
                }
                return result;
            }

            /// <summary>
            /// decrements the reference count for this object. should only be called when paired with an AddRef call where the AddRef call returned true.
            /// </summary>
            public void Release()
            {

                //
                // if there is a configured max number of jobs, lock access to the _refCount. decrement the refCount if it is positive, otherwise set it to 0.
                //
                if (MaxConcurrentJobs > 0)
                {
                    lock (_refCountSync)
                    {
                        if (_refCount > 0)
                        {
                            Interlocked.Decrement(ref _refCount);
                        }
                        else
                        {
                            _refCount = 0;
                        }
                    }
                }
            }


        }

    }
}

