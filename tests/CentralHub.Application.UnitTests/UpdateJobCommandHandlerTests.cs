using CentralHub.Application.Exceptions;
using CentralHub.Application.Features.Clients.DTOs;
using CentralHub.Application.Features.Jobs.Commands.UpdateJob;
using CentralHub.Application.Interfaces;
using CentralHub.Core.Domain.Aggregates.JobAggregate;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentralHub.Application.UnitTests
{
    [TestFixture]
    public class UpdateJobCommandHandlerTests
    {
        private Mock<IJobRepository> _mockJobRepository;
        private Mock<IClientRepository> _mockClientRepository;
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<ICurrentUserService> _mockCurrentUserService;
        private UpdateJobCommandHandler _handler;

        private Guid _testTenantId;
        private Guid _testClientId;
        private Guid _testPropertyId;
        private Job _testJob;
        private ClientDto _mockClientDto;

        [SetUp]
        public void Setup()
        {
            _mockJobRepository = new Mock<IJobRepository>();
            _mockClientRepository = new Mock<IClientRepository>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockCurrentUserService = new Mock<ICurrentUserService>();

            _testTenantId = Guid.NewGuid();
            _testClientId = Guid.NewGuid();
            _testPropertyId = Guid.NewGuid();

            _mockCurrentUserService.Setup(s => s.TenantId).Returns(_testTenantId);

            // Create a real Job object to be "found" by the repository
            _testJob = Job.Create(_testTenantId, _testClientId, _testPropertyId, DateTime.UtcNow.AddDays(1));

            _mockJobRepository
                .Setup(r => r.FindByIdAsync(_testJob.Id, _testTenantId))
                .ReturnsAsync(_testJob);

            // Create a mock Client DTO for validation
            var mockPropertyDto = new PropertyDto(_testPropertyId, "123 Main St", "City", "ST", "12345", null);
            _mockClientDto = new ClientDto(_testClientId, "Test Client", "test@test.com", null, new List<PropertyDto> { mockPropertyDto });

            _mockClientRepository
                .Setup(r => r.GetByIdAsync(_testClientId, _testTenantId))
                .ReturnsAsync(_mockClientDto);

            // Create the handler to test
            _handler = new UpdateJobCommandHandler(
                _mockJobRepository.Object,
                _mockClientRepository.Object,
                _mockUnitOfWork.Object,
                _mockCurrentUserService.Object
            );
        }

        [Test]
        public async Task Handle_Should_UpdateJob_And_SaveChanges_WhenValid()
        {
            // Arrange
            var command = new UpdateJobCommand
            {
                JobId = _testJob.Id,
                ClientId = _testClientId,
                PropertyId = _testPropertyId,
                Status = JobStatus.Completed,
                ScheduledStartTime = DateTime.UtcNow.AddDays(2),
                Notes = "Updated notes"
            };

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            // Check that the Job's methods were called (the job object itself is updated)
            Assert.That(_testJob.Status, Is.EqualTo(JobStatus.Completed));
            Assert.That(_testJob.Notes, Is.EqualTo("Updated notes"));

            // Check that the Unit of Work saved the changes
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(CancellationToken.None), Times.Once);
        }

        [Test]
        public void Handle_Should_ThrowNotFoundException_WhenJobDoesNotExist()
        {
            // Arrange
            _mockJobRepository
                .Setup(r => r.FindByIdAsync(It.IsAny<Guid>(), _testTenantId))
                .ReturnsAsync((Job?)null); // Job not found

            var command = new UpdateJobCommand { JobId = Guid.NewGuid() };

            // Act & Assert
            Assert.ThrowsAsync<NotFoundException>(async () =>
                await _handler.Handle(command, CancellationToken.None)
            );
        }

        [Test]
        public void Handle_Should_ThrowNotFoundException_WhenClientDoesNotExist()
        {
            // Arrange
            _mockClientRepository
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), _testTenantId))
                .ReturnsAsync((ClientDto?)null); // Client not found

            var command = new UpdateJobCommand { JobId = _testJob.Id, ClientId = Guid.NewGuid() };

            // Act & Assert
            Assert.ThrowsAsync<NotFoundException>(async () =>
                await _handler.Handle(command, CancellationToken.None)
            );
        }
    }
}
