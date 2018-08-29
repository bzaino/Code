using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace ASA.Web.WTF.configuration
{
    class DependencyConfigurationElement : ConfigurationElement
    {

        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get
            {
                return this["name"] as string;
            }
        }

        [ConfigurationProperty("class", IsRequired = true)]
        public string Class
        {
            get
            {
                return this["class"] as string;
            }
        }
    }
}
