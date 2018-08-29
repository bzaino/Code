using System;
using System.Configuration;

namespace Asa.Salt.Web.Services.Configuration.Jobs
{

    public class JobConfigurationSection : ConfigurationSection
    {

        [ConfigurationProperty("jobs")]
        public JobCollectionConfiguration Jobs
        {
            get { return ((JobCollectionConfiguration)(base["jobs"])); }
        }
    }

    [ConfigurationCollection(typeof(JobConfigurationElement))]
    public class JobCollectionConfiguration : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new JobConfigurationElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((JobConfigurationElement)(element)).Type;
        }

        public JobConfigurationElement this[int idx]
        {
            get { return (JobConfigurationElement)BaseGet(idx); }
        }
    }


    public class JobConfigurationElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get { return (string)base["name"]; }
            set { base["name"] = value; }
        }

        /// <summary>
        /// Is the interval for processing this service host.
        /// </summary>
        [ConfigurationProperty("interval", DefaultValue = "01:00:00")]
        public TimeSpan Interval
        {
            get { return (TimeSpan)base["interval"]; }
            set { base["interval"] = value; }
        }

        /// <summary>
        /// Indicates if custom debugging is enabled.
        /// </summary>
        [ConfigurationProperty("debugEnabled", DefaultValue = false)]
        public bool DebugEnabled
        {
            get { return (bool)base["debugEnabled"]; }
            set { base["debugEnabled"] = value; }
        }
        /// <summary>
        /// Boolean value indicating if this processor is enabled or disabled (default = false).
        /// </summary>
        [ConfigurationProperty("enabled", DefaultValue = false)]
        public bool Enabled
        {
            get { return (bool)base["enabled"]; }
            set { base["enabled"] = value; }
        }
        /// <summary>
        /// Is the .NET class type for the processor object.
        /// </summary>
        [ConfigurationProperty("type")]
        public string Type
        {
            get { return (string)base["type"]; }
            set { base["type"] = value; }
        }

        /// <summary>
        /// Max number of jobs of this type that may execute concurrently. Value less than 1 indicates no limit. Default value is 0.
        /// </summary>
        [ConfigurationProperty("maxConcurrentJobs")]
        public int MaxConcurrentJobs
        {
            get { return (int)base["maxConcurrentJobs"]; }
            set { base["maxConcurrentJobs"] = value; }
        }


    }

}
