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
            return _repo.GetAll(filters,user);
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
    }
}
