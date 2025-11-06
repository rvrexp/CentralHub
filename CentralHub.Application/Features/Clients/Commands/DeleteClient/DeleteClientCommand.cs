using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentralHub.Application.Features.Clients.Commands.DeleteClient
{
    // This command only needs the ID of the client to delete
    public record DeleteClientCommand(Guid Id) : IRequest;
}
