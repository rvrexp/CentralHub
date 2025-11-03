using CentralHub.Application.Features.Clients.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentralHub.Application.Features.Clients.Queries.GetClientById
{
    // Query object carrying data to get a client
    public record GetClientByIdQuery(Guid ClientId) : IRequest<ClientDto>; // Expects a ClientDto back
}
