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
                         Title = "Blister line #1 failure",
                         Description = "The blister packaging machine stops randomly after the aluminum foil jams.",
                         Status = Domain.Enum.TicketStatus.Open,
                         DateCreated = DateTime.Now,
                     },
                     new Ticket()
                     {
                         Id = Guid.NewGuid(),
                         Title = "Weight sensor malfunction on the cartoning line",
                         Description = "The weight sensor rejects valid cartons as incomplete; calibration is required.",
                         Status = Domain.Enum.TicketStatus.InProgress,
                         DateCreated = DateTime.Now,
                     },
                     new Ticket()
                     {
                         Id = Guid.NewGuid(),
                         Title = "Conveyor belt motor overheating",
                         Description = "The conveyor belt drive motor overheats after about 2 hours of continuous operation.",
                         Status = Domain.Enum.TicketStatus.Closed,
                         DateCreated = DateTime.Now.AddHours(-2),
                         DateClosed = DateTime.Now,
                     },
                     new Ticket()
                     {
                         Id = Guid.NewGuid(),
                         Title = "Batch number thermal transfer printer failure",
                         Description = "The batch number and expiry date printer keeps jamming the transfer ribbon.",
                         Status = Domain.Enum.TicketStatus.Open,
                         DateCreated = DateTime.Now.AddHours(-1),
                     },
                     new Ticket()
                     {
                         Id = Guid.NewGuid(),
                         Title = "Hydraulic oil leak in the forming press",
                         Description = "A hydraulic oil leak was detected under the blister forming press, risking product contamination.",
                         Status = Domain.Enum.TicketStatus.InProgress,
                         DateCreated = DateTime.Now.AddHours(-3),
                     },
                     new Ticket()
                     {
                         Id = Guid.NewGuid(),
                         Title = "Palletizing robot out of calibration",
                         Description = "The palletizing robot stacks cartons incorrectly on the pallet; position recalibration is required.",
                         Status = Domain.Enum.TicketStatus.Closed,
                         DateCreated = DateTime.Now.AddHours(-4),
                         DateClosed = DateTime.Now.AddHours(-2),
                     },
                     new Ticket()
                     {
                         Id = Guid.NewGuid(),
                         Title = "Air compressor failure in the production hall",
                         Description = "A pressure drop in the pneumatic system is causing downtime on several packaging machines.",
                         Status = Domain.Enum.TicketStatus.Open,
                         DateCreated = DateTime.Now.AddHours(-5),
                     },
                     new Ticket()
                     {
                         Id = Guid.NewGuid(),
                         Title = "PLC controller error on the shrink-wrap line",
                         Description = "The PLC controller randomly raises error E-42 and stops the bundle shrink-wrap cycle.",
                         Status = Domain.Enum.TicketStatus.InProgress,
                         DateCreated = DateTime.Now.AddHours(-6),
                     },
                     new Ticket()
                     {
                         Id = Guid.NewGuid(),
                         Title = "Worn bearing in the blister forming module",
                         Description = "Excessive vibration and noise from the forming module indicate a worn main shaft bearing.",
                         Status = Domain.Enum.TicketStatus.Closed,
                         DateCreated = DateTime.Now.AddHours(-7),
                         DateClosed = DateTime.Now.AddHours(-5),
                     },
                     new Ticket()
                     {
                         Id = Guid.NewGuid(),
                         Title = "Quality control vision system failure",
                         Description = "Vision system cameras fail to detect missing tablets in blisters; optics recalibration is required.",
                         Status = Domain.Enum.TicketStatus.Open,
                         DateCreated = DateTime.Now.AddHours(-8),
                     },
                     new Ticket()
                     {
                         Id = Guid.NewGuid(),
                         Title = "Vacuum forming chamber leak",
                         Description = "The vacuum forming chamber fails to hold the required vacuum; a cracked seal is suspected.",
                         Status = Domain.Enum.TicketStatus.InProgress,
                         DateCreated = DateTime.Now.AddHours(-9),
                     },
                     new Ticket()
                     {
                         Id = Guid.NewGuid(),
                         Title = "Main drive inverter failure on the packaging machine",
                         Description = "The inverter raises an overcurrent fault and shuts down the main drive of the carton packaging machine.",
                         Status = Domain.Enum.TicketStatus.Closed,
                         DateCreated = DateTime.Now.AddHours(-10),
                         DateClosed = DateTime.Now.AddHours(-8),
                     });

                     context.SaveChanges();
            }
        }
    }
}
