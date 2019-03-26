using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SpongeBob_Mall.Controllers
{
    public class HomeController : Controller
    {

        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }
    }
}