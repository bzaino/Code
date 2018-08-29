using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Helpers;

//TODO: Factor our use of ASA.Web.Services.ASAMemberService namespace at this level
using ASA.Web.Services.ASAMemberService;
using ASA.Web.Services.ASAMemberService.ServiceContracts; 

using ASA.Web.Sites.SALT.Models;

using ASA.Web.WTF;
using ASA.Web.WTF.Integration.MVC3;


namespace ASA.Web.Sites.SALT.Controllers
{
    public class HomeController : AsaController
    {
        private const string CLASSNAME = "ASA.Web.Sites.SALT.Controllers.HomeController";
        static ASA.Log.ServiceLogger.IASALog _log = ASA.Log.ServiceLogger.ASALogManager.GetLogger(CLASSNAME);

        //This is only used in 2 actions, replacing with calls in the methods responsible
        //private IAsaMemberAdapter _memberAdapter = null;

        public HomeController()
        {
            String logMethodName = ".ctor() - ";
            _log.Debug(logMethodName + "Begin Method");

            //We don't need to do this here, there are a couple actions that need the member adapter 
            //but we can load the adapter in those actions rather than loading the adapter for the
            //whole class
            //_memberAdapter = new AsaMemberAdapter();
            
            _log.Debug(logMethodName + "End Method");
        }

        public ActionResult Index()
        {
            return Redirect("~/index.html");
        }

        [DisableContentLoad]
        public ActionResult RedirectLogOn(string ReturnUrl, string domain)
        {
            String logMethodName = ".RedirectLogOn(string ReturnUrl, string domain) - ";

            // Clean Up Cookies
            if (this.ControllerContext.HttpContext.Request.Cookies.AllKeys.Contains("IndividualId"))
            {
                _log.Debug(logMethodName + "Expiring IndividualId cookie");

                HttpCookie cookie = this.ControllerContext.HttpContext.Request.Cookies["IndividualId"];
                cookie.Domain = "saltmoney.org";
                cookie.Expires = DateTime.Now.AddDays(-1);
                this.ControllerContext.HttpContext.Response.Cookies.Add(cookie);
            }
            if (this.ControllerContext.HttpContext.Request.Cookies.AllKeys.Contains("UserGuid"))
            {
                HttpCookie cookie = this.ControllerContext.HttpContext.Request.Cookies["UserGuid"];
                cookie.Expires = DateTime.Now.AddDays(-1);
                this.ControllerContext.HttpContext.Response.Cookies.Add(cookie);
            }

            if (String.Compare(this.ControllerContext.HttpContext.Request.ContentType, "application/json", StringComparison.CurrentCultureIgnoreCase) == 0)
            {
                this.ControllerContext.HttpContext.Response.StatusCode = 401;
                return Json(new { Authorization = "NotAllowed" }, JsonRequestBehavior.AllowGet);
            }
            else
                return Redirect("~/index.html?ReturnUrl=" + Server.UrlEncode(domain + ReturnUrl));
        }
    }
}
