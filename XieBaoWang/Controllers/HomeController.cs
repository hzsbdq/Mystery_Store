using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XieBaoWang.Controllers
{
    public class HomeController : Controller
    {
        //接收User参数，和数据库校对，存入Session
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About(string name,int id)
        {
            ViewBag.Message = name+":"+id;

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}