using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

using ASA.Web;
using ASA.Web.WTF.Integration.MVC3;

namespace ASA.Web.Sites.SALT
{
    /// <summary>
    /// Top level application wrapper for SALT
    /// </summary>
    public class SALTApplication : AsaMvcApplication
    {
        //TODO: Figure out why SALTApplication log statements do not write to the log
        //it appears to have something to do with the inhertiance chain of the class but 
        //im honestly not sure - jfm
        private const string CLASSNAME = "ASA.Web.Sites.SALT.SALTApplication";
        private static ASA.Log.ServiceLogger.IASALog _log;

        public SALTApplication() : base()
        {
            //The MvcApplication is responsible for initilizing logging. If we try to get a logger
            //before logging has been configured then this class will have no logging for the life of 
            //the application. 
            //
            //To fix this we break the normal logger get pattern and load in the constructor after
            //the application base has done its job. 
            //
            //We only want to do this the first time we need to (the logger is static) so we check 
            //for null. 
            //Note, other than registering for events and setting this logger you should avoid doing
            //too much in an appliction constructor as they are called with every system request. 
            if (_log == null)
            {
                _log = ASA.Log.ServiceLogger.ASALogManager.GetLogger(CLASSNAME);
            }

            //Hooking in before the application starts up
            BeforeApplicationStart += new EventHandler(PreApplicationStart);
            BeforeApplicationEnd += new EventHandler(PreApplicationShutDown);
        }

        //Since all the commidized functions are now in AsaMvcApplication we only need to register
        //the routes and polices we care about. The base implmentation will handle adding the correct
        //routes for error, published content and common handling needs for the framework. 
        protected override void RegisterRoutes(RouteCollection routes)
        {
            String logMethodName = ".RegisterSiteRoutes() - ";
            _log.Debug(logMethodName + "Begin Method");


            if (routes != null)
            {
                // Dont bother routing these....
                routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
                routes.IgnoreRoute("{*favicon}", new { favicon = @"(.*/)?favicon.ico(/.*)?" });

                //Standard controller routes, Required due to catch all PublishedContentController
                routes.MapRoute("Home", "Home/{action}/{id}", new { controller = "Home", action = "Index", id = UrlParameter.Optional }); // What is this one for? Login modals? Please comment
                routes.MapRoute("Account", "Account/{action}/{id}", new { controller = "Account", action = "Index", id = UrlParameter.Optional });
                routes.MapRoute("Root", "", new { controller = "Home", action = "Index" });  //routes all requests to root --> Home/Index
                routes.MapRoute("Config", "Config/{action}", new { controller = "Config", action = "Index" });
            }
            else
            {
                //Is it actually possible for this to happen? Is there a case where we expect an external
                //instnace consumer to call this method rather than it being called from within the Global.asax.cs?
                //Is there a case where MVC may pass us a null object collection here?
                _log.Error(logMethodName + "The RouteCollection is null!");
                throw new SALTException("The RouteCollection is null!");
            }

            //Alwyas run the base method last here. If there needs to be inserts at the head or foot of the collections
            //its best to do this witl a completed collection. 
            base.RegisterRoutes(routes);
            _log.Debug(logMethodName + "End Method");
        }

        #region Application Startup and Shutdown
        /// <summary>
        /// This method will be called just before the main application code begins to load. 
        /// </summary>
        /// <param name="sender">AsaMvcApplication</param>
        /// <param name="e">EventArgs</param>
        /// This is the reccomended way to hook into keey application events. It is not reccomened
        /// or desirable to override key application lifecycle event methods unless completly neccassary
        /// it is very easy to break the lifecycle or introduce stability/security flaws when overriding
        /// lifecycle events. 
        /// This method serves as an example and an easy way for the SALT application to have a friendly
        /// log header.
        private void PreApplicationStart(Object sender, EventArgs e)
        {
            String logMethodName = ".PreApplicationStart(Object sender, EventArgs e) - ";
            _log.Debug(logMethodName + "Begin Method");

            _log.Info("=====================================================================");
            _log.Info("====== American Student Assistance - SALT Web Application v1.0 ======");
            _log.Info("=====================================================================");
            _log.Info("~~ Designed and developed  by ASA's ISD Web Devlopment Team ~~");
            _log.Info("-----========= SALT Web Application STARTING =======-----");

            AfterApplicationEnd += new EventHandler(PostApplicationStart);
            _log.Debug(logMethodName + "End Method");
        }

        private void PostApplicationStart(Object sender, EventArgs e)
        {
            String logMethodName = ".PostApplicationStart(Object sender, EventArgs e) - ";
            _log.Debug(logMethodName + "Begin Method");

            _log.Info("-----========= SALT Web Application STARTUP COMPLETED ========-----");
            _log.Debug(logMethodName + "End Method");
        }

        private void PreApplicationShutDown(Object sender, EventArgs e)
        {
            String logMethodName = ".PreApplicationShutDown(Object sender, EventArgs e) - ";
            _log.Debug(logMethodName + "Begin Method");

            _log.Info("-----========= SALT Web Application SHUTTING DOWN =========-----");

            AfterApplicationEnd += new EventHandler(PostApplicationShutDown);
            _log.Debug(logMethodName + "End Method");
        }

        private void PostApplicationShutDown(Object sender, EventArgs e)
        {
            String logMethodName = ".PostApplicationShutDown(Object sender, EventArgs e) - ";
            _log.Debug(logMethodName + "Begin Method");

            _log.Info("-----====== SALT Web Application SHUTDOWN COMPLETED ======-----");
            _log.Info("=====================================================================");
            _log.Info("====== American Student Assistance - SALT Web Application v1.0 ======");
            _log.Info("=====================================================================");

            _log.Debug(logMethodName + "End Method");
        }
        #endregion
    }
}
