using MiniTicketSystem.Application.DTOs;
using MiniTicketSystem.Application.Models;
using MiniTicketSystem.Domain.Entities;
using MiniTicketSystem.Domain.Enum;

namespace MiniTicketSystem.Application.Interfaces
{
    public interface ITicketRepository
    {
        Task<IEnumerable<Ticket>> GetAllAsync();
        Task<TicketPage> GetPageAsync(
        TicketStatus? status,
        string? search,
        int page,
        int pageSize);
        Task<Ticket?> GetByIdAsync(Guid id);
        Task AddAsync(Ticket ticket);
        Task UpdateAsync(Ticket ticket);
    }
}
