using Eventflow.Application.Services;
using Eventflow.Domain.Exceptions;
using Eventflow.Domain.Interfaces.Repositories;
using Eventflow.Domain.Models.Common;
using Moq;

namespace Eventflow.Tests.Services
{
    [TestClass]
    public class StatusServiceTests
    {
        private Mock<IStatusRepository> _mockStatusRepository;
        private StatusService _statusService;

        [TestInitialize]
        public void Setup()
        {
            _mockStatusRepository = new Mock<IStatusRepository>();
            _statusService = new StatusService(_mockStatusRepository.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_ShouldThrowException_WhenRepositoryIsNull()
        {
            // Act 
            var service = new StatusService(null!);
        }

        [TestMethod]
        public async Task GetAllStatusOptionsAsync_ShouldReturnList_WhenRepositoryReturnsOptions()
        {
            // Arrange
            var expectedStatuses = new List<DropdownOption>()
            {
                new DropdownOption { Id = "1", Name = "Accepted" },
                new DropdownOption { Id = "2", Name = "Declined" },
                new DropdownOption { Id = "3", Name = "Pending" },
            };

            _mockStatusRepository
                .Setup(repo => repo.GetAllStatusOptionsAsync())
                .ReturnsAsync(expectedStatuses);

            // Act
            var result = await _statusService.GetAllStatusOptionsAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Count);
            Assert.AreEqual("Accepted", result[0].Name);
            Assert.AreEqual("1", result[0].Id);
        }

        [TestMethod]
        public async Task GetAllStatusOptionsAsync_ShouldReturnEmptyList_WhenRepositoryReturnsEmpty()
        {
            // Arrange
            _mockStatusRepository
                .Setup(repo => repo.GetAllStatusOptionsAsync())
                .ReturnsAsync(new List<DropdownOption>());

            // Act
            var result = await _statusService.GetAllStatusOptionsAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(StatusRetrievalException))]
        public async Task GetAllStatusOptionsAsync_ShouldThrowException_WhenRepositoryThrows()
        {
            // Arrange
            _mockStatusRepository
                .Setup(repo => repo.GetAllStatusOptionsAsync())
                .ThrowsAsync(new StatusRetrievalException("Fail"));

            // Act
            await _statusService.GetAllStatusOptionsAsync();
        }
    }
}
