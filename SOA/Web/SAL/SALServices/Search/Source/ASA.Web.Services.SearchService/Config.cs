using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Collections;

namespace ASA.Web.Services.SearchService
{
    public static class Config
    {
        private static Hashtable indexConnection = (Hashtable)ConfigurationManager.GetSection("indexConnection");

        //put all config entries here.. "Fincher" style.  ;-)
        private static string _searchHost = "";
        public static string SearchHost
        {
            get
            {
                try
                {
                    _searchHost = indexConnection["SearchHost"].ToString();
                    return _searchHost;
                }
                catch
                {
                    return _searchHost;
                }
            }
        }

        private static string _scholarshipApiURL = "";
        public static string ScholarshipApiURL
        {
            get
            {
                try
                {
                    _scholarshipApiURL = ConfigurationManager.AppSettings["ScholarshipsURL"].ToString();
                    return _scholarshipApiURL;
                }
                catch
                {
                    return _scholarshipApiURL;
                }
            }
        }

        private static string _scholarshipApiToken = "";
        public static string ScholarshipApiToken
        {
            get
            {
                try
                {
                    _scholarshipApiToken = ConfigurationManager.AppSettings["ScholarshipsAuthToken"].ToString();
                    return _scholarshipApiToken;
                }
                catch
                {
                    return _scholarshipApiToken;
                }
            }
        }

        private static int _searchPort = 20000;
        public static int SearchPort
        {                
            get
            {
                try
                {
                    _searchPort = Int32.Parse(indexConnection["SearchPort"].ToString());
                    return _searchPort;
                }
                catch
                {
                    return _searchPort;
                }
            }
        }

        private static string _itlHost = "";
        public static string ITLHost
        {
            get
            {
                try
                {
                    _itlHost = indexConnection["ITLHost"].ToString();
                    return _itlHost;
                }
                catch
                {
                    return _itlHost;
                }
            }
        }

        private static int _itlPort = 8888;
        public static int ITLPort
        {
            get
            {
                try
                {
                    _itlPort = Int32.Parse(indexConnection["ITLPort"].ToString());
                    return _itlPort;
                }
                catch
                {
                    return _itlPort;
                }
            }
        }

        private static int _searchApiLimit;
        public static int SearchApiLimit
        {
            get
            {
                try
                {
                    _searchApiLimit = Convert.ToInt32(ConfigurationManager.AppSettings["SearchApiLimit"].ToString());
                    return _searchApiLimit;
                }
                catch
                {
                    return _searchApiLimit;
                }
            }
        }

        private static int _cookieMaxLength;
        public static int CookieMaxLength
        {
            get
            {
                try
                {
                    _cookieMaxLength = Convert.ToInt32(ConfigurationManager.AppSettings["CookieMaxLength"].ToString());
                    return _cookieMaxLength;
                }
                catch
                {
                    return _cookieMaxLength;
                }
            }
        }
    }
}
