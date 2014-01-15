using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HelpDesk.Classes.Helpers;
using HelpDesk.Models;
using NLog;

namespace HelpDesk.Classes.Repositories
{
    public class TeamMemberRepo
    {
        private readonly DataHelpers _dh = new DataHelpers();
        private readonly DataContext _db = new DataContext();
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public JsonData Post(TeamMember newRecord, User user)
        {
            try
            {
                if (newRecord == null) throw new ArgumentNullException("The new" + " record is null");

                newRecord.UpdatedAt = DateTime.Now;
                newRecord.CreatedAt = DateTime.Now;
                newRecord.IsDeleted = false;
                newRecord.CreatedById = user.Id;
                newRecord.UpdatedById = user.Id;
                _db.TeamMembers.Add(newRecord);
                _db.SaveChanges();

                return _dh.ReturnJsonData(newRecord, true, "Team Member has been added successfully", 1);
            }
            catch (Exception e)
            {
                return _dh.ExceptionProcessor(e);
            }
        }

        public JsonData Update(TeamMember updatedRecord, User user)
        {
            try
            {
                if (updatedRecord == null) throw new ArgumentNullException("The update" + " record is null");

                var oRecord = _db.TeamMembers.First(p => p.Id == updatedRecord.Id);
                oRecord.TeamId = updatedRecord.TeamId;
                //oRecord.Team = updatedRecord.Team;
                oRecord.UserId = updatedRecord.UserId;
                //oRecord.User = updatedRecord.User;
                oRecord.IsLead = updatedRecord.IsLead;
                oRecord.UpdatedAt = DateTime.Now;
                oRecord.CreatedById = user.Id;
                oRecord.UpdatedById = user.Id;
                _db.SaveChanges();

                return _dh.ReturnJsonData(oRecord, true, "Team Member record has been successfully updated", 1);

            }
            catch (Exception e)
            {
                return _dh.ExceptionProcessor(e);
            }
        }

        public JsonData Delete(int id)
        {
            try
            {
                var delRecord = _db.TeamMembers.First(p => p.Id == id);
                _db.TeamMembers.Remove(delRecord);
                //delRecord.IsDeleted = true;
                _db.SaveChanges();

                return _dh.ReturnJsonData(delRecord, true, "TeamMember has been deleted successfully", 1);

            }
            catch (Exception e)
            {
                return _dh.ExceptionProcessor(e);
            }
        }

        public JsonData Get(int id)
        {
            try
            {
                var data = _db.TeamMembers.FirstOrDefault(p => p.Id == id && p.IsDeleted == false);
                var success = false;
                var message = "";
                var total = 0;
                if (data != null)
                {
                    success = true;
                    message = "TeamMember loaded successfully";
                    total = 1;
                }
                return _dh.ReturnJsonData(data, success, message, total);

            }
            catch (Exception e)
            {
                return _dh.ExceptionProcessor(e);
            }
        }

        public JsonData GetAll()
        {
            try
            {
                var data = _db.TeamMembers.Where(p => p.IsDeleted == false);
                var total = data.Count();
                var success = false;
                var message = "";
                if (total > 0)
                {
                    success = true;
                    message = "TeamMembers loaded successfully";
                }
                return _dh.ReturnJsonData(data, success, message, total);

            }
            catch (Exception e)
            {
                return _dh.ExceptionProcessor(e);
            }
        }
    }
}