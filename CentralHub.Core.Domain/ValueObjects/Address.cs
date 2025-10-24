using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentralHub.Core.Domain.ValueObjects
{
    // Value Object: Represents an address, defined by its attributes. Immutable.
    public record Address
    {
        public string Street { get; }
        public string City { get; }
        public string State { get; }
        public string ZipCode { get; }

        private Address() { }

        public Address(string street, string city, string state, string zipCode)
        {
            // Basic validation - ensure no null or empty strings
            if (string.IsNullOrWhiteSpace(street)) throw new ArgumentNullException(nameof(street));
            if (string.IsNullOrWhiteSpace(city)) throw new ArgumentNullException(nameof(city));
            if (string.IsNullOrWhiteSpace(state)) throw new ArgumentNullException(nameof(state));
            if (string.IsNullOrWhiteSpace(zipCode)) throw new ArgumentNullException(nameof(zipCode));
            // Add more specific validation if needed (e.g., zip code format)

            Street = street;
            City = city;
            State = state;
            ZipCode = zipCode;
        }
    }
}
