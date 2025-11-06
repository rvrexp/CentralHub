using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CentralHub.Application.Features.Clients.Commands.UpdateClient
{
    public class UpdateClientCommand : IRequest // A command doesn't have to return data, so just IRequest
    {
        // The ID will come from the URL route, so we hide it from the request body
        [JsonIgnore]
        public Guid Id { get; set; }

        // These properties will come from the request body
        public string Name { get; set; }
        public string Email { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
