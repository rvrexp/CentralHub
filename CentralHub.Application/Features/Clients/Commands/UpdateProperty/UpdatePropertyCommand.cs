using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CentralHub.Application.Features.Clients.Commands.UpdateProperty
{
    public class UpdatePropertyCommand : IRequest
    {
        // These will be set from the URL route
        [JsonIgnore]
        public Guid ClientId { get; set; }
        [JsonIgnore]
        public Guid PropertyId { get; set; }

        // This is the data from the request body
        public string? Notes { get; set; }
    }
}
