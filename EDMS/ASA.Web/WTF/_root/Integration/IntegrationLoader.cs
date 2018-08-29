using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Reflection;
using ASA.Web.WTF.configuration;
using System.Configuration;
using System.Collections;

namespace ASA.Web.WTF.Integration
{
    public class IntegrationLoader
    {

        private const string CLASSNAME = "ASA.Web.WTF.Integration.IntegrationLoader";
        static ASA.Log.ServiceLogger.IASALog _log = ASA.Log.ServiceLogger.ASALogManager.GetLogger(CLASSNAME);

        //static IContextDataProvider _contextDataProvider;
        //static ISecurityAdapter _securityAdapter;

        static Dictionary<string, object> _dictDependencies = new Dictionary<string,object>();

        static DependencyConfigurationSection _contextConfig;

        private static Object _lock = new Object();

        static IntegrationLoader()
        {
            String logMethodName = ".ctor() - ";
            _log.Debug(logMethodName + "Begin Method");

            Init();

            _log.Debug(logMethodName + "End Method");

        }

        public static T LoadDependency<T>(string name)
        {
            string logMethodName = ".LoadDependency<T>(string name) - ";
            string strType="";

            lock (_lock)
            {

                //Is it already loaded?
                if (_dictDependencies.ContainsKey(name))
                {
                    object oDependency = _dictDependencies[name];
                    if (oDependency is T)
                    {
                        return (T)oDependency;
                    }
                    else
                    {
                        String message = logMethodName + "type " + strType + " does not implement " + typeof(T).FullName;
                        _log.Fatal(message);

                        throw new WtfException(message);
                    }

                }

                // Is Name in Config?
                foreach (DependencyConfigurationElement elem in _contextConfig.Dependencies)
                {
                    if (elem.Name == name)
                    {
                        strType = elem.Class;
                        break;
                    }
                }

                if (strType == "")
                {
                    string message = logMethodName + "No dependency configured for name: " + name;
                    _log.Fatal(message);
                    throw new WtfException(message);
                }

                Type tDependency;
                // Next load type
                try
                {
                    _log.Debug(logMethodName + "Loading dependecy type: " + strType);

                    tDependency = Type.GetType(strType);
                }
                catch (Exception ex)
                {
                    String message = logMethodName + "Unable to load dependency type: " + strType;
                    _log.Fatal(message, ex);

                    throw new WtfException(message, ex);
                }

                if (tDependency == null)
                {
                    String message = logMethodName + "Unable to load dependency type: " + strType;
                    _log.Fatal(message);

                    throw new WtfException(message);
                }

                // Finally create instances. 
                try
                {
                    _log.Debug(logMethodName + "Creating Instance for " + name + ", " + strType);
                    object oDependency = Activator.CreateInstance(tDependency);
                    if (oDependency is T)
                    {
                        _dictDependencies.Add(name, (T)oDependency);
                    }
                    else
                    {
                        String message = logMethodName + "type " + strType + " does not implement " + typeof(T).FullName;
                        _log.Fatal(message);

                        throw new WtfException(message);
                    }
                }
                catch (Exception ex)
                {
                    String message = logMethodName + "Unable to create an instance for " + name + ", " + strType;
                    _log.Fatal(message, ex);

                    throw new WtfException(message, ex);
                }
                // COV 10334 - This should be inside the lock block.
                return (T)_dictDependencies[name];
            }

        }

        private static void Init()
        {
            String logMethodName = ".Init() - ";
            _log.Debug(logMethodName + "Begin Method");

            if (_contextConfig == null)
            {
                _log.Debug(logMethodName + "LOADING Context - Configuration, ContextDataProvider and SecurityAdapter");

                _contextConfig = null;


                // First load configuration
                try
                {
                    _log.Debug(logMethodName + "Loading ASADependenciesConfiguration");
                    _contextConfig = (DependencyConfigurationSection)ConfigurationManager.GetSection("ASADependencies");
                }
                catch (Exception ex)
                {
                    String message = logMethodName + "Unable to load ASADependencies configuration";
                    _log.Fatal(message, ex);

                    throw new WtfException(message, ex);
                }

                if (_contextConfig == null)
                {
                    String message = logMethodName + "Unable to load ASADependencies configuration";
                    _log.Fatal(message);

                    throw new WtfException(message);
                }

            }
            else
            {
                _log.Debug(logMethodName + "WTF configuration, skipping load...");
            }

            _log.Debug(logMethodName + "End Method");
        }

        public static IContextDataProvider CurrentContextDataProvider
        {
            get
            {
                return LoadDependency<IContextDataProvider>("dataProvider");
            }

        }

        public static ISecurityAdapter CurrentSecurityAdapter
        {
            get
            {
                return LoadDependency<ISecurityAdapter>("securityAdapter");
            }

        }

        //public static IAsaMemberAdapter CurrentMemberAdapter
        //{
        //    get
        //    {
        //        return LoadDependency<IAsaMemberAdapter>("memberAdapter");
        //    }
        //}
        //public static ISelfReportedAdapter CurrentSelfReportedAdapter
        //{
        //    get
        //    {
        //        throw new NotImplementedException();
        //    }
        //}

    }
}
