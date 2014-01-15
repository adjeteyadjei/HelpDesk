using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HelpDesk.Models
{
    public class RolePermission : AuditFileds
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string RoleName { get; set; }
        //public virtual Role Role { get; set; }
        public bool CanAdd { get; set; }
        public bool CanEdit { get; set; }
        public bool CanView { get; set; }
        public bool CanForward { get; set; }
        public bool CanAssign { get; set; }
    }
}