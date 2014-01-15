using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using HelpDesk.Classes.Helpers;
using HelpDesk.Models;
using NLog;

namespace HelpDesk.Classes.Repositories
{
    public class UserRepo
    {
        private readonly DataHelpers _dh = new DataHelpers();
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly DataContext _db = new DataContext();
        private readonly SecurityHelpers _sh = new SecurityHelpers();

        public JsonData Post(UserModel newRecord)
        {
            try
            {
                using (var scope = new TransactionScope())
                {
                    using (var db = new DataContext())
                    {
                        if (newRecord == null) throw new ArgumentNullException("The new" + " record is null");

                        var username = db.Users.FirstOrDefault(x => x.UserName == newRecord.Username);
                        if (username != null)
                        {
                            throw new Exception("This username has already been taken");
                        }

                        var roleId = 1.ToString();
                        
                        var newUser = new User
                                          {
                                              UserName = newRecord.Username,
                                              //Password = _sh.HashPassword(newRecord.Password),
                                              FullName = newRecord.FullName,
                                              Email = newRecord.Email,
                                              PhoneNumber = newRecord.PhoneNumber,
                                              Picture = newRecord.Picture,
                                              UpdatedAt = DateTime.Now,
                                              CreatedAt = DateTime.Now,
                                              IsDeleted = false,
                                              CreatedById = _sh.GetLoggedInUser().Id,
                                              UpdatedById = _sh.GetLoggedInUser().Id,
                                          };

                        
                        db.Users.Add(newUser);
                        db.SaveChanges();
                        newRecord.UserId = newUser.Id;

                        PostTeamMember(newRecord, db);

                        scope.Complete();
                        return _dh.ReturnJsonData(newRecord, true, "User has been added successfully", 1);
                    }
                }

            }
            catch (Exception e)
            {
                Logger.Log(LogLevel.Error, e.ToString);
                return _dh.ReturnJsonData(null, false, e.Message);
            }
        }

        public JsonData UpdateProfile(User updatedRecord)
        {
            try
            {
                using (var db = new DataContext())
                {
                    if (updatedRecord == null) throw new ArgumentNullException("The update" + " record is null");

                    var oRecord = db.Users.First(p => p.Id == updatedRecord.Id);
                    //oRecord.FirstName = updatedRecord.FirstName;
                    //oRecord.LastName = updatedRecord.LastName;
                    //oRecord.OtherNames = updatedRecord.OtherNames;
                    oRecord.FullName = updatedRecord.FullName;
                        //updatedRecord.FirstName + " " + updatedRecord.OtherNames + " " + updatedRecord.LastName;
                    oRecord.Email = updatedRecord.Email;
                    oRecord.PhoneNumber = updatedRecord.PhoneNumber;
                    oRecord.UpdatedAt = DateTime.Now;
                    oRecord.UpdatedById = _sh.GetLoggedInUser().Id;
                    //db.SaveChanges();

                    return _dh.ReturnJsonData(oRecord, true, "User record has been successfully updated", 1);
                }
            }
            catch (Exception e)
            {
                Logger.Log(LogLevel.Error, e.ToString);
                return _dh.ReturnJsonData(null, false, e.ToString());
            }
        }

        public JsonData UpdatePicture(User updatedRecord)
        {
            try
            {
                using (var db = new DataContext())
                {
                    if (updatedRecord == null) throw new ArgumentNullException("The update" + " record is null");

                    var oRecord = db.Users.First(p => p.Id == updatedRecord.Id);
                    oRecord.Picture = updatedRecord.Picture;
                    oRecord.UpdatedAt = DateTime.Now;
                    oRecord.UpdatedById = _sh.GetLoggedInUser().Id;
                    db.SaveChanges();

                    return _dh.ReturnJsonData(oRecord, true, "User record has been successfully updated", 1);
                }
            }
            catch (Exception e)
            {
                Logger.Log(LogLevel.Error, e.ToString);
                return _dh.ReturnJsonData(null, false, e.ToString());
            }
        }

        public JsonData UpdateLoginDetails(LoginDetails updatedRecord)
        {
            try
            {
                using (var db = new DataContext())
                {
                    if (updatedRecord == null) throw new ArgumentNullException("The update" + " record is null");

                    var oRecord = db.Users.FirstOrDefault(p => p.Id.Equals(updatedRecord.UserId));
                    if (oRecord != null)
                    {
                        oRecord.UserName = updatedRecord.UserName;
                        //oRecord.Password = _sh.HashPassword(updatedRecord.Password);
                        oRecord.UpdatedAt = DateTime.Now;
                        oRecord.UpdatedById = _sh.GetLoggedInUser().Id;
                    }
                    db.SaveChanges();

                    return _dh.ReturnJsonData(oRecord, true, "User record has been successfully updated", 1);
                }
            }
            catch (Exception e)
            {
                Logger.Log(LogLevel.Error, e.ToString);
                return _dh.ReturnJsonData(null, false, e.ToString());
            }
        }

