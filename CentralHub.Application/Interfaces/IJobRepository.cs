using CentralHub.Application.Common.Models;
using CentralHub.Application.Features.Jobs.DTOs;
using CentralHub.Core.Domain.Aggregates.JobAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentralHub.Application.Interfaces
{
    public interface IJobRepository
    {
        // --- Command Methods ---
        Task AddAsync(Job job);
        Task<Job?> FindByIdAsync(Guid jobId, Guid tenantId);
        void Delete(Job job);

        // --- Query Methods ---
        Task<JobDto?> GetJobByIdAsync(Guid jobId, Guid tenantId);
        Task<PagedResult<JobSummaryDto>> GetJobsForDateRangeAsync(Guid tenantId, DateTime startDate, DateTime endDate, int pageNumber, int pageSize);
    }
}
