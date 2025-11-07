using CentralHub.Application.Exceptions;
using CentralHub.Application.Interfaces;
using CentralHub.Core.Domain.Aggregates.ClientAggregate;
using CentralHub.Core.Domain.Aggregates.JobAggregate;
using MediatR;

namespace CentralHub.Application.Features.Jobs.Commands.CreateJob
{
    public class CreateJobCommandHandler : IRequestHandler<CreateJobCommand, Guid>
    {
        private readonly IJobRepository _jobRepository;
        private readonly IClientRepository _clientRepository; // Need this to check if Client/Property exist
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public CreateJobCommandHandler(
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

        public async Task<Guid> Handle(CreateJobCommand request, CancellationToken cancellationToken)
        {
            var tenantId = _currentUserService.TenantId;

            // --- Validation Step ---
            // Ensure the Client and Property exist and belong to this tenant.
            // We use the new, efficient DTO-based query for this check.
            var client = await _clientRepository.GetByIdAsync(request.ClientId, tenantId);
            if (client is null)
            {
                throw new NotFoundException(nameof(Client), request.ClientId);
            }

            var property = client.Properties.FirstOrDefault(p => p.Id == request.PropertyId);
            if (property is null)
            {
                throw new NotFoundException(nameof(Property), request.PropertyId);
            }
            // --- End Validation ---

            // Create the domain entity
            var job = Job.Create(
                tenantId,
                request.ClientId,
                request.PropertyId,
                request.ScheduledStartTime,
                request.ScheduledEndTime,
                request.Notes
            );

            await _jobRepository.AddAsync(job);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return job.Id;
        }
    }
}
