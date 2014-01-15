using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HelpDesk.Classes.Helpers;
using HelpDesk.Models;
using NLog;

namespace HelpDesk.Classes.Repositories
{
    public class PriorityRepo
    {
        private readonly DataHelpers _dh = new DataHelpers();
        private readonly DataContext _db = new DataContext();
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public JsonData Post(Priority newRecord, User user)
        {
            try
            {
                if (newRecord == null) throw new ArgumentNullException("The new" + " record is null");

                newRecord.UpdatedAt = DateTime.Now;
                newRecord.CreatedAt = DateTime.Now;
                newRecord.IsDeleted = false;
                newRecord.CreatedById = user.Id;
                newRecord.UpdatedById = user.Id;
                _db.Priorities.Add(newRecord);
                _db.SaveChanges();

                return _dh.ReturnJsonData(newRecord, true, "Priority has been added successfully", 1);
            }
            catch (Exception e)
            {
                return _dh.ExceptionProcessor(e);
            }
        }

        public JsonData Update(Priority updatedRecord, User user)
        {
            try
            {
                if (updatedRecord == null) throw new ArgumentNullException("The update" + " record is null");

                var oRecord = _db.Priorities.First(p => p.Id == updatedRecord.Id);
                oRecord.Name = updatedRecord.Name;
                oRecord.Description = updatedRecord.Description;
                oRecord.UpdatedAt = DateTime.Now;
                oRecord.UpdatedById = user.Id;
                //_db.SaveChanges();

                return _dh.ReturnJsonData(oRecord, true, "Priority has been successfully updated", 1);

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
                var delRecord = _db.Priorities.First(p => p.Id == id);
                _db.Priorities.Remove(delRecord);
                //delRecord.IsDeleted = true;
                _db.SaveChanges();

                return _dh.ReturnJsonData(delRecord, true, "Priority has been deleted successfully", 1);

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
                var data = _db.Priorities.FirstOrDefault(p => p.Id == id && p.IsDeleted == false);
                var success = false;
                var message = "";
                var total = 0;
                if (data == null) return _dh.ReturnJsonData(data, success, message, total);
                success = true;
                message = "Priority loaded successfully";
                total = 1;
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
                var data = _db.Priorities.Where(p => p.IsDeleted == false);
                var total = data.Count();
                var message = "No Priority was found";
                if (total <= 0) return _dh.ReturnJsonData(data, false, message, total);
                
                message = "Priorities loaded successfully";
                return _dh.ReturnJsonData(data, true, message, total);

            }
            catch (Exception e)
            {
                return _dh.ExceptionProcessor(e);
            }
        }
    }
}