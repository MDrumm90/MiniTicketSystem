using Microsoft.AspNetCore.Mvc;
using MiniTicketSystem.Application.DTOs;
using MiniTicketSystem.Application.Exceptions;
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
        public async Task<ActionResult<Application.DTOs.PagedResult<TicketDto>>> GetPagedTickets(TicketStatus? status, string? search, int page = 1, int pageSize = 10)
        {
            try
            {
                var result = await _ticketQueryService.GetPagedTicketsAsync(status, search, page, pageSize);
                return Ok(result);
            }
            catch (TicketServiceException ex)
            {
                return ServiceError(ex);
            }
        }

        [HttpGet(Name = "Get")]
        public async Task<ActionResult<IEnumerable<TicketDto>>> Get()
        {
            try
            {
                var result = await _ticketQueryService.GetAllTicketsAsync();
                return Ok(result);
            }
            catch (TicketServiceException ex)
            {
                return ServiceError(ex);
            }
        }

        [HttpGet("{id:guid}", Name = "GetTicketById")]
        public async Task<ActionResult<TicketDto>> GetById(Guid id)
        {
            try
            {
                var result = await _ticketQueryService.GetTicketByIdAsync(id);
                if (result is null)
                {
                    return NotFound();
                }

                return Ok(result);
            }
            catch (TicketServiceException ex)
            {
                return ServiceError(ex);
            }
        }

        [HttpPost]
        public async Task<ActionResult<TicketDto>> CreateTicket(TicketInsertDto dto)
        {
            try
            {
                var result = await _ticketCommandService.CreateTicketAsync(dto);
                return Created($"/tickets/{result.Id}", result);
            }
            catch (TicketServiceException ex)
            {
                return ServiceError(ex);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<TicketDto>> UpdateTicket(TicketUpdateDto dto)
        {
            try
            {
                var result = await _ticketCommandService.UpdateTicketAsync(dto);
                if (result is null)
                {
                    return NotFound();
                }

                return Ok(result);
            }
            catch (TicketServiceException ex)
            {
                return ServiceError(ex);
            }
        }

        private ObjectResult ServiceError(TicketServiceException ex) =>
            Problem(
                title: "Wystąpił błąd",
                detail: ex.Message,
                statusCode: StatusCodes.Status500InternalServerError);
    }
}
