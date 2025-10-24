using CentralHub.Core.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentralHub.Core.Domain.Aggregates.ClientAggregate
{
    // Aggregate Root: Represents a customer. Manages its associated Properties.
    public class Client
    {
        public Guid Id { get; private set; } // sequential GUID later
        public Guid TenantId { get; private set; } // Identifies which business owns this client
        public string Name { get; private set; }
        public string Email { get; private set; }
        public string? PhoneNumber { get; private set; }

        // Encapsulated collection of Properties
        private readonly List<Property> _properties = new();
        public IReadOnlyCollection<Property> Properties => _properties.AsReadOnly();

        // Private parameterless constructor for EF Core
        private Client() { }

        // Public factory method to create a valid Client, enforcing business rules (invariants)
        public static Client Create(Guid tenantId, string name, string email, string? phoneNumber = null)
        {
            // Enforce invariants - rules that must always be true for a valid Client
            if (tenantId == Guid.Empty) throw new ArgumentException("TenantId cannot be empty.", nameof(tenantId));
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Client name cannot be empty.", nameof(name));
            if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("Client email cannot be empty.", nameof(email));

            return new Client
            {
                Id = Guid.NewGuid(), // Consider sequential GUID here later
                TenantId = tenantId,
                Name = name,
                Email = email,
                PhoneNumber = phoneNumber
            };
        }

        // Method to update client information, enforcing rules
        public void UpdateInfo(string name, string email, string? phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Client name cannot be empty.", nameof(name));
            if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("Client email cannot be empty.", nameof(email));

            Name = name;
            Email = email;
            PhoneNumber = phoneNumber;
        }

        // Method to add a property, managed by the Aggregate Root
        public void AddProperty(Address address, string? notes = null)
        {
            // Rule: Prevent adding duplicate property addresses for the same client
            if (_properties.Any(p => p.Address == address))
            {
                throw new InvalidOperationException($"Client already has a property at address: {address.Street}");
            }

            // Create the property using its internal factory method
            var newProperty = Property.Create(address, notes);
            _properties.Add(newProperty);
        }

        // Method for removing a property
        // public void RemoveProperty(Guid propertyId) {  }
    }
}
