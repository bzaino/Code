using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace ASA.Web.Services.AddrValidationService
{
    class Config
    {

        //<!-- QasOn set to true is default behavior. If Qas address validation needs to be turned off, set the QasOn value to false-->
        //<add key ="QasOn" value ="true" />
        private static bool _QasOn = true;
        public static bool QasOn
        {
            get
            {
                try
                {
                    _QasOn = Boolean.Parse(ConfigurationManager.AppSettings["QasOn"].ToString());
                    return _QasOn;
                }
                catch
                {
                    return _QasOn;
                }
            }
        }


        //<!-- The name of the QAS layout located in QASSERVE.INI used to format the address output-->
        //<add key ="QASLayout" value ="Database layout" />
        private static string _QasLayout = "Database layout";
        public static string QasLayout
        {
            get
            {
                try
                {
                    _QasLayout = ConfigurationManager.AppSettings["QasLayout"].ToString();
                    return _QasLayout;
                }
                catch
                {
                    return _QasLayout;
                }
            }
        }

        //<!-- The threshold from 1 to 100 that QAS will use when verifying an address -->
        //<add key ="QASThreshold" value ="50" />
        private static int _QasThreshold = 50;
        public static int QasThreshold
        {
            get
            {
                try
                {
                    _QasThreshold = Int32.Parse(ConfigurationManager.AppSettings["QasThreshold"].ToString());
                    return _QasThreshold;
                }
                catch
                {
                    return _QasThreshold;
                }
            }
        }

        //<!-- The Intensity value (Exact, Close, Extensive) that QAS will use when verifying an address -->
        //<add key ="QASIntensity" value ="Close" />
        private static string _QasIntensity = "Close";
        public static string QasIntensity
        {
            get
            {
                try
                {
                    _QasIntensity = ConfigurationManager.AppSettings["QasIntensity"].ToString();
                    return _QasIntensity;
                }
                catch
                {
                    return _QasIntensity;
                }
            }
        }

        //<add key ="QASTimeout" value ="20000" />
        private static int _QasTimeout = 20000;
        public static int QasTimeout
        {
            get
            {
                try
                {
                    _QasTimeout = Int32.Parse(ConfigurationManager.AppSettings["QasTimeout"].ToString());
                    return _QasTimeout;
                }
                catch
                {
                    return _QasTimeout;
                }
            }
        }

        private static int _QasPickListLength = 3;
        public static int QasPickListLength
        {
            get
            {
                try
                {
                    _QasPickListLength = Int32.Parse(ConfigurationManager.AppSettings["QasPickListLength"].ToString());
                    return _QasPickListLength;
                }
                catch
                {
                    return _QasPickListLength;
                }
            }
        }


    }
}
