using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity.EntityFramework;

namespace HelpDesk.Models
{
    public class Role : IdentityRole
    {
        //[Key]
        //public string Id { get; set; }

        //[Required]
        //public string Name { get; set; }

        public string Description { get; set; }

        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        //public string CreatedById { get; set; }
        //public virtual User CreatedBy { get; set; }
        //public string UpdatedById { get; set; }
        //public virtual User UpdatedBy { get; set; }
    }
}