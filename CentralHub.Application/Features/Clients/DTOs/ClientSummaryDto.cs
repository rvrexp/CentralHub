using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentralHub.Application.Features.Clients.DTOs
{
    // DTO for client lists
    public record ClientSummaryDto(
        Guid Id,
        string Name,
        string Email,
        string? PhoneNumber);
}
