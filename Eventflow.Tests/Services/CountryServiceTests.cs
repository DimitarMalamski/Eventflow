using Eventflow.Application.Services;
using Eventflow.Domain.Exceptions;
using Eventflow.Domain.Interfaces.Repositories;
using Eventflow.Domain.Models.Entities;
using Moq;

namespace Eventflow.Tests.Services
{
    [TestClass]
    public class CountryServiceTests
    {
        private Mock<ICountryRepository> _mockCountryRepository;
        private CountryService _countryService;

        [TestInitialize]
        public void Setup()
        {
            _mockCountryRepository = new Mock<ICountryRepository>();
            _countryService = new CountryService(_mockCountryRepository.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_ShouldThrowException_WhenRepositoryIsNull()
        {
            // Act
            var service = new CountryService(null!);
        }

        [TestMethod]
        public async Task GetCountriesByContinentIdAsync_ShouldReturnSortedList_WhenRepositoryReturnsUnsorted()
        {
            // Arrange
            var unsortedCountries = new List<Country>()
            {
                new Country { Id = 3, Name = "Zimbabwe" },
                new Country { Id = 1, Name = "Albania" },
                new Country { Id = 2, Name = "Brazil" }
            };

            _mockCountryRepository
                .Setup(repo => repo.GetAllCountriesByContinentIdAsync(1))
                .ReturnsAsync(unsortedCountries);

            // Act
            var result = await _countryService.GetCountriesByContinentIdAsync(1);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Count);
            Assert.AreEqual("Albania", result[0].Name);
            Assert.AreEqual("Brazil", result[1].Name);
            Assert.AreEqual("Zimbabwe", result[2].Name);
        }

        [TestMethod]
        public async Task GetCountriesByContinentIdAsync_ShouldReturnEmptyList_WhenRepositoryReturnsEmpty()
        {
            // Arrange
            _mockCountryRepository
                .Setup(repo => repo.GetAllCountriesByContinentIdAsync(99))
                .ReturnsAsync(new List<Country>());

            // Act
            var result = await _countryService.GetCountriesByContinentIdAsync(99);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod] // TODO
        [ExpectedException(typeof(CountryRetrievalException))]
        public async Task GetCountriesByContinentIdAsync_ShouldThrowException_WhenRepositoryThrows()
        {
            // Arrange
            _mockCountryRepository
                .Setup(repo => repo.GetAllCountriesByContinentIdAsync(1))
                .ThrowsAsync(new Exception("Fail!"));

            // Act
            await _countryService.GetCountriesByContinentIdAsync(1);
        }

        [DataTestMethod]
        [DataRow(-1)]
        [DataRow(-15)]
        [DataRow(0)]
        public async Task GetCountriesByContinentIdAsync_ShouldThrow_WhenContinentIdIsZeroOrNegative(int id)
        {
            // Act
            await Assert.ThrowsExceptionAsync<ArgumentOutOfRangeException>(() =>
                _countryService.GetCountriesByContinentIdAsync(id));

        }

        [TestMethod]
        public async Task GetCountriesByContinentIdAsync_ShouldIgnoreNullOrWhitespaceNames()
        {
            // Arrange
            var mixedCountries = new List<Country>
            {
                new Country { Id = 1, Name = "Germany" },
                new Country { Id = 2, Name = "" },
                new Country { Id = 3, Name = "  " },
                new Country { Id = 4, Name = "France" }
            };

            _mockCountryRepository
                .Setup(repo => repo.GetAllCountriesByContinentIdAsync(1))
                .ReturnsAsync(mixedCountries);

            // Act
            var result = await _countryService.GetCountriesByContinentIdAsync(1);

            // Assert
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("France", result[0].Name);
            Assert.AreEqual("Germany", result[1].Name);
        }
    }
}
