using System;
using System.Collections.Generic;
using System.Configuration;
using Asa.Salt.Web.Services.Configuration.ActiveDirectory;
using Asa.Salt.Web.Services.Configuration.Mail;

namespace Asa.Salt.Web.Services.Configuration.Jobs
{
   public class ApplicationJobConfiguration : IApplicationJobConfiguration
   {

       /// <summary>
       /// Gets the job configuration.
       /// </summary>
       /// <returns></returns>
      public JobConfiguration GetConfiguration()
      {
          var configuration = new JobConfiguration();
          var section = (JobConfigurationSection)ConfigurationManager.GetSection("jobConfiguration");

          configuration.Jobs = new List<Job>();

          //COV-10356 -- added test for null Jobs
          if (section != null && section.Jobs != null)
          {
              foreach (JobConfigurationElement job in section.Jobs)
              {
                  configuration.Jobs.Add(new Job()
                      {
                          Name = job.Name,
                          DebugEnabled = job.DebugEnabled,
                          Enabled = job.Enabled,
                          Interval = job.Interval,
                          MaxConcurrentJobs = job.MaxConcurrentJobs,
                          Type = job.Type
                      });
              }
          }

          return configuration;
      }
   }
}
