using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace hello_world_aspnet_framework_4_6_1.Controllers
{
    public class HomeController : Controller
    {
        int timeToLive = 60;
        int timeToUnhealthy = 30;
        DateTime timeStarted = DateTime.Now;

        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View();
        }

        public ActionResult Alive()
        {
            return new HttpStatusCodeResult((isAlive() ? 200 : 500));
        }

        public ActionResult Healthy()
        {
            return new HttpStatusCodeResult((isHealthy() ? 200 : 500));
        }

        private bool isAlive()
        {
            if (timeStarted.Second+timeToLive < DateTime.Now.Second)
            {
                return false;
            }

            return true;
        }        

        private bool isHealthy()
        {
            if (timeStarted.Second+timeToUnhealthy < DateTime.Now.Second)
            {
                return false;
            }

            return true;
        }
    }
}
