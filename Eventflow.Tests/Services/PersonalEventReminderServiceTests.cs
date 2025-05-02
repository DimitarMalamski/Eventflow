using Eventflow.Application.Services;
using Eventflow.Domain.Enums;
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

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task GetPaginatedFilteredPersonalRemindersAsync_Throws_WhenPageLessThanOne()
        {
            // Arrange
            _mockPersonalEventReminderRepository
                .Setup(repo => repo.GetUnreadPersonalRemindersForTodayAsync(It.IsAny<int>()))
                .ReturnsAsync(new List<PersonalEventReminder>());

            // Act
            await _personalEventReminderService.GetPaginatedFilteredPersonalRemindersAsync(
                userId: 1,
                status: ReminderStatus.Unread,
                search: null,
                sortBy: null,
                page: 0,
                pageSize: 5);
        }

        [TestMethod]
        public async Task GetPaginatedFilteredPersonalRemindersAsync_FiltersBySearchTerm()
        {
            // Arrange
            var personalEventReminders = new List<PersonalEventReminder>
            {
                new PersonalEventReminder { Title = "Buy Milk", Date = DateTime.Today },
                new PersonalEventReminder { Title = "Dentist Appointment", Date = DateTime.Today },
                new PersonalEventReminder { Title = "Workout", Date = DateTime.Today }
            };

            _mockPersonalEventReminderRepository
                .Setup(repo => repo.GetUnreadPersonalRemindersForTodayAsync(It.IsAny<int>()))
                .ReturnsAsync(personalEventReminders);

            // Act
            var result = await _personalEventReminderService.GetPaginatedFilteredPersonalRemindersAsync(
                userId: 1,
                status: ReminderStatus.Unread,
                search: "dentist",
                sortBy: null,
                page: 1,
                pageSize: 10);

            // Assert
            Assert.AreEqual(1, result.PersonalReminders.Count);
            Assert.AreEqual("Dentist Appointment", result.PersonalReminders[0].Title);
        }

        [TestMethod]
        public async Task GetPaginatedFilteredPersonalRemindersAsync_SortsByDate()
        {
            // Arrange
            var personalEventReminders = new List<PersonalEventReminder>
            {
                new PersonalEventReminder { Title = "B", Date = new DateTime(2025, 1, 2) },
                new PersonalEventReminder { Title = "A", Date = new DateTime(2025, 1, 1) },
                new PersonalEventReminder { Title = "C", Date = new DateTime(2025, 1, 3) }
            };

            _mockPersonalEventReminderRepository
                .Setup(repo => repo.GetUnreadPersonalRemindersForTodayAsync(It.IsAny<int>()))
                .ReturnsAsync(personalEventReminders);

            // Act
            var result = await _personalEventReminderService.GetPaginatedFilteredPersonalRemindersAsync(
                userId: 1,
                status: ReminderStatus.Unread,
                search: null,
                sortBy: "date",
                page: 1,
                pageSize: 10);

            // Assert
            Assert.AreEqual("A", result.PersonalReminders[0].Title);
            Assert.AreEqual("C", result.PersonalReminders[result.PersonalReminders.Count - 1].Title);
        }

        [TestMethod]
        public async Task GetPaginatedFilteredPersonalRemindersAsync_FallsBackToDefaultSort_WhenSortByInvalid()
        {
            // Arrange
            var personalEventReminders = new List<PersonalEventReminder>
            {
                new PersonalEventReminder { Id = 2, Title = "B", Date = DateTime.Today },
                new PersonalEventReminder { Id = 1, Title = "A", Date = DateTime.Today },
                new PersonalEventReminder { Id = 3, Title = "C", Date = DateTime.Today }
            };

            _mockPersonalEventReminderRepository
                .Setup(repo => repo.GetUnreadPersonalRemindersForTodayAsync(It.IsAny<int>()))
                .ReturnsAsync(personalEventReminders);

            // Act
            var result = await _personalEventReminderService.GetPaginatedFilteredPersonalRemindersAsync(
                userId: 1,
                status: ReminderStatus.Unread,
                search: null,
                sortBy: "fail",
                page: 1,
                pageSize: 10);

            // Assert
            Assert.AreEqual(1, result.PersonalReminders[0].Id);
            Assert.AreEqual(3, result.PersonalReminders[result.PersonalReminders.Count - 1].Id);
        }

        [TestMethod]
        public async Task GetPaginatedFilteredPersonalRemindersAsync_ReturnsCorrectPageSlice()
        {
            // Arrange
            var personalEventReminders = new List<PersonalEventReminder>
            {
                new PersonalEventReminder { Title = "First", Date = DateTime.Today },
                new PersonalEventReminder { Title = "Second", Date = DateTime.Today },
                new PersonalEventReminder { Title = "Third", Date = DateTime.Today }
            };

            _mockPersonalEventReminderRepository
                .Setup(repo => repo.GetUnreadPersonalRemindersForTodayAsync(It.IsAny<int>()))
                .ReturnsAsync(personalEventReminders);

            // Act
            var result = await _personalEventReminderService.GetPaginatedFilteredPersonalRemindersAsync(
                userId: 1,
                status: ReminderStatus.Unread,
                search: null,
                sortBy: "id",
                page: 2,
                pageSize: 1);

            // Assert
            Assert.AreEqual(1, result.PersonalReminders.Count);
            Assert.AreEqual("Second", result.PersonalReminders[0].Title);
            Assert.AreEqual(3, result.TotalPages);
            Assert.AreEqual(2, result.CurrentPage);
        }

        [TestMethod]
        public async Task GetPaginatedFilteredPersonalRemindersAsync_ReturnsEmpty_WhenNoReminders()
        {
            // Arrange
            _mockPersonalEventReminderRepository
                .Setup(repo => repo.GetUnreadPersonalRemindersForTodayAsync(It.IsAny<int>()))
                .ReturnsAsync(new List<PersonalEventReminder>());

            // Act
            var result = await _personalEventReminderService.GetPaginatedFilteredPersonalRemindersAsync(
                userId: 1,
                status: ReminderStatus.Unread,
                search: null,
                sortBy: "date",
                page: 1,
                pageSize: 10);

            // Assert
            Assert.AreEqual(0, result.PersonalReminders.Count);
            Assert.AreEqual(1, result.TotalPages);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task GetPaginatedFilteredPersonalRemindersAsync_Throws_WhenPageSizeIsZero()
        {
            // Arrange
            _mockPersonalEventReminderRepository
                .Setup(repo => repo.GetUnreadPersonalRemindersForTodayAsync(It.IsAny<int>()))
                .ReturnsAsync(new List<PersonalEventReminder>());

            // Act
            await _personalEventReminderService.GetPaginatedFilteredPersonalRemindersAsync(
                userId: 1,
                status: ReminderStatus.Unread,
                search: null,
                sortBy: null,
                page: 1,
                pageSize: 0);
        }

        [TestMethod]
        public async Task GetPaginatedFilteredPersonalRemindersAsync_SearchIsCaseInsensitive()
        {
            // Arrange
            var personalEventReminders = new List<PersonalEventReminder>
            {
                new PersonalEventReminder { Title = "Dentist Appointment", Date = DateTime.Today }
            };

            _mockPersonalEventReminderRepository
                .Setup(repo => repo.GetUnreadPersonalRemindersForTodayAsync(It.IsAny<int>()))
                .ReturnsAsync(personalEventReminders);

            // Act
            var result = await _personalEventReminderService.GetPaginatedFilteredPersonalRemindersAsync(
                userId: 1,
                status: ReminderStatus.Unread,
                search: "DENTIST",
                sortBy: null,
                page: 1,
                pageSize: 10);

            // Assert
            Assert.AreEqual(1, result.PersonalReminders.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(ReminderNotFoundException))]
        public async Task MarkPersonalEventReminderAsReadAsync_Throws_WhenReminderNotFound()
        {
            // Arrange
            int personalReminderId = 99;
            int userId = 1;

            _mockPersonalEventReminderRepository
                .Setup(repo => repo.GetPersonalReminderByIdAsync(personalReminderId))
                .ReturnsAsync((PersonalEventReminder?)null);

            // Act
            await _personalEventReminderService.MarkPersonalEventReminderAsReadAsync(personalReminderId, userId);
        }

        [TestMethod]
        [ExpectedException(typeof(UnauthorizedReminderAccessException))]
        public async Task MarkPersonalEventReminderAsReadAsync_Throws_WhenUserDoesNotOwnReminder()
        {
            // Arrange
            int reminderId = 1;
            int userId = 1;

            var personalReminder = new PersonalEventReminder
            {
                Id = reminderId,
                UserId = 2
            };

            _mockPersonalEventReminderRepository
                .Setup(repo => repo.GetPersonalReminderByIdAsync(reminderId))
                .ReturnsAsync(personalReminder);

            // Act
            await _personalEventReminderService.MarkPersonalEventReminderAsReadAsync(reminderId, userId);
        }

        [TestMethod]
        public async Task MarkPersonalEventReminderAsReadAsync_Succeeds_WhenUserOwnsReminder()
        {
            // Arrange
            int reminderId = 1;
            int userId = 1;

            var personalEventReminder = new PersonalEventReminder
            {
                Id = reminderId,
                UserId = userId
            };

            _mockPersonalEventReminderRepository
                .Setup(repo => repo.GetPersonalReminderByIdAsync(reminderId))
                .ReturnsAsync(personalEventReminder);

            // Act
            await _personalEventReminderService.MarkPersonalEventReminderAsReadAsync(reminderId, userId);

            // Verify
            _mockPersonalEventReminderRepository
                .Verify(repo => repo.MarkPersonalReminderAsReadAsync(reminderId, userId),
            Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(ReminderNotFoundException))]
        public async Task ToggleLikeAsync_Throws_WhenReminderNotFound()
        {
            // Arrange
            int reminderId = 100;
            int userId = 1;

            _mockPersonalEventReminderRepository
                .Setup(repo => repo.GetPersonalReminderByIdAsync(reminderId))
                .ReturnsAsync((PersonalEventReminder?)null);

            // Act
            await _personalEventReminderService.ToggleLikeAsync(reminderId, userId);
        }

        [TestMethod]
        [ExpectedException(typeof(UnauthorizedReminderAccessException))]
        public async Task ToggleLikeAsync_Throws_WhenUserDoesNotOwnReminder()
        {
            // Arrange
            int reminderId = 1;
            int userId = 1;

            var personalEventReminder = new PersonalEventReminder
            {
                Id = reminderId,
                UserId = 2,
                IsLiked = false
            };

            _mockPersonalEventReminderRepository
                .Setup(repo => repo.GetPersonalReminderByIdAsync(reminderId))
                .ReturnsAsync(personalEventReminder);

            // Act
            await _personalEventReminderService.ToggleLikeAsync(reminderId, userId);
        }

        [TestMethod]
        public async Task ToggleLikeAsync_Unlikes_WhenReminderIsAlreadyLiked()
        {
            // Arrange
            int reminderId = 1;
            int userId = 1;

            var personalEventReminder = new PersonalEventReminder
            {
                Id = reminderId,
                UserId = userId,
                IsLiked = true
            };

            _mockPersonalEventReminderRepository
                .Setup(repo => repo.GetPersonalReminderByIdAsync(reminderId))
                .ReturnsAsync(personalEventReminder);

            // Act
            var result = await _personalEventReminderService.ToggleLikeAsync(reminderId, userId);

            // Assert
            Assert.IsFalse(result);
            _mockPersonalEventReminderRepository
                .Verify(repo => repo.UnlikePersonalReminderAsync(reminderId, userId),
            Times.Once);
        }

        [TestMethod]
        public async Task ToggleLikeAsync_Likes_WhenReminderIsNotLiked()
        {
            // Arrange
            int reminderId = 1;
            int userId = 1;

            var personalEventReminder = new PersonalEventReminder
            {
                Id = reminderId,
                UserId = userId,
                IsLiked = false
            };

            _mockPersonalEventReminderRepository
                .Setup(repo => repo.GetPersonalReminderByIdAsync(reminderId))
                .ReturnsAsync(personalEventReminder);

            // Act
            var result = await _personalEventReminderService.ToggleLikeAsync(reminderId, userId);

            // Assert
            Assert.IsTrue(result);
            _mockPersonalEventReminderRepository
                .Verify(repo => repo.LikePersonalReminderAsync(reminderId, userId), Times.Once);
        }

        [TestMethod]
        public async Task HasUnreadRemindersForTodayAsync_ReturnsTrue_WhenRepositoryReturnsTrue()
        {
            // Arrange
            int userId = 1;

            _mockPersonalEventReminderRepository
                .Setup(repo => repo.HasUnreadPersonalRemindersForTodayAsync(userId))
                .ReturnsAsync(true);

            // Act
            var result = await _personalEventReminderService.HasUnreadRemindersForTodayAsync(userId);

            // Assert
            Assert.IsTrue(result);

            // Verify
            _mockPersonalEventReminderRepository
                .Verify(repo => repo.HasUnreadPersonalRemindersForTodayAsync(userId),
            Times.Once);
        }

        [TestMethod]
        public async Task HasUnreadRemindersForTodayAsync_ReturnsFalse_WhenRepositoryReturnsFalse()
        {
            // Arrange
            int userId = 1;

            _mockPersonalEventReminderRepository
                .Setup(repo => repo.HasUnreadPersonalRemindersForTodayAsync(userId))
                .ReturnsAsync(false);

            // Act
            var result = await _personalEventReminderService.HasUnreadRemindersForTodayAsync(userId);

            // Assert
            Assert.IsFalse(result);

            // Verify
            _mockPersonalEventReminderRepository
                .Verify(repo => repo.HasUnreadPersonalRemindersForTodayAsync(userId), 
            Times.Once);
        }

        [TestMethod]
        public async Task GetLikedReminderCountAsync_ReturnsCorrectCount_WhenRemindersExist()
        {
            // Arrange
            int userId = 1;

            var likedPersonalEventReminders = new List<PersonalEventReminder>
            {
                new PersonalEventReminder { Id = 1, UserId = userId, IsLiked = true },
                new PersonalEventReminder { Id = 2, UserId = userId, IsLiked = true }
            };

            _mockPersonalEventReminderRepository
                .Setup(repo => repo.GetLikedRemindersByUserAsync(userId))
                .ReturnsAsync(likedPersonalEventReminders);

            // Act
            var likedPersonalEventRemindersCount = await _personalEventReminderService.GetLikedReminderCountAsync(userId);

            // Assert
            Assert.AreEqual(2, likedPersonalEventRemindersCount);
        }

        [TestMethod]
        public async Task GetLikedReminderCountAsync_ReturnsZero_WhenNoReminders()
        {
            // Arrange
            int userId = 1;

            _mockPersonalEventReminderRepository
                .Setup(repo => repo.GetLikedRemindersByUserAsync(userId))
                .ReturnsAsync(new List<PersonalEventReminder>());

            // Act
            var likedPersonalEventRemindersCount = await _personalEventReminderService.GetLikedReminderCountAsync(userId);

            // Assert
            Assert.AreEqual(0, likedPersonalEventRemindersCount);
        }

        [TestMethod]
        public async Task CountUnreadRemindersForTodayAsync_ReturnsCorrectCount_WhenUnreadRemindersExist()
        {
            // Arrange
            int userId = 1;

            var likedPersonalEventReminders = new List<PersonalEventReminder>
            {
                new PersonalEventReminder { Id = 1, UserId = userId, Status = ReminderStatus.Unread },
                new PersonalEventReminder { Id = 2, UserId = userId, Status = ReminderStatus.Unread }
            };

            _mockPersonalEventReminderRepository
                .Setup(repo => repo.GetUnreadPersonalRemindersForTodayAsync(userId))
                .ReturnsAsync(likedPersonalEventReminders);

            // Act
            var unreadPersonalEventRemindersCount = await _personalEventReminderService.CountUnreadRemindersForTodayAsync(userId);

            // Assert
            Assert.AreEqual(2, unreadPersonalEventRemindersCount);
        }

        [TestMethod]
        public async Task CountUnreadRemindersForTodayAsync_ReturnsZero_WhenNoUnreadReminders()
        {
            // Arrange
            int userId = 1;

            _mockPersonalEventReminderRepository
                .Setup(repo => repo.GetUnreadPersonalRemindersForTodayAsync(userId))
                .ReturnsAsync(new List<PersonalEventReminder>());

            // Act
            var unreadPersonalEventRemindersCount = await _personalEventReminderService.CountUnreadRemindersForTodayAsync(userId);

            // Assert
            Assert.AreEqual(0, unreadPersonalEventRemindersCount);
        }

        [TestMethod]
        public async Task GetPaginatedLikedRemindersAsync_ReturnsEmpty_WhenNoReminders()
        {
            // Arrange
            _mockPersonalEventReminderRepository
                .Setup(repo => repo.GetLikedRemindersByUserAsync(It.IsAny<int>()))
                .ReturnsAsync(new List<PersonalEventReminder>());

            // Act
            var result = await _personalEventReminderService.GetPaginatedLikedRemindersAsync(
                userId: 1, 
                search: null, 
                sortBy: null, 
                page: 1,
                pageSize: 10);

            // Assert
            Assert.AreEqual(0, result.PersonalReminders.Count);
            Assert.AreEqual(1, result.TotalPages);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task GetPaginatedLikedRemindersAsync_Throws_WhenPageIsLessThanOne()
        {
            // Act
            await _personalEventReminderService.GetPaginatedLikedRemindersAsync(
                userId: 1, 
                search: null, 
                sortBy: null, 
                page: 0,
                pageSize: 10);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task GetPaginatedLikedRemindersAsync_Throws_WhenPageSizeIsZero()
        {
            // Act
            await _personalEventReminderService.GetPaginatedLikedRemindersAsync(
                userId: 1,
                search: null, 
                sortBy: null, 
                page: 1, 
                pageSize: 0);
        }

        [TestMethod]
        public async Task GetPaginatedLikedRemindersAsync_AppliesSearchFilter()
        {
            // Arrange
            var personalEventReminders = new List<PersonalEventReminder>
            {
                new PersonalEventReminder { Title = "Gym", Date = DateTime.Today, IsLiked = true },
                new PersonalEventReminder { Title = "Cook Eggs", Date = DateTime.Today, IsLiked = true }
            };

            _mockPersonalEventReminderRepository
                .Setup(repo => repo.GetLikedRemindersByUserAsync(It.IsAny<int>()))
                .ReturnsAsync(personalEventReminders);

            // Act
            var result = await _personalEventReminderService.GetPaginatedLikedRemindersAsync(
                userId: 1,
                search: "eggs",
                sortBy: null, 
                page: 1, 
                pageSize: 10);

            // Assert
            Assert.AreEqual(1, result.PersonalReminders.Count);
            Assert.AreEqual("Cook Eggs", result.PersonalReminders[0].Title);
        }

        [TestMethod]
        public async Task GetPaginatedLikedRemindersAsync_SortsByDate()
        {
            // Arrange
            var personalEventReminders = new List<PersonalEventReminder>
            {
                new PersonalEventReminder { Title = "C", Date = new DateTime(2025, 1, 3), IsLiked = true },
                new PersonalEventReminder { Title = "A", Date = new DateTime(2025, 1, 1), IsLiked = true },
                new PersonalEventReminder { Title = "B", Date = new DateTime(2025, 1, 2), IsLiked = true }
            };

            _mockPersonalEventReminderRepository
                .Setup(repo => repo.GetLikedRemindersByUserAsync(It.IsAny<int>()))
                .ReturnsAsync(personalEventReminders);

            // Act
            var result = await _personalEventReminderService.GetPaginatedLikedRemindersAsync(
                userId: 1, 
                search: null, 
                sortBy: "date", 
                page: 1, 
                pageSize: 10);

            // Assert
            Assert.AreEqual("A", result.PersonalReminders[0].Title);
            Assert.AreEqual("C", result.PersonalReminders[result.PersonalReminders.Count - 1].Title);
        }

        [TestMethod]
        public async Task GetPaginatedLikedRemindersAsync_ReturnsCorrectSlice()
        {
            // Arrange
            var personalEventReminders = new List<PersonalEventReminder>
            {
                new PersonalEventReminder { Title = "1", IsLiked = true },
                new PersonalEventReminder { Title = "2", IsLiked = true },
                new PersonalEventReminder { Title = "3", IsLiked = true }
            };

            _mockPersonalEventReminderRepository
                .Setup(repo => repo.GetLikedRemindersByUserAsync(It.IsAny<int>()))
                .ReturnsAsync(personalEventReminders);

            // Act
            var result = await _personalEventReminderService.GetPaginatedLikedRemindersAsync(
                userId: 1,
                search: null, 
                sortBy: "id", 
                page: 2, 
                pageSize: 1);

            // Assert
            Assert.AreEqual("2", result.PersonalReminders[0].Title);
            Assert.AreEqual(3, result.TotalPages);
            Assert.AreEqual(2, result.CurrentPage);
        }

        [TestMethod]
        public async Task GetPaginatedLikedRemindersAsync_FallsBackToDefaultSort_WhenSortByIsNull()
        {
            // Arrange
            var personalEventLikedReminders = new List<PersonalEventReminder>
            {
                new PersonalEventReminder { Id = 3, Title = "C", IsLiked = true },
                new PersonalEventReminder { Id = 1, Title = "A", IsLiked = true },
                new PersonalEventReminder { Id = 2, Title = "B", IsLiked = true }
            };

            _mockPersonalEventReminderRepository
                .Setup(repo => repo.GetLikedRemindersByUserAsync(It.IsAny<int>()))
                .ReturnsAsync(personalEventLikedReminders);

            // Act
            var result = await _personalEventReminderService.GetPaginatedLikedRemindersAsync(
                userId: 1, 
                search: null, 
                sortBy: null, 
                page: 1, 
                pageSize: 10);

            Assert.AreEqual(1, result.PersonalReminders[0].Id);
        }

        [TestMethod]
        public async Task GetPaginatedLikedRemindersAsync_Filters_ByPersonalEventTitle()
        {
            // Arrange
            var personalEventLikedReminders = new List<PersonalEventReminder>
            {
                new PersonalEventReminder
                {
                    Title = "Reminder 1",
                    PersonalEvent = new PersonalEvent { Title = "Yoga Class" },
                    IsLiked = true
                },
                new PersonalEventReminder
                {
                    Title = "Reminder 2",
                    PersonalEvent = new PersonalEvent { Title = "Meeting" },
                    IsLiked = true
                }
            };

            _mockPersonalEventReminderRepository
                .Setup(repo => repo.GetLikedRemindersByUserAsync(It.IsAny<int>()))
                .ReturnsAsync(personalEventLikedReminders);

            // Act
            var result = await _personalEventReminderService.GetPaginatedLikedRemindersAsync(
                userId: 1, 
                search: "yoga", 
                sortBy: null, 
                page: 1, 
                pageSize: 10);

            // Assert
            Assert.AreEqual(1, result.PersonalReminders.Count);
            Assert.AreEqual("Reminder 1", result.PersonalReminders[0].Title);
        }

        [TestMethod]
        public async Task GetPaginatedLikedRemindersAsync_DoesNotThrow_WhenPersonalEventIsNull()
        {
            // Arrange
            var personalEventLikedReminders = new List<PersonalEventReminder>
            {
                new PersonalEventReminder
                {
                    Title = "Reminder without event",
                    Description = "Tests",
                    Date = DateTime.Today,
                    IsLiked = true,
                    PersonalEvent = null
                }
            };

            _mockPersonalEventReminderRepository
                .Setup(repo => repo.GetLikedRemindersByUserAsync(It.IsAny<int>()))
                .ReturnsAsync(personalEventLikedReminders);

            // Act
            var result = await _personalEventReminderService.GetPaginatedLikedRemindersAsync(
                userId: 1,
                search: "anything",
                sortBy: null,
                page: 1,
                pageSize: 10);

            // Assert
            Assert.AreEqual(0, result.PersonalReminders.Count);
        }
    }
}
