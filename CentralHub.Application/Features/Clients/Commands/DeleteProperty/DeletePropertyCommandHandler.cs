using CentralHub.Application.Exceptions;
using CentralHub.Application.Interfaces;
using CentralHub.Core.Domain.Aggregates.ClientAggregate;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentralHub.Application.Features.Clients.Commands.DeleteProperty
{
    public class DeletePropertyCommandHandler : IRequestHandler<DeletePropertyCommand>
    {
        private readonly IClientRepository _clientRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public DeletePropertyCommandHandler(
            IClientRepository clientRepository,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService)
        {
            _clientRepository = clientRepository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task Handle(DeletePropertyCommand request, CancellationToken cancellationToken)
        {
            var tenantId = _currentUserService.TenantId;

            // 1. Load the aggregate root (the Client)
            var client = await _clientRepository.FindByIdAsync(request.ClientId, tenantId);
            if (client is null)
            {
                throw new NotFoundException(nameof(Client), request.ClientId);
            }

            // 2. Use the Domain entity's own method to remove the property
            try
            {
                client.RemoveProperty(request.PropertyId);
            }
            catch (InvalidOperationException ex)
            {
                // This catches the error if the property ID doesn't exist
                throw new NotFoundException(nameof(Property), request.PropertyId);
            }

            // 3. Save the changes. EF Core is smart enough to see
            //    a property was removed from the Client's collection and will delete it.
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
