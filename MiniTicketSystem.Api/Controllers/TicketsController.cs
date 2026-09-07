using Microsoft.AspNetCore.Mvc;
using MiniTicketSystem.Application.DTOs;
using MiniTicketSystem.Application.Interfaces;
using MiniTicketSystem.Domain.Enum;

namespace MiniTicketSystem.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TicketsController : ControllerBase
    {
        private readonly ITicketQueryService _ticketQueryService;
        private readonly ITicketCommandService _ticketCommandService;

        public TicketsController(ITicketQueryService ticketQueryService, ITicketCommandService ticketCommandService)
        {
            _ticketQueryService = ticketQueryService;
            _ticketCommandService = ticketCommandService;
        }

        [HttpGet("Paged", Name = "PagedTickets")]
        public async Task<Application.DTOs.PagedResult<TicketDto>> GetPagedTickets(TicketStatus? status, string? search, int page = 1, int pageSize = 10)
        {
            var result = await _ticketQueryService.GetPagedTicketsAsync(status, search, page, pageSize);
            return result;
        }

        [HttpGet(Name = "Get")]
        public async Task<IEnumerable<TicketDto>> Get()
        {
            var result = await _ticketQueryService.GetAllTicketsAsync();
            return result;
        }

        [HttpPost]
        public async Task<ActionResult<TicketDto>> CreateTicket(TicketInsertDto dto)
        {
            var result = await _ticketCommandService.CreateTicketAsync(dto);
            return Created($"/tickets/{result.Id}", result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<TicketDto>> UpdateTicket(TicketUpdateDto dto)
        {
            var result = await _ticketCommandService.UpdateTicketAsync(dto);
            if (result is null)
            {
                return NotFound();
            }

            return Ok(result);
        }
    }
}
