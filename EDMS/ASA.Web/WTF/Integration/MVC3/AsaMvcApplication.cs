using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

using ASA.Web;

using System.Configuration;
using DirectoryServicesWrapper;
using System.Web.Security;

namespace ASA.Web.WTF.Integration.MVC3
{
    /// <summary>
    /// Top Level Harness for an ASA Web Application using MVC. This class provides the wireup to 
    /// integration testing and startup procedures for an MVC application leveraging the ASA.Web frameworks
    /// and API's
    /// 
    /// This class should be inherited by either the Global.asax.cs class or whatever base
    /// class is being used for the MVC application. 
    /// 
    /// Note this class instance provides addtional application life-cycle events that may be subscribed to. 
    /// </summary>
    public class AsaMvcApplication : HttpApplication
    {
        private const string CLASSNAME = "ASA.Web.WTF.Integration.MVC3.AsaMvcApplication";
        private static ASA.Log.ServiceLogger.IASALog _log = ASA.Log.ServiceLogger.ASALogManager.GetLogger(CLASSNAME);

        //Note: See IWebApplication.cs for application lifecycle
        private static bool _bInitialized = false; //QC 3956: Force HTTPs for the site
        private static bool _bForceHttps = false; //QC 3956: Force HTTPs for the site

        private ApplicationState CurrentAppState { get; set; }

        private enum ApplicationState
        {
            NotStarted,
            StartingUp,
            ApplicationRunning,
            ShuttingDown,
            Disabled,
            Error, 
            InSession, 
            InRequest,
            InAppRequest
        }


        private static ASAIntegration _configuration;
        //private static Boolean _sessionPersistenceTested = false;

        //private static Boolean _applicationStarted = false;

        #region Class Start
        private static Boolean _loggingStarted = false;

        //Name for the value of the current request key in persistence.
        private const string __currentRequestKeyLookupName = "ASAMVCAPP[CurrentRequestKey]";


        /// <summary>
        /// Class Constructor - Best pratice is not to override or use this.
        /// Keep in mind if you do that this is called with every request and your
        /// context at this state is uncertian without doing pre-checking of multiple values
        /// </summary>
        public AsaMvcApplication()
        {
            String logMethodName = ".ctor() - ";

            BeginRequest += (o, e) => { OnAfterRequestStart(e); };
            EndRequest += (o, e) => { OnAfterRequestEnd(e); };
            MapRequestHandler += (o, e) => { OnAfterApplicationRequestStart(e); };
            PostRequestHandlerExecute += (o, e) => { OnAfterApplicationRequestEnd(e); };

            CurrentAppState = ApplicationState.NotStarted;
            
            if (!_loggingStarted)
            {
                //We load logging first and foremost so we can start tracking the 
                //application load process as early as possible. Further this way
                //if logging load fails we can simply ingore it and move on 
                //while a failure in filters or routes will cause the application startup to 
                //abort.
                try
                {
                    log4net.Config.XmlConfigurator.Configure();
                    _log.Info(logMethodName + "ASA MVC Web Application Logger Started - APPLICATION LOGGING START");
                }
                catch (Exception ex)
                {
                    //There is nothing we can do here, there is no way to log this failue
                    //and we don't want to abort the application just because logging won't load
                    _log.Info(logMethodName + "Exception caught. Message: " + ex.Message);
                }

                _loggingStarted = true;
            }


            //NOTE: Logging statements from this method will not show up on initial load. 
            //post application launch they will behave normally

            _log.Debug(logMethodName + "Begin Method");

            if (_configuration == null)
            {
                try
                {
                    _log.Debug(logMethodName + "Getting ASAIntegration Config");
                    _configuration = (ASAIntegration)ConfigurationManager.GetSection("asaIntegration");
                }
                catch (Exception ex)
                {
                    _log.Error(logMethodName + "Unable to load integration configuration", ex);
                    throw new MVCIntegrationException("Unable to load integration configuration", ex);
                }
            }
            //ASAContextLoader handles lower level lifecycle concerns
            //like preloading integration/content, and providing application 
            //level context for integration interactions. 
            //This call give the context loader an easy way to hook into the application lifecycle
            //at the earliest possible point. 
            ASAContextLoader.RegisterApplication(this);
            _log.Debug(logMethodName + "End Method");
        }

