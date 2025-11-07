using CentralHub.Application.Exceptions;
using CentralHub.Application.Features.Jobs.DTOs;
using CentralHub.Application.Features.Jobs.Queries.GetJobById;
using CentralHub.Application.Interfaces;
using Moq;

namespace CentralHub.Application.UnitTests
{
    [TestFixture]
    public class GetJobByIdQueryHandlerTests
    {
        private Mock<IJobRepository> _mockJobRepository;
        private Mock<ICurrentUserService> _mockCurrentUserService;
        private GetJobByIdQueryHandler _handler;
        private Guid _testTenantId;

        [SetUp]
        public void Setup()
        {
            _mockJobRepository = new Mock<IJobRepository>();
            _mockCurrentUserService = new Mock<ICurrentUserService>();

            _testTenantId = Guid.NewGuid();
            _mockCurrentUserService.Setup(s => s.TenantId).Returns(_testTenantId);

            _handler = new GetJobByIdQueryHandler(
                _mockJobRepository.Object,
                _mockCurrentUserService.Object
            );
        }

        [Test]
        public async Task Handle_Should_ReturnJobDto_WhenJobExists()
        {
            // Arrange
            var jobId = Guid.NewGuid();
            var query = new GetJobByIdQuery(jobId);
            var mockJobDto = new JobDto { Id = jobId };

            _mockJobRepository
                .Setup(r => r.GetJobByIdAsync(jobId, _testTenantId))
                .ReturnsAsync(mockJobDto);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(jobId));
        }

        [Test]
        public void Handle_Should_ThrowNotFoundException_WhenJobDoesNotExist()
        {
            // Arrange
            var query = new GetJobByIdQuery(Guid.NewGuid());

            _mockJobRepository
                .Setup(r => r.GetJobByIdAsync(It.IsAny<Guid>(), _testTenantId))
                .ReturnsAsync((JobDto?)null); // Return null

            // Act & Assert
            Assert.ThrowsAsync<NotFoundException>(async () =>
                await _handler.Handle(query, CancellationToken.None)
            );
        }
    }
}
