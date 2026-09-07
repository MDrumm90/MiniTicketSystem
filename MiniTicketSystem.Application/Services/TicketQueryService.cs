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

        /// <summary>
        /// Gets all tickets asynchronously.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains an IEnumerable of TicketDto.</returns>
        public async Task<IEnumerable<TicketDto>> GetAllTicketsAsync()
        {
            var tickets = await _repository.GetAllAsync();
            return tickets.Select(x => new TicketDto(
                x.Id,
                x.Title,
                x.Description,
                x.Status,
                x.UpdatedAt,
                x.DateCreated,
                x.DateClosed));
        }

        /// <summary>
        /// Gets a paged list of tickets asynchronously.
        /// </summary>
        /// <param name="status">The status to filter tickets by.</param>
        /// <param name="search">The search term to filter tickets by title or description.</param>
        /// <param name="page">The page number to retrieve.</param>
        /// <param name="pageSize">The number of tickets per page.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a PagedResult of TicketDto.</returns>
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
                    x.Status,
                    x.UpdatedAt,
                    x.DateCreated,
                    x.DateClosed))
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

        /// <summary>
        /// Gets a single ticket by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the ticket.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the TicketDto if found; otherwise, null.</returns>
        public async Task<TicketDto?> GetTicketByIdAsync(Guid id)
        {
            var ticket = await _repository.GetByIdAsync(id);
            if (ticket is null)
            {
                return null;
            }

            return new TicketDto(
                ticket.Id,
                ticket.Title,
                ticket.Description,
                ticket.Status,
                ticket.UpdatedAt,
                ticket.DateCreated,
                ticket.DateClosed);
        }
    }
}
