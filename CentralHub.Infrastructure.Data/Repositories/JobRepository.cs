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

    }
}
