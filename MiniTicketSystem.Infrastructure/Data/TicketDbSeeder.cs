using MiniTicketSystem.Domain.Entities;

namespace MiniTicketSystem.Infrastructure.Data
{
    public static class TicketDbSeeder
    {
        public static async Task SeedDataAsync(TicketDbContext context)
        {
            if (!context.Tickets.Any())
            {
               await context.Tickets.AddRangeAsync(
                    new Ticket()
                    {
                        Id = Guid.NewGuid(),
                        Title = "Sample Ticket 1",
                        Description = "This is a sample ticket.",
                        Status = Domain.Enum.TicketStatus.Open,
                        DateCreated = DateTime.Now,
                    },
                    new Ticket()
                    {
                        Id = Guid.NewGuid(),
                        Title = "Sample Ticket 2",
                        Description = "This is another sample ticket.",
                        Status = Domain.Enum.TicketStatus.InProgress,
                        DateCreated = DateTime.Now,
                    },
                    new Ticket()
                    {
                        Id = Guid.NewGuid(),
                        Title = "Sample Ticket 3",
                        Description = "This is yet another sample ticket.",
                        Status = Domain.Enum.TicketStatus.Closed,
                        DateCreated = DateTime.Now.AddHours(-2),
                        DateClosed = DateTime.Now,
                    }

                );

                await context.SaveChangesAsync();
            }
        }
    }
}
