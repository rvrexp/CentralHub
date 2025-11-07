using CentralHub.Application.Common.Models;
using CentralHub.Application.Features.Jobs.DTOs;
using CentralHub.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentralHub.Application.Features.Jobs.Queries.GetJobsForDateRange
{
    public class GetJobsForDateRangeQueryHandler : IRequestHandler<GetJobsForDateRangeQuery, PagedResult<JobSummaryDto>>
    {
        private readonly IJobRepository _jobRepository;
        private readonly ICurrentUserService _currentUserService;

        public GetJobsForDateRangeQueryHandler(IJobRepository jobRepository, ICurrentUserService currentUserService)
        {
            _jobRepository = jobRepository;
            _currentUserService = currentUserService;
        }

        public async Task<PagedResult<JobSummaryDto>> Handle(GetJobsForDateRangeQuery request, CancellationToken cancellationToken)
        {
            var tenantId = _currentUserService.TenantId;
            return await _jobRepository.GetJobsForDateRangeAsync(
                tenantId,
                request.StartDate,
                request.EndDate,
                request.PageNumber,
                request.PageSize
            );
        }
    }
}
