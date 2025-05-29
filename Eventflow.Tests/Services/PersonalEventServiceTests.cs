using Eventflow.Application.Services;
using Eventflow.Domain.Helper;
using Eventflow.Domain.Interfaces.Repositories;
using Eventflow.Domain.Models.Entities;
using Eventflow.DTOs.DTOs;
using Eventflow.ViewModels.Invite.Enums;
using Moq;

namespace Eventflow.Tests.Services {
   [TestClass]
   public class PersonalEventServiceTests {
      private Mock<IPersonalEventRepository> _mockPersonalEventRepository;
      private Mock<ICategoryRepository> _mockCategoryRepository;
      private Mock<IUserRepository> _mockUserRepository;
      private Mock<IInviteRepository> _mockInviteRepository;
      private PersonalEventService _service;

      [TestInitialize]
      public void Setup()
      {
         _mockPersonalEventRepository = new Mock<IPersonalEventRepository>();
         _mockCategoryRepository = new Mock<ICategoryRepository>();
         _mockUserRepository = new Mock<IUserRepository>();
         _mockInviteRepository = new Mock<IInviteRepository>();

         _service = new PersonalEventService(
               _mockPersonalEventRepository.Object,
               _mockCategoryRepository.Object,
               _mockUserRepository.Object,
               _mockInviteRepository.Object);
      }

      [TestMethod]
      [ExpectedException(typeof(ArgumentNullException))]
      public void Constructor_Throws_WhenPersonalEventRepositoryIsNull()
      {
         // Act
         new PersonalEventService(
               null!,
               _mockCategoryRepository.Object,
               _mockUserRepository.Object,
               _mockInviteRepository.Object);
      }

      [TestMethod]
      [ExpectedException(typeof(ArgumentNullException))]
      public void Constructor_Throws_WhenCategoryRepositoryIsNull()
      {
         // Act
         new PersonalEventService(
               _mockPersonalEventRepository.Object,
               null!,
               _mockUserRepository.Object,
               _mockInviteRepository.Object);
      }

      [TestMethod]
      [ExpectedException(typeof(ArgumentNullException))]
      public void Constructor_Throws_WhenUserRepositoryIsNull()
      {
         // Act
         new PersonalEventService(
               _mockPersonalEventRepository.Object,
               _mockCategoryRepository.Object,
               null!,
               _mockInviteRepository.Object);
      }

      [TestMethod]
      [ExpectedException(typeof(ArgumentNullException))]
      public void Constructor_Throws_WhenInviteRepositoryIsNull()
      {
         // Act
         new PersonalEventService(
               _mockPersonalEventRepository.Object,
               _mockCategoryRepository.Object,
               _mockUserRepository.Object,
               null!);
      }

      [TestMethod]
      public async Task CreateAsync_CallsRepository_WithCorrectEvent() {
         // Arrange 
         var personalEvent = new PersonalEvent {
            Id = 1,
            Title = "Workout",
            Description = "Leg day",
            Date = DateTime.Today,
            UserId = 1
         };

         // Act
         await _service.CreateAsync(personalEvent);

         // Assert
         _mockPersonalEventRepository.Verify(
                repo => repo.CreateEventAsync(It.Is<PersonalEvent>(e =>
                    e.Id == personalEvent.Id &&
                    e.Title == personalEvent.Title &&
                    e.Description == personalEvent.Description &&
                    e.Date == personalEvent.Date &&
                    e.UserId == personalEvent.UserId
                )),
         Times.Once);
      }

      [TestMethod]
      public async Task GetPersonalEventByIdAsync_ReturnsCorrectEvent() {
         // Arrange
         var expected = new PersonalEvent {
            Id = 5,
            Title = "Test"
         };

         _mockPersonalEventRepository
            .Setup(repo => repo.GetPersonalEventByIdAsync(5))
            .ReturnsAsync(expected);
         
         // Act 
         var result = await _service.GetPersonalEventByIdAsync(5);

         // Assert
         Assert.IsNotNull(result);
         Assert.AreEqual(expected.Id, result?.Id);
         Assert.AreEqual(expected.Title, result?.Title);
      }

