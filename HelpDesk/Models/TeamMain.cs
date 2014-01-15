using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HelpDesk.Models
{
    public class TeamMain
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public int TeamId { get; set; }
        public virtual Team Team { get; set; }
        //public TeamMain[] Relations { get; set; }
        public UserModel[] Members { get; set; }
        
    }
}