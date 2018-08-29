using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using ASA.Web.Sites.SALT.Models;
using ASA.Web.Logging.configuration;
using ASA.Web.WTF.Integration.MVC3;
using ASA.Web.Services.ASAMemberService.ServiceContracts;
using ASA.Web.Logging;
using System.Web.Helpers;
using System.Text.RegularExpressions;
using ASA.Web.Services.ASAMemberService;

namespace ASA.Web.Sites.SALT.Controllers
{
    public class LoansController : AsaController
    {
        public ActionResult Index()
        {

            if (Request.UrlReferrer != null)
            {


                string path = Request.UrlReferrer.ToString();

                if (!string.IsNullOrEmpty(path))
                {
                    string fromRegister = (from t in path.Split('/')
                                           where t == "Register"
                                           select t).FirstOrDefault();

                    if (!string.IsNullOrEmpty(fromRegister))
                    {
                        ViewBag.NewUser = true;
                    }

                }
            }
            
            return View();
        }

        [OutputCache(CacheProfile = "DoNotCache")]
        public ActionResult Loan()
        {
            return View("Overlay/Loan");
        }

        // GET
        public ActionResult Welcome()
        {
            return View("Overlay/Welcome");
        }

        //[DisableContentLoad]
        public ActionResult WhatToDo(int mode)
        {
            ViewData["Mode"] = mode;

            return View("Overlay/WhatToDo");
        }

        public ActionResult NoLoans()
        {
            return View();
        }
        public ActionResult LoanServicers()
        {
            return View();
        }
        public ActionResult RepaymentOptions()
        {
            return View();
        }

        [DisableContentLoad]
        public ActionResult NoLoanSSN()
        {
            return View("Overlay/NoLoanSSN");
        }

    }
}
