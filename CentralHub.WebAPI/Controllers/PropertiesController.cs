using CentralHub.Application.Features.Clients.Commands.AddPropertyToClient;
using CentralHub.Application.Features.Clients.Commands.DeleteProperty;
using CentralHub.Application.Features.Clients.Commands.UpdateProperty;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CentralHub.WebAPI.Controllers
{
    [ApiController]
    [Route("api/clients")]
    [Authorize] 
    public class PropertiesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PropertiesController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        // POST /api/clients/{clientId}/properties
        [HttpPost("{clientId:guid}/properties")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddPropertyToClient(Guid clientId, [FromBody] AddPropertyToClientCommand command)
        {
            command.ClientId = clientId;
            var propertyId = await _mediator.Send(command);

            // We can build a "GetPropertyById" endpoint later. For now, just return the ID.
            return CreatedAtAction(nameof(ClientsController.GetClientById), new { id = clientId }, new { newPropertyId = propertyId });
        }

        // PUT /api/clients/{clientId}/properties/{propertyId}
        [HttpPut("{clientId:guid}/properties/{propertyId:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateProperty(Guid clientId, Guid propertyId, [FromBody] UpdatePropertyCommand command)
        {
            // Set the IDs from the route
            command.ClientId = clientId;
            command.PropertyId = propertyId;

            await _mediator.Send(command);

            return NoContent(); // Standard for a successful PUT
        }

        // DELETE /api/clients/{clientId}/properties/{propertyId}
        [HttpDelete("{clientId:guid}/properties/{propertyId:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteProperty(Guid clientId, Guid propertyId)
        {
            var command = new DeletePropertyCommand { ClientId = clientId, PropertyId = propertyId };
            await _mediator.Send(command);
            return NoContent();
        }
    }
}
