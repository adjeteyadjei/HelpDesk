using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HelpDesk.Classes.Helpers;
using HelpDesk.Models;
using NLog;

namespace HelpDesk.Classes.Repositories
{
    public class StatusRepo
    {
        private readonly DataHelpers _dh = new DataHelpers();
        private readonly DataContext _db = new DataContext();
        private readonly SecurityHelpers _sh = new SecurityHelpers();
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public JsonData Post(Status newRecord, User user)
        {
            try
            {
                if (newRecord == null) throw new ArgumentNullException("The new" + " record is null");

                newRecord.UpdatedAt = DateTime.Now;
                newRecord.CreatedAt = DateTime.Now;
                newRecord.IsDeleted = false;
                newRecord.UpdatedById = user.Id;
                newRecord.CreatedById = user.Id;
                _db.Statuses.Add(newRecord);
                _db.SaveChanges();

                return _dh.ReturnJsonData(newRecord, true, "New Status has been added successfully", 1);
            }
            catch (Exception e)
            {
                return _dh.ExceptionProcessor(e);
            }
        }

        public JsonData Update(Status updatedRecord, User user)
        {
            try
            {
                if (updatedRecord == null) throw new ArgumentNullException("The update" + " record is null");

                var oRecord = _db.Statuses.First(p => p.Id == updatedRecord.Id);
                oRecord.Name = updatedRecord.Name;
                oRecord.Description = updatedRecord.Description;
                oRecord.UpdatedAt = DateTime.Now;
                oRecord.UpdatedById = user.Id;
                //_db.SaveChanges();

                return _dh.ReturnJsonData(oRecord, true, "Status record has been successfully updated", 1);

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
                var delRecord = _db.Statuses.First(p => p.Id == id);
                _db.Statuses.Remove(delRecord);
                //delRecord.IsDeleted = true;
                _db.SaveChanges();

                return _dh.ReturnJsonData(delRecord, true, "Status has been deleted successfully", 1);

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
                var data = _db.Statuses.FirstOrDefault(p => p.Id == id && p.IsDeleted == false);
                var success = false;
                var message = "";
                var total = 0;
                if (data != null)
                {
                    success = true;
                    message = "Status loaded successfully";
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
                var data = _db.Statuses.Where(p => p.IsDeleted == false);
                var total = data.Count();
                var success = false;
                var message = "";
                if (total > 0)
                {
                    success = true;
                    message = "Statuses loaded successfully";
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