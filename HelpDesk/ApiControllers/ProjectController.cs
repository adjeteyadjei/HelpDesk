using System.Web.Http;
using HelpDesk.Classes.Repositories;
using HelpDesk.Classes.Helpers;
using HelpDesk.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Type = HelpDesk.Models.Type;

namespace HelpDesk.ApiControllers
{
    [Authorize]
    public class ProjectController : ApiController
    {
        private readonly ProjectRepo _repo = new ProjectRepo();
        public static DataContext MyContext = new DataContext();
        public ProjectController()
            : this(new UserManager<User>(new UserStore<User>(MyContext)))
        {
        }

        public ProjectController(UserManager<User> userManager)
        {
            UserManager = userManager;
        }

        public UserManager<User> UserManager { get; private set; }

        public JsonData Get(Filter filters)
        {
            var user = UserManager.FindByName(User.Identity.Name);
            return _repo.GetAll(filters, user);
        }
        public JsonData Post(ProjectModel clientData)
        {
            var user = UserManager.FindByName(User.Identity.Name);
            return _repo.Post(clientData, user);
        }

        public JsonData Put(ProjectModel clientData)
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
