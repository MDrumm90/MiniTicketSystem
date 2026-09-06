using MiniTicketSystem.Domain.Entities;

namespace MiniTicketSystem.Application.Models
{
    public record TicketPage(
        IReadOnlyList<Ticket> Items,
        int TotalCount);
}
