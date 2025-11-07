using CentralHub.Application.Exceptions;
using CentralHub.Application.Features.Clients.DTOs;
using CentralHub.Application.Features.Clients.Queries.GetClientById;
using CentralHub.Application.Interfaces;
using Moq;

namespace CentralHub.Application.UnitTests
{
    [TestFixture]
    public class GetClientByIdQueryHandlerTests
    {
        private Mock<IClientRepository> _mockClientRepository;
        private Mock<ICurrentUserService> _mockCurrentUserService;
        private GetClientByIdQueryHandler _handler;
        private Guid _testTenantId;

        [SetUp]
        public void Setup()
        {
            // 1. Arrange (Mocking)
            _mockClientRepository = new Mock<IClientRepository>();
            _mockCurrentUserService = new Mock<ICurrentUserService>();

            _testTenantId = Guid.NewGuid();
            _mockCurrentUserService.Setup(s => s.TenantId).Returns(_testTenantId);

            // Create the handler to test
            _handler = new GetClientByIdQueryHandler(
                _mockClientRepository.Object,
                _mockCurrentUserService.Object
            );
        }

        [Test]
        public async Task Handle_Should_ReturnClientDto_WhenClientExists()
        {
            // 2. Arrange (Input & Mock Setup)
            var clientId = Guid.NewGuid();
            var query = new GetClientByIdQuery(clientId);

            var mockClientDto = new ClientDto(clientId, "Test", "test@test.com", null, new List<PropertyDto>());

            // Set up the repository mock to return our fake client DTO
            _mockClientRepository
                .Setup(r => r.GetByIdAsync(clientId, _testTenantId))
                .ReturnsAsync(mockClientDto);

            // 3. Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // 4. Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(clientId));
            Assert.That(result.Name, Is.EqualTo("Test"));
        }

        [Test]
        public void Handle_Should_ThrowNotFoundException_WhenClientDoesNotExist()
        {
            // 2. Arrange (Input & Mock Setup)
            var clientId = Guid.NewGuid();
            var query = new GetClientByIdQuery(clientId);

            // Set up the repository mock to return null
            _mockClientRepository
                .Setup(r => r.GetByIdAsync(clientId, _testTenantId))
                .ReturnsAsync((ClientDto?)null); // Return a null DTO

            // 3. Act & 4. Assert
            // We verify that the handler correctly throws a NotFoundException
            Assert.ThrowsAsync<NotFoundException>(async () =>
                await _handler.Handle(query, CancellationToken.None)
            );
        }
    }
}
