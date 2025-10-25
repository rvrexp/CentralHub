using CentralHub.Core.Domain.Aggregates.ClientAggregate;

namespace CentralHub.Application.Interfaces
{
    /// <summary>
    /// Defines the contract for data access operations related to the Client aggregate.
    /// </summary>
    public interface IClientRepository
    {
        /// <summary>
        /// Finds a client by their unique identifier, ensuring they belong to the correct tenant.
        /// </summary>
        /// <param name="clientId">The client's unique ID.</param>
        /// <param name="tenantId">The tenant's unique ID.</param>
        /// <returns>The Client aggregate including its Properties, or null if not found.</returns>
        Task<Client?> GetByIdAsync(Guid clientId, Guid tenantId);

        /// <summary>
        /// Adds a new client aggregate to be tracked by the unit of work.
        /// </summary>
        /// <param name="client">The client aggregate to add.</param>
        /// <remarks>This method typically adds the entity to the DbContext's change tracker. SaveChangesAsync is called separately.</remarks>
        Task AddAsync(Client client);

        // Note: Update/Delete operations often leverage the DbContext's change tracking
        // and are implicitly handled when SaveChangesAsync is called. Explicit methods
        // can be added if specific logic beyond simple tracking is needed.
    }
}
