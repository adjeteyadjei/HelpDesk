using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using System.Web.Mvc;
using HelpDesk.Classes.Helpers;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using HelpDesk.Models;
using HelpDesk.Providers;
using HelpDesk.Results;
using NLog;

namespace HelpDesk.Controllers
{
    [System.Web.Http.Authorize]
    [ValidateAntiForgeryToken]
    [System.Web.Http.RoutePrefix("api/Account")]
    public class AccountController : ApiController
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly DataHelpers _dh = new DataHelpers();

        private const string LocalLoginProvider = "Local";

        public AccountController()
            : this(Startup.UserManagerFactory(), Startup.OAuthOptions.AccessTokenFormat)
        {
        }

        public AccountController(UserManager<IdentityUser> userManager,
            ISecureDataFormat<AuthenticationTicket> accessTokenFormat)
        {
            UserManager = userManager;
            AccessTokenFormat = accessTokenFormat;
        }

        public UserManager<IdentityUser> UserManager { get; private set; }
        public ISecureDataFormat<AuthenticationTicket> AccessTokenFormat { get; private set; }

        // POST api/Account/Register
        [System.Web.Http.AllowAnonymous]
        [System.Web.Http.Route("Register")]
        public async Task<JsonData> Register(UserModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    throw new Exception(BadRequest(ModelState).ToString());
                }

                var roleId = GetRoleId(model);

                var manager = new UserManager<User>(new UserStore<User>(new DataContext()));

                var user = new User()
                {
                    UserName = model.Username,
                    //FirstName = model.FirstName,
                    //OtherNames = model.OtherNames,
                    FullName = model.FullName,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    Picture = model.Picture,
                    IsDeleted = false,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    CreatedById = User.Identity.GetUserId(),
                    UpdatedById = User.Identity.GetUserId()
                };
                var result = manager.Create(user, model.Password);
                if (!result.Succeeded) throw new Exception(result.Errors.ToString());

                if (model.TeamMain == null)
                    return _dh.ReturnJsonData(user, true, "User has been registered successfully", 1);

                model.UserId = user.Id;
                AddTeamMember(model);

