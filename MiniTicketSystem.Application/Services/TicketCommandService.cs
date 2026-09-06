using MiniTicketSystem.Application.DTOs;
using MiniTicketSystem.Application.Interfaces;
using MiniTicketSystem.Domain.Entities;
using MiniTicketSystem.Domain.Enum;

namespace MiniTicketSystem.Application.Services
{
    public class TicketCommandService : ITicketCommandService
    {
        private readonly ITicketRepository _repository;

        public TicketCommandService(ITicketRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Creates a new ticket based on the provided TicketInsertDto. It generates a new GUID for the ticket ID, sets the status to Open, and records the current UTC time as the date created. The ticket is then added to the repository, and a TicketDto representing the newly created ticket is returned.
        /// </summary>
        /// <param name="dto">The TicketInsertDto containing the details of the ticket to be created.</param>
        /// <returns> The newly created TicketDto.</returns>
        public async Task<TicketDto> CreateTicketAsync(TicketInsertDto dto)
        {
            var ticket = new Ticket
            {
                Id = Guid.NewGuid(),
                Title = dto.Title,
                Description = dto.Description,
                Status = TicketStatus.Open,
                DateCreated = DateTime.UtcNow,
                UpdatedAt = null,
            };

            await _repository.AddAsync(ticket);

            return new TicketDto(ticket.Id, ticket.Title, ticket.Description, ticket.Status, ticket.UpdatedAt, ticket.DateCreated, ticket.DateClosed);
        }


        /// <summary>
        /// Updates an existing ticket based on the provided TicketUpdateDto. If the ticket is not found, it returns null. If the ticket is found, it updates the title, description, status, and date closed (if applicable), and returns the updated TicketDto.
        /// </summary>
        /// <param name="dto">The TicketUpdateDto containing the details of the ticket to be updated.</param>
        /// <returns> The updated TicketDto if the ticket is found; otherwise, null.</returns>
        public async Task<TicketDto?> UpdateTicketAsync(TicketUpdateDto dto)
        {
            var ticket = await _repository.GetByIdAsync(dto.Id);
            if (ticket is null)
            {
                return null;
            }

            ticket.Title = dto.Title;
            ticket.Description = dto.Description;

            if (dto.Status == TicketStatus.Closed && ticket.Status != TicketStatus.Closed)
            {
                ticket.DateClosed = DateTime.UtcNow;
            }
            else if (dto.Status != TicketStatus.Closed && ticket.Status == TicketStatus.Closed)
            {
                ticket.DateClosed = default;
            }

            ticket.Status = dto.Status;
            ticket.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(ticket);

            return new TicketDto(ticket.Id, ticket.Title, ticket.Description, ticket.Status, ticket.UpdatedAt, ticket.DateCreated, ticket.DateClosed);

        }
    }
}
