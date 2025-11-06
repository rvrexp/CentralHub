using CentralHub.Application.Exceptions;
using CentralHub.Application.Interfaces;
using CentralHub.Core.Domain.Aggregates.ClientAggregate;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentralHub.Application.Features.Clients.Commands.UpdateClient
{
    public class UpdateClientCommandHandler : IRequestHandler<UpdateClientCommand>
    {
        private readonly IClientRepository _clientRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public UpdateClientCommandHandler(
            IClientRepository clientRepository,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService)
        {
            _clientRepository = clientRepository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task Handle(UpdateClientCommand request, CancellationToken cancellationToken)
        {
            var tenantId = _currentUserService.TenantId;

            // 1. Get the full, tracked entity using our new repository method
            var clientToUpdate = await _clientRepository.FindByIdAsync(request.Id, tenantId);

            // 2. Check if it exists
            if (clientToUpdate is null)
            {
                throw new NotFoundException(nameof(Client), request.Id);
            }

            // 3. Use the Domain entity's own method to perform the update
            //    This keeps our business rules (invariants) safe
            clientToUpdate.UpdateInfo(request.Name, request.Email, request.PhoneNumber);

            // 4. Save the changes
            //    We don't need to call an "Update" method on the repository
            //    because the DbContext is already tracking the clientToUpdate entity.
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
