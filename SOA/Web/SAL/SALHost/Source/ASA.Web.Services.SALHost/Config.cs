using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace ASA.Web.Services.SALHost
{
    public class Config
    {
        private static bool _startAlertService = false;
        public static bool StartAlertService
        {
            get
            {
                try
                {
                    _startAlertService = Boolean.Parse(ConfigurationManager.AppSettings["startAlertService"].ToString());
                    return _startAlertService;
                }
                catch
                {
                    return _startAlertService;
                }
            }
        }

        private static bool _startContentService = false;
        public static bool StartContentService
        {
            get
            {
                try
                {
                    return _startContentService = Boolean.Parse(ConfigurationManager.AppSettings["startContentService"].ToString());
                }
                catch
                {
                    
                    return _startContentService;
                }
            }
        }

        private static bool _startMembershipService = false;
        public static bool StartMembershipService
        {
            get
            {
                try
                {
                    _startMembershipService = Boolean.Parse(ConfigurationManager.AppSettings["startMembershipService"].ToString());
                    return _startMembershipService;
                }
                catch
                {
                    return _startMembershipService;
                }
            }
        }

        private static bool _startReminderService = false;
        public static bool StartReminderService
        {
            get
            {
                try
                {
                    _startReminderService = Boolean.Parse(ConfigurationManager.AppSettings["startReminderService"].ToString());
                    return _startReminderService;
                }
                catch
                {
                    return _startReminderService;
                }
            }
        }

        private static bool _startSearchService = false;
        public static bool StartSearchService
        {
            get
            {
                try
                {
                    _startSearchService = Boolean.Parse(ConfigurationManager.AppSettings["startSearchService"].ToString());
                    return _startSearchService;
                }
                catch
                {
                    return _startSearchService;
                }
            }
        }

        private static bool _startSelfReportedService = false;
        public static bool StartSelfReportedService
        {
            get
            {
                try
                {
                    _startSelfReportedService = Boolean.Parse(ConfigurationManager.AppSettings["startSelfReportedService"].ToString());
                    return _startSelfReportedService;
                }
                catch
                {
                    return _startSelfReportedService;
                }
            }
        }

        private static bool _startSurveyService = false;
        public static bool StartSurveyService
        {
            get
            {
                try
                {
                    _startSurveyService = Boolean.Parse(ConfigurationManager.AppSettings["startSurveyService"].ToString());
                    return _startSurveyService;
                }
                catch
                {
                    return _startSurveyService;
                }
            }
        }
    }
}