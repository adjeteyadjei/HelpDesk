using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using HelpDesk.Classes.Helpers;
using HelpDesk.Models;
using Type = HelpDesk.Models.Type;

namespace HelpDesk.Classes.Repositories
{
    public class TicketRepo
    {
        private readonly DataHelpers _dh = new DataHelpers();
        private readonly GeneralHelpers _gh = new GeneralHelpers();

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
                            ProjectId = newRecord.Project.Id,
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

                        CreateTicketDetail(newTicket, user, db);
                        CreateTicketActivity("Created a new ticket", newTicket.Id, user.Id, db);
                        db.SaveChanges();
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

        public void CreateTicketDetail(Ticket newRecord, User user, DataContext db)
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
                AssignedById = newRecord.AssignedById,
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
                        oRecord.Subject = updatedRecord.Subject;
                        oRecord.Description = updatedRecord.Description;
                        oRecord.TypeId = updatedRecord.Type.Id;
                        oRecord.StatusId = updatedRecord.StatusId;
                        oRecord.PriorityId = updatedRecord.PriorityId;
                        oRecord.AssignedToId = updatedRecord.AssignedToId;
                        oRecord.AssignedById = updatedRecord.AssignedById;
                        oRecord.UpdatedAt = DateTime.Now;

                        CreateTicketDetail(oRecord, user, db);
                        CreateTicketActivity("Updated the ticket", oRecord.Id, user.Id, db);
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

                        CreateTicketDetail(delRecord, user, db);
                        db.SaveChanges();
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

        public JsonData GetAll(Filter filters, User user)
        {
            try
            {
                using (var db = new DataContext())
                {
                    var closedStatusId = _gh.GetStatusId(_dh.GetStatuses()[4]);
                    var ts = db.Tickets.Where(p => p.IsDeleted == false && p.StatusId != closedStatusId);
                    var message = " No ticket found";
                    if (!ts.Any()) return _dh.ReturnJsonData(null, false, message, 0);

                    var data = new List<TicketModel>();
                    data.Clear();

                    var tickets = FilterTickets(ts.ToList(), filters, user, db);
                    data.AddRange(tickets.Select(ticket => new TicketModel
                    {
                        Id = ticket.Id,
                        Subject = ticket.Subject,
                        Description = ticket.Description,
                        Code = ticket.Code,
                        Project = new Project
                        {
                            Id = ticket.Project.Id,
                            Name = ticket.Project.Name,
                            Description = ticket.Project.Description,
                            IsActive = ticket.Project.IsActive
                        },
                        //ProjectId = ticket.ProjectId,
                        Type = new Type
                        {
                            Id = ticket.Type.Id,
                            Name = ticket.Type.Name,
                            Description = ticket.Type.Description
                        },
                        //TypeId = ticket.TypeId,
                        Status = new Status
                        {
                            Id = ticket.Status.Id,
                            Name = ticket.Status.Name,
                            Description = ticket.Status.Description
                        },
                        Priority = new Priority
                        {
                            Id = ticket.Priority.Id,
                            Name = ticket.Priority.Name,
                            Description = ticket.Priority.Description
                        },
                        //StatusId = ticket.StatusId,
                        //PriorityId = ticket.PriorityId,
                        ParentTicketId = ticket.ParentTicketId,
                        AssignedTo = new User
                        {
                            Id = ticket.AssignedTo.Id,
                            UserName = ticket.AssignedTo.UserName,
                            FullName = ticket.AssignedTo.FullName,
                            Email = ticket.AssignedTo.Email,
                            PhoneNumber = ticket.AssignedTo.PhoneNumber,
                            Picture = ticket.AssignedTo.Picture,
                            DateOfBirth = ticket.AssignedTo.DateOfBirth
                        },
                        //AssignedToId = ticket.AssignedToId,
                        AssignedBy = new User
                        {
                            Id = ticket.AssignedBy.Id,
                            UserName = ticket.AssignedBy.UserName,
                            FullName = ticket.AssignedBy.FullName,
                            Email = ticket.AssignedBy.Email,
                            PhoneNumber = ticket.AssignedBy.PhoneNumber,
                            Picture = ticket.AssignedBy.Picture,
                            DateOfBirth = ticket.AssignedBy.DateOfBirth
                        },
                        CreatedAt = ticket.CreatedAt,
                        TicketDetail = GetTicketDetails(ticket.Id, db).ToList(),
                        Comments = GetTicketComments(ticket.Id, db).ToList() 
                        //AssignedById = ticket.AssignedById

                    }));
                    message = "Tickets loaded successfully";
                    return _dh.ReturnJsonData(data, true, message, data.Count());
                }
            }
            catch (Exception e)
            {
                return _dh.ExceptionProcessor(e);
            }
        }

