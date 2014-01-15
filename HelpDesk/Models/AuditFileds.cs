using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HelpDesk.Models
{
    public class AuditFileds
    {
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string CreatedById { get; set; }
        public virtual User CreatedBy { get; set; }
        public string UpdatedById { get; set; }
        public virtual User UpdatedBy { get; set; }
    }
}