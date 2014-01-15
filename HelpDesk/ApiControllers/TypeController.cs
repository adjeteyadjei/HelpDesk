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
    public class TypeController : ApiController
    {
        private readonly TypeRepo _repo = new TypeRepo();
        public static DataContext MyContext = new DataContext();
        public TypeController()
            : this(new UserManager<User>(new UserStore<User>(MyContext)))
        {
        }

        public TypeController(UserManager<User> userManager)
        {
            UserManager = userManager;
        }

        public UserManager<User> UserManager { get; private set; }

        public JsonData Get(Filter filters)
        {
            return _repo.GetAll(filters);
        }
        public JsonData Post(Type clientData)
        {
            var user = UserManager.FindByName(User.Identity.Name);
            return _repo.Post(clientData, user);
        }

        public JsonData Put(Type clientData)
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
