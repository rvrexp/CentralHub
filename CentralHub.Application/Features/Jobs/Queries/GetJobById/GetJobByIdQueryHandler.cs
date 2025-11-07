using CentralHub.Application.Exceptions;
using CentralHub.Application.Features.Jobs.DTOs;
using CentralHub.Application.Interfaces;
using CentralHub.Core.Domain.Aggregates.JobAggregate;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentralHub.Application.Features.Jobs.Queries.GetJobById
{
    public class GetJobByIdQueryHandler : IRequestHandler<GetJobByIdQuery, JobDto>
    {
        private readonly IJobRepository _jobRepository;
        private readonly ICurrentUserService _currentUserService;

        public GetJobByIdQueryHandler(IJobRepository jobRepository, ICurrentUserService currentUserService)
        {
            _jobRepository = jobRepository;
            _currentUserService = currentUserService;
        }

        public async Task<JobDto> Handle(GetJobByIdQuery request, CancellationToken cancellationToken)
        {
            var tenantId = _currentUserService.TenantId;
            var job = await _jobRepository.GetJobByIdAsync(request.JobId, tenantId);

            if (job is null)
            {
                throw new NotFoundException(nameof(Job), request.JobId);
            }

            return job;
        }
    }
}
