using CentralHub.Application.Interfaces;
using CentralHub.Infrastructure.Data.DbContext;

namespace CentralHub.Infrastructure.Data
{
    // Implements IUnitOfWork using EF Core
    public class UnitOfWork : IUnitOfWork
    {
        private readonly CentralHubDbContext _dbContext;

        public UnitOfWork(CentralHubDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        // Commits changes tracked by the DbContext
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
