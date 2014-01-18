using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HelpDesk.Classes.Helpers;
using HelpDesk.Models;

namespace HelpDesk.Classes.Repositories
{
    public class TicketDetailRepo
    {
        private readonly DataHelpers _dh = new DataHelpers();
        private readonly DataContext _db = new DataContext();

        public TicketDetail CreateTicketDetail(Ticket newTicket, TicketModel newRecord, User user)
        {
            return new TicketDetail
            {
                TicketId = newTicket.Id,
                Description = newRecord.Description,
                StatusId = newTicket.StatusId,
                UpdatedAt = DateTime.Now,
                CreatedAt = DateTime.Now,
                CreatedById = user.Id,
                UpdatedById = user.Id,
                IsDeleted = false
            };
        }

        public JsonData Update(TicketDetail updatedRecord, User user)
        {
            try
            {
                if (updatedRecord == null) throw new ArgumentNullException("The update" + " record is null");

                var oRecord = _db.TicketDetails.First(p => p.Id == updatedRecord.Id);
                oRecord.TicketId = updatedRecord.TicketId;
                oRecord.Description = updatedRecord.Description;
                oRecord.StatusId = updatedRecord.StatusId;
                oRecord.UpdatedAt = DateTime.Now;
                oRecord.UpdatedById = user.Id;
                _db.SaveChanges();

                return _dh.ReturnJsonData(oRecord, true, "Ticket Detail record has been successfully updated", 1);

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
                var delRecord = _db.TicketDetails.First(p => p.Id == id);
                _db.TicketDetails.Remove(delRecord);
                //delRecord.IsDeleted = true;
                _db.SaveChanges();

                return _dh.ReturnJsonData(delRecord, true, "Ticket Detail has been deleted successfully", 1);

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
                var data = _db.TicketDetails.FirstOrDefault(p => p.Id == id && p.IsDeleted == false);
                var success = false;
                var message = "";
                var total = 0;
                if (data != null)
                {
                    success = true;
                    message = "Ticket Detail loaded successfully";
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
                var data = _db.TicketDetails.Where(p => p.IsDeleted == false);
                var total = data.Count();
                var success = false;
                var message = "";
                if (total > 0)
                {
                    success = true;
                    message = "Ticket Details loaded successfully";
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