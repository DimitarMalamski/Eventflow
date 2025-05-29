using Eventflow.Application.Services;
using Eventflow.Domain.Enums;
using Eventflow.Domain.Helper;
using Eventflow.Domain.Interfaces.Repositories;
using Eventflow.Domain.Models.Entities;
using Moq;

namespace Eventflow.Tests.Services {
   [TestClass]
   public class InviteServiceTests {
      private Mock<IInviteRepository> _mockInviteRepository;
      private Mock<IUserRepository> _mockUserRepository;
      private InviteService _service;

      [TestInitialize]
      public void Setup()
      {
         _mockInviteRepository = new Mock<IInviteRepository>();
         _mockUserRepository = new Mock<IUserRepository>();
         _service = new InviteService(_mockInviteRepository.Object, _mockUserRepository.Object);
      }

      [TestMethod]
      [ExpectedException(typeof(ArgumentNullException))]
      public void Constructor_Throws_WhenInviteRepositoryIsNull()
      {
         // Act
         new InviteService(null!, _mockUserRepository.Object);
      }

      [TestMethod]
      [ExpectedException(typeof(ArgumentNullException))]
      public void Constructor_Throws_WhenUserRepositoryIsNull()
      {
         // Act
         new InviteService(_mockInviteRepository.Object, null!);
      }

      [TestMethod]
      public async Task CountPendingInvitesAsync_ReturnsCorrectCount()
      {
         // Arrange
         int userId = 5;
         var mockInvites = new List<Invite> { new Invite(), new Invite() };
         _mockInviteRepository
            .Setup(repo => repo.GetInvitesByUserAndStatusAsync(userId, 1))
            .ReturnsAsync(mockInvites);

         // Act
         var result = await _service.CountPendingInvitesAsync(userId);

         // Assert
         Assert.AreEqual(2, result);
      }

      [TestMethod]
      public async Task AutoDeclineExpiredInvitesAsync_UpdatesAllStatuses()
      {
         // Arrange
         var expiredInvites = new List<Invite>
         {
            new Invite { Id = 1 },
            new Invite { Id = 2 },
         };

         _mockInviteRepository
            .Setup(repo => repo.GetExpiredPendingInvitesAsync())
            .ReturnsAsync(expiredInvites);

         // Act
         await _service.AutoDeclineExpiredInvitesAsync();

         // Assert
         _mockInviteRepository.Verify(r => r.UpdateInviteStatusAsync(1, 3), Times.Once);
         _mockInviteRepository.Verify(r => r.UpdateInviteStatusAsync(2, 3), Times.Once);
      }

      [TestMethod]
      public async Task CreateOrResetInviteAsync_Throws_WhenUserIsNullOrDeleted()
      {
         // Arrange
         var invite = new Invite { InvitedUserId = 100 };
         _mockUserRepository
            .Setup(repo => repo.GetUserByIdAsync(invite.InvitedUserId))
            .ReturnsAsync((User?)null); // Simulate user not found

         // Act & Assert
         await Assert.ThrowsExceptionAsync<InvalidOperationException>(() =>
            _service.CreateOrResetInviteAsync(invite));
      }

      [TestMethod]
      public async Task CreateOrResetInviteAsync_ReturnsExpectedResult_WhenUserExists()
      {
         // Arrange
         var invite = new Invite { InvitedUserId = 42 };
         var user = new User { Id = 42, IsDeleted = false };

         _mockUserRepository
            .Setup(repo => repo.GetUserByIdAsync(invite.InvitedUserId))
            .ReturnsAsync(user);

         _mockInviteRepository
            .Setup(repo => repo.CreateOrResetInviteAsync(invite))
            .ReturnsAsync(InviteActionResult.Created);

         // Act
         var result = await _service.CreateOrResetInviteAsync(invite);

         // Assert
         Assert.AreEqual(InviteActionResult.Created, result);
      }

