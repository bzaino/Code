using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace ASA.Web.WTF.configuration
{
    class DependencyConfigurationSection : ConfigurationSection
    {
        [ConfigurationProperty("dependencies")]
        public DependencyConfigurationElementCollection Dependencies
        {
            get
            {
                return this["dependencies"] as DependencyConfigurationElementCollection;
            }
        }
    }
}
