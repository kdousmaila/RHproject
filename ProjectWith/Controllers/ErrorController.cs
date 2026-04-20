using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProjectWith.Controllers
{
    public class ErrorController : Controller
    {
        // GET: ErrorController
      
        public ActionResult Unauthorized()
        {
            return View("Unauthorized");
        }
    }
}