        /// <summary>
        /// Class Init - Best pratice is not to override or use this.
        /// Keep in mind if you do that this is called with every request and your
        /// context at this state is uncertian without doing pre-checking of multiple values
        /// </summary>
        /// Note: This method is actually part of the reqest lifcycle and is ALWAYS called first
        /// on any requst. Note there is no context here, the request has not even been through
        /// security checks at this point. Be VERY careful with any processing done at this stage.
        public override void Init()
        {
            //NOTE: Logging statements from this method will not show up on initial load. 
            //post application launch they will behave normally
            String logMethodName = ".Init() - ";
            _log.Debug(logMethodName + "Begin Method");

            base.Init();

            _log.Debug(logMethodName + "End Method");
        }
        #endregion



        #region Application Lifecycle
        /// <summary>
        /// Provides end of application startup tasks. Be sure to include base.Application_Start at the Beginning
        /// of your implmentation when overriding. 
        /// 
        /// Alternative access with BeforeApplicationStart and AfterApplicationStart events.
        /// </summary>
        /// <param name="sender">HttpApplication</param>
        /// <param name="e">Event Args</param>
        protected virtual void Application_Start()
        {
            String logMethodName = ".Application_Start() - ";
            _log.Debug(logMethodName + "Begin Method");

            CurrentAppState = ApplicationState.StartingUp;

            // ======================= EVENT SIGNALING BEFORE PROCESSING MUST ALWAYS RUN FIRST =======================
            _log.Debug(logMethodName + "Siginaling Application Start Event Name: OnBeforeApplicationStart....");
            OnBeforeApplicationStart();
            _log.Debug(logMethodName + "All application start hooks processed for OnBeforeApplicationStart....");
            // ======================= EVENT SIGNALING BEFORE PROCESSING MUST ALWAYS RUN FIRST =======================

            _log.Info(logMethodName + "========= ASA MVC Web Application Startup Routine =========");

            #region MVC Start Tasks
            try
            {
                _log.Info(logMethodName + "Registering ASP.NET MVC Filters");
                RegisterGlobalFilters(GlobalFilters.Filters);

                _log.Info(logMethodName + "Registering ASP.NET MVC Routes");
                RegisterRoutes(RouteTable.Routes);

                _log.Info(logMethodName + "Removing HTTP MVC Response Header");
                MvcHandler.DisableMvcResponseHeader = true;
                
            }
            catch (Exception ex)
            {
                _log.Error(logMethodName + "Application Start failed!", ex);
                throw new MVCIntegrationException("Application Start failed!", ex);
            }
            #endregion


            // ======================= EVENT SIGNALING AFTER PROCESSING MUST ALWAYS RUN LAST =======================
            _log.Debug(logMethodName + "Siginaling Application Start Event Name: OnAfterApplicationStart....");
            OnAfterApplicationStart();
            _log.Debug(logMethodName + "All application start hooks processed for OnAfterApplicationStart....");
            // ======================= EVENT SIGNALING AFTER PROCESSING MUST ALWAYS RUN LAST =======================

            LogFilters(GlobalFilters.Filters);
            LogRoutes(RouteTable.Routes);

            //_applicationStarted = true;

            CurrentAppState = ApplicationState.ApplicationRunning;

            _log.Info(logMethodName + "========= ASA MVC Web Application Startup Routine Completed =========");
            _log.Debug(logMethodName + "End Method");

        }


        /// <summary>
        /// Fires after processing of steps in the socket request layer prior to routing or application target spcific context.
        /// 
        /// USE WITH CARE - This is prior to having the request authorized by security
        /// </summary>
        public event EventHandler AfterRequestStart;
        protected virtual void OnAfterRequestStart(EventArgs args = null)
        {
            if (AfterRequestStart != null)
            {
                AfterRequestStart(this, args != null ? args : EventArgs.Empty);
            }
        }

        /// <summary>
        /// Fires after processing of steps in the socket layer after the applciation request has completed.
        /// </summary>
        public event EventHandler AfterRequestEnd;
        protected virtual void OnAfterRequestEnd(EventArgs args = null)
        {
            if (AfterRequestEnd != null)
            {
                AfterRequestEnd(this, args != null ? args : EventArgs.Empty);
            }
        }

        protected void Application_PreSendRequestHeaders()
        {
            //Remove "Server" header from Response - JIRA 
            HttpContext.Current.Response.Headers.Remove("Server");
        }

        //QC 3956: Force HTTPs for the site (NOTE: previously tried [RequireHttps] on
        // controller methods releated to Logon and LoginPage, but this did not work
        // due to CORS implementation for Logon Overlays... hence we are going with this 
        // solution which forces HTTPs for all requests to the site.)
        protected void Application_BeginRequest()
        {
            if (_bForceHttps == true && !Context.Request.Url.ToString().ToLower().EndsWith("monitor/health.html"))
            {
                if (!Context.Request.IsSecureConnection)
                    Response.Redirect(Context.Request.Url.ToString().Replace("http:", "https:"));
            }
        }

