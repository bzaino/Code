using System;
using System.Configuration;

namespace Asa.Salt.Web.Services.Configuration.ActiveDirectory
{
   
   public class ActiveDirectoryConfigurationSection : ConfigurationSection
   {

      [ConfigurationProperty("serverName", IsRequired = true)]
      public string ServerName
      {
         get { return (string)this["serverName"]; }
         set { this["serverName"] = value; }
      }

      [ConfigurationProperty("baseDN", IsRequired = true)]
      public string BaseDN
      {
         get { return (string)this["baseDN"]; }
         set { this["baseDN"] = value; }
      }

      [ConfigurationProperty("connectionProtection", DefaultValue = "Secure", IsRequired = false)]
      public string ConnectionProtection
      {
         get { return (string)this["connectionProtection"]; }
         set { this["connectionProtection"] = value; }
      }

      [ConfigurationProperty("connectionUsername", IsRequired = true)]
      public string Username
      {
         get { return (string)this["connectionUsername"]; }
         set { this["connectionUsername"] = value; }
      }

      [ConfigurationProperty("connectionPassword", IsRequired = true)]
      public string Password
      {
         get { return (string)this["connectionPassword"]; }
         set { this["connectionPassword"] = value; }
      }


   }

}
