using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HelpDesk.Models
{
    public class TicketModel
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }
        public int ProjectId { get; set; }
        public virtual Project Project { get; set; }
        public int TypeId { get; set; }
        public virtual Type Type { get; set; }
        public int StatusId { get; set; }
        public virtual Status Status { get; set; }
        public int PriorityId { get; set; }
        public virtual Priority Priority { get; set; }
        public int? ParentTicketId { get; set; }
        //public virtual Ticket ParentTicket { get; set; }
        public string AssignedToId { get; set; }
        public virtual User AssignedTo { get; set; }
        public string AssignedById { get; set; }
        public virtual User AssignedBy { get; set; }
        public int ParentId { get; set; }
        public List<TicketDetail> TicketDetail { get; set; }
        public List<CommentViewModel> Comments { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}