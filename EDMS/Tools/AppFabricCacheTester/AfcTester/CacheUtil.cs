using System;
using Microsoft.ApplicationServer.Caching;
using System.Collections.Generic;

namespace AfcTester
{
    public class CacheUtil
    {
        private static DataCacheFactory _factory = null;
        private static DataCache _cache = null;

        public static String HostName { get; set; }
        public static Int32 CachePort { get; set; }
        public static String CacheName { get; set; }

        public static DataCacheFactoryConfiguration Configuration { get; set; }
        public static DataCacheSecurity CacheSecurity { get; set; }

        public static DataCache GetCache()
        {
            if (_cache != null)
                return _cache;

            if(string.IsNullOrEmpty(HostName))
                HostName = "adevweb019";

            CachePort = 22233;
            CacheName = "default";

            List<DataCacheServerEndpoint> servers = new List<DataCacheServerEndpoint>(1);

            servers.Add(new DataCacheServerEndpoint(HostName, CachePort));

            Configuration = new DataCacheFactoryConfiguration();

            Configuration.SecurityProperties = CacheSecurity;
            
            Configuration.Servers = servers;

            Configuration.LocalCacheProperties = new DataCacheLocalCacheProperties();

            DataCacheClientLogManager.ChangeLogLevel(System.Diagnostics.TraceLevel.Off);

            _factory = new DataCacheFactory(Configuration);

            _cache = _factory.GetCache(CacheName);

            return _cache;
        }

        public static DataCacheSecurity SetCacheSecurity(string mode, string level)
        {
            DataCacheSecurityMode _mode = DataCacheSecurityMode.Transport;
            DataCacheProtectionLevel _level = DataCacheProtectionLevel.EncryptAndSign;

            switch(mode)
            {
                case "none":
                    _mode = DataCacheSecurityMode.None;
                    break;
            }
            switch(level)
            {
                case "none":
                    _level = DataCacheProtectionLevel.None;
                    break;                
                case "sign":
                    _level = DataCacheProtectionLevel.Sign;
                    break;
            }

            return new DataCacheSecurity(_mode, _level);
        }
    }
}