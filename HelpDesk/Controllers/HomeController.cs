using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using HelpDesk.ApiControllers;
using HelpDesk.Classes.Helpers;

namespace HelpDesk.Controllers
{
    public class HomeController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";

            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Login(Models.LoginModel model)
        {
            using (var ac = new AccountController())
            {
                var res = await ac.Login(model);

                if (res.success)
                {
                    FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);

                    var cookie = new HttpCookie("accessToken", res.message);
                    Response.Cookies.Add(cookie);
                    Response.AddHeader("accessToken", res.message);
                    return RedirectToAction("Index");
                }

                ModelState.AddModelError("", res.message);
                return View();
            }
        }

        public ActionResult LogOut()
        {
            var res = new AccountController().Logout();
            FormsAuthentication.SignOut();
            return RedirectToAction("Index");
        }
    }
}
