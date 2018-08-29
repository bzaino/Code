using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace HttpsRequestWrapper
{
    public class Config
    {
        private static int _requestRetryCount;
        public static int RequestRetryCount
        {
            get
            {
                try
                {
                    _requestRetryCount = ConfigurationManager.AppSettings["RequestRetryCount"] != string.Empty ? Convert.ToInt32(ConfigurationManager.AppSettings["RequestRetryCount"]) : 2;
                    return _requestRetryCount;
                }
                catch
                {
                    return _requestRetryCount;
                }
            }
        }

        private static int _individualScholarshipTimeout;
        public static int IndividualScholarshipTimeout
        {
            get
            {
                try
                {
                    _individualScholarshipTimeout = Convert.ToInt32(ConfigurationManager.AppSettings["IndividualScholarshipTimeout"].ToString());
                    return _individualScholarshipTimeout;
                }
                catch
                {
                    return _individualScholarshipTimeout;
                }
            }
        }
    }
}