        public TicketDetail[] GetTicketDetails(int ticketId, DataContext db)
        {
            var details = db.TicketDetails.Where(p => p.IsDeleted == false && p.TicketId == ticketId).ToList();
            return details.Select(
                detail => new TicketDetail
                {
                    Code = detail.Code,
                    Status = new Status
                    {
                        Id = detail.Status.Id,
                        Name = detail.Status.Name,
                        Description = detail.Status.Description
                    },
                    Priority = new Priority
                    {
                        Id = detail.Priority.Id,
                        Name = detail.Priority.Name,
                        Description = detail.Priority.Description
                    },
                    AssignedTo = new User
                    {
                        Id = detail.AssignedTo.Id,
                        UserName = detail.AssignedTo.UserName,
                        FullName = detail.AssignedTo.FullName,
                        Email = detail.AssignedTo.Email,
                        PhoneNumber = detail.AssignedTo.PhoneNumber,
                        Picture = detail.AssignedTo.Picture,
                        DateOfBirth = detail.AssignedTo.DateOfBirth
                    },
                    AssignedBy = new User
                    {
                        Id = detail.AssignedBy.Id,
                        UserName = detail.AssignedBy.UserName,
                        FullName = detail.AssignedBy.FullName,
                        Email = detail.AssignedBy.Email,
                        PhoneNumber = detail.AssignedBy.PhoneNumber,
                        Picture = detail.AssignedBy.Picture,
                        DateOfBirth = detail.AssignedBy.DateOfBirth
                    } /**/
                }).ToArray();

        }

        public CommentViewModel[] GetTicketComments(int ticketId, DataContext db)
        {
            var data = new List<CommentViewModel>();
            data.Clear();
            var comments = db.TicketComments.Where(p => p.IsDeleted == false && p.TicketId == ticketId).ToList();
            if (!comments.Any()) return data.ToArray();
            return comments.Select(
                comment => new CommentViewModel
                {
                    Comment = comment.Comment,
                    CreatedAt = comment.CreatedAt,
                    CreatedBy = new User
                    {
                        Id = comment.CreatedBy.Id,
                        UserName = comment.CreatedBy.UserName,
                        FullName = comment.CreatedBy.FullName,
                        Email = comment.CreatedBy.Email,
                        PhoneNumber = comment.CreatedBy.PhoneNumber,
                        Picture = comment.CreatedBy.Picture,
                        DateOfBirth = comment.CreatedBy.DateOfBirth
                    }
                }).ToArray();
        }

