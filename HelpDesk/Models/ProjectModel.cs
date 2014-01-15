using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HelpDesk.Models
{
    public class ProjectModel
    {
        public int ProjectId { get; set; }
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public bool IsActive { get; set; }

        public TeamMain[] Teams { get; set; }
        public UserModel[] Leaders { get; set; }
    }
}