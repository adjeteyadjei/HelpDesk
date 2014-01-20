using System;
using System.Collections.Generic;
using System.Web.Http;
using HelpDesk.Classes.Helpers;
using HelpDesk.Classes.Repositories;
using HelpDesk.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace HelpDesk.ApiControllers
{
    [Authorize]
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

        public JsonData Get(Filter filters)
        {
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

        [Route("api/ticket/close")]
        [HttpPut]
        [Authorize]
        public JsonData Close(TicketModel ticket)
        {
            var user = UserManager.FindByName(User.Identity.Name);
            return _repo.UpdateStatus(ticket.TicketId, user, "Close");
        }

        [Route("api/ticket/open")]
        [HttpGet]
        [Authorize]
        public JsonData Open(int ticketId)
        {
            var user = UserManager.FindByName(User.Identity.Name);
            return _repo.UpdateStatus(ticketId, user, "Open");
        }

        [Route("api/ticket/resolve")]
        [HttpPut]
        [Authorize]
        public JsonData Resolve(TicketModel ticket)
        {
            var user = UserManager.FindByName(User.Identity.Name);
            return _repo.UpdateStatus(ticket.TicketId, user, "Resolve");
        }

        [Route("api/ticket/comment")]
        [HttpPut]
        [Authorize]
        public JsonData Comment(CommentViewModel comment)
        {
            var user = UserManager.FindByName(User.Identity.Name);
            return _repo.Comment(comment, user);
        }

        [Route("api/dashboard/summaries")]
        [HttpGet]
        [Authorize]
        public JsonData Dashboard()
        {
            var dh = new DataHelpers();
            try
            {
                
                var dash = new DashboardModel();
                var user = UserManager.FindByName(User.Identity.Name);
                //var user = UserManager.FindByName("administrator");
                dash.Activities = _repo.GetTicketActivities(user);
                dash.TicketStats = (List<TicketModel>)_repo.GetAll(new Filter {}, user).data;

                return dh.ReturnJsonData(dash, true, "Dashboard data loaded", 1);
            }
            catch (Exception e)
            {
                return dh.ExceptionProcessor(e);
            }
        }
    }
}
