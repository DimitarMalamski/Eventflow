using Eventflow.Application.Services.Interfaces;
using Eventflow.Domain.Interfaces.Repositories;
using Eventflow.Domain.Models.Models;
using static Eventflow.Application.Security.PasswordHasher;

namespace Eventflow.Application.Services
{
    public class UserService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<User?> LoginAsync(string loginInput, string password)
        {
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
            if (string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(firstname) ||
                string.IsNullOrWhiteSpace(email))
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
                RoleId = 2
            };

            int rowsAffected = await _userRepository.RegisterUserAsync(newUser);
            return rowsAffected > 0;
        }
    }
}
