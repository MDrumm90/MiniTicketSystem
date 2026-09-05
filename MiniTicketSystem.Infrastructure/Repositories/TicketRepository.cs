using MiniTicketSystem.Application.Interfaces;
using MiniTicketSystem.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace MiniTicketSystem.Infrastructure.Repositories
{
    public class TicketRepository : ITicketRepository
    {
        private readonly TicketDbContext _context;

        public TicketRepository(TicketDbContext context)
        {
            _context = context;
        }
    }
}
