using Eventflow.Application.Services;
using Eventflow.Domain.Common;
using Eventflow.Domain.Interfaces.Repositories;
using Eventflow.Domain.Models.Models;
using Moq;
using static Eventflow.Application.Security.PasswordHasher;
using static Eventflow.Domain.Common.ValidationConstants.User;

namespace Eventflow.Tests.Services
{
    [TestClass]
    public class UserServiceTests
    {
        private Mock<IUserRepository> _mockUserRepository;
        private UserService _userService;

        [TestInitialize]
        public void Setup()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _userService = new UserService(_mockUserRepository.Object);
        }

        [TestMethod]
        public async Task LoginAsync_ShouldReturnUser_WhenPasswordIsCorrect()
        {
            // Arrange
            string password = "MyPassword";
            string salt = GenerateRandomSalt();
            string hashed = HashPassword(password, salt);

            User user = new User
            {
                Username = "Mitko",
                PasswordHash = hashed,
                Salt = salt
            };

            _mockUserRepository
                .Setup(repo => repo.GetUserByInputAsync("Mitko"))
                .ReturnsAsync(user);

            // Act
            var result = await _userService.LoginAsync("Mitko", password);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Mitko", result.Username);
        }

        [TestMethod]
        public async Task LoginAsync_ShouldReturnNull_WhenPasswordIsIncorrect()
        {
            // Arrange
            string correctPassword = "CorrectPassword";
            string wrongPassword = "WrongPassword";
            string salt = GenerateRandomSalt();
            string hashed = HashPassword(correctPassword, salt);

            User user = new User
            {
                Username = "Mitko",
                PasswordHash = hashed,
                Salt = salt
            };

            _mockUserRepository
                .Setup(repo => repo.GetUserByInputAsync("Mitko"))
                .ReturnsAsync(user);

            // Act
            var result = await _userService.LoginAsync("Mitko", wrongPassword);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task LoginAsync_ShouldReturnNull_WhenUserDoesNotExist()
        {
            // Arrange
            _mockUserRepository
                .Setup(repo => repo.GetUserByInputAsync("null"))
                .ReturnsAsync((User?)null);

            // Act
            var result = await _userService.LoginAsync("null", "password");

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task RegisterAsync_ShouldReturnFalse_WhenRequiredFieldsAreMissing()
        {
            // Arrange
            string username = "";
            string password = "";
            string firstname = "";
            string email = "";

            // Act
            var result = await _userService.RegisterAsync(username, password, firstname, null, email);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task RegisterAsync_ShouldReturnFalse_WhenUserAlreadyExists()
        {
            // Arrange
            _mockUserRepository
                .Setup(repo => repo.UserExistsAsync("mitko", "mitko@gmail.com"))
                .ReturnsAsync(true);

            // Act
            var result = await _userService.RegisterAsync("mitko", "password", "Mitko", null, "mitko@gmail.com");

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task RegisterAsync_ShouldReturnFalse_WhenRegisterFailsInRepository()
        {
            // Arrange
            _mockUserRepository
                .Setup(repo => repo.UserExistsAsync("mitko", "mitko@gmail.com"))
                .ReturnsAsync(false);

            _mockUserRepository
                .Setup(repo => repo.RegisterUserAsync(It.IsAny<User>()))
                .ReturnsAsync(0); // fail

            // Act
            var result = await _userService.RegisterAsync("mitko", "password", "Dimitar", null, "mitko@gmail.com");

            // Assert
            Assert.IsFalse(result);
        }

        [DataTestMethod]
        [DataRow("a", false)] // too short
        [DataRow("aaa", false)] // too short
        [DataRow("abcd", true)] // valid
        [DataRow("validUsername", true)] // valid
        [DataRow("x______________________________________________x", false)] // too long
        public async Task RegisterAsync_ShouldValidateUsernameLength(string username, bool shouldBeValid)
        {
            // Arrange
            string password = "ValidPassword.123";
            string firstname = "Mitko";
            string email = "mitko@gmail.com";

            _mockUserRepository
                .Setup(repo => repo.UserExistsAsync(username, email))
                .ReturnsAsync(false);

            _mockUserRepository
                .Setup(repo => repo.RegisterUserAsync(It.IsAny<User>()))
                .ReturnsAsync(1);

            // Act
            var result = await _userService.RegisterAsync(
                username,
                password,
                firstname,
                null,
                email);

            // Assert
            if (shouldBeValid)
            {
                Assert.IsTrue(result);
            }
            else
            {
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task RegisterAsync_ShouldReturnFalse_WhenPasswordTooLong()
        {
            // Arrange
            string longPassword = new string('x', userPasswordMaxLength + 1);

            // Act
            var result = await _userService.RegisterAsync(
                "mitko",
                longPassword,
                "Mitko",
                null,
                "mitko@gmail.com"
            );

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task RegisterAsync_ShouldReturnFalse_WhenEmailTooLong()
        {
            // Arrange
            string longEmail = new string('x', userEmailMaxLength + 1);

            // Act
            var result = await _userService.RegisterAsync(
                "mitko",
                "ValidPassword.123",
                "Mitko",
                null,
                longEmail
            );

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task RegisterAsync_ShouldReturnTrue_WhenUserIsSuccessfullyCreated()
        {
            // Arrange
            string username = "mitko";
            string password = "secure123";
            string firstname = "Dimitar";
            string? lastname = null;
            string email = "mitko@gmail.com";

            _mockUserRepository
                .Setup(repo => repo.UserExistsAsync(username, email))
                .ReturnsAsync(false);

            _mockUserRepository
                .Setup(repo => repo.RegisterUserAsync(It.IsAny<User>()))
                .ReturnsAsync(1);

            // Act
            var result = await _userService.RegisterAsync(username, password, firstname, lastname, email);

            // Assert
            Assert.IsTrue(result);

            // Verify
            _mockUserRepository
                .Verify(repo => repo.RegisterUserAsync(It.Is<User>(u =>

                    u.Username == username &&
                    u.Firstname == firstname &&
                    u.Lastname == lastname &&
                    u.Email == email &&
                    u.RoleId == 2 &&
                    !string.IsNullOrWhiteSpace(u.Salt) &&
                    !string.IsNullOrWhiteSpace(u.PasswordHash)
                )),
                Times.Once
            );
        }

        [TestMethod]
        public async Task GetUserByIdAsync_ShouldReturnUser_WhenExists()
        {
            // Arrange
            User expectedUser = new User
            {
                Id = 1,
                Username = "mitko"
            };

            _mockUserRepository
                .Setup(repo => repo.GetUserByIdAsync(1))
                .ReturnsAsync(expectedUser);

            // Act
            var result = await _userService.GetUserByIdAsync(1);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("mitko", result.Username);
        }

        [TestMethod]
        public async Task GetUserByUsernameAsync_ShouldReturnUser_WhenExists()
        {
            // Arrange
            User expectedUser = new User
            {
                Id = 1,
                Username = "mitko"
            };

            _mockUserRepository
                .Setup(repo => repo.GetUserByInputAsync("mitko"))
                .ReturnsAsync(expectedUser);

            // Act
            var result = await _userService.GetUserByUsernameAsync("mitko");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("mitko", result.Username);
        }
    }
}