      [TestMethod]
      public async Task GetPersonalEventsCountAsync_ReturnsCorrectCount() {
         // Arrange
         _mockPersonalEventRepository
            .Setup(repo => repo.GetPersonalEventsCountAsync())
            .ReturnsAsync(42);
         
         // Act
         int count = await _service.GetPersonalEventsCountAsync();

         // Assert
         Assert.AreEqual(42, count);
      }

      [TestMethod]
      public async Task UpdatePersonalEventAsync_CallsRepository_WithCorrectEvent()
      {
         // Arrange
         var personalEvent = new PersonalEvent
         {
            Id = 10,
            Title = "Meeting",
            Description = "Project update",
            Date = DateTime.Today.AddDays(1),
            UserId = 2
         };

         // Act
         await _service.UpdatePersonalEventAsync(personalEvent);

         // Assert
         _mockPersonalEventRepository.Verify(
            repo => repo.UpdatePersonalEventAsync(It.Is<PersonalEvent>(e =>
                  e.Id == 10 &&
                  e.Title == "Meeting" &&
                  e.Description == "Project update" &&
                  e.Date == personalEvent.Date &&
                  e.UserId == 2
            )),
            Times.Once);
      }

      [TestMethod]
      public async Task GetRecentPersonalEventsAsync_ReturnsCorrectDtos()
      {
         // Arrange
         var events = new List<PersonalEvent>
         {
            new PersonalEvent { Id = 1, Title = "Run", UserId = 1 },
            new PersonalEvent { Id = 2, Title = "Read", UserId = 2 }
         };

         var userMap = new Dictionary<int, string>
         {
            { 1, "alice" },
            { 2, "bob" }
         };

         _mockPersonalEventRepository
            .Setup(repo => repo.GetRecentPersonalEventsAsync(2))
            .ReturnsAsync(events);

         _mockUserRepository
            .Setup(repo => repo.GetUsernamesByIdsAsync(It.IsAny<List<int>>()))
            .ReturnsAsync(userMap);

         // Act
         var result = await _service.GetRecentPersonalEventsAsync(2);

         // Assert
         Assert.AreEqual(2, result.Count);
         Assert.AreEqual("alice", result[0].CreatorUsername);
         Assert.AreEqual("bob", result[1].CreatorUsername);
      }

      [TestMethod]
      public async Task SoftDeleteEventAsync_ReturnsFalse_WhenEventIsNull()
      {
         // Arrange
         _mockPersonalEventRepository
            .Setup(repo => repo.GetPersonalEventByIdAsync(99))
            .ReturnsAsync((PersonalEvent?)null);

         // Act
         var result = await _service.SoftDeleteEventAsync(99, 1);

         // Assert
         Assert.IsFalse(result);
      }

      [TestMethod]
      public async Task SoftDeleteEventAsync_ReturnsFalse_WhenEventIsDeleted()
      {
         // Arrange
         _mockPersonalEventRepository
            .Setup(repo => repo.GetPersonalEventByIdAsync(99))
            .ReturnsAsync(new PersonalEvent { 
               Id = 99,
               IsDeleted = true
         });

         // Act
         var result = await _service.SoftDeleteEventAsync(99, 1);

         // Assert
         Assert.IsFalse(result);
      }

      [TestMethod]
      public async Task SoftDeleteEventAsync_ReturnsFalse_WhenUserIsNotOwner()
      {
         // Arrange
         _mockPersonalEventRepository
            .Setup(repo => repo.GetPersonalEventByIdAsync(99))
            .ReturnsAsync(new PersonalEvent { 
               Id = 99,
               IsDeleted = false,
               UserId = 2
         });

         // Act
         var result = await _service.SoftDeleteEventAsync(99, 1);

         // Assert
         Assert.IsFalse(result);
      }

      [TestMethod]
      public async Task SoftDeleteEventAsync_SoftDeletes_WhenConditionsAreMet()
      {
         // Arrange
         var pe = new PersonalEvent { 
            Id = 1,
            UserId = 1,
            IsDeleted = false
         };

         _mockPersonalEventRepository
            .Setup(repo => repo.GetPersonalEventByIdAsync(1))
            .ReturnsAsync(pe);

         // Act
         var result = await _service.SoftDeleteEventAsync(1, 1);

         // Assert
         Assert.IsTrue(result);

         _mockInviteRepository.Verify(repo => repo.DeleteInvitesByEventIdAsync(1), Times.Once);
         _mockPersonalEventRepository.Verify(repo => repo.SoftDeleteEventAsync(1), Times.Once);
      }

