using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.Mvc.Async;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Diagnostics;



using System.Collections.Specialized;
using System.Configuration;

using System.IO;
using ADTool.Models;
using ADTool.Controllers;
using System.Text;




namespace ADTool.Controllers
{
    public class HomeController : AsyncController
    {

        //ToDo: Cahnge below to make the Webservices Tool Work
        //Roles = "amsa\\.Everyone"
        //Roles = "amsa\\.BSSServicesScrumTeam"
        [Authorize(Roles = "amsa\\.Everyone")]
        public ActionResult Index()
        {

            return View("About");
        }

        public ActionResult About()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(LogOnModel model)
        {
            //ToDo: Cahnge below to make the Webservices Tool Work
            return View("../Ad/Validate", model);
        }

    }
}