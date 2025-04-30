using Eventflow.Application.Services.Interfaces;
using Eventflow.Domain.Enums;
using Eventflow.Domain.Interfaces.Repositories;
using Eventflow.Domain.Models.Models;
using static Eventflow.Application.Helper.InputValidator;
using static Eventflow.Application.Security.PasswordHasher;

namespace Eventflow.Application.Services
{
    public class UserService : IAuthService, IUserService
    {
        private readonly IUserRepository _userRepository;
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<User?> GetUserByIdAsync(int userId)
            => await _userRepository.GetUserByIdAsync(userId);
        public async Task<User?> GetUserByUsernameAsync(string username)
            => await _userRepository.GetUserByInputAsync(username);
        public async Task<User?> LoginAsync(string loginInput, string password)
        {
            loginInput = loginInput.Trim().ToLower();
            password = password.Trim();

            if (!IsValidLoginInput(loginInput) || !IsValidLoginPassword(password))
            {
                return null;
            }

            User? user = await _userRepository.GetUserByInputAsync(loginInput);

            if (user == null)
            {
                return null;
            }

            if (VerifyPassword(password, user.PasswordHash, user.Salt))
            {
                return user;
            }

            return null;
        }
        public async Task<bool> RegisterAsync(string username, string password, string firstname, string? lastname, string email)
        {
            username = username.Trim().ToLower();
            email = email.Trim().ToLower();
            password = password.Trim();
            firstname = firstname.Trim();
            lastname = lastname?.Trim();

            if (!IsValidUsername(username)
                || !IsValidPassword(password)
                || !IsValidFirstname(firstname)
                || !IsValidEmail(email))
            {
                return false;
            }

            if (await _userRepository.UserExistsAsync(username, email))
            {
                return false;
            }

            string passwordSalt = GenerateRandomSalt();
            string hashedPassword = HashPassword(password, passwordSalt);

            User newUser = new User
            {
                Username = username,
                PasswordHash = hashedPassword,
                Salt = passwordSalt,
                Firstname = firstname,
                Lastname = lastname,
                Email = email,
                RoleId = (int)Domain.Enums.Role.User
            };

            int rowsAffected = await _userRepository.RegisterUserAsync(newUser);
            return rowsAffected > 0;
        }
    }
}