        public JsonData UpdateStatus(int ticketId, User user, string theStatus)
        {
            try
            {
                using (var db = new DataContext())
                {
                    var statusId = 0;
                    var action = "";
                    switch (theStatus)
                    {
                        case "Open":
                            statusId = _gh.GetStatusId(_dh.GetStatuses()[1]);
                            CheckAssignedTo(ticketId, user, db, "Open");
                            action = "Opened the ticket";
                            break;
                        case "Resolve":
                            statusId = _gh.GetStatusId(_dh.GetStatuses()[3]);
                            CheckAssignedTo(ticketId, user, db, "Resolve");
                            action = "Resolved the ticket";
                            break;
                        case "Close":
                            statusId = _gh.GetStatusId(_dh.GetStatuses()[4]);
                            CheckTicketOwner(ticketId, user, db);
                            action = "Closed the ticket";
                            break;
                        case "Pending":
                            statusId = _gh.GetStatusId(_dh.GetStatuses()[2]);
                            break;
                        default:
                            throw new Exception("Status is not defined");
                    }
                    var ticket = db.Tickets.FirstOrDefault(p => p.Id == ticketId);
                    if (ticket == null) throw new Exception("No Ticket Found");

                    ticket.UpdatedAt = DateTime.Now;
                    ticket.StatusId = statusId;
                    ticket.UpdatedById = user.Id;

                    CreateTicketDetail(ticket, user, db);
                    CreateTicketActivity(action, ticket.Id, user.Id, db);

                    db.SaveChanges();

                    var tick =
                        new TicketModel
                        {
                            Id = ticket.Id,
                            Subject = ticket.Subject,
                            Description = ticket.Description,
                            Code = ticket.Code,
                            Project = new Project
                            {
                                Id = ticket.Project.Id,
                                Name = ticket.Project.Name,
                                Description = ticket.Project.Description,
                                IsActive = ticket.Project.IsActive
                            },
                            //ProjectId = ticket.ProjectId,
                            Type = new Type
                            {
                                Id = ticket.Type.Id,
                                Name = ticket.Type.Name,
                                Description = ticket.Type.Description
                            },
                            //TypeId = ticket.TypeId,
                            Status = new Status
                            {
                                Id = ticket.Status.Id,
                                Name = ticket.Status.Name,
                                Description = ticket.Status.Description
                            },
                            Priority = new Priority
                            {
                                Id = ticket.Priority.Id,
                                Name = ticket.Priority.Name,
                                Description = ticket.Priority.Description
                            },
                            //StatusId = ticket.StatusId,
                            //PriorityId = ticket.PriorityId,
                            ParentTicketId = ticket.ParentTicketId,
                            AssignedTo = new User
                            {
                                Id = ticket.AssignedTo.Id,
                                UserName = ticket.AssignedTo.UserName,
                                FullName = ticket.AssignedTo.FullName,
                                Email = ticket.AssignedTo.Email,
                                PhoneNumber = ticket.AssignedTo.PhoneNumber,
                                Picture = ticket.AssignedTo.Picture,
                                DateOfBirth = ticket.AssignedTo.DateOfBirth
                            },
                            //AssignedToId = ticket.AssignedToId,
                            AssignedBy = new User
                            {
                                Id = ticket.AssignedBy.Id,
                                UserName = ticket.AssignedBy.UserName,
                                FullName = ticket.AssignedBy.FullName,
                                Email = ticket.AssignedBy.Email,
                                PhoneNumber = ticket.AssignedBy.PhoneNumber,
                                Picture = ticket.AssignedBy.Picture,
                                DateOfBirth = ticket.AssignedBy.DateOfBirth
                            },
                            CreatedAt = ticket.CreatedAt,
                            TicketDetail = GetTicketDetails(ticket.Id, db).ToList(),
                            Comments = GetTicketComments(ticket.Id, db).ToList()
                            //AssignedById = ticket.AssignedById

                        };
                    return _dh.ReturnJsonData(tick, true, "Ticket has been Updated", 1);
                }
            }
            catch (Exception e)
            {
                return _dh.ExceptionProcessor(e);
            }
        }

        public void CheckTicketOwner(int ticketId, User user, DataContext db)
        {
            var ticket = db.Tickets.FirstOrDefault(p => p.Id == ticketId);
            if (ticket != null && ticket.CreatedById != user.Id)
                throw new Exception("You cannot close the ticket because you did not create it");
        }

        public void CheckAssignedTo(int ticketId, User user, DataContext db, string msg)
        {
            var ticket = db.Tickets.FirstOrDefault(p => p.Id == ticketId);
            if (ticket != null && ticket.AssignedToId != user.Id)
                throw new Exception("This ticket is not assigned to you. <br/> You cannot " + msg + " it.");
        }

        public JsonData Comment(CommentViewModel newRecord, User user)
        {
            try
            {
                using (var db = new DataContext())
                {
                    if (newRecord == null) throw new ArgumentNullException("The new" + " record is null");

                    var newcomment = new TicketComment
                    {
                        TicketId = newRecord.TicketId,
                        Comment = newRecord.Comment,
                        UpdatedAt = DateTime.Now,
                        CreatedAt = DateTime.Now,
                        CreatedById = user.Id,
                        UpdatedById = user.Id,
                        IsDeleted = false
                    };

                    db.TicketComments.Add(newcomment);
                    db.SaveChanges();

                    var ticket = (List<TicketModel>)GetAll(new Filter {TicketId = newRecord.TicketId}, user).data;


                    return _dh.ReturnJsonData(ticket[0], true, "Comment Noted", 1);
                }
            }
            catch (Exception e)
            {
                return _dh.ExceptionProcessor(e);
            }
        }

