using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HelpDesk.Controllers
{
    [Authorize]
    public class AdministrationController : Controller
    {
        //
        // GET: /Administration/
        [Authorize(Roles = "Administrator")]
        public ActionResult Team()
        {
            return View();
        }
        [Authorize(Roles = "Administrator, TeamLead")]
        public ActionResult Users()
        {
            return View();
        }

    }
}
