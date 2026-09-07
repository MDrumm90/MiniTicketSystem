using MiniTicketSystem.Application.DTOs;
using MiniTicketSystem.Domain.Enum;

namespace MiniTicketSystem.Application.Interfaces
{
    public interface ITicketQueryService
    {
        Task<IEnumerable<TicketDto>> GetAllTicketsAsync();
        Task<PagedResult<TicketDto>> GetPagedTicketsAsync(
        TicketStatus? status,
        string? search,
        int page,
        int pageSize);
        Task<TicketDto?> GetTicketByIdAsync(Guid id);
    }
}
