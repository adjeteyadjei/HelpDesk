using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using HelpDesk.Classes.Helpers;
using HelpDesk.Models;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;

namespace HelpDesk.Controllers
{
    public class SecurityController : Controller
    {
        private readonly DataHelpers _dh = new DataHelpers();
        private readonly SecurityHelpers _sh = new SecurityHelpers();

        public static DataContext MyContext = new DataContext();
        public SecurityController()
            : this(new UserManager<User>(new UserStore<User>(MyContext)))
        {
        }

        public SecurityController(UserManager<User> userManager)
        {
            UserManager = userManager;
        }

        public UserManager<User> UserManager { get; private set; }
        //
        // GET: /Security/
        [AllowAnonymous]
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<JsonData> Login(LoginModel model)
        {
            try
            {
                if (!ModelState.IsValid) throw new Exception("Please check the login details");
                var user = await UserManager.FindAsync(model.UserName, model.Password);

                if (user == null) throw new Exception("Please check the login details");

                var authenticationManager = System.Web.HttpContext.Current.GetOwinContext().Authentication;
                authenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
                var identity = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
                authenticationManager.SignIn(new AuthenticationProperties { IsPersistent = model.RememberMe }, identity);

                return _dh.ReturnJsonData(user, true, "Login was successful");
            }
            catch (Exception e)
            {
                return _dh.ExceptionProcessor(e);
            }
        }

        [HttpGet]
        [Authorize]
        public JsonData Logout()
        {
            var authenticationManager = System.Web.HttpContext.Current.GetOwinContext().Authentication;
            authenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return _dh.ReturnJsonData(null, true, "User has been logged out", 1);
        }

        [HttpGet]
        [Authorize]
        public JsonData Details()
        {
            try
            {
                var user = _sh.GetUserWithName(User.Identity.Name);

                var data = new UserProfileModel
                {
                    FullName = user.FullName,
                    Phone = user.PhoneNumber,
                    DateOfBirth = user.DateOfBirth,
                    Email = user.Email,
                    UserName = user.UserName

                };

                return _dh.ReturnJsonData(data, true, "Profile Loaded");
            }
            catch (Exception e)
            {
                return _dh.ExceptionProcessor(e);
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<JsonData> Manage(ChangePasswordModel model)
        {
            try
            {
                var username = User.Identity.Name;
                //var user = await _sh.GetUserByName(username);
                var result = await _sh.ChangePassword(username, model);

                if (!result.Succeeded)
                {
                    throw new Exception(string.Join(", ", result.Errors));
                }
                return _dh.ReturnJsonData(null, true, "Password Changed Successfully");
            }
            catch (Exception e)
            {
                return _dh.ExceptionProcessor(e);
            }
        }

        [Authorize]
        [HttpGet]
        public string Check()
        {
            return User.Identity.Name;
            //return User.Identity.IsAuthenticated.ToString();
        }

    }
}