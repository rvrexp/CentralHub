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
        private readonly ICurrentUserService _currentUserService; // Will be injected

        public GetClientByIdQueryHandler(
            IClientRepository clientRepository,
            ICurrentUserService currentUserService)
        {
            _clientRepository = clientRepository;
            _currentUserService = currentUserService;
        }

        public async Task<ClientDto> Handle(GetClientByIdQuery request, CancellationToken cancellationToken)
        {
            var tenantId = _currentUserService.TenantId; // Get TenantId

            var client = await _clientRepository.GetByIdAsync(request.ClientId, tenantId); // Use TenantId

            if (client is null)
            {
                throw new NotFoundException(nameof(Client), request.ClientId);
            }

            // Map domain entity to DTO
            return new ClientDto(
                client.Id,
                client.Name,
                client.Email,
                client.PhoneNumber,
                client.Properties.Select(p => new PropertyDto(
                    p.Id,
                    p.Address.Street,
                    p.Address.City,
                    p.Address.State,
                    p.Address.ZipCode,
                    p.Notes
                )).ToList()
            );
        }
    }
}
