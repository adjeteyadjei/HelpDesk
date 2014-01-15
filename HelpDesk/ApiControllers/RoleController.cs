using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using HelpDesk.Classes.Helpers;
using HelpDesk.Classes.Repositories;
using HelpDesk.Models;
using Microsoft.AspNet.Identity.EntityFramework;

namespace HelpDesk.ApiControllers
{
    public class RoleController : ApiController
    {
        private readonly RoleRepo _repo = new RoleRepo();

        //public JsonData Get()
        //{
        //    return _repo.GetAll();
        //}

        //public string Get(int id)
        //{
        //    return "value";
        //}

        //public JsonData Post(IdentityRole clientData)
        //{
        //    return _repo.Post(clientData);
        //}

        //public JsonData Put(int id, Role clientData)
        //{
        //    return _repo.Update(clientData);
        //}

        //public JsonData Delete(string id)
        //{
        //    return _repo.Delete(id);
        //}
    }
}
