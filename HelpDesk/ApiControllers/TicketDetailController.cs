using System.Web.Http;
using HelpDesk.Classes.Repositories;
using HelpDesk.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace HelpDesk.ApiControllers
{
    public class TicketDetailController : ApiController
    {
        private readonly TicketDetailRepo _repo = new TicketDetailRepo();
        public static DataContext MyContext = new DataContext();
        public TicketDetailController()
            : this(new UserManager<User>(new UserStore<User>(MyContext)))
        {
        }

        public TicketDetailController(UserManager<User> userManager)
        {
            UserManager = userManager;
        }

        public UserManager<User> UserManager { get; private set; }

        /*public JsonData Get()
        {
            return _repo.GetAll();
        }

        public JsonData Get(int id)
        {
            return _repo.Get(id);
        }*/

        /*public JsonData Post(TicketDetail clientData)
        {
            return _repo.Post(clientData);
        }

        public JsonData Put(int id, TicketDetail clientData)
        {
            return _repo.Update(clientData);
        }

        public JsonData Delete(int id)
        {
            return _repo.Delete(id);
        }*/
    }
}
