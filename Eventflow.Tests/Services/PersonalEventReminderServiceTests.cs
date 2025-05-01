using Eventflow.Application.Services;
using Eventflow.Domain.Exceptions;
using Eventflow.Domain.Interfaces.Repositories;
using Eventflow.Domain.Models.Models;
using Moq;

namespace Eventflow.Tests.Services
{
    [TestClass]
    public class PersonalEventReminderServiceTests
    {
        private Mock<IPersonalEventReminderRepository> _mockPersonalEventReminderRepository;
        private Mock<IPersonalEventRepository> _mockPersonalEventRepository;
        private PersonalEventReminderService _personalEventReminderService;

        [TestInitialize]
        public void Setup()
        {
            _mockPersonalEventReminderRepository = new Mock<IPersonalEventReminderRepository>();
            _mockPersonalEventRepository = new Mock<IPersonalEventRepository>();
            _personalEventReminderService = new PersonalEventReminderService(
                _mockPersonalEventReminderRepository.Object,
                _mockPersonalEventRepository.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_ThrowsException_WhenPersonalEventReminderRepositoryIsNull()
        {
            // Act
            var service = new PersonalEventReminderService(
                null!,
                new Mock<IPersonalEventRepository>().Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_ThrowsException_WhenPersonalEventRepositoryIsNull()
        {
            // Act
            var service = new PersonalEventReminderService(
                new Mock<IPersonalEventReminderRepository>().Object,
                null!);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task CreatePersonalEventReminderAsync_Throws_WhenReminderIsNull()
        {
            // Act
            await _personalEventReminderService.CreatePersonalEventReminderAsync(null!, 1);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidReminderInputException))]
        public async Task CreatePersonalEventReminderAsync_Throws_WhenTitleIsEmpty()
        {
            // Arrange
            int userId = 1;

            var personalEventReminder = new PersonalEventReminder
            {
                Title = "",
                Date = DateTime.Today,
                PersonalEventId = 1,
                UserId = userId
            };

            // Act
            await _personalEventReminderService.CreatePersonalEventReminderAsync(personalEventReminder, userId);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidReminderInputException))]
        public async Task CreatePersonalEventReminderAsync_Throws_WhenDateIsInPast()
        {
            // Arrage
            int userId = 1;
            var date = DateTime.Today.AddDays(-1);

            var personalEventReminder = new PersonalEventReminder
            {
                Title = "Test",
                Date = date,
                PersonalEventId = 1,
                UserId = userId
            };

            // Act
            await _personalEventReminderService.CreatePersonalEventReminderAsync(personalEventReminder, userId);
        }

        [TestMethod]
        [ExpectedException(typeof(UnauthorizedReminderAccessException))]
        public async Task CreatePersonalEventReminderAsync_Throws_WhenUserHasNoAccess()
        {
            //Arrange
            int userId = 1;
            int personalEventId = 1;

            var personalEventReminder = new PersonalEventReminder
            {
                Title = "Test",
                Date = DateTime.Today,
                PersonalEventId = personalEventId,
                UserId = userId
            };

            _mockPersonalEventRepository
                .Setup(r => r.UserHasAccessToEventAsync(personalEventId, userId))
                .ReturnsAsync(false);

            // Act
            await _personalEventReminderService.CreatePersonalEventReminderAsync(personalEventReminder, userId);
        }

        [TestMethod]
        public async Task CreatePersonalEventReminderAsync_CallsRepository_WithCorrectReminder()
        {
            // Arrange
            int userId = 1;
            int personalEventId = 1;

            var personalEventReminder = new PersonalEventReminder
            {
                Id = 1,
                Title = "Test",
                UserId = userId,
                Date = DateTime.Today,
                PersonalEventId = personalEventId,
            };

            _mockPersonalEventRepository
                .Setup(repo => repo.UserHasAccessToEventAsync(personalEventId, userId))
                .ReturnsAsync(true);

            // Act
            await _personalEventReminderService.CreatePersonalEventReminderAsync(personalEventReminder, userId);

            // Verify
            _mockPersonalEventReminderRepository
                .Verify(
                    r => r.CreatePersonalReminderAsync(It.Is<PersonalEventReminder>(rem =>
                        rem.Id == 1 &&
                        rem.Title == "Test" &&
                        rem.UserId == userId &&
                        rem.Date == DateTime.Today &&
                        rem.PersonalEventId == personalEventId
                    )), 
                    Times.Once);
        }
    }
}