      [TestMethod]
      public async Task GetEventsWithCategoryNamesAsync_FiltersOutPastEvents_And_MapsCorrectly()
      {
         // Arrange
         var userId = 1;
         var today = DateTime.Today;
         var futureEvent = new PersonalEvent
         {
            Id = 1,
            Title = "Event",
            Description = "Future",
            Date = today.AddDays(1),
            UserId = userId,
            CategoryId = 10,
            IsCompleted = false,
            IsGlobal = false
         };
         var pastEvent = new PersonalEvent
         {
            Id = 2,
            Title = "Past",
            Description = "Old",
            Date = today.AddDays(-1),
            UserId = userId,
            CategoryId = 10
         };

         _mockPersonalEventRepository
            .Setup(repo => repo.GetByUserAndMonthAsync(userId, today.Year, today.Month))
            .ReturnsAsync(new List<PersonalEvent> { futureEvent, pastEvent });

         _mockCategoryRepository
            .Setup(repo => repo.GetAllCategoriesAsync())
            .ReturnsAsync(new List<Category> { new Category { Id = 10, Name = "Fitness" } });

         _mockUserRepository
            .Setup(repo => repo.GetUserByIdAsync(It.IsAny<int>()))
            .ReturnsAsync((int id) => new User
            {
               Id = id,
               Username = "pookie",
               IsDeleted = false
            });

         _mockUserRepository
            .Setup(repo => repo.GetUsernamesByEventIdAsync(1))
            .ReturnsAsync(new List<string> { "pookie" });

         // Act
         var result = await _service.GetEventsWithCategoryNamesAsync(userId, today.Year, today.Month);

         // Assert
         Assert.AreEqual(1, result.Count);
         Assert.AreEqual("Fitness", result[0].CategoryName);
         Assert.IsFalse(result[0].IsInvited);
         Assert.AreEqual("pookie", result[0].CreatorUsername);
      }

      [TestMethod]
      public async Task GetAllManageEventsAsync_ReturnsMappedDtos()
      {
         // Arrange
         var events = new List<PersonalEvent>
         {
            new PersonalEvent { 
               Id = 1,
               Title = "Test",
               UserId = 1,
               CategoryId = 5,
               Date = DateTime.Today }
         };
         var users = new List<User>
         {
            new User { Id = 1, Username = "admin", Email = "admin@test.com" },
            new User { Id = 2, Username = "user", Email = "user@test.com" }
         };
         var categories = new List<Category>
         {
            new Category { Id = 5, Name = "Work" }
         };
         var invites = new List<Invite>
         {
            new Invite { InvitedUserId = 2, StatusId = 1 }
         };

         _mockPersonalEventRepository
            .Setup(repo => repo.GetAllPersonalEventsAsync())
            .ReturnsAsync(events);

         _mockUserRepository
            .Setup(repo => repo.GetAllUsersAsync())
            .ReturnsAsync(users);

         _mockCategoryRepository
            .Setup(repo => repo.GetAllCategoriesAsync())
            .ReturnsAsync(categories);

         _mockInviteRepository
            .Setup(repo => repo.GetInvitesByEventIdAsync(1))
            .ReturnsAsync(invites);

         // Act
         var result = await _service.GetAllManageEventsAsync();

         // Assert
         Assert.AreEqual(1, result.Count);
         var dto = result[0];
         Assert.AreEqual("Test", dto.Title);
         Assert.AreEqual("Work", dto.CategoryName);
         Assert.AreEqual("admin", dto.OwnerUsername);
         Assert.AreEqual(1, dto.Participants.Count);
         Assert.AreEqual("user", dto.Participants[0].Username);
      }

