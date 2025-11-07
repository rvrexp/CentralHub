using CentralHub.Application.Exceptions;
using CentralHub.Application.Interfaces;
using CentralHub.Core.Domain.Aggregates.JobAggregate;
using MediatR;

namespace CentralHub.Application.Features.Jobs.Commands.DeleteJob
{
    public class DeleteJobCommandHandler : IRequestHandler<DeleteJobCommand>
    {
        private readonly IJobRepository _jobRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public DeleteJobCommandHandler(
            IJobRepository jobRepository,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService)
        {
            _jobRepository = jobRepository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task Handle(DeleteJobCommand request, CancellationToken cancellationToken)
        {
            var tenantId = _currentUserService.TenantId;

            var job = await _jobRepository.FindByIdAsync(request.JobId, tenantId);
            if (job is null)
            {
                throw new NotFoundException(nameof(Job), request.JobId);
            }

            _jobRepository.Delete(job);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