      [TestMethod]
      public async Task GetPaginatedFilteredInvitesAsync_ReturnsCorrectPaginationAndFiltersKickedOut()
      {
         // Arrange
         var userId = 1;
         var statusId = 1;
         var page = 1;
         var pageSize = 2;

         var invites = new List<Invite>
         {
            new Invite { Id = 1, StatusId = 1, PersonalEventId = 100, InvitedUserId = 2 },
            new Invite { Id = 2, StatusId = 2, PersonalEventId = 101, InvitedUserId = 3 },
            new Invite { Id = 3, StatusId = InviteStatusHelper.KickedOut, PersonalEventId = 102, InvitedUserId = 4 }
         };

         _mockInviteRepository
            .Setup(repo => repo.GetInvitesByUserAndStatusAsync(userId, statusId))
            .ReturnsAsync(invites);

         // Act
         var result = await _service.GetPaginatedFilteredInvitesAsync(
            userId, statusId, search: null, sortBy: null, page, pageSize);

         Assert.AreEqual(1, result.TotalPages);
         Assert.AreEqual(1, result.CurrentPage);
         Assert.AreEqual(2, result.Invites.Count);
         Assert.IsFalse(result.Invites.Any(i => i.Id == 3));
      }

      [TestMethod]
      public async Task GetInviteAsync_ReturnsNull_WhenInviteNotFound()
      {
         // Arrange
         int eventId = 1, invitedUserId = 2;

         _mockInviteRepository
            .Setup(r => r.GetInviteByEventAndUserAsync(eventId, invitedUserId))
            .ReturnsAsync((Invite?)null);

         // Act
         var result = await _service.GetInviteAsync(eventId, invitedUserId);

         // Assert
         Assert.IsNull(result);
      }

      [TestMethod]
      public async Task GetInviteAsync_ReturnsDto_WhenInviteExists()
      {
         // Arrange
         int eventId = 1, invitedUserId = 2;
         var invite = new Invite
         {
            Id = 10,
            PersonalEventId = eventId,
            InvitedUserId = invitedUserId,
            StatusId = 1
         };

         var user = new User
         {
            Id = invitedUserId,
            Username = "TestUser"
         };

         _mockInviteRepository
            .Setup(r => r.GetInviteByEventAndUserAsync(eventId, invitedUserId))
            .ReturnsAsync(invite);

         _mockUserRepository
            .Setup(r => r.GetUserByIdAsync(invitedUserId))
            .ReturnsAsync(user);

         // Act
         var result = await _service.GetInviteAsync(eventId, invitedUserId);

         // Assert
         Assert.IsNotNull(result);
         Assert.AreEqual(invite.Id, result!.Id);
         Assert.AreEqual(invite.PersonalEventId, result.EventId);
         Assert.AreEqual(invite.InvitedUserId, result.InvitedUserId);
         Assert.AreEqual(invite.StatusId, result.StatusId);
         Assert.AreEqual("Pending", result.Status);
      }

      [TestMethod]
      public async Task DeleteInviteAsync_CallsRepositoryWithCorrectArguments()
      {
         // Arrange
         int eventId = 5;
         int invitedUserId = 12;

         _mockInviteRepository
            .Setup(r => r.SoftDeleteInviteAsync(eventId, invitedUserId))
            .Returns(Task.CompletedTask)
            .Verifiable();

         // Act
         await _service.DeleteInviteAsync(eventId, invitedUserId);

         // Assert
         _mockInviteRepository.Verify(r =>
            r.SoftDeleteInviteAsync(eventId, invitedUserId), Times.Once);
      }

      [TestMethod]
      public async Task LeaveEventAsync_CallsRepositoryWithCorrectArguments()
      {
         // Arrange
         int userId = 7;
         int eventId = 21;

         _mockInviteRepository
            .Setup(r => r.MarkInviteAsLeftAsync(userId, eventId))
            .Returns(Task.CompletedTask)
            .Verifiable();

         // Act
         await _service.LeaveEventAsync(userId, eventId);

         // Assert
         _mockInviteRepository.Verify(r =>
            r.MarkInviteAsLeftAsync(userId, eventId), Times.Once);
      }

      [TestMethod]
      public async Task InviteExistsAsync_ReturnsTrue_WhenInviteExists()
      {
         // Arrange
         int eventId = 3;
         int invitedUserId = 14;

         _mockInviteRepository
            .Setup(r => r.InviteExistsAsync(eventId, invitedUserId))
            .ReturnsAsync(true);

         // Act
         var result = await _service.InviteExistsAsync(eventId, invitedUserId);

         // Assert
         Assert.IsTrue(result);
      }

