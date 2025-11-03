using CentralHub.Core.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentralHub.Core.Domain.Aggregates.ClientAggregate
{
    // Entity: A service location for a Client. Part of the Client Aggregate.
    public class Property
    {
        public Guid Id { get; private set; }
        public Address Address { get; private set; } 
        public string? Notes { get; private set; }

        private Property() { }

        // Internal factory method to create a valid Property.
        // The Client Aggregate Root will be responsible for creating properties.
        internal static Property Create(Address address, string? notes = null)
        {
            return new Property
            {
                Id = Guid.NewGuid(), // sequential GUID later
                Address = address,
                Notes = notes
            };
        }

        public void UpdateNotes(string? newNotes)
        {
            Notes = newNotes;
        }
    }
}
