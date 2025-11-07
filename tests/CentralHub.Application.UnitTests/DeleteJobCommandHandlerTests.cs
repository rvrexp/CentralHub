using CentralHub.Application.Exceptions;
using CentralHub.Application.Features.Jobs.Commands.DeleteJob;
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
    public class DeleteJobCommandHandlerTests
    {
        private Mock<IJobRepository> _mockJobRepository;
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<ICurrentUserService> _mockCurrentUserService;
        private DeleteJobCommandHandler _handler;
        private Guid _testTenantId;
        private Job _testJob;

        [SetUp]
        public void Setup()
        {
            _mockJobRepository = new Mock<IJobRepository>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockCurrentUserService = new Mock<ICurrentUserService>();

            _testTenantId = Guid.NewGuid();
            _mockCurrentUserService.Setup(s => s.TenantId).Returns(_testTenantId);

            _testJob = Job.Create(_testTenantId, Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow);

            _mockJobRepository
                .Setup(r => r.FindByIdAsync(It.IsAny<Guid>(), _testTenantId))
                .ReturnsAsync(_testJob); // Return the test job by default

            _handler = new DeleteJobCommandHandler(
                _mockJobRepository.Object,
                _mockUnitOfWork.Object,
                _mockCurrentUserService.Object
            );
        }

        [Test]
        public async Task Handle_Should_CallDelete_And_SaveChanges_WhenJobExists()
        {
            // Arrange
            var command = new DeleteJobCommand(_testJob.Id);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _mockJobRepository.Verify(r => r.Delete(_testJob), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(CancellationToken.None), Times.Once);
        }

        [Test]
        public void Handle_Should_ThrowNotFoundException_WhenJobDoesNotExist()
        {
            // Arrange
            _mockJobRepository
                .Setup(r => r.FindByIdAsync(It.IsAny<Guid>(), _testTenantId))
                .ReturnsAsync((Job?)null); // Job not found

            var command = new DeleteJobCommand(Guid.NewGuid());

            // Act & Assert
            Assert.ThrowsAsync<NotFoundException>(async () =>
                await _handler.Handle(command, CancellationToken.None)
            );
        }
    }
}
