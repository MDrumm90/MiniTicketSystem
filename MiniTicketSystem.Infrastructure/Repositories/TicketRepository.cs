using Microsoft.EntityFrameworkCore;
using MiniTicketSystem.Application.Interfaces;
using MiniTicketSystem.Application.Models;
using MiniTicketSystem.Domain.Entities;
using MiniTicketSystem.Domain.Enum;
using MiniTicketSystem.Infrastructure.Data;

namespace MiniTicketSystem.Infrastructure.Repositories
{
    public class TicketRepository : ITicketRepository
    {
        private readonly TicketDbContext _context;

        public TicketRepository(TicketDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Ticket>> GetAllAsync()
        {
            return await _context.Tickets
                  .AsNoTracking()
                  .ToListAsync();
        }

        /// <summary>
        /// Gets a paginated list of tickets based on the provided status, search term, page number, and page size.
        /// </summary>
        /// <param name="status">The status to filter tickets by.</param>
        /// <param name="search">The search term to filter tickets by title or description.</param>
        /// <param name="page">The page number to retrieve.</param>
        /// <param name="pageSize">The number of tickets per page.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a TicketPage.</returns>
        public async Task<TicketPage> GetPageAsync(
       TicketStatus? status,
       string? search,
       int page,
       int pageSize)
        {
            var query = _context.Tickets.AsQueryable();

            if (status.HasValue)
            {
                query = query.Where(x => x.Status == status.Value);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(x =>
                    x.Title.Contains(search) ||
                    x.Description.Contains(search));
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(x => x.DateCreated)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new TicketPage(items, totalCount);
        }

        /// <summary>
        /// Gets a ticket by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the ticket.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the ticket if found; otherwise, null.</returns>
        public async Task<Ticket?> GetByIdAsync(Guid id)
        {
            return await _context.Tickets
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        /// <summary>
        /// Adds a new ticket to the database.
        /// </summary>
        /// <param name="ticket">The ticket to add.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task AddAsync(Ticket ticket)
        {
            await _context.Tickets.AddAsync(ticket);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Updates an existing ticket in the database.
        /// </summary>
        /// <param name="ticket">The ticket to update.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task UpdateAsync(Ticket ticket)
        {
            _context.Tickets.Update(ticket);
            await _context.SaveChangesAsync();
        }
    }
}
