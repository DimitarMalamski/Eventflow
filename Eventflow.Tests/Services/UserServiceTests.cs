using Eventflow.Application.Services;
using Eventflow.Domain.Exceptions;
using Eventflow.Domain.Interfaces.Repositories;
using Eventflow.Domain.Models.Models;
using Moq;
using static Eventflow.Application.Security.PasswordHasher;
using static Eventflow.Domain.Common.ValidationConstants.User;
using Role = Eventflow.Domain.Enums.Role;

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
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_ShouldThrowArgumentNullException_WhenRepositoryIsNull()
        {
            // Act
            var service = new UserService(null!);
        }

        [TestMethod]
        public async Task LoginAsync_ShouldReturnUser_WhenPasswordIsCorrect()
        {
            // Arrange
            string password = "MyPassword.123";
            string salt = GenerateRandomSalt();
            string hashed = HashPassword(password, salt);

            User user = new User
            {
                Username = "mitko",
                PasswordHash = hashed,
                Salt = salt
            };

            _mockUserRepository
                .Setup(repo => repo.GetUserByInputAsync("mitko"))
                .ReturnsAsync(user);

            // Act
            var result = await _userService.LoginAsync("Mitko", password);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("mitko", result.Username);
        }

        [TestMethod]
        public async Task LoginAsync_ShouldReturnNull_WhenPasswordIsIncorrect()
        {
            // Arrange
            string correctPassword = "CorrectPassword.123";
            string wrongPassword = "WrongPassword.123";
            string salt = GenerateRandomSalt();
            string hashed = HashPassword(correctPassword, salt);

            User user = new User
            {
                Username = "mitko",
                PasswordHash = hashed,
                Salt = salt
            };

            _mockUserRepository
                .Setup(repo => repo.GetUserByInputAsync("mitko"))
                .ReturnsAsync(user);

            // Act
            var result = await _userService.LoginAsync("Mitko", wrongPassword);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidLoginInputException))]
        public async Task LoginAsync_ShouldReturnNull_WhenUserDoesNotExist()
        {
            // Arrange
            _mockUserRepository
                .Setup(repo => repo.GetUserByInputAsync("null"))
                .ReturnsAsync((User?)null);

            // Act
            var result = await _userService.LoginAsync("null", "password");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidLoginInputException))]
        public async Task LoginAsync_ShouldReturnNull_WhenPasswordIsEmpty()
        {
            // Arrange
            string loginInput = "mitko";
            string password = "";

            // Act
            var result = await _userService.LoginAsync(loginInput, password);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidLoginInputException))]
        public async Task LoginAsync_ShouldReturnNull_WhenLoginInputIsEmpty()
        {
            // Arrange
            string loginInput = "";
            string password = "ValidPassword.123";

            // Act
            var result = await _userService.LoginAsync(loginInput, password);
        }

        [TestMethod]
        public async Task LoginAsync_ShouldReturnUser_WhenUsernameHasDifferentCasing()
        {
            // Arrange
            string password = "ValidPassword.123";
            string salt = GenerateRandomSalt();
            string hashed = HashPassword(password, salt);

            User user = new User
            {
                Username = "mitko",
                Email = "mitko@gmail.com",
                PasswordHash = hashed,
                Salt = salt
            };

            _mockUserRepository
                .Setup(repo => repo.GetUserByInputAsync("mitko"))
                .ReturnsAsync(user);

            // Act
            var result = await _userService.LoginAsync("MITKO", password);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("mitko", result.Username);
        }

        [TestMethod]
        public async Task LoginAsync_ShouldReturnUser_WhenEmailHasDifferentCasing()
        {
            // Arrange
            string password = "ValidPassword.123";
            string salt = GenerateRandomSalt();
            string hashed = HashPassword(password, salt);

            User user = new User
            {
                Username = "mitko",
                Email = "mitko@gmail.com",
                PasswordHash = hashed,
                Salt = salt
            };

            _mockUserRepository
                .Setup(repo => repo.GetUserByInputAsync("mitko@gmail.com"))
                .ReturnsAsync(user);

            // Act
            var result = await _userService.LoginAsync("MITKO@GMAIL.COM", password);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("mitko", result.Username);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidLoginInputException))]
        public async Task LoginAsync_ShouldThrowException_WhenInputIsNullOrEmpty()
        {
            // Act
            await _userService.LoginAsync("", "");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidLoginInputException))]
        public async Task LoginAsync_ShouldThrowException_WhenPasswordIsInvalid()
        {
            // Act
            await _userService.LoginAsync("mitko", "short");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidRegistrationInputException))]
        public async Task RegisterAsync_ShouldReturnFalse_WhenRequiredFieldsAreMissing()
        {
            // Arrange
            string username = "";
            string password = "";
            string firstname = "";
            string email = "";

            // Act
            var result = await _userService.RegisterAsync(username, password, firstname, null, email);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidRegistrationInputException))]
        public async Task RegisterAsync_ShouldReturnFalse_WhenUserAlreadyExists()
        {
            // Arrange
            _mockUserRepository
                .Setup(repo => repo.UserExistsAsync("mitko", "mitko@gmail.com"))
                .ReturnsAsync(true);

            // Act
            var result = await _userService.RegisterAsync("mitko", "password", "Mitko", null, "mitko@gmail.com");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidRegistrationInputException))]
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
        }

        [DataTestMethod]
        [DataRow("a", false)]
        [DataRow("aaa", false)]
        [DataRow("xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx", false)]
        public async Task RegisterAsync_ShouldThrowException_WhenUsernameInvalid(string username, bool shouldBeValid)
        {
            // Arrange
            string password = "ValidPassword.123";
            string firstname = "Mitko";
            string email = "mitko@gmail.com";

            // Act
            await Assert.ThrowsExceptionAsync<InvalidRegistrationInputException>(() =>
                _userService.RegisterAsync(username, password, firstname, null, email));
        }

        [DataTestMethod]
        [DataRow("abcd")]
        [DataRow("validUsername")]
        public async Task RegisterAsync_ShouldSucceed_WhenUsernameIsValid(string username)
        {
            // Arrange
            string password = "ValidPassword.123";
            string firstname = "Mitko";
            string email = "mitko@gmail.com";

            _mockUserRepository
                .Setup(repo => repo.UserExistsAsync(username.ToLower(), email.ToLower()))
                .ReturnsAsync(false);

            _mockUserRepository
                .Setup(repo => repo.RegisterUserAsync(It.IsAny<User>()))
                .ReturnsAsync(1);

            // Act
            var result = await _userService.RegisterAsync(username, password, firstname, null, email);

            // Assert
            Assert.IsTrue(result);
        }

        [DataTestMethod]
        [DataRow("a")]
        [DataRow("aaa")]
        [DataRow("xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx")]
        public async Task RegisterAsync_ShouldThrowException_WhenFirstnameInvalid(string firstname)
        {
            // Arrange
            string password = "ValidPassword.123";
            string username = "Mitko";
            string email = "mitko@gmail.com";
            
            // Act
            await Assert.ThrowsExceptionAsync<InvalidRegistrationInputException>(() =>
                _userService.RegisterAsync(username, password, firstname, null, email));
        }

        [DataTestMethod]
        [DataRow("abcd")]
        [DataRow("validFirstname")]
        public async Task RegisterAsync_ShouldSucceed_WhenFirstnameIsValid(string firstname)
        {
            // Arrange
            string password = "ValidPassword.123";
            string username = "Mitko";
            string email = "mitko@gmail.com";

            _mockUserRepository
                .Setup(repo => repo.UserExistsAsync(username.ToLower(), email.ToLower()))
                .ReturnsAsync(false);

            _mockUserRepository
                .Setup(repo => repo.RegisterUserAsync(It.IsAny<User>()))
                .ReturnsAsync(1);

            // Act
            var result = await _userService.RegisterAsync(username, password, firstname, null, email);

            // Assert
            Assert.IsTrue(result);
        }

        [DataTestMethod]
        [DataRow("password")]
        [DataRow("PASSWORD123")]
        [DataRow("Password")]              
        [DataRow("Password1")]             
        [DataRow("12345678!")]
        [DataRow("short1!")]
        [ExpectedException(typeof(InvalidRegistrationInputException))]
        public async Task RegisterAsync_ShouldReturnFalse_WhenPasswordRegexFails(string password)
        {
            // Act
            var result = await _userService.RegisterAsync("mitko", password, "Mitko", null, "mitko@email.com");
        }

        [DataTestMethod]
        [DataRow("ValidPassword.123")]
        [DataRow("Another$123")]
        [DataRow("PassW0rd!")]
        public async Task RegisterAsync_ShouldReturnTrue_ForValidPasswords(string password)
        {
            // Arrange
            _mockUserRepository
                .Setup(repo => repo.UserExistsAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(false);

            _mockUserRepository
                .Setup(repo => repo.RegisterUserAsync(It.IsAny<User>()))
                .ReturnsAsync(1);

            var result = await _userService.RegisterAsync("mitko", password, "Mitko", null, "mitko@email.com");

            Assert.IsTrue(result);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidRegistrationInputException))]
        public async Task RegisterAsync_ShouldThrowException_WhenUsernameIsEmpty()
        {
            // Act
            await _userService.RegisterAsync("", "ValidPassword.123", "Mitko", null, "mitko@email.com");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidRegistrationInputException))]
        public async Task RegisterAsync_ShouldThrowException_WhenEmailInvalid()
        {
            // Act
            await _userService.RegisterAsync("mitko", "ValidPassword.123", "Mitko", null, "incorrect-email");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidRegistrationInputException))]
        public async Task RegisterAsync_ShouldThrowException_WhenUserAlreadyExists()
        {
            // Arrange
            _mockUserRepository
                .Setup(repo => repo.UserExistsAsync("mitko", "mitko@email.com"))
                .ReturnsAsync(true);

            // Act
            await _userService.RegisterAsync("mitko", "ValidPassword.123", "Mitko", null, "mitko@email.com");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidRegistrationInputException))]
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
        }

        [TestMethod]
        public async Task RegisterAsync_ShouldAssignDefaultUserRole()
        {
            // Arrange
            string username = "mitko";
            string email = "mitko@gmail.com";

            _mockUserRepository
                .Setup(repo => repo.UserExistsAsync(username.ToLower(), email.ToLower()))
                .ReturnsAsync(false);

            _mockUserRepository
                .Setup(repo => repo.RegisterUserAsync(It.Is<User>(u =>
                    u.RoleId == (int)Role.User
                )))
                .ReturnsAsync(1);

            // Act
            var result = await _userService.RegisterAsync(username, "ValidPassword.123", "Mitko", null, email);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidRegistrationInputException))]
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
        }

        [TestMethod]
        public async Task RegisterAsync_ShouldReturnTrue_WhenUserIsSuccessfullyCreated()
        {
            // Arrange
            string username = "mitko";
            string password = "Secure.123";
            string firstname = "Dimitar";
            string? lastname = null;
            string email = "mitko@gmail.com";

            _mockUserRepository
                .Setup(repo => repo.UserExistsAsync(username.ToLower(), email.ToLower()))
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

                    u.Username == username.ToLower() &&
                    u.Firstname == firstname &&
                    (u.Lastname ?? "") == (lastname ?? "") &&
                    u.Email == email &&
                    u.RoleId == (int)Role.User &&
                    !string.IsNullOrWhiteSpace(u.Salt) &&
                    !string.IsNullOrWhiteSpace(u.PasswordHash)
                )),
                Times.Once
            );
        }

        [TestMethod]
        public async Task RegisterAsync_ShouldReturnTrue_WhenLastnameIsNull()
        {
            // Arrange
            _mockUserRepository
                .Setup(repo => repo.UserExistsAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(false);

            _mockUserRepository
                .Setup(repo => repo.RegisterUserAsync(It.IsAny<User>()))
                .ReturnsAsync(1);

            // Act
            var result = await _userService.RegisterAsync("Mitko", "ValidPassword.123", "Mitko", null, "mitko@gmail.com");

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task RegisterAsync_ShouldLowercaseUsernameAndEmail()
        {
            // Arrange
            string username = "MiTkO";
            string email = "Mitko@Email.com";

            _mockUserRepository
                .Setup(repo => repo.UserExistsAsync(username.ToLower(), email.ToLower()))
                .ReturnsAsync(false);

            _mockUserRepository
                .Setup(repo => repo.RegisterUserAsync(It.Is<User>(u =>
                    u.Username == username.ToLower() &&
                    u.Email == email.ToLower()
                )))
                .ReturnsAsync(1);

            // Act 
            var result = await _userService.RegisterAsync(username, "ValidPassword.123", "Mitko", null, email);

            // Assert
            Assert.IsTrue(result);

            // Verify
            _mockUserRepository.Verify(repo =>
                repo.RegisterUserAsync(
                    It.Is<User>(u =>
                        u.Username == username.ToLower() &&
                        u.Email == email.ToLower()
                    )),
                    Times.Once);
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
        public async Task GetUserByIdAsync_ShouldReturnNull_WhenUserNotFound()
        {
            // Arrange
            _mockUserRepository
                .Setup(repo => repo.GetUserByIdAsync(99))
                .ReturnsAsync((User?)null);

            // Act
            var result = await _userService.GetUserByIdAsync(99);

            // Assert
            Assert.IsNull(result);
        }

        [DataTestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public async Task GetUserByIdAsync_ShouldThrowException_WhenIdIsInvalid(int id)
        {
            // Act
            await _userService.GetUserByIdAsync(id);
        }

        [TestMethod]
        public async Task GetUserByUsernameAsync_ShouldReturnNull_WhenUserNotFound()
        {
            // Arrange
            _mockUserRepository
                .Setup(repo => repo.GetUserByInputAsync("mitko"))
                .ReturnsAsync((User?)null);

            // Act
            var result = await _userService.GetUserByUsernameAsync("mitko");

            // Assert
            Assert.IsNull(result);
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
                .Setup(repo => repo.GetByUsernameAsync("mitko"))
                .ReturnsAsync(expectedUser);

            // Act
            var result = await _userService.GetUserByUsernameAsync("mitko");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("mitko", result.Username);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task GetUserByUsernameAsync_ShouldThrowException_WhenUsernameInvalid()
        {
            // Act
            await _userService.GetUserByUsernameAsync("");
        }
    }
}
