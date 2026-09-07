using System.ComponentModel.DataAnnotations;
using MiniTicketSystem.Domain.Enum;

namespace MiniTicketSystem.Application.DTOs
{
    public record TicketInsertDto(
        string Title,
        string Description);
}
