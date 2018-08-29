using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

using ASA.Web.Services.Common;
using System.Configuration;

namespace ASA.Web.WTF.Integration.MVC3
{
    public static class ASAContextLoader
    {
        private const string CLASSNAME = "ASA.Web.WTF.Integration.MVC3.ASAContextLoader";
        static ASA.Log.ServiceLogger.IASALog _log = ASA.Log.ServiceLogger.ASALogManager.GetLogger(CLASSNAME);

        private static ASAIntegration _configuration;

        static ASAContextLoader() 
        {
            String logMethodName = ".ctor() - ";
            _log.Debug(logMethodName + "Begin Method");

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

            _log.Debug(logMethodName + "End Method");       
        }

        internal static void RegisterApplication(AsaMvcApplication application)
        {

            String logMethodName = ".RegisterApplication(AsaMvcApplication application) - ";
            _log.Debug(logMethodName + "Begin Method");

            //Wire into key application events to control preloading/testing/diagnostics/cleanup/etc
            application.AfterApplicationStart += (o, e) => { _log.Info(logMethodName + "====== ASA Web Application Component Startup ======"); };
            application.AfterApplicationEnd += (o, e) => { _log.Info(logMethodName + "====== ASA Web Application Component Shut Down ======"); };

            application.AfterSessionStart += (o, e) => { _log.Info(logMethodName + "======= Starting Session " + HttpContext.Current.Session.SessionID.ToString() + " ========"); };
            application.AfterSessionEnd += (o, e) => { _log.Info(logMethodName + "======= Ending Session ========"); };

            //Order here represents the oreder in which these events fire during request processing
            application.AfterRequestStart += new EventHandler(ASAContextLoader.HttpRequestStart);
            application.AfterApplicationRequestStart += new EventHandler(ASAContextLoader.ApplicationRequestStart);
            application.AfterApplicationRequestEnd += new EventHandler(ASAContextLoader.ApplicationRequestComplete);
            application.AfterRequestEnd += new EventHandler(ASAContextLoader.HttpRequestComplete);

            //application.BeginRequest += new EventHandler(ASAContextLoader.HttpRequestStart);
            //application.MapRequestHandler += new EventHandler(ASAContextLoader.ApplicationRequestStart);
            //application.PostRequestHandlerExecute += new EventHandler(ASAContextLoader.ApplicationRequestComplete);
            //application.EndRequest += new EventHandler(ASAContextLoader.HttpRequestComplete);

            _log.Debug(logMethodName + "End Method");
        }

        public static void HttpRequestStart(Object sender, EventArgs e)
        {
            String logMethodName = ".HttpRequestStart(Object sender, EventArgs e) - ";
            _log.Debug(logMethodName + "Begin Method");

            _log.Info(logMethodName + "======= Starting HttpRequest " 
                                    + HttpContext.Current.Request.HttpMethod + ":" 
                                    + HttpContext.Current.Request.Url.AbsoluteUri
                                    //+ HttpContext.Current.Request.UrlReferrer != null && HttpContext.Current.Request.UrlReferrer.AbsoluteUri != null ? " - Reffered From: " + HttpContext.Current.Request.UrlReferrer.AbsoluteUri : String.Empty 
                                    + " ========");

            //SWD-9176 - For some reason, users are reporting issues where they are unable to click the login/sign up links. 
            //Found out this is because cookies are getting in a weird state, with an individual ID but no MemberId or ASPXAuth cookie.
            //In this case, we should remove the Individual ID cookie
            var individualIdCookie = HttpContext.Current.Request.Cookies.Get("IndividualId");
            var memberIdCookie = HttpContext.Current.Request.Cookies.Get("MemberId");
            var aspxAuthCookie = HttpContext.Current.Request.Cookies.Get(".ASPXAUTH");

            if (individualIdCookie != null && memberIdCookie == null && aspxAuthCookie == null)
            {
                _log.Debug(logMethodName + "Deleting orphaned individual ID");

                HttpCookie myCookie = new HttpCookie("IndividualId");
                myCookie.Expires = DateTime.Now.AddDays(-1);
                HttpContext.Current.Response.Cookies.Add(myCookie);
            }

            _log.Debug(logMethodName + "End Method");

        }

        public static void HttpRequestComplete(Object sender, EventArgs e)
        {
            String logMethodName = ".HttpRequestComplete(Object sender, EventArgs e) - ";
            _log.Debug(logMethodName + "Begin Method");

            _log.Info(logMethodName + "======= HttpRequest Complete "
                                    + HttpContext.Current.Request.HttpMethod + ":"
                                    + HttpContext.Current.Request.Url.AbsoluteUri
                                    //+ HttpContext.Current.Request.UrlReferrer.AbsoluteUri != null && HttpContext.Current.Request.UrlReferrer.AbsoluteUri != null ? " - Reffered From: " + HttpContext.Current.Request.UrlReferrer.AbsoluteUri : String.Empty
                                    + " ========");

            _log.Debug(logMethodName + "End Method");
        }            

        /// <summary>
        /// Method wired from Global.asax.cs to represent the first call into the ASA Application stack. Any calls before this event may have unexpected behaviors.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void ApplicationRequestStart(Object sender, EventArgs e)
        {

            if (HttpContext.Current != null
                && HttpContext.Current.Handler != null
                && HttpContext.Current.Handler.GetType() == typeof(MvcHandler)) //Order of operation on this if statement is very specific. Using && ensures if eval will abort before doing anything that causes an exception
            {
                //We do logging a bit different here to keep the debug logs from having a bunch of irrelvant information about requests that were skipped by the main application. 
                //This is due to the fact that the event hook we use is fired by ALL requests coming to IIS but we only care about working with the ones for MVC.
                //This method is our PRE-Load area for any data or processes we want to handle at the beginning of every request headed for the MVC application,
                //TODO: This should likely fire its own event that allows other developers to extend from rather than extending directly into a loader critical for core-site components. 
                String logMethodName = ".ApplicationRequestStart(Object sender, EventArgs e) - ";
                _log.Debug(logMethodName + "Begin Method");
                _log.Info(logMethodName + "======= Starting ApplicationRequest " + HttpContext.Current.Request.RawUrl + " ========");

                Guid requestId = Guid.NewGuid();
                if (HttpContext.Current.Items["SiteMember"] == null)
                {
                    _log.Debug(logMethodName + "No SiteMember in context, loading");
                    SiteMember member = null;

                    member = IntegrationLoader.LoadDependency<ISiteMembership>("siteMembership").GetMember();

                    HttpContext.Current.Items["SiteMember"] = member;

                }

                _log.Debug(logMethodName + "End Method");
            } 
        }

        public static void ApplicationRequestComplete(Object sender, EventArgs e)
        {
            if (HttpContext.Current != null
                && HttpContext.Current.Handler != null
                && HttpContext.Current.Handler.GetType() == typeof(MvcHandler)) //Order of operation on this if statement is very specific. Using && ensures if eval will abort before doing anything that causes an exception
            {
                String logMethodName = ".ApplicationRequestComplete(Object sender, EventArgs e) - ";
                _log.Debug(logMethodName + "Begin Method");

                _log.Info(logMethodName + "======= ApplicationRequest Complete " + HttpContext.Current.Request.RawUrl + " ========");

                _log.Debug(logMethodName + "End Method");
            }
        }
    }
}
