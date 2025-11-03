using CentralHub.Application.Interfaces;
using CentralHub.Core.Domain.Aggregates.ClientAggregate;
using CentralHub.Infrastructure.Data.DbContext;
using Microsoft.EntityFrameworkCore;

namespace CentralHub.Infrastructure.Data.Repositories
{
    public class ClientRepository : IClientRepository
    {
        private readonly CentralHubDbContext _dbContext;

        public ClientRepository(CentralHubDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task AddAsync(Client client)
        {
            await _dbContext.Clients.AddAsync(client);
        }

        public async Task<Client?> GetByIdAsync(Guid clientId, Guid tenantId)
        {
            return await _dbContext.Clients
                .Include(c => c.Properties) // Eager load Properties
                .FirstOrDefaultAsync(c => c.Id == clientId && c.TenantId == tenantId); // Tenant check
        }
    }
}
