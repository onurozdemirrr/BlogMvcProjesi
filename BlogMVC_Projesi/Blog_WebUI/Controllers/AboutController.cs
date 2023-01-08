using Blog_WebUI.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Blog_WebUI.Controllers
{
    public class AboutController : Controller
    {
        [HandleException]
        // GET: About
        public ActionResult Index()
        {
            return View();
        }
    }
}