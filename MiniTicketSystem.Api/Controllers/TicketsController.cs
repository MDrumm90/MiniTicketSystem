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

        public TicketsController(ITicketQueryService ticketQueryService)
        {
            _ticketQueryService = ticketQueryService;
        }


        [HttpGet(Name = "PagedTickets")]
        public async Task<Application.DTOs.PagedResult<TicketDto>> GetPagedTickets(TicketStatus? status, string? search, int page = 1, int pageSize = 10)
        {
            var result = await _ticketQueryService.GetPagedTicketsAsync(status, search, page, pageSize);
            return result;
        }

        [HttpGet(Name = "Get")]
        public async Task<IEnumerable<TicketDto>> Get(string? search)
        {
            var result = await _ticketQueryService.GetAllTicketsAsync();
            return result;
        }
    }
}
