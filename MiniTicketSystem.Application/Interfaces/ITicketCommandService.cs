using MiniTicketSystem.Application.DTOs;

namespace MiniTicketSystem.Application.Interfaces
{
    public interface ITicketCommandService
    {
        Task<TicketDto> CreateTicketAsync(TicketInsertDto dto);
        Task<TicketDto?> UpdateTicketAsync(TicketUpdateDto dto);
    }
}
