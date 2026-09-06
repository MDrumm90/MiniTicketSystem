using System.ComponentModel.DataAnnotations;
using MiniTicketSystem.Domain.Enum;

namespace MiniTicketSystem.Application.DTOs
{
    public record TicketUpdateDto(
        [property: Required] string Title,
        Guid Id,
        string Description,
        TicketStatus Status);
}
