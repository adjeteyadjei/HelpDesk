using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;
using System.Web.Http;
using HelpDesk.Classes.Helpers;
using HelpDesk.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;

namespace HelpDesk.ApiControllers
{
    public class AccountController : ApiController
    {
        private readonly DataHelpers _dh = new DataHelpers();
        private readonly SecurityHelpers _sh = new SecurityHelpers();
        public static DataContext MyContext = new DataContext();
        public AccountController()
            : this(new UserManager<User>(new UserStore<User>(MyContext)))
        {
        }

        public AccountController(UserManager<User> userManager)
        {
            UserManager = userManager;
            UserManager.UserValidator = new UserValidator<User>(UserManager)
            {
                AllowOnlyAlphanumericUserNames =
                    false
            };
        }
        public UserManager<User> UserManager { get; private set; }

        // GET security/signin
        [Route("api/account/signin")]
        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public async Task<JsonData> Login(LoginModel model)
        {
            try
            {
                if (!ModelState.IsValid) throw new Exception("Please check the login details");
                var user = await UserManager.FindAsync(model.UserName, model.Password);

                if (user == null) throw new Exception("Please check the login details");

                var authenticationManager = HttpContext.Current.GetOwinContext().Authentication;
                authenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                var identity = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
                authenticationManager.SignIn(new AuthenticationProperties{IsPersistent = model.RememberMe}, identity);

                var ticket = new AuthenticationTicket(identity, new AuthenticationProperties());
                var token = Startup.OAuthOptions.AccessTokenFormat.Protect(ticket);

                return _dh.ReturnJsonData(user, true, token);
            }
            catch (Exception e)
            {
                return _dh.ExceptionProcessor(e);
            }
        }

        [System.Web.Http.Route("api/account/signoff")]
        [HttpGet]
        //[System.Web.Http.Authorize]
        public JsonData Logout()
        {
            var authenticationManager = HttpContext.Current.GetOwinContext().Authentication;
            authenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return _dh.ReturnJsonData(null, true, "User has been logged out", 1);
        }


        [Route("api/account/signup")]
        [HttpPost]
        [Authorize]
        //[ValidateAntiForgeryToken]
        public async Task<JsonData> Register(UserModel model)
        {
            try
            {
                //using (var scope = new TransactionScope())
                //{
                    using (var myContext = new DataContext())
                    {
                        var userManager = new UserManager<User>(new UserStore<User>(myContext))
                        {
                            UserValidator = new UserValidator<User>(UserManager)
                            {
                                AllowOnlyAlphanumericUserNames =
                                    false
                            }
                        };

                        var currentUser = userManager.FindByName(User.Identity.Name);

                        var currentUserRoles = currentUser.Roles.ToList();

                        var roleName = GetNewUsersRole(model);

                        if (!ModelState.IsValid) throw new Exception("Please check the registration details");

                        var user = new User
                        {
                            UserName = model.UserName,
                            FullName = model.FullName,
                            Email = model.Email,
                            PhoneNumber = model.PhoneNumber,
                            Picture = model.Picture,
                            DateOfBirth=DateTime.Now.AddYears(-20),
                            IsDeleted = false,
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now

                        };
                        var result = await userManager.CreateAsync(user, model.Password);
                        if (!result.Succeeded) throw new Exception(string.Join(", ", result.Errors));

                        userManager.AddToRole(user.Id, roleName);

                        /*if (model.Team == null)
                            return _dh.ReturnJsonData(user, true, "User has been added successfully", 1);*/

                        //model.UserId = user.Id;
                        //AddTeamMember(model, myContext, currentUser);

                        myContext.SaveChanges();
                        //scope.Complete();
                        return _dh.ReturnJsonData(user, true, "User has been created");
                    }
                //}
            }
            catch (Exception e)
            {
                return _dh.ExceptionProcessor(e);
            }
        }

        public string GetNewUsersRole(UserModel newRecord)
        {
            var allRoles = _dh.GetRoles();

            return newRecord.IsAdmin ? allRoles[1] : allRoles[3];
        }