        /// <summary>
        /// Provides end of application cleanup tasks. Be sure to include base.Application_End at the end
        /// of your implmentation when overriding. 
        /// 
        /// Alternative access with BeforeApplicationEnd and AfterApplicationEnd events.
        /// </summary>
        /// <param name="sender">HttpApplication</param>
        /// <param name="e">Event Args</param>
        protected virtual void Application_End(object sender, EventArgs e)
        {
            String logMethodName = ".Application_End(object sender, EventArgs e) - ";
            _log.Debug(logMethodName + "Begin Method");

            // ======================= EVENT SIGNALING BEFORE PROCESSING MUST ALWAYS RUN FIRST =======================
            _log.Debug(logMethodName + "Siginaling Application Event Name: OnBeforeApplicationEnd....");
            OnBeforeApplicationEnd();
            _log.Debug(logMethodName + "All application hooks processed for OnBeforeApplicationEnd....");
            // ======================= EVENT SIGNALING BEFORE PROCESSING MUST ALWAYS RUN FIRST =======================

            _log.Info(logMethodName + "SALT Application Shutting Down - Cleaning up our toys....");

            // ======================= EVENT SIGNALING AFTER PROCESSING MUST ALWAYS RUN LAST =======================
            _log.Debug(logMethodName + "Siginaling Application Event Name: OnAfterApplicationEnd....");
            OnAfterApplicationEnd();
            _log.Debug(logMethodName + "All application hooks processed for OnAfterApplicationEnd....");
            // ======================= EVENT SIGNALING AFTER PROCESSING MUST ALWAYS RUN LAST =======================

            _log.Debug(logMethodName + "End Method");
        }
        #endregion

        #region Session Lifecycle
        /// <summary>
        /// Provides end of session startup tasks. Be sure to include base.Session_Start at the beginning
        /// of your implmentation when overriding. 
        /// 
        /// Alternative access with BeforeSessionStart and AfterSessionStart events.
        /// </summary>
        /// <param name="sender">HttpApplication</param>
        /// <param name="e">Event Args</param>
        protected virtual void Session_Start(object sender, EventArgs e)
        {
            String logMethodName = ".Session_Start(object sender, EventArgs e) - ";
            _log.Debug(logMethodName + "Begin Method");

            _log.Info(logMethodName + "New User session starting");


            // ======================= EVENT SIGNALING AFTER PROCESSING MUST ALWAYS RUN LAST =======================
            _log.Debug(logMethodName + "Siginaling Application Event Name: OnAfterSessionStart....");
            OnAfterSessionStart();
            _log.Debug(logMethodName + "All application hooks processed for OnAfterSessionStart....");
            // ======================= EVENT SIGNALING AFTER PROCESSING MUST ALWAYS RUN LAST =======================

            CurrentAppState = ApplicationState.InSession;

            _log.Debug(logMethodName + "End Method");
        }

        /// <summary>
        /// Provides end of session cleanup tasks. Be sure to include base.Session_End at the end
        /// of your implmentation when overriding. 
        /// 
        /// Alternative access with BeforeSessionEnd and AfterSessionEnd events.
        /// </summary>
        /// <param name="sender">HttpApplication</param>
        /// <param name="e">Event Args</param>
        protected virtual void Session_End(object sender, EventArgs e)
        {
            String logMethodName = ".Session_End(object sender, EventArgs e) - ";
            _log.Debug(logMethodName + "Begin Method");

            _log.Info(logMethodName + "User session ending, cleaning up...");

            // ======================= EVENT SIGNALING AFTER PROCESSING MUST ALWAYS RUN LAST =======================
            _log.Debug(logMethodName + "Siginaling Application Event Name: OnAfterSessionEnd....");
            OnAfterSessionEnd();
            _log.Debug(logMethodName + "All application hooks processed for OnAfterSessionEnd....");
            // ======================= EVENT SIGNALING AFTER PROCESSING MUST ALWAYS RUN LAST =======================

            _log.Debug(logMethodName + "End Method");
        }
        #endregion

        #region MVC Functions (Routing, Filters, etc)
        /// <summary>
        /// Registers default filters for framework intgration. 
        /// Be sure to include base.RegisterGlobalFilters at the end of your method when overriding. 
        /// </summary>
        /// <param name="routes">RouteCollection from GlobalFilters.Filters</param>
        protected virtual void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            String logMethodName = ".RegisterGlobalFilters(GlobalFilterCollection filters) - ";
            _log.Debug(logMethodName + "Begin Method");

