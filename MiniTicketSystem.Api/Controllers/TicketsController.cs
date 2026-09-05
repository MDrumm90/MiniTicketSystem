using Microsoft.AspNetCore.Mvc;

namespace MiniTicketSystem.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TicketsController : ControllerBase
    {


        [HttpGet(Name = "GetTickets")]
        public IEnumerable<Ticket> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new Ticket
            {
            })
            .ToArray();
        }
    }
}
