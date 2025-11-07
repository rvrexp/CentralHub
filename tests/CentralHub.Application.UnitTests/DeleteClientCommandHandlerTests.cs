using CentralHub.Application.Exceptions;
using CentralHub.Application.Features.Clients.Commands.DeleteClient;
using CentralHub.Application.Interfaces;
using CentralHub.Core.Domain.Aggregates.ClientAggregate;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentralHub.Application.UnitTests
{
    [TestFixture]
    public class DeleteClientCommandHandlerTests
    {
        private Mock<IClientRepository> _mockClientRepository;
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<ICurrentUserService> _mockCurrentUserService;
        private DeleteClientCommandHandler _handler;
        private Guid _testTenantId;
        private Client _testClient; // A real client object to "delete"

        [SetUp]
        public void Setup()
        {
            // 1. Arrange (Mocking)
            _mockClientRepository = new Mock<IClientRepository>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockCurrentUserService = new Mock<ICurrentUserService>();

            _testTenantId = Guid.NewGuid();
            _mockCurrentUserService.Setup(s => s.TenantId).Returns(_testTenantId);

            _testClient = Client.Create(_testTenantId, "Test Client", "test@test.com");

            // Set up the repository's FindByIdAsync method
            _mockClientRepository
                .Setup(r => r.FindByIdAsync(It.IsAny<Guid>(), _testTenantId))
                .ReturnsAsync(_testClient); // By default, return the test client

            // Create the handler to test
            _handler = new DeleteClientCommandHandler(
                _mockClientRepository.Object,
                _mockUnitOfWork.Object,
                _mockCurrentUserService.Object
            );
        }

        [Test]
        public async Task Handle_Should_CallDelete_And_SaveChangesAsync_WhenClientExists()
        {
            // 2. Arrange (Input)
            var command = new DeleteClientCommand(_testClient.Id);

            // 3. Act
            await _handler.Handle(command, CancellationToken.None);

            // 4. Assert
            // Check that the repository's Delete method was called with the correct client
            _mockClientRepository.Verify(
                r => r.Delete(_testClient),
                Times.Once
            );

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

            var command = new DeleteClientCommand(Guid.NewGuid()); // A random, non-existent ID

            // 3. Act & 4. Assert
            Assert.ThrowsAsync<NotFoundException>(async () =>
                await _handler.Handle(command, CancellationToken.None)
            );
        }
    }
}
