using CentralHub.Application.Exceptions;
using CentralHub.Application.Features.Clients.DTOs;
using CentralHub.Application.Features.Jobs.Commands.CreateJob;
using CentralHub.Application.Interfaces;
using CentralHub.Core.Domain.Aggregates.JobAggregate;
using Moq;

namespace CentralHub.Application.UnitTests
{
    [TestFixture]
    public class CreateJobCommandHandlerTests
    {
        private Mock<IJobRepository> _mockJobRepository;
        private Mock<IClientRepository> _mockClientRepository;
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<ICurrentUserService> _mockCurrentUserService;
        private CreateJobCommandHandler _handler;

        private Guid _testTenantId;
        private Guid _testClientId;
        private Guid _testPropertyId;
        private ClientDto _mockClientDto;

        [SetUp]
        public void Setup()
        {
            // 1. Arrange (Mocking)
            _mockJobRepository = new Mock<IJobRepository>();
            _mockClientRepository = new Mock<IClientRepository>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockCurrentUserService = new Mock<ICurrentUserService>();

            // Setup mock IDs
            _testTenantId = Guid.NewGuid();
            _testClientId = Guid.NewGuid();
            _testPropertyId = Guid.NewGuid();

            // Setup mock user
            _mockCurrentUserService.Setup(s => s.TenantId).Returns(_testTenantId);

            // Setup a mock client DTO that includes the valid property
            var mockPropertyDto = new PropertyDto(_testPropertyId, "123 Main St", "City", "ST", "12345", null);
            _mockClientDto = new ClientDto(_testClientId, "Test Client", "test@test.com", null, new List<PropertyDto> { mockPropertyDto });

            // Setup the client repository to return the mock client by default
            _mockClientRepository
                .Setup(r => r.GetByIdAsync(_testClientId, _testTenantId))
                .ReturnsAsync(_mockClientDto);

            // Create the handler to test
            _handler = new CreateJobCommandHandler(
                _mockJobRepository.Object,
                _mockClientRepository.Object,
                _mockUnitOfWork.Object,
                _mockCurrentUserService.Object
            );
        }

        [Test]
        public async Task Handle_Should_CreateJob_WhenClientAndPropertyAreValid()
        {
            // 2. Arrange (Input)
            var command = new CreateJobCommand
            {
                ClientId = _testClientId,
                PropertyId = _testPropertyId,
                ScheduledStartTime = DateTime.UtcNow.AddDays(1)
            };

            // 3. Act
            var jobId = await _handler.Handle(command, CancellationToken.None);

            // 4. Assert
            // Verify a job was added to the repository
            _mockJobRepository.Verify(
                r => r.AddAsync(It.Is<Job>(j =>
                    j.TenantId == _testTenantId &&
                    j.ClientId == _testClientId)),
                Times.Once
            );

            // Verify the transaction was saved
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(CancellationToken.None), Times.Once);
            Assert.That(jobId, Is.Not.EqualTo(Guid.Empty));
        }

        [Test]
        public void Handle_Should_ThrowNotFoundException_WhenClientDoesNotExist()
        {
            // 2. Arrange (Input & Mock Override)
            // Setup the repository to return null for this test
            _mockClientRepository
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), _testTenantId))
                .ReturnsAsync((ClientDto?)null);

            var command = new CreateJobCommand { ClientId = Guid.NewGuid(), PropertyId = _testPropertyId };

            // 3. Act & 4. Assert
            Assert.ThrowsAsync<NotFoundException>(async () =>
                await _handler.Handle(command, CancellationToken.None)
            );
        }

        [Test]
        public void Handle_Should_ThrowNotFoundException_WhenPropertyDoesNotExistOnClient()
        {
            // 2. Arrange (Input)
            // Use a valid ClientId, but a non-existent PropertyId
            var nonExistentPropertyId = Guid.NewGuid();
            var command = new CreateJobCommand { ClientId = _testClientId, PropertyId = nonExistentPropertyId };

            // 3. Act & 4. Assert
            // The handler will find the client, but not the property in its list
            Assert.ThrowsAsync<NotFoundException>(async () =>
                await _handler.Handle(command, CancellationToken.None)
            );
        }
    }
}