      [TestMethod]
      public async Task GetGlobalEventsWithCategoryAsync_MapsCorrectly()
      {
         // Arrange
         var year = 2025;
         var month = 5;

         var globalEvent = new PersonalEvent
         {
            Id = 1,
            Title = "Global Event",
            Description = "Everyone joins",
            Date = new DateTime(year, month, 10),
            CategoryId = 10,
            UserId = 1,
            IsGlobal = true,
            IsCompleted = false
         };

         _mockPersonalEventRepository
            .Setup(repo => repo.GetGlobalEventsAsync(year, month))
            .ReturnsAsync(new List<PersonalEvent> { globalEvent });

         _mockCategoryRepository
            .Setup(repo => repo.GetAllCategoriesAsync())
            .ReturnsAsync(new List<Category> { new Category { Id = 10, Name = "Celebration" } });

         _mockUserRepository
            .Setup(repo => repo.GetAllUsersAsync())
            .ReturnsAsync(new List<User> { new User { Id = 1, Username = "global_creator" } });

         // Act
         var result = await _service.GetGlobalEventsWithCategoryAsync(year, month);

         // Assert
         Assert.AreEqual(1, result.Count);
         var e = result[0];
         Assert.AreEqual("Global Event", e.Title);
         Assert.AreEqual("Celebration", e.CategoryName);
         Assert.AreEqual("global_creator", e.CreatorUsername);
         Assert.IsTrue(e.IsGlobal);
      }

      [TestMethod]
      public async Task UpdateEventFromAdminAsync_UpdatesAndReturnsMappedDto()
      {
         // Arrange
         var dto = new EditEventDto
         {
            EventId = 1,
            Title = "Updated",
            Description = "Updated Desc",
            Date = DateTime.Today,
            CategoryId = 2
         };

         var existingEvent = new PersonalEvent
         {
            Id = 1,
            Title = "Old",
            Description = "Old Desc",
            Date = DateTime.Today.AddDays(-1),
            CategoryId = 1,
            UserId = 1
         };

         var user = new User { Id = 1, Username = "admin" };
         var category = new Category { Id = 2, Name = "Updated Cat" };

         _mockPersonalEventRepository
            .Setup(repo => repo.GetPersonalEventByIdAsync(1))
            .ReturnsAsync(existingEvent);

         _mockUserRepository
            .Setup(repo => repo.GetUserByIdAsync(1))
            .ReturnsAsync(user);

         _mockCategoryRepository
            .Setup(repo => repo.GetCategoryByIdAsync(2))
            .ReturnsAsync(category);

         _mockInviteRepository
            .Setup(repo => repo.GetInvitesByEventIdAsync(1))
            .ReturnsAsync(new List<Invite>());

         _mockUserRepository
            .Setup(repo => repo.GetAllUsersAsync())
            .ReturnsAsync(new List<User>());

         // Act
         var result = await _service.UpdateEventFromAdminAsync(dto);

         // Assert
         Assert.IsNotNull(result);
         Assert.AreEqual("Updated", result.Title);
         Assert.AreEqual("Updated Cat", result.CategoryName);
         Assert.AreEqual("admin", result.OwnerUsername);
      }

      [TestMethod]
      public async Task GetAcceptedInvitedEventsAsync_ReturnsCorrectDtos()
      {
         // Arrange
         int userId = 2;
         int year = DateTime.Today.Year;
         int month = DateTime.Today.Month;

         var invitedEvent = new PersonalEvent
         {
            Id = 1,
            Title = "Invited Event",
            Description = "You're in!",
            Date = new DateTime(year, month, 15),
            CategoryId = 5,
            UserId = 1,
            IsGlobal = false,
            IsCompleted = false
         };

         var category = new Category { Id = 5, Name = "Work" };
         var creator = new User { Id = 1, Username = "alice", IsDeleted = false };

         _mockPersonalEventRepository
            .Setup(repo => repo.GetAcceptedInvitedEventsAsync(userId))
            .ReturnsAsync(new List<PersonalEvent> { invitedEvent });

         _mockCategoryRepository
            .Setup(repo => repo.GetAllCategoriesAsync())
            .ReturnsAsync(new List<Category> { category });

         _mockUserRepository
            .Setup(repo => repo.GetUserByIdAsync(1))
            .ReturnsAsync(creator);

         _mockUserRepository
            .Setup(repo => repo.GetUsernamesByEventIdAsync(1))
            .ReturnsAsync(new List<string> { "alice", "bob" });

         // Act
         var result = await _service.GetAcceptedInvitedEventsAsync(userId, year, month);

         // Assert
         Assert.AreEqual(1, result.Count);
         var dto = result[0];
         Assert.AreEqual("Invited Event", dto.Title);
         Assert.AreEqual("Work", dto.CategoryName);
         Assert.IsTrue(dto.IsInvited);
         Assert.AreEqual("alice", dto.CreatorUsername);
         Assert.IsFalse(dto.IsCreator);
         Assert.AreEqual(2, dto.ParticipantUsernames.Count);
      }

