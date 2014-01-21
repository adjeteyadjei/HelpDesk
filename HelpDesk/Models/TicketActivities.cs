using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HelpDesk.Models
{
    public class TicketActivity
    {
        public int Id { get; set; }
        public string AgentId { get; set; }
        public virtual User Agent { get; set; }
        public string Action { get; set; }
        public int TicketId { get; set; }
        public virtual Ticket Ticket { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedById { get; set; }
        public virtual User CreatedBy { get; set; }
    }

    public class DashboardModel
    {
        public List<TicketActivity> Activities { get; set; }
        public TicketStats TicketStats { get; set; }
    }

    public class TicketStats
    {
        public List<TicketModel> New { get; set; }
        public List<TicketModel> Open { get; set; }
        public List<TicketModel> Pending { get; set; }
        public List<TicketModel> Solved { get; set; }
    }
}