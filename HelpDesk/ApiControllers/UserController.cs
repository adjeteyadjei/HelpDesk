using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using HelpDesk.Classes.Helpers;
using HelpDesk.Classes.Repositories;
using HelpDesk.Models;

namespace HelpDesk.ApiControllers
{
    [RoutePrefix("api/User")]
    public class UserController : ApiController
    {
        private readonly UserRepo _repo = new UserRepo();

        //[Route("AllUsers")]
        public JsonData Get()
        {
            return _repo.GetAll();
        }

        public string Get(int id)
        {
            return "value";
        }

        public JsonData Post(UserModel clientData)
        {
            return _repo.Post(clientData);
        }

        [Route("Login")]
        public JsonData Login(LoginDetails clientData)
        {
            return _repo.Login(clientData);
        }

        [Route("UpdateProfile")]
        public JsonData UpdateProfile(User clientData)
        {
            return _repo.UpdateProfile(clientData);
        }

        [Route("updatepicture")]
        public JsonData UpdatePicture(User clientData)
        {
            return _repo.UpdatePicture(clientData);
        }

        [Route("UpdateLoginDetails")]
        public JsonData UpdateLoginDetails(LoginDetails clientData)
        {
            return _repo.UpdateLoginDetails(clientData);
        }

        [Route("Delete")]
        public JsonData Delete(string id)
        {
            return _repo.Delete(id);
        }
    }
}
