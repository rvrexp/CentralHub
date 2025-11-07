using CentralHub.Application.Exceptions;
using CentralHub.Application.Interfaces;
using CentralHub.Core.Domain.Aggregates.ClientAggregate;
using CentralHub.Core.Domain.Aggregates.JobAggregate;
using MediatR;

namespace CentralHub.Application.Features.Jobs.Commands.UpdateJob
{
    public class UpdateJobCommandHandler : IRequestHandler<UpdateJobCommand>
    {
        private readonly IJobRepository _jobRepository;
        private readonly IClientRepository _clientRepository; // For validation
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public UpdateJobCommandHandler(
            IJobRepository jobRepository,
            IClientRepository clientRepository,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService)
        {
            _jobRepository = jobRepository;
            _clientRepository = clientRepository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task Handle(UpdateJobCommand request, CancellationToken cancellationToken)
        {
            var tenantId = _currentUserService.TenantId;

            // 1. Find the job to update
            var job = await _jobRepository.FindByIdAsync(request.JobId, tenantId);
            if (job is null)
            {
                throw new NotFoundException(nameof(Job), request.JobId);
            }

            // 2. Validate that the Client/Property exist and belong to this tenant
            var client = await _clientRepository.GetByIdAsync(request.ClientId, tenantId);
            if (client is null)
            {
                throw new NotFoundException(nameof(Client), request.ClientId);
            }
            if (client.Properties.All(p => p.Id != request.PropertyId))
            {
                throw new NotFoundException(nameof(Property), request.PropertyId);
            }

            // 3. Use domain methods to update the job
            job.UpdateDetails(
                request.ScheduledStartTime,
                request.ScheduledEndTime,
                request.Notes
            );
            job.UpdateStatus(request.Status);
            // Might need to block updating ClientId/PropertyId, but allowing it for now

            // 4. Save changes
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