        [Route("api/account/update")]
        [HttpPut]
        //[ValidateAntiForgeryToken]
        public async Task<JsonData> UpdateUser(UserModel model)
        {
            try
            {
                //using (var scope = new TransactionScope())
                //{
                using (var myContext = new DataContext())
                {
                    var userManager = new UserManager<User>(new UserStore<User>(myContext))
                    {
                        UserValidator = new UserValidator<User>(UserManager)
                        {
                            AllowOnlyAlphanumericUserNames =
                                false
                        }
                    };

                    //var currentUser = await userManager.FindByIdAsync(User.Identity.GetUserId());

                    //var currentUserRoles = currentUser.Roles.ToList();

                    var roleName = GetNewUsersRole(model);

                    if (!ModelState.IsValid) throw new Exception("Please check the registration details");

                    var us = myContext.Users.FirstOrDefault(p => p.Id == model.UserId);

                    //us.Id = model.Id;
                    //us.UserName = model.UserName;
                    us.FullName = model.FullName;
                    us.Email = model.Email;
                    us.PhoneNumber = model.PhoneNumber;
                    us.Picture = model.Picture;
                    us.DateOfBirth = model.DateOfBirth;
                    us.IsDeleted = false;
                    us.UpdatedAt = DateTime.Now;

                    myContext.SaveChanges();

                    /*var user = new User
                    {
                        Id = model.Id,
                        UserName = model.UserName,
                        FullName = model.FullName,
                        Email = model.Email,
                        PhoneNumber = model.PhoneNumber,
                        Picture = model.Picture,
                        DateOfBirth = model.DateOfBirth,
                        IsDeleted = false,
                        UpdatedAt = DateTime.Now

                    };
                    var result = await userManager.UpdateAsync(user);
                    if (!result.Succeeded) throw new Exception(string.Join(", ", result.Errors));*/

                    var roles = await userManager.GetRolesAsync(us.Id);

                    foreach (var role in roles)
                    {
                        await userManager.RemoveFromRoleAsync(us.Id, role);
                    }

                    userManager.AddToRole(us.Id, roleName);

                    myContext.SaveChanges();
                    //scope.Complete();
                    return _dh.ReturnJsonData(us, true, "User has been updated");
                }
                //}
            }
            catch (Exception e)
            {
                return _dh.ExceptionProcessor(e);
            }
        }

        [Route("api/account/delete")]
        [HttpDelete]
        public async Task<JsonData> DeleteUser(UserModel model)
        {
            try
            {
                using (var scope = new TransactionScope())
                {
                using (var myContext = new DataContext())
                {
                    var userManager = new UserManager<User>(new UserStore<User>(myContext))
                    {
                        UserValidator = new UserValidator<User>(UserManager)
                        {
                            AllowOnlyAlphanumericUserNames =
                                false
                        }
                    };

                    //var currentUser = userManager.FindByName(User.Identity.Name);

                    //var currentUserRoles = currentUser.Roles.ToList();

                    var roleName = GetNewUsersRole(model);

                    if (!ModelState.IsValid) throw new Exception("Please check the registration details");

                    var roles = await userManager.GetRolesAsync(model.UserId);

                    foreach (var role in roles)
                    {
                        await userManager.RemoveFromRoleAsync(model.UserId, role);
                    }

                    var user = new User
                    {
                        Id = model.Id,
                        UserName = model.UserName,
                        FullName = model.FullName,
                        Email = model.Email,
                        PhoneNumber = model.PhoneNumber,
                        Picture = model.Picture,
                        DateOfBirth = model.DateOfBirth,
                        IsDeleted = false,
                        UpdatedAt = DateTime.Now

                    };
                    var result = await userManager.RemovePasswordAsync(user.Id);
                    if (!result.Succeeded) throw new Exception(string.Join(", ", result.Errors));

                    var aa = myContext.Users.Remove(user);
                    myContext.SaveChanges();

                    myContext.SaveChanges();
                    scope.Complete();
                    return _dh.ReturnJsonData(user, true, "User has been updated");
                }
                }
            }
            catch (Exception e)
            {
                return _dh.ExceptionProcessor(e);
            }
        }

        /*public void AddTeamMember(UserModel newRecord, DataContext db, User user)
        {
            var teamMember = new TeamMember
            {
                TeamId = newRecord.Team.Id,
                UserId = newRecord.UserId,
                IsLead = newRecord.IsLeader,
                UpdatedAt = DateTime.Now,
                CreatedAt = DateTime.Now,
                IsDeleted = false,
                CreatedById = user.Id,
                UpdatedById = user.Id
            };
            db.TeamMembers.Add(teamMember);
        }*/

