using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentralHub.Application.Features.Clients.Commands.DeleteProperty
{
    public class DeletePropertyCommand : IRequest
    {
        public Guid ClientId { get; set; }
        public Guid PropertyId { get; set; }
    }
}
