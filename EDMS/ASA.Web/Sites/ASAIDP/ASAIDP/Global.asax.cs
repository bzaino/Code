using System;
using System.Web.Mvc;
using System.Web.Routing;
using IDP.ASBSSOAdapter;
using Ninject;
using ASBIdentitySource.Plugin;

using ASA.Log.ServiceLogger;
using ComponentSpace.SAML2;
using ASAIDP.Session;

namespace ASAIDP
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        private static Boolean _loggingStarted = false;
        private static readonly IASALog _log = ASALogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );

        }

        protected void Application_Start()
        {
            if (!_loggingStarted)
            {
                try
                {
                    log4net.Config.XmlConfigurator.Configure();
                    _log.Info("ASAIDP Application Logger Started - APPLICATION START");
                }
                catch (Exception ex)
                {
                    //There is nothing we can do here, there is no way to log this failue
                    //and we don't want to abort the application just because logging won't load
                    _log.Info("ASAIDP Logging Start Exception => " + ex.ToString());
                }

                _loggingStarted = true;
            }

            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            SAMLController.SSOSessionStore = new CookieSSOSession();

            ASBSSOAdapterModule.Init();            

            IKernel kernel = ASBSSOAdapterModule.GetKernel();

            kernel.Bind<IDVerifier>().To<ASAIDP.SSO.Plugins.SiteMemberPlugin>().
                   Named("ASAIDP.SSO.Plugins.SiteMemberPlugin");
        }

        protected void Application_End(object sender, EventArgs e)
        {
            //  Code that runs on application shutdown
            _log.Info("ASAIDP - APPLICATION SHUTDOWN");
        }

        protected void Application_PreSendRequestHeaders(Object source, EventArgs e)
        {
            Context.Response.CacheControl = "no-cache";
        }
    }
}