        public List<Ticket> FilterTickets(List<Ticket> tickets, Filter filters, User user, DataContext db)
        {
            var firstOrDefault = db.TeamMembers.FirstOrDefault(p => p.UserId == user.Id && p.IsDeleted == false);
            if (firstOrDefault != null)
            {
                var team = firstOrDefault.Team;
                var teamProjects = db.ProjectTeams.Where(p => p.TeamId == team.Id && p.IsDeleted == false).ToList();
                tickets = teamProjects.Aggregate(tickets,
                    (current, teamProject) => current.Where(p => p.ProjectId == teamProject.ProjectId).ToList());
            }

            if (filters != null && filters.ProjectId != 0)
            {
                tickets = tickets.Where(p => p.ProjectId == filters.ProjectId).ToList();
            }

            if (filters != null && filters.TicketId != 0)
            {
                tickets = tickets.Where(p => p.Id == filters.TicketId).ToList();
            }


            return tickets;
        }

        public void CreateTicketActivity(string action, int ticketId, string userId, DataContext db)
        {
            var ticketActivity = new TicketActivity
            {
                TicketId = ticketId,
                AgentId = userId,
                Action = action,
                CreatedAt = DateTime.Now
            };

            db.TicketActivities.Add(ticketActivity);
            db.SaveChanges();
        }

        public List<TicketActivity> GetTicketActivities(User user)
        {
            using (var db = new DataContext())
            {
                var acts = new List<TicketActivity>();
                acts.Clear();
                var firstOrDefault = db.TeamMembers.FirstOrDefault(p => p.UserId == user.Id && p.IsDeleted == false);
                if (firstOrDefault == null)
                {
                    if (db.TicketActivities != null)
                    {
                        var list = db.TicketActivities.ToList();
                        list.AddRange(acts.Select(
                            act=> new TicketActivity
                            {
                               Id = act.Id,
                               Agent = new User
                               {
                                   Id = act.Agent.Id,
                                   UserName = act.Agent.UserName,
                                   FullName = act.Agent.FullName,
                                   Email = act.Agent.Email,
                                   PhoneNumber = act.Agent.PhoneNumber,
                                   Picture = act.Agent.Picture,
                                   DateOfBirth = act.Agent.DateOfBirth
                               },
                               Action=act.Action,
                               CreatedAt = act.CreatedAt,
                               Ticket = new Ticket
                               {
                                   Code = act.Ticket.Code,
                                   Subject = act.Ticket.Subject,
                                   Description=act.Ticket.Description
                               }
                            }));
                        return list;
                    }
                    return acts;
                }
                    

                var team = firstOrDefault.Team;
                var data = new List<Ticket>();
                data.Clear();
                var teamProjects = db.ProjectTeams.Where(p => p.TeamId == team.Id && p.IsDeleted == false).ToList();
                foreach (var teamProject in teamProjects)
                {
                    data.AddRange(db.Tickets.Where(p => p.ProjectId == teamProject.ProjectId).ToList());
                }

                foreach (var ticket in data)
                {
                    acts.AddRange(db.TicketActivities.Where(p => p.TicketId == ticket.Id).ToList().Select(
                            act=> new TicketActivity
                            {
                               Id = act.Id,
                               Agent = new User
                               {
                                   Id = act.Agent.Id,
                                   UserName = act.Agent.UserName,
                                   FullName = act.Agent.FullName,
                                   Email = act.Agent.Email,
                                   PhoneNumber = act.Agent.PhoneNumber,
                                   Picture = act.Agent.Picture,
                                   DateOfBirth = act.Agent.DateOfBirth
                               },
                               Action=act.Action,
                               CreatedAt = act.CreatedAt,
                               Ticket = new Ticket
                               {
                                   Code = act.Ticket.Code,
                                   Subject = act.Ticket.Subject,
                                   Description=act.Ticket.Description
                               }
                            }));
                }
                return acts;
            }
        }
    }
}