            filters.Add(new HandleErrorAttribute());

            _log.Debug(logMethodName + "End Method");
        }

        /// <summary>
        /// Registers default routes for framework intgration. 
        /// Be sure to include base.RegisterRoutes at the end of your method when overriding. 
        /// </summary>
        /// <param name="routes">RouteCollection from RouteTable.Routes</param>
        protected virtual void RegisterRoutes(RouteCollection routes)
        {
            String logMethodName = ".RegisterSiteRoutes() - ";
            _log.Debug(logMethodName + "Begin Method");

            //This method of blocking MVC from trying to process requests for non-existant static files seems to work well. 
            // Note: Right now these will get added to the end of a route collection if this class is inherited
            routes.IgnoreRoute("Content/{*pathInfo}");
            routes.IgnoreRoute("PublishedContent/{*pathInfo}");
            routes.IgnoreRoute("Assets/Scripts/{*pathInfo}");
            routes.IgnoreRoute("Views/{*pathInfo}");
            routes.IgnoreRoute("css/{*pathInfo}");

            //Catch all route if nothing else above matches. The published content controller will attempt to find a content file at
            //the requested path and load it with a generic view file. 
            //routes.MapRoute(
            //    "PublishedContent", // Route name
            //    "{*id}", // URL with parameters
            //    new { controller = "PublishedContent", action = "Details", id = UrlParameter.Optional } // Parameter defaults
            //);

            _log.Debug(logMethodName + "End Method");
        }
        #endregion

        /// <summary>
        /// Application Error event receiver. Overide this to process error events.
        /// Remember to call base.Application_Error after you have completed processing or
        /// error handling for proper logging/redirection will be broken site-wide.
        /// 
        /// Alternatively you may also listen for the Error event from this class which does not 
        /// have the base call requirement post processing.
        /// </summary>
        /// <param name="sender">HttpApplication</param>
        /// <param name="e">Event arguments</param>
        protected virtual void Application_Error(object sender, EventArgs e)
        {
            String logMethodName = ".Application_Error(object sender, EventArgs e) - ";
            _log.Debug(logMethodName + "Begin Method");

            Exception exception = Server.GetLastError();

            //TODO: Right now any error that makes it this far is just logged and sent 
            //on its way for ASP.NET and IIS to handle with the correct re-directs.
            //There may be states where we want to do something else instead of the defalt
            //redirect configuration. Analysis should be done on various negative case
            //states we may want to handle for here, in particular for any security vectors
            //that throwing/processing a default response may be undesirable.

            // cmak. SWD-5581. log Request.Path errors as WARN.  else log exception as error.  note that string compare is case sensitive.
            Mvc3Helper helper = new Mvc3Helper();
            if ( helper.IsRequestPathException(exception) )
            {
                _log.Warn(logMethodName + "There has been a possible security warning with the ASA Web Application", exception);
            }
            else
            {
                _log.Error(logMethodName + "There has been an error with the ASA Web Application", exception);
            }

            _log.Debug(logMethodName + "End Method");
        }

        #region Logging helpers
        /// <summary>
        /// Writes to the log a list of the provided routes as Info statements
        /// </summary>
        /// <param name="filters">RouteCollection in most cases from RouteTable.Routes</param>
        protected void LogRoutes(RouteCollection routes)
        {
            String logMethodName = ".LogCurrentRoutes(RouteCollection routes) - ";
            _log.Debug(logMethodName + "Begin Method");
            _log.Info(logMethodName + "====== Currenlty Registered Routes ======");

            if (routes != null)
            {
                using (routes.GetReadLock())
                {
                    foreach (RouteBase routeBase in routes)
                    {
                        Route route = routeBase as Route;
                        if (route != null)
                        {
                            _log.Info(logMethodName
                                + "Handler : " + route.RouteHandler.GetType().FullName.ToString()
                                + " | Route - Url Pattern : " + route.Url);
                        }
                    }
                }
            }

            _log.Debug(logMethodName + "End Method");
        }

