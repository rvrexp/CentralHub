using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentralHub.Application.Features.Clients.DTOs
{
    // Data Transfer Object for Property details
    public record PropertyDto(
        Guid Id,
        string Street,
        string City,
        string State,
        string ZipCode,
        string? Notes);
}
