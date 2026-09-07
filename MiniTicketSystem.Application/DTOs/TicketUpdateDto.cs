using System.ComponentModel.DataAnnotations;
using MiniTicketSystem.Domain.Enum;

namespace MiniTicketSystem.Application.DTOs
{
    public record TicketUpdateDto(
        string Title,
        Guid Id,
        string Description,
        TicketStatus Status);
}
