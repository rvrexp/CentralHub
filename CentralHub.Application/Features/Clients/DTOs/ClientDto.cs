using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentralHub.Application.Features.Clients.DTOs
{
    // Data Transfer Object for Client details, including Properties
    public record ClientDto(
        Guid Id,
        string Name,
        string Email,
        string? PhoneNumber,
        List<PropertyDto> Properties);
}
