using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity.EntityFramework;

namespace HelpDesk.Models
{
    public class Team : LookUp
    {
        //[NotMapped]
        //public int TeamId { get; set; }
    }

    public class Project : LookUp
    {
        public bool IsActive { get; set; }
    }

    public class Type : LookUp
    {
    }

    public class Priority : LookUp
    {
    }

    public class Status : LookUp
    {
    }

}