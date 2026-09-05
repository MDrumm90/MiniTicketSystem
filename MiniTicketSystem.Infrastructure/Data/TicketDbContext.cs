using Microsoft.EntityFrameworkCore;
using MiniTicketSystem.Domain.Entities;
using System.Net.Sockets;

namespace MiniTicketSystem.Infrastructure.Data
{
    public class TicketDbContext : DbContext
    {
        public TicketDbContext(DbContextOptions<TicketDbContext> options)
            : base(options)
        {
        }

        public DbSet<Ticket> Tickets => Set<Ticket>();
    }
}
