using CentralHub.Application.Features.Clients.Commands.CreateClient;
using CentralHub.Application.Interfaces;
using CentralHub.Core.Domain.Aggregates.ClientAggregate;
using Moq;

namespace CentralHub.Application.UnitTests
{
    [TestFixture]
    public class CreateClientCommandHandlerTests
    {
        private Mock<IClientRepository> _mockClientRepository;
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<ICurrentUserService> _mockCurrentUserService;
        private CreateClientCommandHandler _handler;

        [SetUp] // This [SetUp] method runs before every single test
        public void Setup()
        {
            // 1. Arrange (Mocking)
            // Create "fake" versions of our dependencies
            _mockClientRepository = new Mock<IClientRepository>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockCurrentUserService = new Mock<ICurrentUserService>();

            // Set up the mock user service to return a consistent test TenantId
            _mockCurrentUserService.Setup(s => s.TenantId)
                .Returns(Guid.NewGuid());

            // Create the actual handler we want to test, injecting the fakes
            _handler = new CreateClientCommandHandler(
                _mockClientRepository.Object,
                _mockUnitOfWork.Object,
                _mockCurrentUserService.Object
            );
        }

        [Test]
        public async Task Handle_Should_CallAddOnRepository_And_SaveChangesOnUnitOfWork_WhenSuccessful()
        {
            // 2. Arrange (Input)
            // Create the command we are going to send
            var command = new CreateClientCommand("Test Client", "test@test.com", "1234567890");

            // 3. Act
            // Call the method we are testing
            var clientId = await _handler.Handle(command, CancellationToken.None);

            // 4. Assert
            // Verify that the methods we expected to be called, were called.

            // Verify that AddAsync was called on the repository exactly ONE time
            _mockClientRepository.Verify(
                r => r.AddAsync(It.Is<Client>(c => c.Name == command.Name)), // Check that the client has the right name
                Times.Once
            );

            // Verify that SaveChangesAsync was called on the unit of work exactly ONE time
            _mockUnitOfWork.Verify(
                u => u.SaveChangesAsync(CancellationToken.None),
                Times.Once
            );

            // Assert that we got a new Guid back
            Assert.That(clientId, Is.Not.EqualTo(Guid.Empty));
        }
    }
}
