using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentralHub.Application.Features.Clients.Commands.CreateClient
{
    // Command object carrying data to create a client
    public record CreateClientCommand(
        string Name,
        string Email,
        string? PhoneNumber) : IRequest<Guid>; // Returns the new client's Guid
}
