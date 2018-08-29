using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ASA.Web.Services.ASAMemberService.ServiceContracts;
using ASA.Web.Services.ASAMemberService;
using ASA.Web.WTF;
using ASA.Web.WTF.Integration.MVC3;
using ASA.Web.Sites.SALT.Models;

namespace ASA.Web.Sites.SALT.Controllers
{
    public class ConfigController : AsaController
    {
        private static ConfigModel _configModel = null;

        public ConfigController()
        {
            _configModel = new ConfigModel();
        }

        private void populateConfigModel()
        {
            _configModel = new ConfigModel();
            _configModel.MemberService = System.Configuration.ConfigurationManager.AppSettings["MemberService"].ToString();
            _configModel.LoanServiceEndpoint = System.Configuration.ConfigurationManager.AppSettings["LoanServiceEndpoint"].ToString();
            _configModel.SelfReportedServiceEndpoint = System.Configuration.ConfigurationManager.AppSettings["SelfReportedServiceEndpoint"].ToString();
            _configModel.AddrValidationServiceEndpoint = System.Configuration.ConfigurationManager.AppSettings["AddrValidationServiceEndpoint"].ToString();
            _configModel.SearchServiceEndpoint = System.Configuration.ConfigurationManager.AppSettings["SearchServiceEndpoint"].ToString();
            _configModel.AlertServiceEndpoint = System.Configuration.ConfigurationManager.AppSettings["AlertServiceEndpoint"].ToString();
            _configModel.ReminderService = System.Configuration.ConfigurationManager.AppSettings["ReminderService"].ToString();
            _configModel.SurveyServiceEndpoint = System.Configuration.ConfigurationManager.AppSettings["SurveyServiceEndpoint"].ToString();
            _configModel.TemplateDirectory = System.Configuration.ConfigurationManager.AppSettings["TemplateDirectory"].ToString();

            /// int minutes = 20;
            int minutes = Convert.ToInt16(System.Configuration.ConfigurationManager.AppSettings["SessionTimeOut"]);
             if (minutes == 0)
             {
                 minutes = 30;
             }
            string loginRedirectPage = Url.Content("~/Home");
            System.Web.Configuration.AuthenticationSection authSection =
                (System.Web.Configuration.AuthenticationSection)System.Configuration.ConfigurationManager.GetSection("system.web/authentication");
            if (authSection != null && authSection.Forms != null)
            {
               // if (authSection.Forms.Timeout != null)
                ///{
               ///     minutes = authSection.Forms.Timeout.Minutes;
                ///}
                if (!string.IsNullOrEmpty(authSection.Forms.LoginUrl))
                {
                    loginRedirectPage = Url.Content(authSection.Forms.LoginUrl);
                }
            }
            
            _configModel.IsAuthenticated = SiteMember.Account.IsAuthenticated.ToString().ToLower();
            _configModel.FormsAuthTimeoutValue = minutes;
            _configModel.LoginRedirectPage = loginRedirectPage;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cfgModel"></param>
        public ConfigController(ConfigModel cfgModel)
        {
            _configModel = cfgModel;
        }

        [HttpPost]
        [DisableContentLoad]
        public ActionResult Index()
        {
            populateConfigModel();
            return Json(_configModel);
        }
    }
}