        /// <summary>
        /// Writes to the log a list of the provided filters as Info statements
        /// </summary>
        /// <param name="filters">GlobalFilterCollection in most cases from GlobalFilters.Filters</param>
        protected void LogFilters(GlobalFilterCollection filters)
        {
            String logMethodName = ".LogCurrentFilters(GlobalFilterCollection filters) - ";
            _log.Debug(logMethodName + "Begin Method");
            _log.Info(logMethodName + "====== Currenlty Registered Filters ======");

            if (filters != null)
            {
                foreach (Filter filter in filters)
                {
                    if (filter != null)
                    {
                        _log.Info(logMethodName
                            + "Filter - Type : " + filter.Instance.GetType().FullName
                            + " | Scope : " + filter.Scope.ToString()
                            + " | Order : " + filter.Order);
                    }
                }
            }

            _log.Debug(logMethodName + "End Method");
        }
        #endregion



        #region Application Request Events

        /// <summary>
        /// Fires after processing application start-up
        /// </summary>
        // Referenced in ASAContextLoader
        public event EventHandler AfterApplicationStart;
        protected virtual void OnAfterApplicationStart(EventArgs args = null)
        {
            if (AfterApplicationStart != null)
            {
                AfterApplicationStart(this, args != null ? args : EventArgs.Empty);
            }
        }

        /// <summary>
        /// Fires before processing application start-up
        /// </summary>
        // Referenced in SALTApplication
        public event EventHandler BeforeApplicationStart;
        protected  void OnBeforeApplicationStart(EventArgs args = null)
        {
            //QC 3956: Force HTTPs for the site
            if (_bInitialized == false)
            {
                _bInitialized = true;
                Boolean.TryParse(System.Configuration.ConfigurationManager.AppSettings["ForceHttpsForSite"], out _bForceHttps);
            }

            if (BeforeApplicationStart != null)
            {
                BeforeApplicationStart(this, args != null ? args : EventArgs.Empty);
            }
        }

        /// <summary>
        /// Fires after processing of steps in the application layer after the socket request has completed its start.
        /// </summary>
        // Referenced in ASAContextLoader
        public event EventHandler AfterApplicationRequestStart;
        protected void OnAfterApplicationRequestStart(EventArgs args = null)
        {
            if (IsMvcHandler())
            {
                if (AfterApplicationRequestStart != null)
                {
                    AfterApplicationRequestStart(this, args != null ? args : EventArgs.Empty);
                }
            }
        }


        /// <summary>
        /// Fires before the application shutdown process begins.
        /// </summary>
        // Referenced in SALTApplication.cs
        public event EventHandler BeforeApplicationEnd;
        protected virtual void OnBeforeApplicationEnd(EventArgs args = null)
        {
            if (BeforeApplicationEnd != null)
            {
                BeforeApplicationEnd(this, args != null ? args : EventArgs.Empty);
            }
        }


        /// <summary>
        /// Fires after the aplication shutdown process has completed.
        /// </summary>
        // Referenced in ASAContextLoader
        public event EventHandler AfterApplicationEnd;
        protected virtual void OnAfterApplicationEnd(EventArgs args = null)
        {
            if (AfterApplicationEnd != null)
            {
                AfterApplicationEnd(this, args != null ? args : EventArgs.Empty);
            }
        }

        /// <summary>
        /// Fires after processing of steps in the application layer's request cleanup
        /// </summary>
        // Referenced in ASAContextLoader
        public event EventHandler AfterApplicationRequestEnd;
        protected void OnAfterApplicationRequestEnd(EventArgs args = null)
        {
            if (IsMvcHandler())
            {
                if (AfterApplicationRequestEnd != null)
                {
                    AfterApplicationRequestEnd(this, args != null ? args : EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Fires after the session shutdown process has completed.
        /// </summary>
        // Referenced in ASAContextLoader
        public event EventHandler AfterSessionEnd;
        protected virtual void OnAfterSessionEnd(EventArgs args = null)
        {
            if (AfterSessionEnd != null)
            {
                AfterSessionEnd(this, args != null ? args : EventArgs.Empty);
            }
        }

        /// <summary>
        /// Fires after the session startup process has completed.
        /// </summary>
        // Referenced in ASAContextLoader
        public event EventHandler AfterSessionStart;
        protected virtual void OnAfterSessionStart(EventArgs args = null)
        {
            if (AfterSessionStart != null)
            {
                AfterSessionStart(this, args != null ? args : EventArgs.Empty);
            }
        }

        private Boolean IsMvcHandler()
        {
            if (HttpContext.Current != null
                && HttpContext.Current.Handler != null
                && HttpContext.Current.Handler.GetType() == typeof(MvcHandler)) //Order of operation on this if statement is very specific. Using && ensures if eval will abort before doing anything that causes an exception
            {
                return true;
            }

            return false;
        }
        #endregion

    }
}