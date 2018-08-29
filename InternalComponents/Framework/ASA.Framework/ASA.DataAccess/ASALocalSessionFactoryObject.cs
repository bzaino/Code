using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.IO;

using Spring.Data.NHibernate;
using NHibernate.Cfg;


namespace ASA.DataAccess
{
    class ASALocalSessionFactoryObject : LocalSessionFactoryObject
    {

        #region Fields

        private string configAssemblyName;

        private string configResourceName;

        #endregion

        #region Properties

        public string ConfigAssemblyName
        {
            set { configAssemblyName = value; }
        }

        public string ConfigResourceName
        {
            set { configResourceName = value; }
        }

        #endregion

        #region Methods

        protected override void PostProcessConfiguration(Configuration config)
        {
            if (configAssemblyName != null && configResourceName != null)
            {
                Assembly assembly = Assembly.Load(configAssemblyName);
                if (assembly == null)
                {
                    throw new FileNotFoundException("Unable to load assembly [" + configAssemblyName + "]");
                }
                config.Configure(assembly, configResourceName);
            }
        }

        #endregion

    }
}
