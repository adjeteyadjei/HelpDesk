using System.Web.Http;
using HelpDesk.Classes.Helpers;
using HelpDesk.Classes.Repositories;
using HelpDesk.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace HelpDesk.ApiControllers
{
    [Authorize]
    public class PriorityController : ApiController
    {
        private readonly PriorityRepo _repo = new PriorityRepo();
        public static DataContext MyContext = new DataContext();
        public PriorityController()
            : this(new UserManager<User>(new UserStore<User>(MyContext)))
        {
        }

        public PriorityController(UserManager<User> userManager)
        {
            UserManager = userManager;
        }

        public UserManager<User> UserManager { get; private set; }

        public JsonData Get()
        {
            return _repo.GetAll();
        }

        public string Get(int id)
        {
            return "value";
        }

        public JsonData Post(Priority clientData)
        {
            var user = UserManager.FindByName(User.Identity.Name);
            return _repo.Post(clientData, user);
        }

        public JsonData Put(Priority clientData)
        {
            var user = UserManager.FindByName(User.Identity.Name);
            return _repo.Update(clientData, user);
        }

        public JsonData Delete(int id)
        {
            return _repo.Delete(id);
        }
    }
}
