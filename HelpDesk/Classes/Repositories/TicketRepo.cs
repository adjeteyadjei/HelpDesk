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
    public class TicketRepo
    {
        private readonly DataHelpers _dh = new DataHelpers();
        private readonly GeneralHelpers _gh = new GeneralHelpers();
        private readonly SecurityHelpers _sh = new SecurityHelpers();

        public JsonData Post(TicketModel newRecord, User user)
        {
            try
            {
                var allStatuses = _dh.GetStatuses();

                using (var scope = new TransactionScope())
                {
                    using (var db = new DataContext())
                    {
                        if (newRecord == null) throw new ArgumentNullException("The new" + " record is null");

                        var newTicket = new Ticket
                        {
                            Subject = newRecord.Subject,
                            Description = newRecord.Description,
                            Code = _gh.GenerateTicketCode(newRecord),
                            ProjectId = newRecord.ProjectId,
                            TypeId = newRecord.Type.Id,
                            StatusId = _gh.GetStatusId(allStatuses[0]),
                            PriorityId = newRecord.Priority.Id,
                            ParentTicketId = newRecord.ParentId,
                            AssignedToId = newRecord.AssignedTo.Id,
                            AssignedById = user.Id,
                            UpdatedAt = DateTime.Now,
                            CreatedAt = DateTime.Now,
                            CreatedById = user.Id,
                            UpdatedById = user.Id,
                            IsDeleted = false
                        };

                        db.Tickets.Add(newTicket);
                        db.SaveChanges();

                        CreateTicketDetail(newRecord, user, db);

                        scope.Complete();
                        return _dh.ReturnJsonData(newTicket, true, "Ticket has been added successfully", 1);
                    }
                }
            }
            catch (Exception e)
            {
                return _dh.ExceptionProcessor(e);
            }
        }

        public void CreateTicketDetail(TicketModel newRecord, User user, DataContext db)
        {
            var newTicketDetail = new TicketDetail
            {
                TicketId = newRecord.Id,
                Subject = newRecord.Subject,
                Description = newRecord.Description,
                Code = newRecord.Code,
                StatusId = newRecord.StatusId,
                PriorityId = newRecord.PriorityId,
                AssignedToId = newRecord.AssignedToId,
                UpdatedAt = DateTime.Now,
                CreatedAt = DateTime.Now,
                CreatedById = user.Id,
                UpdatedById = user.Id,
                IsDeleted = false
            };

            db.TicketDetails.Add(newTicketDetail);
        }

        public JsonData Update(TicketModel updatedRecord, User user)
        {
            try
            {
                using (var scope = new TransactionScope())
                {
                    using (var db = new DataContext())
                    {
                        if (updatedRecord == null) throw new ArgumentNullException("The update" + " record is null");

                        var oRecord = db.Tickets.First(p => p.Id == updatedRecord.Id);
                        /*//oRecord.Title = updatedRecord.Subject;
                //oRecord.Code = updatedRecord.;
                //oRecord.TypeId = updatedRecord.Type.Id;
                oRecord.StatusId = updatedRecord.StatusId;
                oRecord.PriorityId = updatedRecord.PriorityId;
                oRecord.ParentTicketId = updatedRecord.ParentTicketId;
                oRecord.AssignedToId = updatedRecord.AssignedToId;
                oRecord.AssignedById = updatedRecord.AssignedById;
                oRecord.UpdatedAt = DateTime.Now;*/
                        db.SaveChanges();
                        scope.Complete();
                        return _dh.ReturnJsonData(oRecord, true, "Ticket record has been successfully updated", 1);
                    }
                }
            }
            catch (Exception e)
            {
                return _dh.ExceptionProcessor(e);
            }
        }

        /*public JsonData Update(TicketDetail updatedRecord, User user)
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
        }*/

        public JsonData Delete(int id, User user)
        {
            try
            {
                using (var scope = new TransactionScope())
                {
                    using (var db = new DataContext())
                    {
                        var delRecord = db.Tickets.First(p => p.Id == id);
                        //db.Tickets.Remove(delRecord);
                        delRecord.IsDeleted = true;
                        delRecord.UpdatedAt = DateTime.Now;
                        db.SaveChanges();

                        //Delete ticket details
                        //todo: add a method to set ticket details isdeleted to false

                        scope.Complete();
                        return _dh.ReturnJsonData(delRecord, true, "Ticket has been deleted successfully", 1);
                    }
                }
            }
            catch (Exception e)
            {
                return _dh.ExceptionProcessor(e);
            }
        }

        /*public JsonData Delete(int id)
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
        }*/

        public JsonData GetAll(Filter filters, User user)
        {
            try
            {
                using (var db = new DataContext())
                {
                    var data = new List<TicketModel>();
                    data.Clear();
                    var tickets =
                        db.Tickets.Where(p => p.IsDeleted == false && p.ProjectId == filters.ProjectId).ToList();
                    var total = tickets.Count();
                    var message = " No ticket found";
                    if (total <= 0) return _dh.ReturnJsonData(data, false, message, total);
                    data.AddRange(tickets.Select(ticket => new TicketModel
                    {
                        Id = ticket.Id,
                        Subject = ticket.Subject,
                        Description = ticket.Description,
                        Code = ticket.Code,
                        Project = ticket.Project,
                        ProjectId = ticket.ProjectId,
                        Type = ticket.Type,
                        TypeId = ticket.TypeId,
                        Status = ticket.Status,
                        StatusId = ticket.StatusId,
                        PriorityId = ticket.PriorityId,
                        ParentTicketId = ticket.ParentTicketId,
                        AssignedTo = ticket.AssignedTo,
                        AssignedToId = ticket.AssignedToId,
                        TicketDetail = GetTicketDetails(ticket.Id, db),
                        AssignedBy = ticket.AssignedBy,
                        AssignedById = ticket.AssignedById

                    }));
                    message = "Tickets loaded successfully";
                    return _dh.ReturnJsonData(data, true, message, total);
                }
            }
            catch (Exception e)
            {
                return _dh.ExceptionProcessor(e);
            }
        }

        public TicketDetail[] GetTicketDetails(int ticketId, DataContext db)
        {
            var details = db.TicketDetails.Where(p => p.IsDeleted == false && p.TicketId == ticketId);
            return details.Select(
                detail => new TicketDetail
                {
                    Code = detail.Code,
                    Status = detail.Status,
                    Priority = detail.Priority,
                    AssignedTo = detail.AssignedTo,
                    AssignedBy = detail.AssignedBy
                }).ToArray();
        }
    }
}