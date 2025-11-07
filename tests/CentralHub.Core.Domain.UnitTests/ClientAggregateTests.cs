namespace CentralHub.Core.Domain.UnitTests
{
    // File: tests/CentralHub.Core.Domain.UnitTests/ClientAggregateTests.cs

    using CentralHub.Core.Domain.Aggregates.ClientAggregate;
    using CentralHub.Core.Domain.ValueObjects; // For Address

   
    [TestFixture] 
    public class ClientAggregateTests
    {
        // A helper method to create a valid client for testing
        private Client CreateTestClient()
        {
            return Client.Create(
                Guid.NewGuid(),
                "Test Client",
                "test@client.com"
            );
        }

        // A helper method to create a valid address for testing
        private Address CreateTestAddress()
        {
            return new Address("123 Main St", "Anytown", "CA", "12345");
        }

        [Test] 
        public void Create_Should_Succeed_WithValidParameters()
        {
            // Act
            var client = CreateTestClient();

            // Assert
            Assert.That(client, Is.Not.Null);
            Assert.That(client.Name, Is.EqualTo("Test Client"));
            Assert.That(client.Properties, Is.Not.Null);
            Assert.That(client.Properties.Count, Is.EqualTo(0));
        }

        [Test]
        public void AddProperty_Should_AddProperty_WhenAddressIsUnique()
        {
            // Arrange
            var client = CreateTestClient();
            var address = CreateTestAddress();

            // Act
            client.AddProperty(address, "Test notes");

            // Assert
            Assert.That(client.Properties.Count, Is.EqualTo(1));
            Assert.That(client.Properties.First().Address, Is.EqualTo(address));
            Assert.That(client.Properties.First().Notes, Is.EqualTo("Test notes"));
        }

        [Test]
        public void AddProperty_Should_ThrowException_WhenAddressIsDuplicate()
        {
            // Arrange
            var client = CreateTestClient();
            var address = CreateTestAddress();
            client.AddProperty(address, "First property"); // Add the address once

            // Act & Assert
            // NUnit's "Assert.Throws" is the standard way to check for exceptions
            Assert.Throws<InvalidOperationException>(() =>
            {
                client.AddProperty(address, "Duplicate property");
            });

            // Final check
            Assert.That(client.Properties.Count, Is.EqualTo(1));
        }
    }
}
