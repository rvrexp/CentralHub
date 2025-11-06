using CentralHub.Application.Exceptions;
using CentralHub.Application.Interfaces;
using CentralHub.Core.Domain.Aggregates.ClientAggregate;
using MediatR;

namespace CentralHub.Application.Features.Clients.Commands.UpdateProperty
{
    public class UpdatePropertyCommandHandler : IRequestHandler<UpdatePropertyCommand>
    {
        private readonly IClientRepository _clientRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public UpdatePropertyCommandHandler(
            IClientRepository clientRepository,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService)
        {
            _clientRepository = clientRepository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task Handle(UpdatePropertyCommand request, CancellationToken cancellationToken)
        {
            var tenantId = _currentUserService.TenantId;

            // 1. Load the aggregate root (the Client)
            var client = await _clientRepository.FindByIdAsync(request.ClientId, tenantId);
            if (client is null)
            {
                throw new NotFoundException(nameof(Client), request.ClientId);
            }

            // 2. Find the specific property within the aggregate's collection
            var propertyToUpdate = client.Properties.FirstOrDefault(p => p.Id == request.PropertyId);
            if (propertyToUpdate is null)
            {
                throw new NotFoundException(nameof(Property), request.PropertyId);
            }

            // 3. Use the entity's own method to perform the update
            propertyToUpdate.UpdateNotes(request.Notes);

            // 4. Save the changes to the *entire* aggregate
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
