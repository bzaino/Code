using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.SessionState;

namespace ADTool
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {

        public void Init(HttpApplication app)
        {
            app.BeginRequest += (new EventHandler(this.OnBeginRequest));
            app.PreRequestHandlerExecute +=
                (new EventHandler(this.OnPreRequestHandlerExecute));
            if (app.Modules["Session"] != null)
            {
                SessionStateModule session = (SessionStateModule)
                   app.Modules["Session"];
                app.AcquireRequestState +=
                   (new EventHandler(this.OnAcquireRequestState));
                session.Start += (new EventHandler(this.OnSessionStart));
            }
        }
        
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
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
        }

        public EventHandler OnBeginRequest { get; set; }

        public EventHandler OnPreRequestHandlerExecute { get; set; }

        public EventHandler OnAcquireRequestState { get; set; }

        public EventHandler OnSessionStart { get; set; }
    }
}