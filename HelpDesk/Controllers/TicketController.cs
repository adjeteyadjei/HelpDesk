using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HelpDesk.Controllers
{
    [Authorize]
    public class TicketController : Controller
    {
        //
        // GET: /Ticket/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult TicketThread(string id)
        {
            ViewBag.id = id;
            return View();
        }

    }
}
