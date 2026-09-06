using MiniTicketSystem.Application.DTOs;
using MiniTicketSystem.Application.Interfaces;
using MiniTicketSystem.Application.Models;
using MiniTicketSystem.Domain.Entities;
using MiniTicketSystem.Domain.Enum;

namespace MiniTicketSystem.Application.Services
{
    public class TicketQueryService : ITicketQueryService
    {
        private readonly ITicketRepository _repository;

        public TicketQueryService(ITicketRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<TicketDto>> GetAllTicketsAsync()
        {
            var tickets = await _repository.GetAllAsync();
            return tickets.Select(x => new TicketDto(
                x.Id,
                x.Title,
                x.Description,
                x.Status));
        }

        public async Task<PagedResult<TicketDto>> GetPagedTicketsAsync(
         TicketStatus? status,
         string? search,
         int page,
         int pageSize)
        {
            page = Math.Max(page, 1);
            pageSize = Math.Clamp(pageSize, 1, 100);
            search = search?.Trim();

            TicketPage result = await _repository.GetPageAsync(
                status,
                search,
                page,
                pageSize);

            var items = result.Items
                .Select(x => new TicketDto(
                    x.Id,
                    x.Title,
                    x.Description,
                    x.Status))
                .ToList();

            var totalPages = (int)Math.Ceiling(
            (double)result.TotalCount / pageSize);

            return new PagedResult<TicketDto>(
                items,
                page,
                pageSize,
                result.TotalCount,
                totalPages);
        }
    }
}