      [TestMethod]
      public async Task InviteExistsAsync_ReturnsFalse_WhenInviteDoesNotExist()
      {
         // Arrange
         int eventId = 3;
         int invitedUserId = 14;

         _mockInviteRepository
            .Setup(r => r.InviteExistsAsync(eventId, invitedUserId))
            .ReturnsAsync(false);

         // Act
         var result = await _service.InviteExistsAsync(eventId, invitedUserId);

         // Assert
         Assert.IsFalse(result);
      }

      [TestMethod]
      public async Task HasUserAcceptedInviteAsync_ReturnsTrue_WhenAccepted()
      {
         // Arrange
         int userId = 5;
         int eventId = 10;

         _mockInviteRepository
            .Setup(r => r.HasUserAcceptedInviteAsync(userId, eventId))
            .ReturnsAsync(true);

         // Act
         var result = await _service.HasUserAcceptedInviteAsync(userId, eventId);

         // Assert
         Assert.IsTrue(result);
      }

      [TestMethod]
      public async Task HasUserAcceptedInviteAsync_ReturnsFalse_WhenNotAccepted()
      {
         // Arrange
         int userId = 5;
         int eventId = 10;

         _mockInviteRepository
            .Setup(r => r.HasUserAcceptedInviteAsync(userId, eventId))
            .ReturnsAsync(false);

         // Act
         var result = await _service.HasUserAcceptedInviteAsync(userId, eventId);

         // Assert
         Assert.IsFalse(result);
      }

      [TestMethod]
      public async Task HasPendingInvitesAsync_ReturnsTrue_WhenInvitesExist()
      {
         // Arrange
         int userId = 8;

         _mockInviteRepository
            .Setup(r => r.HasPendingInvitesAsync(userId))
            .ReturnsAsync(true);

         // Act
         var result = await _service.HasPendingInvitesAsync(userId);

         // Assert
         Assert.IsTrue(result);
      }

      [TestMethod]
      public async Task HasPendingInvitesAsync_ReturnsFalse_WhenNoInvites()
      {
         // Arrange
         int userId = 8;

         _mockInviteRepository
            .Setup(r => r.HasPendingInvitesAsync(userId))
            .ReturnsAsync(false);

         // Act
         var result = await _service.HasPendingInvitesAsync(userId);

         // Assert
         Assert.IsFalse(result);
      }

      [TestMethod]
      public async Task GetAllInvitesByUserIdAsync_ReturnsExpectedInvites()
      {
         // Arrange
         var userId = 1;
         var expected = new List<Invite> { new Invite(), new Invite() };

         _mockInviteRepository.Setup(r => r.GetAllInvitesByUserIdAsync(userId))
                              .ReturnsAsync(expected);

         // Act
         var result = await _service.GetAllInvitesByUserIdAsync(userId);

         // Assert
         Assert.AreEqual(2, result.Count);
      }

      [TestMethod]
      public async Task GetInvitesByUserAndStatusAsync_ReturnsFilteredList()
      {
         // Arrange
         var userId = 1;
         var statusId = 2;
         var expected = new List<Invite> { new Invite { StatusId = statusId } };

         _mockInviteRepository.Setup(r => r.GetInvitesByUserAndStatusAsync(userId, statusId))
                              .ReturnsAsync(expected);

         // Act
         var result = await _service.GetInvitesByUserAndStatusAsync(userId, statusId);

         // Assert
         Assert.AreEqual(statusId, result[0].StatusId);
      }

      [TestMethod]
      public async Task UpdateInviteStatusAsync_CallsRepository()
      {
         // Arrange
         var inviteId = 99;
         var statusId = 3;

         _mockInviteRepository.Setup(r => r.UpdateInviteStatusAsync(inviteId, statusId))
                              .Returns(Task.CompletedTask)
                              .Verifiable();

         // Act
         await _service.UpdateInviteStatusAsync(inviteId, statusId);

         // Assert
         _mockInviteRepository.Verify(r => r.UpdateInviteStatusAsync(inviteId, statusId), Times.Once);
      }

      [TestMethod]
      public async Task GetPaginatedFilteredInvitesAsync_Throws_WhenPageIsInvalid()
      {
         // Arrange
         _mockInviteRepository
            .Setup(r => r.GetInvitesByUserAndStatusAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(new List<Invite>());
         
         // Act & Assert
         await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
            _service.GetPaginatedFilteredInvitesAsync(1, 1, null, null, 0, 10));
      }
   }
}