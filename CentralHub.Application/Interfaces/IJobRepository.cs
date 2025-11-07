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
        // Task<JobDto?> GetByIdAsync(Guid jobId, Guid tenantId);
        // Task<PagedResult<JobSummaryDto>> GetJobsForDateRangeAsync(...);
    }
}
