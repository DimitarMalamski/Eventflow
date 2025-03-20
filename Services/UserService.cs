using Eventflow.Models.Models;
using Eventflow.Repositories.Interfaces;
using Eventflow.Services.Interfaces;
using static Eventflow.Utilities.PasswordHasher;

namespace Eventflow.Services
{
    public class UserService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public User? Login(string loginInput, string password)
        {
            User? user = _userRepository.GetUserByInput(loginInput);

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
        public bool Register(string username, string password, string firstname, string? lastname, string email)
        {
            if (string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(firstname) ||
                string.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            if (_userRepository.UserExists(username, email))
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

            int rowsAffected = _userRepository.RegisterUser(newUser);
            return rowsAffected > 0;
        }
    }
}
