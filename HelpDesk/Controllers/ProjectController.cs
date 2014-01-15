using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HelpDesk.Controllers
{
    [Authorize]
    public class ProjectController : Controller
    {
        //
        // GET: /Project/
        [Authorize(Roles = "Administrator")]
        public ActionResult Index()
        {
            return View();
        }
	}
}