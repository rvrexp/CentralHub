using CentralHub.Application.Common.Models;
using CentralHub.Application.Features.Jobs.DTOs;
using CentralHub.Application.Interfaces;
using CentralHub.Core.Domain.Aggregates.JobAggregate;
using CentralHub.Infrastructure.Data.DbContext;
using Microsoft.EntityFrameworkCore;

namespace CentralHub.Infrastructure.Data.Repositories
{
    public class JobRepository : IJobRepository
    {
        private readonly CentralHubDbContext _dbContext;

        public JobRepository(CentralHubDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(Job job)
        {
            await _dbContext.Jobs.AddAsync(job);
        }

        public async Task<Job?> FindByIdAsync(Guid jobId, Guid tenantId)
        {
            // Find the job, ensuring it's for the correct tenant
            return await _dbContext.Jobs
                .FirstOrDefaultAsync(j => j.Id == jobId && j.TenantId == tenantId);
        }

        public void Delete(Job job)
        {
            _dbContext.Jobs.Remove(job);
        }

        // --- Query Implementations ---

        public async Task<JobDto?> GetJobByIdAsync(Guid jobId, Guid tenantId)
        {
            return await _dbContext.Jobs
                .AsNoTracking() // Read-only query
                .Where(j => j.Id == jobId && j.TenantId == tenantId)
                .Select(j => new JobDto // Project directly to DTO
                {
                    Id = j.Id,
                    ClientId = j.ClientId,
                    PropertyId = j.PropertyId,
                    Status = j.Status,
                    ScheduledStartTime = j.ScheduledStartTime,
                    ScheduledEndTime = j.ScheduledEndTime,
                    Notes = j.Notes
                })
                .FirstOrDefaultAsync();
        }

        public async Task<PagedResult<JobSummaryDto>> GetJobsForDateRangeAsync(Guid tenantId, DateTime startDate, DateTime endDate, int pageNumber, int pageSize)
        {
            var query = _dbContext.Jobs
                .AsNoTracking()
                .Where(j => j.TenantId == tenantId &&
                            j.ScheduledStartTime >= startDate &&
                            j.ScheduledStartTime <= endDate)
                .OrderBy(j => j.ScheduledStartTime);

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(j => new JobSummaryDto(
                    j.Id,
                    j.ClientId,
                    j.PropertyId,
                    j.Status,
                    j.ScheduledStartTime,
                    j.ScheduledEndTime
                ))
                .ToListAsync();

            return new PagedResult<JobSummaryDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
    }
}