                return _dh.ReturnJsonData(user, true, "User has been registered successfully", 1);
            }
            catch (Exception e)
            {

                Logger.Log(LogLevel.Error, e.ToString);
                return _dh.ReturnJsonData(null, false, e.ToString());
            }
        }

        public void AddTeamMember(UserModel newRecord)
        {
            using (var db = new DataContext())
            {
                if (newRecord.TeamMain == null)
                    return;

                var teamMember = new TeamMember
                {
                    TeamId = newRecord.TeamMain.TeamId,
                    UserId = newRecord.UserId,
                    IsLead = newRecord.IsLeader,
                    UpdatedAt = DateTime.Now,
                    CreatedAt = DateTime.Now,
                    IsDeleted = false,
                    CreatedById = User.Identity.GetUserId(),
                    UpdatedById = User.Identity.GetUserId()
                };
                db.TeamMembers.Add(teamMember);
                db.SaveChanges();
            }
        }

        public string GetRoleId(UserModel newRecord)
        {
            using (var db = new DataContext())
            {
                var roleId = 2.ToString();

                if (newRecord != null && newRecord.TeamMain != null)
                    roleId = 4.ToString();
                

                if (newRecord != null && (newRecord.TeamMain != null && newRecord.IsLeader))
                    roleId = 3.ToString();

                return roleId;
            }
        }


        // POST api/Account/Login
        [System.Web.Http.AllowAnonymous]
        [System.Web.Http.Route("Login")]
        public async Task<JsonData> Login(LoginDetails model)
        {
            try
            {
                
                if (!ModelState.IsValid)
                {
                    throw new Exception(BadRequest(ModelState).ToString());
                }

                Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);

                var userStore = new UserStore<User>();
                var userManager = new UserManager<User>(userStore);
                var user = userManager.FindAsync(model.UserName, model.Password);

                //var result = await UserManager.AddLoginAsync(user.Id, new UserLoginInfo(LocalLoginProvider, user.UserName));

                //if (!result.Succeeded) throw new Exception(result.Errors.ToString());

                return _dh.ReturnJsonData(user, true, "User has been logged in successfully", 1);
                
            }
            catch (Exception e)
            {
                Logger.Log(LogLevel.Error, e.ToString);
                return _dh.ReturnJsonData(null, false, e.ToString());
            }
        }

        // POST api/Account/Logout
        [System.Web.Http.Route("Logout")]
        public JsonData Logout()
        {
            Authentication.SignOut(CookieAuthenticationDefaults.AuthenticationType);
            return _dh.ReturnJsonData(null, true, "User has been logged out", 1);
        }

        // POST api/Account/ChangePassword
        [System.Web.Http.Route("ChangePassword")]
        public async Task<JsonData> ChangePassword(ChangePasswordModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    throw new Exception(BadRequest(ModelState).ToString());
                }

                var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword,
                    model.NewPassword);
                var errorResult = GetErrorResult(result);

                var data = errorResult ?? Ok();
                return data != Ok()
                    ? _dh.ReturnJsonData(data, false, "Error changing password", 0)
                    : _dh.ReturnJsonData(data, true, "Password has been changed successfully", 1);
            }
            catch (Exception e)
            {
                Logger.Log(LogLevel.Error, e.ToString);
                return _dh.ReturnJsonData(null, false, e.ToString());
            } 
        }

        /*// POST api/Account/SetPassword
        [System.Web.Http.Route("SetPassword")]
        public async Task<JsonData> SetPassword(SetPasswordBindingModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    throw new Exception(BadRequest(ModelState).ToString());
                }

                var result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
                var errorResult = GetErrorResult(result);

                var data = errorResult ?? Ok();
                return data != Ok()
                    ? _dh.ReturnJsonData(data, false, "Error setting password", 0)
                    : _dh.ReturnJsonData(data, true, "Password has been setted successfully", 1);
            }
            catch (Exception e)
            {
                Logger.Log(LogLevel.Error, e.ToString);
                return _dh.ReturnJsonData(null, false, e.ToString());
            }  
        }*/

        /*// GET api/Account/UserInfo
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [System.Web.Http.Route("UserInfo")]
        public UserInfoViewModel GetUserInfo()
        {
            var externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

            return new UserInfoViewModel
            {
                UserName = User.Identity.GetUserName(),
                HasRegistered = externalLogin == null,
                LoginProvider = externalLogin != null ? externalLogin.LoginProvider : null
            };
        }*/

        /*// GET api/Account/ManageInfo?returnUrl=%2F&generateState=true
        [System.Web.Http.Route("ManageInfo")]
        public async Task<ManageInfoViewModel> GetManageInfo(string returnUrl, bool generateState = false)
        {
            IdentityUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

            if (user == null)
            {
                return null;
            }

            var logins = new List<UserLoginInfoViewModel>();

            foreach (IdentityUserLogin linkedAccount in user.Logins)
            {
                logins.Add(new UserLoginInfoViewModel
                {
                    LoginProvider = linkedAccount.LoginProvider,
                    ProviderKey = linkedAccount.ProviderKey
                });
            }

            if (user.PasswordHash != null)
            {
                logins.Add(new UserLoginInfoViewModel
                {
                    LoginProvider = LocalLoginProvider,
                    ProviderKey = user.UserName,
                });
            }

            return new ManageInfoViewModel
            {
                LocalLoginProvider = LocalLoginProvider,
                UserName = user.UserName,
                Logins = logins,
                ExternalLoginProviders = GetExternalLogins(returnUrl, generateState)
            };
        }*/

        /*// POST api/Account/AddExternalLogin
        [System.Web.Http.Route("AddExternalLogin")]
        public async Task<IHttpActionResult> AddExternalLogin(AddExternalLoginBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);

            AuthenticationTicket ticket = AccessTokenFormat.Unprotect(model.ExternalAccessToken);

            if (ticket == null || ticket.Identity == null || (ticket.Properties != null
                && ticket.Properties.ExpiresUtc.HasValue
                && ticket.Properties.ExpiresUtc.Value < DateTimeOffset.UtcNow))
            {
                return BadRequest("External login failure.");
            }

            ExternalLoginData externalData = ExternalLoginData.FromIdentity(ticket.Identity);

            if (externalData == null)
            {
                return BadRequest("The external login is already associated with an account.");
            }

            IdentityResult result = await UserManager.AddLoginAsync(User.Identity.GetUserId(),
                new UserLoginInfo(externalData.LoginProvider, externalData.ProviderKey));

            IHttpActionResult errorResult = GetErrorResult(result);

            if (errorResult != null)
            {
                return errorResult;
            }

            return Ok();
        }*/

        /*// POST api/Account/RemoveLogin
        [System.Web.Http.Route("RemoveLogin")]
        public async Task<IHttpActionResult> RemoveLogin(RemoveLoginBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result;

            if (model.LoginProvider == LocalLoginProvider)
            {
                result = await UserManager.RemovePasswordAsync(User.Identity.GetUserId());
            }
            else
            {
                result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(),
                    new UserLoginInfo(model.LoginProvider, model.ProviderKey));
            }

            IHttpActionResult errorResult = GetErrorResult(result);

            if (errorResult != null)
            {
                return errorResult;
            }

            return Ok();
        }*/

        /*// GET api/Account/ExternalLogin
        [System.Web.Http.OverrideAuthentication]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalCookie)]
        [System.Web.Http.AllowAnonymous]
        [System.Web.Http.Route("ExternalLogin", Name = "ExternalLogin")]
        public async Task<IHttpActionResult> GetExternalLogin(string provider, string error = null)
        {
            if (error != null)
            {
                return Redirect(Url.Content("~/") + "#error=" + Uri.EscapeDataString(error));
            }

            if (!User.Identity.IsAuthenticated)
            {
                return new ChallengeResult(provider, this);
            }

            ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

            if (externalLogin == null)
            {
                return InternalServerError();
            }

            if (externalLogin.LoginProvider != provider)
            {
                Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);
                return new ChallengeResult(provider, this);
            }

            IdentityUser user = await UserManager.FindAsync(new UserLoginInfo(externalLogin.LoginProvider,
                externalLogin.ProviderKey));

            bool hasRegistered = user != null;

            if (hasRegistered)
            {
                Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);
                ClaimsIdentity oAuthIdentity = await UserManager.CreateIdentityAsync(user,
                    OAuthDefaults.AuthenticationType);
                ClaimsIdentity cookieIdentity = await UserManager.CreateIdentityAsync(user,
                    CookieAuthenticationDefaults.AuthenticationType);
                AuthenticationProperties properties = ApplicationOAuthProvider.CreateProperties(user.UserName);
                Authentication.SignIn(properties, oAuthIdentity, cookieIdentity);
            }
            else
            {
                IEnumerable<Claim> claims = externalLogin.GetClaims();
                ClaimsIdentity identity = new ClaimsIdentity(claims, OAuthDefaults.AuthenticationType);
                Authentication.SignIn(identity);
            }

            return Ok();
        }*/

        /*// GET api/Account/ExternalLogins?returnUrl=%2F&generateState=true
        [System.Web.Http.AllowAnonymous]
        [System.Web.Http.Route("ExternalLogins")]
        public IEnumerable<ExternalLoginViewModel> GetExternalLogins(string returnUrl, bool generateState = false)
        {
            var descriptions = Authentication.GetExternalAuthenticationTypes();
            var logins = new List<ExternalLoginViewModel>();

            string state;

            if (generateState)
            {
                const int strengthInBits = 256;
                state = RandomOAuthStateGenerator.Generate(strengthInBits);
            }
            else
            {
                state = null;
            }

            foreach (var description in descriptions)
            {
                var login = new ExternalLoginViewModel
                {
                    Name = description.Caption,
                    Url = Url.Route("ExternalLogin", new
                    {
                        provider = description.AuthenticationType,
                        response_type = "token",
                        client_id = Startup.PublicClientId,
                        redirect_uri = new Uri(Request.RequestUri, returnUrl).AbsoluteUri,
                        state = state
                    }),
                    State = state
                };
                logins.Add(login);
            }

            return logins;
        }*/

        
        /*// POST api/Account/RegisterExternal
        [System.Web.Http.OverrideAuthentication]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [System.Web.Http.Route("RegisterExternal")]
        public async Task<IHttpActionResult> RegisterExternal(RegisterExternalBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

            if (externalLogin == null)
            {
                return InternalServerError();
            }

            IdentityUser user = new IdentityUser
            {
                UserName = model.UserName
            };
            user.Logins.Add(new IdentityUserLogin
            {
                LoginProvider = externalLogin.LoginProvider,
                ProviderKey = externalLogin.ProviderKey
            });
            IdentityResult result = await UserManager.CreateAsync(user);
            IHttpActionResult errorResult = GetErrorResult(result);

            if (errorResult != null)
            {
                return errorResult;
            }

            return Ok();
        }*/

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                UserManager.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Helpers

        private IAuthenticationManager Authentication
        {
            get { return Request.GetOwinContext().Authentication; }
        }

        private IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }

                if (ModelState.IsValid)
                {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }

        private class ExternalLoginData
        {
            public string LoginProvider { get; set; }
            public string ProviderKey { get; set; }
            public string UserName { get; set; }

            public IList<Claim> GetClaims()
            {
                IList<Claim> claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.NameIdentifier, ProviderKey, null, LoginProvider));

                if (UserName != null)
                {
                    claims.Add(new Claim(ClaimTypes.Name, UserName, null, LoginProvider));
                }

                return claims;
            }

            public static ExternalLoginData FromIdentity(ClaimsIdentity identity)
            {
                if (identity == null)
                {
                    return null;
                }

                Claim providerKeyClaim = identity.FindFirst(ClaimTypes.NameIdentifier);

                if (providerKeyClaim == null || String.IsNullOrEmpty(providerKeyClaim.Issuer)
                    || String.IsNullOrEmpty(providerKeyClaim.Value))
                {
                    return null;
                }

                if (providerKeyClaim.Issuer == ClaimsIdentity.DefaultIssuer)
                {
                    return null;
                }

                return new ExternalLoginData
                {
                    LoginProvider = providerKeyClaim.Issuer,
                    ProviderKey = providerKeyClaim.Value,
                    UserName = identity.FindFirstValue(ClaimTypes.Name)
                };
            }
        }

        private static class RandomOAuthStateGenerator
        {
            private static RandomNumberGenerator _random = new RNGCryptoServiceProvider();

            public static string Generate(int strengthInBits)
            {
                const int bitsPerByte = 8;

                if (strengthInBits % bitsPerByte != 0)
                {
                    throw new ArgumentException("strengthInBits must be evenly divisible by 8.", "strengthInBits");
                }

                int strengthInBytes = strengthInBits / bitsPerByte;

                byte[] data = new byte[strengthInBytes];
                _random.GetBytes(data);
                return HttpServerUtility.UrlTokenEncode(data);
            }
        }

        #endregion
    }
}
