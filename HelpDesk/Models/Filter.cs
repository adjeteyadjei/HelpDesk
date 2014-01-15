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
}