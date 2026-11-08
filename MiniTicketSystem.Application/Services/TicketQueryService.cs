using Microsoft.Extensions.Logging;
using MiniTicketSystem.Application.DTOs;
using MiniTicketSystem.Application.Exceptions;
using MiniTicketSystem.Application.Interfaces;
using MiniTicketSystem.Application.Models;
using MiniTicketSystem.Domain.Entities;
using MiniTicketSystem.Domain.Enum;

namespace MiniTicketSystem.Application.Services
{
    public class TicketQueryService : ITicketQueryService
    {
        private readonly ITicketRepository _repository;
        private readonly ILogger<TicketQueryService> _logger;

        public TicketQueryService(ITicketRepository repository, ILogger<TicketQueryService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        /// <summary>
        /// Gets all tickets asynchronously.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains an IEnumerable of TicketDto.</returns>
        /// <exception cref="TicketServiceException">Thrown when the tickets could not be retrieved.</exception>
        public async Task<IEnumerable<TicketDto>> GetAllTicketsAsync()
        {
            try
            {
                var tickets = await _repository.GetAllAsync();
                return tickets.Select(x => new TicketDto(
                    x.Id,
                    x.Title,
                    x.Description,
                    x.Status,
                    x.UpdatedAt,
                    x.DateCreated,
                    x.DateClosed))
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while retrieving all tickets");
                throw new TicketServiceException("Wystąpił błąd podczas pobierania zgłoszeń. Spróbuj ponownie później.", ex);
            }
        }

        /// <summary>
        /// Gets a paged list of tickets asynchronously.
        /// </summary>
        /// <param name="status">The status to filter tickets by.</param>
        /// <param name="search">The search term to filter tickets by title or description.</param>
        /// <param name="page">The page number to retrieve.</param>
        /// <param name="pageSize">The number of tickets per page.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a PagedResult of TicketDto.</returns>
        /// <exception cref="TicketServiceException">Thrown when the tickets could not be retrieved.</exception>
        public async Task<PagedResult<TicketDto>> GetPagedTicketsAsync(
         TicketStatus? status,
         string? search,
         int page,
         int pageSize)
        {
            page = Math.Max(page, 1);
            pageSize = Math.Clamp(pageSize, 1, 100);
            search = search?.Trim();

            try
            {
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
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error while retrieving paged tickets (Status: {Status}, Search: {Search}, Page: {Page}, PageSize: {PageSize})",
                    status, search, page, pageSize);
                throw new TicketServiceException("Wystąpił błąd podczas pobierania zgłoszeń. Spróbuj ponownie później.", ex);
            }
        }

        /// <summary>
        /// Gets a single ticket by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the ticket.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the TicketDto if found; otherwise, null.</returns>
        /// <exception cref="TicketServiceException">Thrown when the ticket could not be retrieved.</exception>
        public async Task<TicketDto?> GetTicketByIdAsync(Guid id)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while retrieving ticket {TicketId}", id);
                throw new TicketServiceException("Wystąpił błąd podczas pobierania zgłoszenia. Spróbuj ponownie później.", ex);
            }
        }
    }
}
