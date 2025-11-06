using CentralHub.Application.Exceptions;
using CentralHub.Application.Features.Clients.DTOs;
using CentralHub.Application.Interfaces;
using CentralHub.Core.Domain.Aggregates.ClientAggregate;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentralHub.Application.Features.Clients.Queries.GetClientById
{
    // Handles the logic for the GetClientByIdQuery
    public class GetClientByIdQueryHandler : IRequestHandler<GetClientByIdQuery, ClientDto>
    {
        private readonly IClientRepository _clientRepository;
        private readonly ICurrentUserService _currentUserService;

        public GetClientByIdQueryHandler(
            IClientRepository clientRepository,
            ICurrentUserService currentUserService)
        {
            _clientRepository = clientRepository;
            _currentUserService = currentUserService;
        }

        public async Task<ClientDto> Handle(GetClientByIdQuery request, CancellationToken cancellationToken)
        {
            var tenantId = _currentUserService.TenantId;

            // 1. Call the new, efficient repository method
            var clientDto = await _clientRepository.GetByIdAsync(request.ClientId, tenantId);

            // 2. Check for null
            if (clientDto is null)
            {
                throw new NotFoundException(nameof(Client), request.ClientId);
            }

            // 3. Return the DTO (no mapping needed!)
            return clientDto;
        }
    }
}
