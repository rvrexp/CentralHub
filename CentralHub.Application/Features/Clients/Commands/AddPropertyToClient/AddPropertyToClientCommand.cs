using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CentralHub.Application.Features.Clients.Commands.AddPropertyToClient
{
    public class AddPropertyToClientCommand : IRequest<Guid> // Returns the new Property's ID
    {
        [JsonIgnore] // The ClientId will come from the URL route
        public Guid ClientId { get; set; }

        // These will come from the request body
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string? Notes { get; set; }
    }
}