        public JsonData Delete(string id)
        {
            try
            {
                using (var db = new DataContext())
                {
                    var delRecord = db.Users.FirstOrDefault(p => p.Id.Equals(id));
                    //_db.Users.Remove(delRecord);
                    if (delRecord != null)
                    {
                        delRecord.IsDeleted = true;
                        db.SaveChanges();

                        return _dh.ReturnJsonData(delRecord, true, "User has been deleted successfully", 1);
                    }
                    throw new Exception("User was not found in the system");
                }
            }
            catch (Exception e)
            {
                Logger.Log(LogLevel.Error, e.ToString);
                var message = e.Message;
                return _dh.ReturnJsonData(null, false, message);
            }
        }

        public JsonData Get(string id)
        {
            try
            {
                var data = _db.Users.FirstOrDefault(p => p.Id.Equals(id) && p.IsDeleted == false);
                var success = false;
                var message = "";
                var total = 0;
                if (data != null)
                {
                    success = true;
                    message = "User loaded successfully";
                    total = 1;
                }
                return _dh.ReturnJsonData(data, success, message, total);

            }
            catch (Exception e)
            {
                Logger.Log(LogLevel.Error, e.ToString);
                return _dh.ReturnJsonData(null, false, e.ToString());
            }
        }

        public JsonData Login(LoginDetails credentials)
        {
            try
            {
                using (var db = new DataContext())
                {
                    var data =
                        db.Users.FirstOrDefault(
                            p => p.UserName == credentials.UserName);
                    // && p.Password == _sh.HashPassword(credentials.Password));
                    var success = false;
                    var message = "Please check the login credentials";
                    var total = 0;
                    if (data != null)
                    {
                        success = true;
                        message = "User logged in successfully successfully";
                        total = 1;
                    }
                    return _dh.ReturnJsonData(data, success, message, total);
                }
            }
            catch (Exception e)
            {
                Logger.Log(LogLevel.Error, e.ToString);
                return _dh.ReturnJsonData(null, false, e.ToString());
            }
        }

        public JsonData GetAll()
        {
            try
            {

                var data = _db.Users.Where(p => p.IsDeleted == false);
                var total = data.Count();
                var success = false;
                var message = "";
                if (total > 0)
                {
                    success = true;
                    message = "Users loaded successfully";
                }
                return _dh.ReturnJsonData(data, success, message, total);

            }
            catch (Exception e)
            {
                Logger.Log(LogLevel.Error, e.ToString);
                return _dh.ReturnJsonData(null, false, e.ToString());
            }
        }

        public void PostTeamMember(UserModel newRecord, DataContext db)
        {
            if (newRecord.TeamMain == null )
                return;

            var teamMember = new TeamMember
                                 {
                                     TeamId = newRecord.TeamMain.TeamId,
                                     UserId = newRecord.UserId,
                                     IsLead = newRecord.IsLeader,
                                     UpdatedAt = DateTime.Now,
                                     CreatedAt = DateTime.Now,
                                     IsDeleted = false,
                                     CreatedById = _sh.GetLoggedInUser().Id,
                                     UpdatedById = _sh.GetLoggedInUser().Id
                                 };
            db.TeamMembers.Add(teamMember);
            db.SaveChanges();
        }

        public void UpdateTeamMember(UserModel updatedRecord, DataContext db)
        {
            var recs = db.TeamMembers.Where(x => x.UserId.Equals(updatedRecord.UserId) && x.IsDeleted == false);
            var cnt = recs.Count();
            if (cnt != 0)
            {
                foreach (var rec in recs.Select(member => db.TeamMembers.FirstOrDefault(x => x.Id == member.Id)).Where(rec => rec != null))
                {
                    rec.IsDeleted = true;
                }
            }

            if (updatedRecord.TeamMain == null)
                return;
            
            var teamMember = new TeamMember
            {
                TeamId = updatedRecord.TeamMain.TeamId,
                UserId = updatedRecord.UserId,
                IsLead = updatedRecord.IsLeader,
                UpdatedAt = DateTime.Now,
                CreatedAt = DateTime.Now,
                IsDeleted = false,
                CreatedById = _sh.GetLoggedInUser().Id,
                UpdatedById = _sh.GetLoggedInUser().Id
            };
            db.TeamMembers.Add(teamMember);
        }
    }
}