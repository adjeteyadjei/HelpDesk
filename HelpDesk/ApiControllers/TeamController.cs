using System.Web.Http;
using HelpDesk.Classes.Helpers;
using HelpDesk.Classes.Repositories;
using HelpDesk.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace HelpDesk.ApiControllers
{
    [Authorize]
    public class TeamController : ApiController
    {
        private readonly TeamRepo _repo = new TeamRepo();
        public static DataContext MyContext = new DataContext();
        public TeamController()
            : this(new UserManager<User>(new UserStore<User>(MyContext)))
        {
        }

        public TeamController(UserManager<User> userManager)
        {
            UserManager = userManager;
        }

        public UserManager<User> UserManager { get; private set; }

        
        public JsonData Get()
        {
            return _repo.GetAll();
        }

        public JsonData Post(TeamMain clientData)
        {
            var name = User.Identity.Name;
            var user = UserManager.FindByName(name);
            return _repo.Post(clientData, user);
        }

       public JsonData Put(TeamMain clientData)
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
