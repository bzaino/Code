using System;
using System.Configuration;

namespace Asa.Salt.Web.Services.Configuration.Mail
{

    public class MailConfigurationSection : ConfigurationSection
    {

        [ConfigurationProperty("sendMailEnabled", DefaultValue = "true", IsRequired = false)]
        public Boolean SendMailEnabled
        {
            get { return (Boolean) this["sendMailEnabled"]; }
            set { this["sendMailEnabled"] = value; }
        }

        [ConfigurationProperty("host", IsRequired = true)]
        public string Host
        {
            get { return (string) this["host"]; }
            set { this["host"] = value; }
        }

        [ConfigurationProperty("port", IsRequired = true)]
        public string Port
        {
            get { return (string) this["port"]; }
            set { this["port"] = value; }
        }

        [ConfigurationProperty("defaultFromAddress", IsRequired = true)]
        public string DefaultFromAddress
        {
            get { return (string) this["defaultFromAddress"]; }
            set { this["defaultFromAddress"] = value; }
        }

        [ConfigurationProperty("username", IsRequired = false)]
        public string Username
        {
            get { return (string) this["username"]; }
            set { this["username"] = value; }
        }

        [ConfigurationProperty("password", IsRequired = false)]
        public string Password
        {
            get { return (string) this["password"]; }
            set { this["password"] = value; }
        }

        [ConfigurationProperty("templates")]
        public TemplateCollectionConfiguration Templates
        {
            get { return ((TemplateCollectionConfiguration) (base["templates"])); }
        }
    }

    [ConfigurationCollection(typeof(TemplateConfigurationElement))]
    public class TemplateCollectionConfiguration : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new TemplateConfigurationElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((TemplateConfigurationElement) (element)).Type;
        }

        public TemplateConfigurationElement this[int idx]
        {
            get { return (TemplateConfigurationElement) BaseGet(idx); }
        }
    }

    public class TemplateConfigurationElement : ConfigurationElement
    {

       
        [ConfigurationProperty("type", IsRequired = true)]
        public String Type
        {
            get { return (String)this["type"]; }
            set { this["type"] = value; }
        }

        [ConfigurationProperty("path", IsRequired = true)]
        public String Path
        {
            get { return (String) this["path"]; }
            set { this["path"] = value; }
        }

        [ConfigurationProperty("subject", IsRequired = true)]
        public String Subject
        {
            get { return (String) this["subject"]; }
            set { this["subject"] = value; }
        }


    }


}


