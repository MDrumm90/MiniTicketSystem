using System.ComponentModel.DataAnnotations;
using MiniTicketSystem.Domain.Enum;

namespace MiniTicketSystem.Application.DTOs
{
    public record TicketInsertDto(
        [property: Required] string Title,
        string Description);
}
