using CentralHub.Application.Features.Clients.Commands.CreateClient;
using CentralHub.Application.Features.Clients.Commands.DeleteClient;
using CentralHub.Application.Features.Clients.Commands.UpdateClient;
using CentralHub.Application.Features.Clients.Queries.GetAllClients;
using CentralHub.Application.Features.Clients.Queries.GetClientById;
using CentralHub.Application.Interfaces;
using CentralHub.Core.Domain.Constants;
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
        // GET /api/clients
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllClients([FromQuery] GetAllClientsQuery query)
        {
            // The query object will be bound from query string parameters (e.g., ?pageNumber=1&pageSize=10)
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        // PUT /api/clients/{id}
        [HttpPut("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateClient(Guid id, [FromBody] UpdateClientCommand command)
        {
            // 1. Manually set the Id from the route, as it's not in the body
            command.Id = id;

            // 2. Send the command to the handler
            await _mediator.Send(command);

            // 3. Return a 204 No Content, which is the standard for a successful PUT
            return NoContent();
        }
        // DELETE /api/clients/{id}
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)] 
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> DeleteClient(Guid id)
        {
            // Create the command with the ID from the route
            var command = new DeleteClientCommand(id);

            await _mediator.Send(command);

            // Return a 204 No Content, which is the standard for a successful DELETE
            return NoContent();
        }
    }
}
