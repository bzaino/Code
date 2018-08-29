using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace ASA.Web.WTF.configuration
{
    class DependencyConfigurationElementCollection : ConfigurationElementCollection
    {
        public DependencyConfigurationElement this[int index]
        {
            get
            {
                return base.BaseGet(index) as DependencyConfigurationElement;
            }
            set
            {
                if (base.BaseGet(index) != null)
                {
                    base.BaseRemoveAt(index);
                }
                this.BaseAdd(index, value);
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new DependencyConfigurationElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((DependencyConfigurationElement)element).Name;
        } 
    }
}
