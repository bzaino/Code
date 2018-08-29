using System;
using System.Configuration;

namespace Asa.Salt.Web.Services.Configuration.ActiveDirectory
{
   public class ApplicationActiveDirectoryConfiguration : IApplicationActiveDirectoryConfiguration
   {
      /// <summary>
      /// Gets the ActiveDirectory configuration.
      /// </summary>
      /// <returns></returns>
      /// <exception cref="System.NotImplementedException"></exception>
      public ActiveDirectoryConfiguration GetConfiguration()
      {
         var configuration = new ActiveDirectoryConfiguration();
         var section = (ActiveDirectoryConfigurationSection)ConfigurationManager.GetSection("activeDirectoryConfiguration");
         if (section != null)
         {
            configuration.ServerName = section.ServerName;
            configuration.BaseDN = section.BaseDN;
            configuration.ConnectionProtection = section.ConnectionProtection;
            configuration.Username = section.Username;
            configuration.Password = section.Password;
         }

         return configuration;
      }
   }
}
