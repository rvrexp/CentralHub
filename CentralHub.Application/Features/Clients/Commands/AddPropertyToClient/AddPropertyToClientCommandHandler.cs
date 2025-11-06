using CentralHub.Application.Exceptions;
using CentralHub.Application.Interfaces;
using CentralHub.Core.Domain.Aggregates.ClientAggregate;
using CentralHub.Core.Domain.ValueObjects;
using MediatR;

namespace CentralHub.Application.Features.Clients.Commands.AddPropertyToClient
{
    public class AddPropertyToClientCommandHandler : IRequestHandler<AddPropertyToClientCommand, Guid>
    {
        private readonly IClientRepository _clientRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public AddPropertyToClientCommandHandler(
            IClientRepository clientRepository,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService)
        {
            _clientRepository = clientRepository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<Guid> Handle(AddPropertyToClientCommand request, CancellationToken cancellationToken)
        {
            var tenantId = _currentUserService.TenantId;

            // 1. Get the parent Client aggregate
            var client = await _clientRepository.FindByIdAsync(request.ClientId, tenantId);
            if (client is null)
            {
                throw new NotFoundException(nameof(Client), request.ClientId);
            }

            // 2. Create the Address Value Object
            var newAddress = new Address(request.Street, request.City, request.State, request.ZipCode);

            // 3. Use the Client aggregate's business logic to add the property
            // This ensures our domain rule (no duplicate addresses) is enforced.
            client.AddProperty(newAddress, request.Notes);

            // 4. Save the changes to the aggregate
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // 5. Find the newly created property to return its ID
            var newProperty = client.Properties.First(p => p.Address == newAddress);
            return newProperty.Id;
        }
    }
}
