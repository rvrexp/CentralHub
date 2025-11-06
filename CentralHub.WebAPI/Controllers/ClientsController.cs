using CentralHub.Application.Features.Clients.Commands.CreateClient;
using CentralHub.Application.Features.Clients.Queries.GetClientById;
using CentralHub.Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CentralHub.WebAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ClientsController : ControllerBase
    {
        private readonly IMediator _mediator; // Injects MediatR
        private readonly ICurrentUserService _currentUser;

        public ClientsController(IMediator mediator, ICurrentUserService currentUserService)
        {
            _mediator = mediator;
            _currentUser = currentUserService;
        }
        // ---  TEST ENDPOINT ---
        [HttpGet("my-info")]
        public IActionResult GetMyInfo()
        {
            // This endpoint will read the claims from the JWT
            // and return them. It proves service works.
            return Ok(new
            {
                Message = "This is a secure endpoint!",
                _currentUser.UserId,
                _currentUser.TenantId,
                _currentUser.Email
            });
        }
        // POST /api/clients
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateClient([FromBody] CreateClientCommand command)
        {
            var clientId = await _mediator.Send(command); // Dispatches the command
                                                          // Returns a 201 Created status with the location of the new resource
            return CreatedAtAction(nameof(GetClientById), new { id = clientId }, new { id = clientId });
        }

        // GET /api/clients/{id}
        [HttpGet("{id:guid}", Name = "GetClientById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetClientById(Guid id)
        {
            var query = new GetClientByIdQuery(id);
            var client = await _mediator.Send(query); // Dispatches the query
            return Ok(client); // Returns 200 OK with the ClientDto
        }
    }
}
