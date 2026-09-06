using MiniTicketSystem.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace MiniTicketSystem.Application.DTOs
{
    public record TicketDto(
        Guid Id,
        string Title,
        string Description,
        TicketStatus Status);
}
