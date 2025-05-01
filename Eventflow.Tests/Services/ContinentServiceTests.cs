using Eventflow.Application.Services;
using Eventflow.Domain.Exceptions;
using Eventflow.Domain.Interfaces.Repositories;
using Eventflow.Domain.Models.Models;
using Moq;

namespace Eventflow.Tests.Services
{
    [TestClass]
    public class ContinentServiceTests
    {
        private Mock<IContinentRepository> _mockContinentRepository;
        private ContinentService _continentService;

        [TestInitialize]
        public void Setup()
        {
            _mockContinentRepository = new Mock<IContinentRepository>();
            _continentService = new ContinentService(_mockContinentRepository.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_ShouldThrowException_WhenRepositoryIsNull()
        {
            // Act
            var service = new ContinentService(null!);
        }

        [TestMethod]
        public async Task OrderContinentByNameAsync_ShouldReturnSortedList_WhenRepositoryReturnsUnsorted()
        {
            // Arrange
            var unsortedContinents = new List<Continent>()
            {
                new Continent { Id = 2, Name = "Europe" },
                new Continent { Id = 1, Name = "Africa" },
                new Continent { Id = 3, Name = "Asia" }
            };

            _mockContinentRepository
                .Setup(repo => repo.GetAllContinentsAsync())
                .ReturnsAsync(unsortedContinents);

            // Act
            var result = await _continentService.OrderContinentByNameAsync();

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Count);
            Assert.AreEqual("Africa", result[0].Name);
            Assert.AreEqual("Asia", result[1].Name);
            Assert.AreEqual("Europe", result[2].Name);
        }

        [TestMethod]
        public async Task OrderContinentByNameAsync_ShouldReturnEmptyList_WhenRepositoryReturnsEmpty()
        {
            // Arrange
            _mockContinentRepository
                .Setup(repo => repo.GetAllContinentsAsync())
                .ReturnsAsync(new List<Continent>());

            // Act
            var result = await _continentService.OrderContinentByNameAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(ContinentRetrievalException))]
        public async Task OrderContinentByNameAsync_ShouldThrowException_WhenRepositoryThrows()
        {
            // Arrange
            _mockContinentRepository
                .Setup(repo => repo.GetAllContinentsAsync())
                .ThrowsAsync(new Exception("Fail!"));

            // Act
            var result = await _continentService.OrderContinentByNameAsync();
        }
    }
}
