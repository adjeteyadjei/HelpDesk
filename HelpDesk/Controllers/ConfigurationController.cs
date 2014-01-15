using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HelpDesk.Controllers
{
    [Authorize]
    public class ConfigurationController : Controller
    {
        //
        // GET: /Configuration/
        [Authorize(Roles = "Administrator")]
        public ActionResult Index()
        {
            return View();
        }
    }
}
