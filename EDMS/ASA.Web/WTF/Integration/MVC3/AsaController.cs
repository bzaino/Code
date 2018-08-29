using System.Web.Mvc;
using System;
using System.Text;
using System.Web.Security;
using System.Web;

namespace ASA.Web.WTF.Integration.MVC3
{
    public abstract class AsaController : Controller, IContextWrapper
    {
        private const string CLASSNAME = "ASA.Web.WTF.Integration.MVC3.AsaController";
        static ASA.Log.ServiceLogger.IASALog _log = ASA.Log.ServiceLogger.ASALogManager.GetLogger(CLASSNAME);

        private SiteMember _siteMember;

        private AsaMvcApplication _application;
        public AsaController()
        {
            String logMethodName = ".ctor() - ";
            _log.Debug(logMethodName + "Begin Method");
            

            _log.Debug(logMethodName + "End Method");
        }

        #region Helper Methods

        /// <summary>
        /// Reimplementing until there is more time to optimize as an attribute. The requirement is to not allow
        /// overlays to be retrieved directly from outside site, since master page formatting (css) and functionality (js) will not 
        /// be pressent. This satisfies the requirement. 
        /// </summary>
        /// <returns></returns>
        protected virtual bool IsValidReferrer()
        {
            //TODO - JHL: Add as controller action attribute
            if (Request.IsAjaxRequest())
            {
                return true;
            }
            return Request.UrlReferrer.Host == Request.Url.Host;
        }

        protected virtual void EmptyCache()
        {
            String logMethodName = ".EmptyCache() - ";
            _log.Debug(logMethodName + " - Begin Method");
            HttpContext.Cache.Remove(User.Identity.Name);
            _log.Debug(logMethodName + " - End Method");

        }
        #endregion

        #region IContextWrapper Members
        
        /// <summary>
        /// Current Site Member Context
        /// </summary>
        public SiteMember SiteMember
        {
            get 
            {
                String logMethodName = "GET.SiteMember - ";
                _log.Debug(logMethodName + " - Begin Method");
                if (_siteMember == null)
                {
                    _siteMember = (SiteMember)HttpContext.Items["SiteMember"];
                }
                _log.Debug(logMethodName + " - End Method");

                return _siteMember;
            }
            set {
                String logMethodName = "SET.SiteMember - ";
                _log.Debug(logMethodName + " - Begin Method");

                _siteMember = value;

                HttpContext.Items["SiteMember"] = _siteMember; //TODO: Analysis of the saftey of allowing this to be set. May create security holes if API consumers are sloppy
                _log.Debug(logMethodName + " - End Method");

            }

        }
        #endregion

        #region Controller Event Overrides
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            String logMethodName = ".OnActionExecuting(ActionExecutingContext filterContext) - ";
            _log.Debug(logMethodName + " - Begin Method - Action Name: " + RouteData.Values["action"] + " Controller: " + RouteData.Values["controller"]);
            _application = HttpContext.ApplicationInstance as AsaMvcApplication;
            base.OnActionExecuting(filterContext);

            _log.Debug(logMethodName + " - End Method");
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            String logMethodName = ".OnActionExecuted(ActionExecutedContext filterContext) - ";
            _log.Debug(logMethodName + " - Begin Method");
            base.OnActionExecuted(filterContext);

            //We need to send sitemember back to HTTPContext so that View and Widget pages have the latest version. 
            //Controllers are the only classes capable of writing back to SiteMember
            //JHL - checking for null here. This was causing _Layout.cshtml page to fail
            //TODO: BUG FIX - Request Context Loading of site member
            if (_siteMember != null)
            {
                HttpContext.Items["SiteMember"] = _siteMember;    
            }
            
            _log.Debug(logMethodName + " - End Method");
        } 
        #endregion

        #region Exception Handling
        protected override void OnException(ExceptionContext filterContext)
        {
            _log.Info("AsaController.OnException START");
            base.OnException(filterContext);

            WriteErrorToLog(filterContext);

            // Output a nice error page 
            if (filterContext != null && filterContext.HttpContext != null && filterContext.HttpContext.IsCustomErrorEnabled)
            {
                filterContext.ExceptionHandled = true;
                HandleErrorInfo errorInfo = new HandleErrorInfo(
                    GetException(filterContext),
                    GetErrorRouteValue(filterContext, "controller"),
                    GetErrorRouteValue(filterContext, "action"));

                ////this.View("Error", errorInfo).ExecuteResult(this.ControllerContext);
            }
            _log.Info("AsaController.OnException END");



        }

        private void WriteErrorToLog(ExceptionContext filterContext)
        {
            String logMethodName = ".WriteErrorToLog(ExceptionContext filterContext)- ";
            _log.Debug(logMethodName + " - Begin Method");

            StringBuilder sb = new StringBuilder();

            //COV 10383
            sb.Append(" UserName = " + (SiteMember != null ? SiteMember.Account.Username : "Not Authenticated"));
            sb.Append(" Controller = " + GetErrorRouteValue(filterContext, "controller"));
            sb.Append(" Action = " + GetErrorRouteValue(filterContext, "action"));
            sb.Append(" Url = " + GetErrorURL(filterContext));
            sb.Append(" Exception = " + GetException(filterContext).ToString());
            
            _log.Error(sb.ToString());
            _log.Debug(logMethodName + " - End Method");


        }
        #endregion

        #region Helper methods
        private Exception GetException(ExceptionContext filterContext)
        {
            String logMethodName = ".GetException(ExceptionContext filterContext) - ";
            _log.Debug(logMethodName + " - Begin Method");
            if (filterContext != null && filterContext.Exception != null)
            {
                return filterContext.Exception;
            }

            _log.Debug(logMethodName + " - End Method");

            return new Exception("Exception not found in ExceptionContext.");
        }

        private string GetErrorRouteValue(ExceptionContext filterContext, string routeValueName)
        {
            String logMethodName = ".GetErrorRouteValue(ExceptionContext filterContext, string routeValueName) - ";
            _log.Debug(logMethodName + " - Begin Method");
            if (filterContext != null && filterContext.RouteData != null && filterContext.RouteData.Values[routeValueName] != null)
            {
                return filterContext.RouteData.Values[routeValueName].ToString();
            }

            _log.Debug(logMethodName + " - End Method");

            return "'" + routeValueName + "' was not found in RouteData";
        }

        private string GetErrorURL(ExceptionContext filterContext)
        {
            String logMethodName = ".GetErrorURL(ExceptionContext filterContext) - ";
            _log.Debug(logMethodName + " - Begin Method");
            string errorUrl = "";

            if (filterContext != null && filterContext.RequestContext != null &&
                filterContext.RequestContext.HttpContext != null && 
                filterContext.RequestContext.HttpContext.Request != null && 
                filterContext.RequestContext.HttpContext.Request.Url != null
                )
            {
                errorUrl = filterContext.RequestContext.HttpContext.Request.Url.ToString();
            }

            _log.Debug(logMethodName + " - End Method");

            return errorUrl;
        }
        #endregion
    }
}
