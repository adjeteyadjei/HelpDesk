using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Mvc;
using HelpDesk.Classes.Helpers;
using HelpDesk.Classes.Repositories;
using HelpDesk.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Filter = HelpDesk.Models.Filter;

namespace HelpDesk.ApiControllers
{
    [System.Web.Http.Authorize]
    public class TicketController : ApiController
    {
        private readonly TicketRepo _repo = new TicketRepo();
        public static DataContext MyContext = new DataContext();

        public TicketController()
            : this(new UserManager<User>(new UserStore<User>(MyContext)))
        {
        }

        public TicketController(UserManager<User> userManager)
        {
            UserManager = userManager;
        }

        public UserManager<User> UserManager { get; private set; }

        public JsonData Get(int ProjectId)
        {
            var filters = new Filter
            {
                ProjectId = ProjectId
            };
            var user = UserManager.FindByName(User.Identity.Name);
            return _repo.GetAll(filters, user);
        }
        public JsonData Get()
        {
            var filters = new Filter();
            var user = UserManager.FindByName(User.Identity.Name);
            return _repo.GetAll(filters, user);
        }

        public JsonData Post(TicketModel clientData)
        {
            //clientData.ParentId = null;
            var user = UserManager.FindByName(User.Identity.Name);
            return _repo.Post(clientData, user);
        }

        public JsonData Put(TicketModel clientData)
        {
            var user = UserManager.FindByName(User.Identity.Name);
            return _repo.Update(clientData, user);
        }

        public JsonData Delete(int id)
        {
            var user = UserManager.FindByName(User.Identity.Name);
            return _repo.Delete(id, user);
        }

        [System.Web.Http.Route("api/ticket/close")]
        [System.Web.Http.HttpPut]
        [System.Web.Http.Authorize]
        public JsonData Close(TicketModel ticket)
        {
            var user = UserManager.FindByName(User.Identity.Name);
            return _repo.UpdateStatus(ticket.TicketId, user, "Close");
        }

        [System.Web.Http.Route("api/ticket/open")]
        [System.Web.Http.HttpGet]
        [System.Web.Http.Authorize]
        public JsonData Open(int ticketId)
        {
            var user = UserManager.FindByName(User.Identity.Name);
            return _repo.UpdateStatus(ticketId, user, "Open");
        }

        [System.Web.Http.Route("api/ticket/resolve")]
        [System.Web.Http.HttpPut]
        [System.Web.Http.Authorize]
        public JsonData Resolve(TicketModel ticket)
        {
            var user = UserManager.FindByName(User.Identity.Name);
            return _repo.UpdateStatus(ticket.TicketId, user, "Resolve");
        }

        [System.Web.Http.Route("api/ticket/comment")]
        [System.Web.Http.HttpPut]
        [System.Web.Http.Authorize]
        public JsonData Comment(CommentViewModel comment)
        {
            var user = UserManager.FindByName(User.Identity.Name);
            return _repo.Comment(comment, user);
        }

        [System.Web.Http.Route("api/dashboard/summaries")]
        [System.Web.Http.HttpGet]
        [System.Web.Http.Authorize]
        public JsonData Dashboard()
        {
            var dh = new DataHelpers();
            try
            {
                
                var dash = new DashboardModel();
                var user = UserManager.FindByName(User.Identity.Name);
                //var user = UserManager.FindByName("administrator");
                dash.Activities = _repo.GetTicketActivities(user);
                var stats = new TicketStats
                {
                    New = _repo.GetTickets(user, "New"),
                    Open =_repo.GetTickets(user,"Open"),
                    Solved =_repo.GetTickets(user,"Solved"),
                    Pending=_repo.GetTickets(user,"Pending")
                };
                dash.TicketStats = stats;

                //dash.TicketStats = (List<TicketModel>)_repo.GetAll(new Filter {}, user).data;

                return dh.ReturnJsonData(dash, true, "Dashboard data loaded", 1);
            }
            catch (Exception e)
            {
                return dh.ExceptionProcessor(e);
            }
        }
    }
}
