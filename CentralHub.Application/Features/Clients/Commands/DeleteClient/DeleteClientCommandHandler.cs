using CentralHub.Application.Exceptions;
using CentralHub.Application.Interfaces;
using CentralHub.Core.Domain.Aggregates.ClientAggregate;
using MediatR;

namespace CentralHub.Application.Features.Clients.Commands.DeleteClient
{
    public class DeleteClientCommandHandler : IRequestHandler<DeleteClientCommand>
    {
        private readonly IClientRepository _clientRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public DeleteClientCommandHandler(
            IClientRepository clientRepository,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService)
        {
            _clientRepository = clientRepository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task Handle(DeleteClientCommand request, CancellationToken cancellationToken)
        {
            var tenantId = _currentUserService.TenantId;

            // 1. Get the full, tracked entity
            var clientToDelete = await _clientRepository.FindByIdAsync(request.Id, tenantId);

            // 2. Check if it exists
            if (clientToDelete is null)
            {
                throw new NotFoundException(nameof(Client), request.Id);
            }

            // 3. Mark the entity for deletion using the repository
            _clientRepository.Delete(clientToDelete);

            // 4. Save the changes
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
