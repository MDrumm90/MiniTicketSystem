using MiniTicketSystem.Domain.Enum;

namespace MiniTicketSystem.Domain.Entities
{
    public class Ticket
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public TicketStatus Status { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateClosed { get; set; }
    }
}