      [TestMethod]
      public async Task GetAcceptedInvitedEventsAsync_FiltersOutWrongMonth()
      {
         // Arrange
         int userId = 2;
         var eventInWrongMonth = new PersonalEvent
         {
            Id = 1,
            Title = "Old Event",
            Date = DateTime.Today.AddMonths(-1),
            UserId = 1,
            CategoryId = 1
         };

         _mockPersonalEventRepository
            .Setup(repo => repo.GetAcceptedInvitedEventsAsync(userId))
            .ReturnsAsync(new List<PersonalEvent> { eventInWrongMonth });

         _mockCategoryRepository.Setup(repo => repo.GetAllCategoriesAsync())
            .ReturnsAsync(new List<Category>());

         _mockUserRepository.Setup(repo => repo.GetUserByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(new User { Id = 1, Username = "bob", IsDeleted = false });

         _mockUserRepository.Setup(repo => repo.GetUsernamesByEventIdAsync(It.IsAny<int>()))
            .ReturnsAsync(new List<string>());

         // Act
         var result = await _service.GetAcceptedInvitedEventsAsync(userId, DateTime.Today.Year, DateTime.Today.Month);

         // Assert
         Assert.AreEqual(0, result.Count);
      }

      [TestMethod]
      public async Task GetParticipantsByEventIdAsync_ReturnsFilteredParticipants()
      {
         // Arrange
         var invites = new List<Invite>
         {
            new Invite { InvitedUserId = 1, StatusId = InviteStatusHelper.Accepted }, 
            new Invite { InvitedUserId = 2, StatusId = InviteStatusHelper.KickedOut }, 
            new Invite { InvitedUserId = 3, StatusId = InviteStatusHelper.Accepted }  
         };

         var users = new List<User>
         {
            new User { Id = 1, Username = "Alice", Email = "alice@test.com" },
            new User { Id = 3, Username = "Charlie", Email = "charlie@test.com" }
         };

         _mockInviteRepository.Setup(repo => repo.GetInvitesByEventIdAsync(10))
            .ReturnsAsync(invites);

         _mockUserRepository.Setup(repo => repo.GetAllUsersAsync())
            .ReturnsAsync(users);

         // Act
         var result = await _service.GetParticipantsByEventIdAsync(10);

         // Assert
         Assert.AreEqual(2, result.Count);
         Assert.IsTrue(result.Any(p => p.Username == "Alice"));
         Assert.IsTrue(result.Any(p => p.Username == "Charlie"));
      }

      [TestMethod]
      public async Task GetEventsWithCategoryNamesAsync_AssignsUncategorized_WhenCategoryIsMissing()
      {
         var userId = 1;
         var today = DateTime.Today;
         var futureEvent = new PersonalEvent
         {
            Id = 1,
            Title = "Uncategorized Event",
            Date = today.AddDays(1),
            UserId = userId,
            CategoryId = 99
         };

         _mockPersonalEventRepository.Setup(r => r.GetByUserAndMonthAsync(userId, today.Year, today.Month))
            .ReturnsAsync(new List<PersonalEvent> { futureEvent });

         _mockCategoryRepository.Setup(r => r.GetAllCategoriesAsync())
            .ReturnsAsync(new List<Category>()); // no matching category

         _mockUserRepository.Setup(r => r.GetUserByIdAsync(userId))
            .ReturnsAsync(new User { Id = userId, Username = "noCatUser", IsDeleted = false });

         _mockUserRepository.Setup(r => r.GetUsernamesByEventIdAsync(1))
            .ReturnsAsync(new List<string>());

         // Act
         var result = await _service.GetEventsWithCategoryNamesAsync(userId, today.Year, today.Month);

         // Assert
         Assert.AreEqual(1, result.Count);
         Assert.AreEqual("Uncategorized", result[0].CategoryName);
      }

      [TestMethod]
      public async Task GetEventsWithCategoryNamesAsync_SkipsDeletedCreators()
      {
         var userId = 1;
         var today = DateTime.Today;
         var eventWithDeletedCreator = new PersonalEvent
         {
            Id = 1,
            Title = "Ghost Event",
            Date = today.AddDays(2),
            UserId = 99, // Deleted user
            CategoryId = 1
         };

         _mockPersonalEventRepository.Setup(r => r.GetByUserAndMonthAsync(userId, today.Year, today.Month))
            .ReturnsAsync(new List<PersonalEvent> { eventWithDeletedCreator });

         _mockCategoryRepository.Setup(r => r.GetAllCategoriesAsync())
            .ReturnsAsync(new List<Category> { new Category { Id = 1, Name = "Life" } });

         _mockUserRepository.Setup(r => r.GetUserByIdAsync(99))
            .ReturnsAsync(new User { Id = 99, Username = "ghost", IsDeleted = true }); // 👻

         _mockUserRepository.Setup(r => r.GetUsernamesByEventIdAsync(1))
            .ReturnsAsync(new List<string>());

         // Act
         var result = await _service.GetEventsWithCategoryNamesAsync(userId, today.Year, today.Month);

         // Assert
         Assert.AreEqual(0, result.Count);
      }

      [TestMethod]
      public async Task GetEventsWithCategoryNamesAsync_ShouldHandleNullCategoryId()
      {
         // Arrange
         int userId = 1;

         var events = new List<PersonalEvent>
         {
            new PersonalEvent
            {
                  Id = 1,
                  UserId = userId,
                  Title = "No Category",
                  Description = "Test event",
                  Date = DateTime.UtcNow.AddDays(1),
                  CategoryId = null
            }
         };

         var users = new List<User>
         {
            new User { Id = userId, Username = "Creator" }
         };

         var categories = new List<Category>();

         var invites = new List<Invite>();

         _mockPersonalEventRepository
            .Setup(r => r.GetByUserAndMonthAsync(userId, It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(events);
         _mockUserRepository
            .Setup(r => r.GetUserByIdAsync(userId))
            .ReturnsAsync(new User { Id = userId, Username = "Creator", IsDeleted = false });

         _mockUserRepository
            .Setup(r => r.GetUsernamesByEventIdAsync(1))
            .ReturnsAsync(new List<string>());
         _mockCategoryRepository.Setup(r => r.GetAllCategoriesAsync()).ReturnsAsync(categories);
         _mockInviteRepository.Setup(repo => repo.GetInvitesByEventIdAsync(It.IsAny<int>()))
               .ReturnsAsync(new List<Invite>());

         // Act
         var result = await _service.GetEventsWithCategoryNamesAsync(userId, DateTime.UtcNow.Month, DateTime.UtcNow.Year);

         // Assert
         Assert.AreEqual(1, result.Count);
         Assert.AreEqual("Uncategorized", result[0].CategoryName);
      }

      [TestMethod]
      public async Task GetGlobalEventsWithCategoryAsync_ShouldHandleMissingCreator()
      {
         // Arrange
         var globalEvents = new List<PersonalEvent>
         {
            new PersonalEvent
            {
                  Id = 1,
                  Title = "Global Event",
                  Description = "Global test",
                  Date = DateTime.UtcNow.AddDays(5),
                  IsGlobal = true,
                  UserId = 999
            }
         };

         var categories = new List<Category>
         {
            new Category { Id = 1, Name = "Training" }
         };

         var users = new List<User>();

         _mockPersonalEventRepository
            .Setup(r => r.GetGlobalEventsAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(globalEvents);
         _mockCategoryRepository.Setup(r => r.GetAllCategoriesAsync()).ReturnsAsync(categories);
         _mockUserRepository.Setup(r => r.GetAllUsersAsync()).ReturnsAsync(users);

         // Act
         var result = await _service.GetGlobalEventsWithCategoryAsync(DateTime.UtcNow.Month, DateTime.UtcNow.Year);

         // Assert
         Assert.AreEqual(1, result.Count);
         Assert.AreEqual("Unknown", result[0].CreatorUsername);
      }
   }
}