using Eventflow.Models.Models;
using Eventflow.Repositories.Interfaces;
using Eventflow.Services.Interfaces;
using Eventflow.Utilities;

namespace Eventflow.Services
{
    public class UserService : IUserService
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

            if (PasswordHasher.VerifyPassword(password, user.PasswordHash))
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

            string hashedPassword = PasswordHasher.HashPassword(password);

            User newUser = new User
            {
                Username = username,
                PasswordHash = hashedPassword,
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
