using CentralHub.Application.Exceptions;
using CentralHub.Application.Features.Clients.Commands.UpdateClient;
using CentralHub.Application.Interfaces;
using CentralHub.Core.Domain.Aggregates.ClientAggregate;
using Moq;

namespace CentralHub.Application.UnitTests
{
    [TestFixture]
    public class UpdateClientCommandHandlerTests
    {
        private Mock<IClientRepository> _mockClientRepository;
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<ICurrentUserService> _mockCurrentUserService;
        private UpdateClientCommandHandler _handler;
        private Guid _testTenantId;
        private Client _testClient; 

        [SetUp]
        public void Setup()
        {
            // 1. Arrange (Mocking)
            _mockClientRepository = new Mock<IClientRepository>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockCurrentUserService = new Mock<ICurrentUserService>();

            _testTenantId = Guid.NewGuid();
            _mockCurrentUserService.Setup(s => s.TenantId).Returns(_testTenantId);

            // Create a real client object to return from the repository
            _testClient = Client.Create(_testTenantId, "Original Name", "original@email.com");

            // Set up the repository's FindByIdAsync method
            _mockClientRepository
                .Setup(r => r.FindByIdAsync(It.IsAny<Guid>(), _testTenantId))
                .ReturnsAsync(_testClient); // By default, return the test client

            // Create the handler to test
            _handler = new UpdateClientCommandHandler(
                _mockClientRepository.Object,
                _mockUnitOfWork.Object,
                _mockCurrentUserService.Object
            );
        }

        [Test]
        public async Task Handle_Should_CallUpdateInfo_And_SaveChangesAsync_WhenClientExists()
        {
            // 2. Arrange (Input)
            var command = new UpdateClientCommand
            {
                Id = _testClient.Id,
                Name = "Updated Name",
                Email = "updated@email.com",
                PhoneNumber = "555-1234"
            };

            // 3. Act
            await _handler.Handle(command, CancellationToken.None);

            // 4. Assert
            // Check that the client's UpdateInfo method was called
            Assert.That(_testClient.Name, Is.EqualTo(command.Name));
            Assert.That(_testClient.Email, Is.EqualTo(command.Email));

            // Check that the Unit of Work saved the changes
            _mockUnitOfWork.Verify(
                u => u.SaveChangesAsync(CancellationToken.None),
                Times.Once
            );
        }

        [Test]
        public void Handle_Should_ThrowNotFoundException_WhenClientDoesNotExist()
        {
            // 2. Arrange (Input & Mock Setup)
            // Override the default setup for this specific test
            _mockClientRepository
                .Setup(r => r.FindByIdAsync(It.IsAny<Guid>(), _testTenantId))
                .ReturnsAsync((Client?)null); // Return null

            var command = new UpdateClientCommand
            {
                Id = Guid.NewGuid(), // A random, non-existent ID
                Name = "Updated Name",
                Email = "updated@email.com"
            };

            // 3. Act & 4. Assert
            Assert.ThrowsAsync<NotFoundException>(async () =>
                await _handler.Handle(command, CancellationToken.None)
            );
        }
    }
}
