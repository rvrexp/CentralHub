using CentralHub.Application.Common.Models;
using CentralHub.Application.Features.Clients.DTOs;
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

        public async Task<PagedResult<ClientSummaryDto>> GetPagedListAsync(Guid tenantId, int pageNumber, int pageSize)
        {
            // Create the base query, filtered by TenantId
            var query = _dbContext.Clients
                .Where(c => c.TenantId == tenantId)
                .OrderBy(c => c.Name); // Always order for stable pagination

            // Get the total count *before* paginating
            var totalCount = await query.CountAsync();

            // Apply pagination and project to the DTO
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(client => new ClientSummaryDto(
                    client.Id,
                    client.Name,
                    client.Email,
                    client.PhoneNumber
                ))
                .ToListAsync();

            // Create and return the paged result
            return new PagedResult<ClientSummaryDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
    }
}
