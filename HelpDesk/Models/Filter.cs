namespace HelpDesk.Models
{
    public class Filter
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string UserId { get; set; }

        public int TeamId { get; set; }

        public int ProjectId { get; set; }

        public int TicketId { get; set; }
    }

    public class Settings
    {
        public bool sendmail { get; set; }
        public string smtp { get; set; }
        public int port { get; set; }
        public string from { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string subject { get; set; }
        public string message { get; set; }
    }

    public class ForwardModel
    {
        public int ticketId { get; set; }
        public int teamId { get; set; }
    }
}