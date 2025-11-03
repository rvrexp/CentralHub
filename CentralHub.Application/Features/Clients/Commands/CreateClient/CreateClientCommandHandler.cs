using CentralHub.Application.Interfaces;
using CentralHub.Core.Domain.Aggregates.ClientAggregate;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentralHub.Application.Features.Clients.Commands.CreateClient
{
    // Handles the logic for the CreateClientCommand
    public class CreateClientCommandHandler : IRequestHandler<CreateClientCommand, Guid>
    {
        private readonly IClientRepository _clientRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService; // Will be injected

        public CreateClientCommandHandler(
            IClientRepository clientRepository,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService)
        {
            _clientRepository = clientRepository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<Guid> Handle(CreateClientCommand request, CancellationToken cancellationToken)
        {
            var tenantId = _currentUserService.TenantId; // Get TenantId from the service

            var client = Client.Create(
                tenantId,
                request.Name,
                request.Email,
                request.PhoneNumber);

            await _clientRepository.AddAsync(client);
            await _unitOfWork.SaveChangesAsync(cancellationToken); // Commit transaction

            return client.Id;
        }
    }
}
