using System;
using System.ServiceModel.Activation;
using System.Web;
using System.Web.Routing;
using ASA.Web.Services.AlertService;
using ASA.Web.Services.ASAMemberService;
using ASA.Web.Services.LessonsService;
using ASA.Web.Services.LessonsService.ServiceContracts;
using ASA.Web.Services.ReminderService;
using ASA.Web.Services.SearchService;
using ASA.Web.Services.SelfReportedService;
using ASA.Web.Services.SurveyService;
using ASA.Web.Services.ContentService;

namespace ASA.Web.Services.SALHost
{
    public class Global : System.Web.HttpApplication
    {

        private const string CLASSNAME = "ASA.Web.Services.SALHost.Global";
        static ASA.Log.ServiceLogger.IASALog _log = ASA.Log.ServiceLogger.ASALogManager.GetLogger(CLASSNAME);

        public static void RegisterRoutes(RouteCollection routes)
        {
            _log.InfoFormat("inside register routes");
            if (routes != null)
            {
                _log.InfoFormat("about to attach routes");
                if (Config.StartAlertService)
                    routes.Add(new ServiceRoute("AlertService", new WebServiceHostFactory(), typeof(Alert)));
                if (Config.StartContentService)
                    routes.Add(new ServiceRoute("ContentService", new WebServiceHostFactory(), typeof(Content)));
                if (Config.StartMembershipService)
                    routes.Add(new ServiceRoute("ASAMemberService", new WebServiceHostFactory(), typeof(ASAMember)));
                if (Config.StartReminderService)
                    routes.Add(new ServiceRoute("ReminderService", new WebServiceHostFactory(), typeof(Reminder)));
                if(Config.StartSearchService)
                   routes.Add(new ServiceRoute("SearchService", new WebServiceHostFactory(), typeof(Search)));
                if(Config.StartSelfReportedService)
                    routes.Add(new ServiceRoute("SelfReportedService", new WebServiceHostFactory(), typeof(SelfReported)));
                if (Config.StartSurveyService)
                    routes.Add(new ServiceRoute("SurveyService", new WebServiceHostFactory(), typeof(Survey)));

                routes.Add(new ServiceRoute("LessonsService", new WebServiceHostFactory(), typeof(Lessons)));
            }
            else
                _log.Error("The RoutesCollection is null!");
        }

       

        protected void Application_Start()
        {
            try
            {
                // Code that runs on application startup
                log4net.Config.XmlConfigurator.Configure();
                _log.InfoFormat("SAL Host Startup");
                RegisterRoutes(RouteTable.Routes);
                //LoginToAvectra();
            }
            catch(Exception ex)
            {
                _log.Fatal("Application was not able to start correctly!", ex);
            }
        }

        protected void Application_EndRequest()
        {
            //We need to override the FormsAuthentication redirect, SAL data endpoints shouldnt redirect to the homepage
            //Check for the proper status code and RedirectPath before changing to a 401 error.
            if (Context.Response.StatusCode == 302 && Context.Response.RedirectLocation.Contains("/Home/RedirectLogOn?ReturnUrl="))
            {
                Context.Response.Clear();
                Context.Response.StatusCode = 401;
            }
        }

        protected void Application_PreSendRequestHeaders()
        {
            //Remove "Server" header from Response - JIRA 
            HttpContext.Current.Response.Headers.Remove("Server");
        }

      

        void Application_End(object sender, EventArgs e)
        {
            //  Code that runs on application shutdown
            _log.InfoFormat("SAL Host Shutdown");
        }

        //protected void Application_Error(object sender, EventArgs e)
        //{
        //    // Code that runs when an unhandled error occurs
        //    //  Code that runs on application shutdown
        //    Log.Info("Application error has o");
        //}

    }
}

