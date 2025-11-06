using CentralHub.Application.Common.Models;
using CentralHub.Application.Features.Clients.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentralHub.Application.Features.Clients.Queries.GetAllClients
{
    // Defines the query, requesting page 1 and size 10 by default
    public record GetAllClientsQuery(int PageNumber = 1, int PageSize = 10)
        : IRequest<PagedResult<ClientSummaryDto>>;
}
