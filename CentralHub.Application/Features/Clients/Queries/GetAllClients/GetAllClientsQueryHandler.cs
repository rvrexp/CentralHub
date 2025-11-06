using CentralHub.Application.Common.Models;
using CentralHub.Application.Features.Clients.DTOs;
using CentralHub.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentralHub.Application.Features.Clients.Queries.GetAllClients
{
    public class GetAllClientsQueryHandler : IRequestHandler<GetAllClientsQuery, PagedResult<ClientSummaryDto>>
    {
        // Use the INTERFACE
        private readonly IClientRepository _clientRepository;
        private readonly ICurrentUserService _currentUserService;

        public GetAllClientsQueryHandler(
            IClientRepository clientRepository, // Inject the INTERFACE
            ICurrentUserService currentUserService)
        {
            _clientRepository = clientRepository;
            _currentUserService = currentUserService;
        }

        public async Task<PagedResult<ClientSummaryDto>> Handle(GetAllClientsQuery request, CancellationToken cancellationToken)
        {
            var tenantId = _currentUserService.TenantId;

            // Call the repository method, which does all the work
            var result = await _clientRepository.GetPagedListAsync(
                tenantId,
                request.PageNumber,
                request.PageSize
            );

            return result;
        }
    }

}
