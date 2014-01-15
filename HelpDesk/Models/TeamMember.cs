using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HelpDesk.Models
{
    public class TeamMember : AuditFileds
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int TeamId { get; set; }
        public virtual Team Team { get; set; }
        public string UserId { get; set; }
        public virtual User User { get; set; }
        //public bool IsLead { get; set; }
    }
}