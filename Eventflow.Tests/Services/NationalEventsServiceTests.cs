using Eventflow.Application.Services;
using Eventflow.Domain.Interfaces.Repositories;
using Eventflow.Domain.Models.Entities;
using Moq;

namespace Eventflow.Tests.Services {

   [TestClass]
   public class NationalEventsServiceTests {
         private Mock<INationalEventRepository> _mockNationalEventRepository;
         private Mock<ICountryRepository> _mockCountryRepository;
         // private Mock<IContinentRepository> _mockContinentRepository;
         // private Mock<IHttpClientFactory> _mockHttpClientFactory;
         private NationalEventsService _service;

         [TestInitialize]
         public void Setup()
         {
               _mockNationalEventRepository = new Mock<INationalEventRepository>();
               _mockCountryRepository = new Mock<ICountryRepository>();
               // _mockContinentRepository = new Mock<IContinentRepository>();
               // _mockHttpClientFactory = new Mock<IHttpClientFactory>();

               // _mockHttpClientFactory.Setup(f => f.CreateClient(It.IsAny<string>()))
               //    .Returns(new HttpClient());

               _service = new NationalEventsService(
                  _mockNationalEventRepository.Object,
                  _mockCountryRepository.Object
                  // _mockContinentRepository.Object,
                  // _mockHttpClientFactory.Object);
               );
         }

         [TestMethod]
         [ExpectedException(typeof(ArgumentNullException))]
         public void Constructor_ShouldThrowException_WhenNationalEventRepositoryIsNull()
         {
            // Arrange
            var mockNationalEventRepository = new Mock<INationalEventRepository>();

            // Act
            var service = new NationalEventsService(mockNationalEventRepository.Object, null!);
         }

         [TestMethod]
         [ExpectedException(typeof(ArgumentNullException))]
         public void Constructor_ShouldThrowException_WhenCountryRepositoryIsNull()
         {
            // Arrange
            var mockCountryRepository = new Mock<ICountryRepository>();

            // Act
            var service = new NationalEventsService(null!, mockCountryRepository.Object);
         }

         [TestMethod]
         public async Task GetNationalHolidaysForCountryAsync_ShouldReturnMappedDtos() {
            // Arrange
            int countryId = 1, year = 2025, month = 5;

            var sampleEvents = new List<NationalEvent>
            {
               new NationalEvent { Title = "Test Day", Description = "Test Desc", Date = new DateTime(2025, 5, 5), CountryId = countryId }
            };

            var country = new Country { 
               Id = countryId,
               Name = "Test"
            };

            _mockNationalEventRepository.Setup(repo => repo.GetNationalHolidaysForCountryAsync(countryId, year, month))
               .ReturnsAsync(sampleEvents);
            _mockCountryRepository.Setup(repo => repo.GetCountryByIdAsync(countryId))
               .ReturnsAsync(country);

            // Act
            var result = await _service.GetNationalHolidaysForCountryAsync(countryId, year, month);

            // Assert 
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Test Day", result[0].Title);
            Assert.AreEqual("Test Desc", result[0].Description);
            Assert.AreEqual("Test", result[0].CountryName);   
         }

         [TestMethod]
         public async Task GetNationalHolidaysForCountryAsync_ShouldUseFallbackIfCountryIsNull() {
            // Arrange
            int countryId = 99, year = 2025, month = 6;

            var sampleEvents = new List<NationalEvent>
            {
                new NationalEvent { Title = "Unknown Day", Description = null, Date = new DateTime(2025, 6, 6), CountryId = countryId }
            };

            _mockNationalEventRepository.Setup(repo => repo.GetNationalHolidaysForCountryAsync(countryId, year, month))
                .ReturnsAsync(sampleEvents);
            _mockCountryRepository.Setup(repo => repo.GetCountryByIdAsync(countryId))
                .ReturnsAsync((Country?)null);

            // Act
            var result = await _service.GetNationalHolidaysForCountryAsync(countryId, year, month);

            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("No description.", result[0].Description);
            Assert.AreEqual("Unknown", result[0].CountryName);
         }

         [TestMethod]
         public async Task GetNationalHolidaysForCountryAsync_ShouldReturnEmptyList_WhenRepositoryReturnsNull() {
            // Arrange
            int countryId = 123, year = 2025, month = 12;

            _mockNationalEventRepository.Setup(repo => repo.GetNationalHolidaysForCountryAsync(countryId, year, month))
               .ReturnsAsync((List<NationalEvent>?)null);
            _mockCountryRepository.Setup(repo => repo.GetCountryByIdAsync(countryId))
               .ReturnsAsync(new Country { Id = countryId, Name = "Placeholder" });

            // Act
            var result = await _service.GetNationalHolidaysForCountryAsync(countryId, year, month);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
         }
   }
}