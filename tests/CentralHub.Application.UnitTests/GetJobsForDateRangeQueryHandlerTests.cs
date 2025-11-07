using CentralHub.Application.Common.Models;
using CentralHub.Application.Features.Jobs.DTOs;
using CentralHub.Application.Features.Jobs.Queries.GetJobsForDateRange;
using CentralHub.Application.Interfaces;
using Moq;

namespace CentralHub.Application.UnitTests
{
    [TestFixture]
    public class GetJobsForDateRangeQueryHandlerTests
    {
        private Mock<IJobRepository> _mockJobRepository;
        private Mock<ICurrentUserService> _mockCurrentUserService;
        private GetJobsForDateRangeQueryHandler _handler;
        private Guid _testTenantId;

        [SetUp]
        public void Setup()
        {
            _mockJobRepository = new Mock<IJobRepository>();
            _mockCurrentUserService = new Mock<ICurrentUserService>();
            _testTenantId = Guid.NewGuid();

            _mockCurrentUserService.Setup(s => s.TenantId).Returns(_testTenantId);

            _handler = new GetJobsForDateRangeQueryHandler(
                _mockJobRepository.Object,
                _mockCurrentUserService.Object
            );
        }

        [Test]
        public async Task Handle_Should_CallRepository_WithCorrectParameters()
        {
            // Arrange
            var startDate = DateTime.UtcNow.AddDays(-1);
            var endDate = DateTime.UtcNow.AddDays(1);
            var pageNumber = 1;
            var pageSize = 10;

            var query = new GetJobsForDateRangeQuery
            {
                StartDate = startDate,
                EndDate = endDate,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var mockResult = new PagedResult<JobSummaryDto>(); // Just need a dummy result

            _mockJobRepository
                .Setup(r => r.GetJobsForDateRangeAsync(_testTenantId, startDate, endDate, pageNumber, pageSize))
                .ReturnsAsync(mockResult);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            // We just care that the repository was called with the exact parameters.
            _mockJobRepository.Verify(
                r => r.GetJobsForDateRangeAsync(_testTenantId, startDate, endDate, pageNumber, pageSize),
                Times.Once
            );

            Assert.That(result, Is.EqualTo(mockResult));
        }
    }
}
