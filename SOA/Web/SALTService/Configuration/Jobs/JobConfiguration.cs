using System;
using System.Collections.Generic;

namespace Asa.Salt.Web.Services.Configuration.Jobs
{
    public class JobConfiguration
    {
        /// <summary>
        /// Gets or sets the jobs.
        /// </summary>
        /// <value>
        /// The jobs.
        /// </value>
        public List<Job> Jobs { get; set; }
    }

    public class Job
    {
        /// <summary>
        /// Gets or sets the name of the job.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Is the interval for processing this service host.
        /// </summary>
        public TimeSpan Interval { get; set; }

        /// <summary>
        /// Indicates if custom debugging is enabled.
        /// </summary>
        public bool DebugEnabled { get; set; }

        /// <summary>
        /// Boolean value indicating if this processor is enabled or disabled (default = false).
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Is the .NET class type for the processor object.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Max number of jobs of this type that may execute concurrently. Value less than 1 indicates no limit. Default value is 0.
        /// </summary>
        public int MaxConcurrentJobs { get; set; }
    }
}
