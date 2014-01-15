using System.Web.Http;
using HelpDesk.Classes.Helpers;
using HelpDesk.Classes.Repositories;
using HelpDesk.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace HelpDesk.ApiControllers
{
    [Authorize]
    public class TeamMemberController : ApiController
    {
        private readonly TeamMemberRepo _repo = new TeamMemberRepo();
        public static DataContext MyContext = new DataContext();
        public TeamMemberController()
            : this(new UserManager<User>(new UserStore<User>(MyContext)))
        {
        }

        public TeamMemberController(UserManager<User> userManager)
        {
            UserManager = userManager;
        }

        public UserManager<User> UserManager { get; private set; }

        public JsonData Get()
        {
            return _repo.GetAll();
        }

        public JsonData Get(int id)
        {
            return _repo.Get(id);
        }

        public JsonData Post(TeamMember clientData)
        {
            var user = UserManager.FindByName(User.Identity.Name);

            return _repo.Post(clientData, user);
        }

        public JsonData Put(int id, TeamMember clientData)
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
