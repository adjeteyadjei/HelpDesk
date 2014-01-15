using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using HelpDesk.Models;
using HelpDesk.Providers;
using HelpDesk.Results;
using NLog;

namespace HelpDesk.Classes.Helpers
{
    public class SecurityHelpers
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly DataHelpers _dh = new DataHelpers();
        public SecurityHelpers()
            : this(new UserManager<User>(new UserStore<User>(new DataContext())))
        {
        }

        public SecurityHelpers(UserManager<User> userManager)
        {
            UserManager = userManager;
        }

        public UserManager<User> UserManager { get; private set; }
        public async Task<User> GetUser(string userId)
        {
            var currentUser = await UserManager.FindByIdAsync(userId);

            return currentUser;
        }

        public async Task<User> GetUserByName(string name)
        {
            var currentUser = await UserManager.FindByNameAsync(name);

            return currentUser;
        }

        public User GetUserWithName(string name)
        {
            var currentUser = UserManager.FindByName(name);

            return currentUser;
        }

        public async Task<IdentityResult> ChangePassword(string userName, ChangePasswordModel model)
        {
            var theUser = await UserManager.FindByNameAsync(userName);
            var result = await UserManager.ChangePasswordAsync(theUser.Id, model.OldPassword, model.NewPassword);
            return result;
        }

        public async Task<List<string>> GetRoles(string userId)
        {
            var roles = await UserManager.GetRolesAsync(userId);

            return roles.ToList();
        }

        public async Task<bool> Login(LoginModel model)
        {
            //var user = await UserManager.FindAsync(model.UserName, model.Password);
            var userManager = new UserManager<User>(new UserStore<User>(new DataContext()));
            var user = userManager.Find(model.UserName, model.Password);

            if (user == null) return false;
            //await SignInAsync(user, true);

            var authenticationManager = HttpContext.Current.GetOwinContext().Authentication;
            authenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            var identity = await userManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            authenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = true }, identity);

            return true;
        }

        /*public bool CheckVerifiedUser(string userId)
        {
            using (var db = new DataContext())
            {
                if (db.Verifications.FirstOrDefault(p => p.Verified && p.User.Id == userId) != null)
                    return true;
            }

            return false;
        }*/

        /*public void ResendVerificationCode(MyUser user, DataContext db)
        {
            //var db = new DataContext();

            //Get the verificationCode
            var verification = db.Verifications.FirstOrDefault(p => p.User.Id == user.Id);
            //send the email
            var code = "";
            if (verification == null)
            {
                code = GenerateVerificationCode(6);

                // save the code in the database
                db.Verifications.Add(
                    new Verification
                    {
                        Code = code,
                        //User = user,
                        UserId = user.Id,
                        Verified = false
                    });
                db.SaveChanges();
            }
            else
            {
                code = verification.Code;
            }
            //send the email
            SendMail(user, code);

        }

        public bool VerifyCode(MyUser user, string code, DataContext db)
        {
            //Get the verificationCode
            var verification = db.Verifications.FirstOrDefault(p => p.User.Id == user.Id && p.Code == code);
            if (verification == null) return false;

            verification.Verified = true;
            db.SaveChanges();
            return true;

        }

        public void SendVerificationCode(MyUser user, DataContext db)
        {
            //Generate the verificationCode
            var code = GenerateVerificationCode(6);

            // save the code in the database
            db.Verifications.Add(
                new Verification
                {
                    Code = code,
                    UserId = user.Id,
                    //User = user,
                    Verified = false
                });
            db.SaveChanges();

            //send the email
            SendMail(user, code);

        }*/

        public static string GenerateVerificationCode(int length, string charset = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789")
        {
            var rand = new Random((int)DateTime.Now.Ticks);
            var charArray = charset.ToCharArray();
            var charLength = charArray.Length;
            var output = new StringBuilder(length);
            for (var i = 0; i < length; i++)
            {
                output.Append(charArray[rand.Next(charLength)]);
            }
            return output.ToString();
        }

        /* public void SendMail(MyUser user, string code)
         {
             const string verificationMessage = "Hello {0},\n Please verify your account with the code below. \nEmail: {1}\nVerification Code: {2}\n\nVisit the NMDS students portal to verify your account.";

             /*var mailMessage = new MailMessage
                                   {
                                       From = new MailAddress("TestAdd@NMDS.gov.ls"),
                                       Subject = "Verification Code",
                                       Body = String.Format(verificationMessage, user.FullName, user.UserName, code)
                                   };
                 mailMessage.To.Add(user.UserName);
            
                 var client = new SmtpClient {EnableSsl = true, UseDefaultCredentials = false, Port = 587};
                 client.Send(mailMessage);#1#

             string error;
             const string smtp = "smtp.googlemail.com";
             const int port = 587;
             const string @from = "richard@ritsoftgh.com";
             const string username = "form-sender@ritsoftgh.com";
             const string password = "rit.forms.pass";
             const string subject = "Your verification code";
             var message = String.Format(verificationMessage, user.FullName, user.UserName, code);
             var to = user.UserName;
             Utils.SendMail(smtp, port, username, password, from, to, subject, message, out error);
         }*/

        public async Task<JsonData> UserProfile(string username)
        {
            try
            {
                var user = await GetUserByName(username);
                /*var user = await _sh.GetUserByName(username);*/

                var data = new UserProfileModel
                {
                    FullName = user.FullName,
                    Phone = user.PhoneNumber,
                    DateOfBirth = user.DateOfBirth
                };

                return _dh.ReturnJsonData(data, true, "Profile Loaded");
            }
            catch (Exception e)
            {
                return _dh.ExceptionProcessor(e);
            }
        }

    }

    public static class Utils
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public static string GenerateVerificationCode(int length, string charset)
        {
            //todo: use the char set
            if (charset == null)
            {
                charset = "abcdefghijklmnopqrstuvwxyz0123456789";
            }
            var charArray = charset.ToCharArray();
            var charLength = charArray.Length;
            var rnd = new Random((int)DateTime.Now.Ticks);
            var output = new StringBuilder(length);
            for (int i = 0; i < length; i++)
            {
                output.Append(charArray[rnd.Next(charLength)]);
            }
            return output.ToString();
        }
        public static bool SendMail(string smtp, int port, string username, string password, string from, string to, string subject, string message)
        {
            string error;
            return SendMail(smtp, port, username, password, from, to, subject, message, out error);
        }

        public static bool SendMail(string smtp, int port, string username, string password,
                            string from, string to, string subject, string message, out string error)
        {
            try
            {
                var cred = new NetworkCredential(username, password);
                var mailClient = new SmtpClient(smtp, port) { EnableSsl = true, Credentials = cred };
                var m = new MailMessage(from, to, subject, message)
                {
                    IsBodyHtml = true,
                    DeliveryNotificationOptions =
                        DeliveryNotificationOptions.OnFailure | DeliveryNotificationOptions.OnSuccess |
                        DeliveryNotificationOptions.Delay
                };
                mailClient.Send(m);
                error = "";
                return true;
            }
            catch (Exception ex)
            {
                error = ex.GetBaseException().ToString();
                Logger.Error(ex);
                return false;
            }
        }
    }

}