using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HelpDesk.Classes.Helpers;
using HelpDesk.Models;
using Type = HelpDesk.Models.Type;

namespace HelpDesk.Classes.Repositories
{
    public class TypeRepo
    {
        private readonly DataHelpers _dh = new DataHelpers();
        private readonly DataContext _db = new DataContext();

        public JsonData Post(Type newRecord, User user)
        {
            try
            {

                if (newRecord == null) throw new ArgumentNullException("The new" + " record is null");

                newRecord.UpdatedAt = DateTime.Now;
                newRecord.CreatedAt = DateTime.Now;
                newRecord.IsDeleted = false;
                newRecord.CreatedById = user.Id;
                newRecord.UpdatedById = user.Id;
                _db.Types.Add(newRecord);
                _db.SaveChanges();

                return _dh.ReturnJsonData(newRecord, true, "Ticket Type Saved successfully", 1);

            }
            catch (Exception e)
            {
                return _dh.ExceptionProcessor(e);
            }
        }

        public JsonData Update(Type updatedRecord, User user)
        {
            try
            {
                if (updatedRecord == null) throw new ArgumentNullException("The update" + " record is null");

                var oRecord = _db.Types.First(p => p.Id == updatedRecord.Id);
                oRecord.Name = updatedRecord.Name;
                oRecord.Description = updatedRecord.Description;
                oRecord.UpdatedAt = DateTime.Now;
                oRecord.UpdatedById = user.Id;
                //_db.SaveChanges();

                return _dh.ReturnJsonData(oRecord, true, "Ticket Type updated successfully", 1);

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
                var delRecord = _db.Types.First(p => p.Id == id);
                _db.Types.Remove(delRecord);
                //delRecord.IsDeleted = true;
                _db.SaveChanges();

                return _dh.ReturnJsonData(delRecord, true, "Ticket Type deleted successfully", 1);

            }
            catch (Exception e)
            {
                return _dh.ExceptionProcessor(e);
            }
        }

        public JsonData GetAll(Filter filter)
        {
            try
            {
                var data = _db.Types.Where(p => p.IsDeleted == false);

                //var dat = _dh.FilterData((List<Object>)data, filter);

                var total = data.Count();
                var success = false;
                var message = "";
                if (total > 0)
                {
                    success = true;
                    message = "Ticket Types loaded successfully";
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