using System.Web.Http;
using HelpDesk.Classes.Helpers;
using HelpDesk.Classes.Repositories;
using HelpDesk.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace HelpDesk.ApiControllers
{
    [Authorize]
    public class StatusController : ApiController
    {
        private readonly StatusRepo _repo = new StatusRepo();
        public static DataContext MyContext = new DataContext();
        public StatusController()
            : this(new UserManager<User>(new UserStore<User>(MyContext)))
        {
        }

        public StatusController(UserManager<User> userManager)
        {
            UserManager = userManager;
        }

        public UserManager<User> UserManager { get; private set; }

        public JsonData Get()
        {
            return _repo.GetAll();
        }

        public JsonData Post(Status clientData)
        {
            var user = UserManager.FindByName(User.Identity.Name);
            return _repo.Post(clientData, user);
        }

        public JsonData Put(Status clientData)
        {
            var user = UserManager.FindByName(User.Identity.Name);
            return _repo.Update(clientData, user);
        }

        [System.Web.Mvc.HttpPost]
        public JsonData DeleteStatus(int id)
        {
            return _repo.Delete(id);
        }
    }
}