        [Route("api/account/details")]
        [HttpGet]
        public async Task<JsonData> UserProfile()
        {
            try
            {
            var username = User.Identity.Name;
            var user = await _sh.GetUserByName(username);

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
        [Route("api/account/ChangePassword")]
        public async Task<JsonData> Manage(ChangePasswordModel model)
        {
            try
            {
                var username = User.Identity.Name;
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

        [Route("api/account/users")]
        [HttpGet]
        [Authorize]
        //[ValidateAntiForgeryToken]
        public JsonData GetUsers( Filter filters)
        {
            try
            {
                var currentUser = _sh.GetUserWithName(User.Identity.Name);
                using (var db = new DataContext())
                {
                    var users = db.Users.ToList();
                    var message = "No User Found";
                    if (!users.Any()) return _dh.ReturnJsonData(null, false, message, 0);

                    //filter the users with a condition
                    users = FilterUsers(users, currentUser, db);
                    var data = new List<UserModel>();
                    data.Clear();

                    data.AddRange(users.Select(user => new UserModel
                    {
                        Team = GetUserTeam(user.Id,db),
                        Id = user.Id,
                        UserName = user.UserName,
                        FullName = user.FullName,
                        Email = user.Email,
                        PhoneNumber = user.PhoneNumber,
                        Picture = user.Picture,
                        DateOfBirth = user.DateOfBirth,
                        IsAdmin = user.Roles.Select(x=>x.Role.Name).Contains("Administrator"),
                        IsDeleted = false,
                        CreatedAt = user.CreatedAt,
                        UpdatedAt = user.UpdatedAt,
                        Roles = user.Roles.Select(x=>x.Role.Name).ToList()
                    }));
                    
                    message = "Users loaded successfully";
                    return _dh.ReturnJsonData(data, true, message, data.Count());
                }
            }
            catch (Exception e)
            {
                return _dh.ExceptionProcessor(e);
            }
        }

        [Route("api/account/users/assign")]
        [HttpGet]
        [Authorize]
        //[ValidateAntiForgeryToken]
        public JsonData GetUsersForAssignment(Filter filters)
        {
            try
            {
                var currentUser = _sh.GetUserWithName(User.Identity.Name);
                using (var db = new DataContext())
                {
                    var data = new List<UserModel>();
                    data.Clear();

                    var users = db.Users.ToList();

                    //filter the users with a condition
                    users = FilterUsersForAssignment(users, currentUser, db);

                    var total = users.Count();
                    var message = "No User Found";
                    if (total <= 0) return _dh.ReturnJsonData(null, false, message, total);

                    data.AddRange(users.Select(user => new UserModel
                    {
                        Team = GetUserTeam(user.Id, db),
                        Id = user.Id,
                        UserName = user.UserName,
                        FullName = user.FullName,
                        Email = user.Email,
                        PhoneNumber = user.PhoneNumber,
                        Picture = user.Picture,
                        DateOfBirth = user.DateOfBirth,
                        IsAdmin = user.Roles.Select(x => x.Role.Name).Contains("Administrator"),
                        IsDeleted = false,
                        CreatedAt = user.CreatedAt,
                        UpdatedAt = user.UpdatedAt,
                        Roles = user.Roles.Select(x => x.Role.Name).ToList()
                    }));

                    message = "Users loaded successfully";
                    return _dh.ReturnJsonData(data, true, message, total);
                }
            }
            catch (Exception e)
            {
                return _dh.ExceptionProcessor(e);
            }
        }

        public List<User> FilterUsers(List<User> users, User currentUser, DataContext db)
        {
            var newUsers = new List<User>();
            newUsers.Clear();
            var allRoles = _dh.GetRoles();
            var roles = currentUser.Roles.Select(x => x.Role.Name).ToList();

            foreach (var role in roles)
            {
                if (role != allRoles[0] && role != allRoles[1])
                {
                    var leader = db.TeamMembers.FirstOrDefault(x => x.UserId == currentUser.Id && !x.IsDeleted);
                    if (leader == null) return newUsers;
                    newUsers = db.TeamMembers.Where(x => x.TeamId == leader.TeamId).Select(x => x.User).ToList();
                }
                else
                {
                    newUsers = users;
                }
            }


            var firstOrDefault = db.TeamMembers.FirstOrDefault(p => p.UserId == currentUser.Id && p.IsDeleted == false);
            if (firstOrDefault == null) return newUsers;
            var team = firstOrDefault.Team;
            var data = new List<User>();
            data.Clear();
            var teamMembers = db.TeamMembers.Where(p => p.TeamId == team.Id && p.IsDeleted == false).ToList();
            data.AddRange(teamMembers.Select(teamMember => newUsers.FirstOrDefault(p => p.Id == teamMember.UserId)));
            return newUsers;
        }

        public List<User> FilterUsersForAssignment(List<User> users, User currentUser, DataContext db)
        {
            var newUsers = new List<User>();
            newUsers.Clear();
            var allRoles = _dh.GetRoles();
            var roles = currentUser.Roles.Select(x => x.Role.Name).ToList();

            foreach (var role in roles)
            {
                if (role != allRoles[0] || role != allRoles[1])
                {
                    var leader = db.TeamMembers.FirstOrDefault(x => x.UserId == currentUser.Id && !x.IsDeleted);
                    newUsers = db.TeamMembers.Where(x => x.TeamId == leader.TeamId).Select(x => x.User).ToList();
                }
                else
                {
                    newUsers = users;
                }
            }

            var firstOrDefault = db.TeamMembers.FirstOrDefault(p => p.UserId == currentUser.Id && p.IsDeleted == false);
            if (firstOrDefault == null) return newUsers;
            var team = firstOrDefault.Team;
            var data = new List<User>();
            data.Clear();
            var teamMembers = db.TeamMembers.Where(p => p.TeamId == team.Id && p.IsDeleted == false).ToList();
            data.AddRange(teamMembers.Select(teamMember => newUsers.FirstOrDefault(p => p.Id == teamMember.UserId)));
            return newUsers;
        }

        public Team GetUserTeam(string userId, DataContext db)
        {
            var tmem = db.TeamMembers.FirstOrDefault(p => p.UserId == userId && p.IsDeleted == false);

            return tmem != null ? new Team{Name = tmem.Team.Name, Description = tmem.Team.Description, Id = tmem.Team.Id} : null;
        }
    }
}
