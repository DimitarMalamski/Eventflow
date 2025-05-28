using Eventflow.Application.Services.Interfaces;
using Eventflow.Domain.Exceptions;
using Eventflow.Domain.Interfaces.Repositories;
using Eventflow.Domain.Models.Entities;
using Eventflow.DTOs.DTOs;
using static Eventflow.Application.Helper.InputValidator;
using static Eventflow.Application.Security.PasswordHasher;
using static Eventflow.Domain.Common.CustomErrorMessages.Login;
using static Eventflow.Domain.Common.CustomErrorMessages.Register;
using static Eventflow.Domain.Common.CustomErrorMessages.UserService;
using Role = Eventflow.Domain.Enums.Role;

namespace Eventflow.Application.Services
{
    public class UserService : IAuthService, IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IInviteRepository _inviteRepository;
        private readonly IRoleRepository _roleRepository;
        public UserService(
            IUserRepository userRepository,
            IInviteRepository inviteRepository,
            IRoleRepository roleRepository)
        {
            _userRepository = userRepository
                ?? throw new ArgumentNullException(nameof(userRepository));
            _inviteRepository = inviteRepository
                ?? throw new ArgumentNullException(nameof(inviteRepository));
            _roleRepository = roleRepository
                ?? throw new ArgumentNullException(nameof(roleRepository));
        }
        public async Task<bool> SoftDeleteUserAsync(int userId) {
            bool success = await _userRepository.SoftDeleteUserAsync(userId);

            if (success) {
                await _inviteRepository.AutoDeclineInvitesOfDeletedUsersAsync();
            }

            return success;
        }
        public async Task<List<AdminUserDto>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllUsersAsync();

            return users.Select(u => new AdminUserDto {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email,
                RoleName = u.RoleId == (int)Role.Admin ? "Admin" : "User",
                Status = u.IsBanned ? "Banned" : "Active"
            })
            .ToList();
        }
        public async Task<List<RecentUserDto>> GetRecentUsersAsync(int count)
        {
            var users = await _userRepository.GetRecentUsersAsync(count);

            return users.Select(u => new RecentUserDto {
                Username = u.Username,
                Email = u.Email
            })
            .ToList();
        }
        public async Task<User?> GetUserByIdAsync(int userId)
        {
                if (userId <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(userId));
                }

                return await _userRepository.GetUserByIdAsync(userId);
        }
        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            if (!IsValidUsername(username)) 
            {
                throw new ArgumentException(userUsernameCannotBeNull, nameof(username));
            }

            return await _userRepository.GetByUsernameAsync(username);
        }
        public async Task<int> GetUserCountAsync()
            => await _userRepository.GetUserCountAsync();
        public async Task<User?> LoginAsync(string loginInput, string password)
        {
            if (string.IsNullOrWhiteSpace(loginInput)
                || string.IsNullOrWhiteSpace(password))
            {
                throw new InvalidLoginInputException(loginInputCannotBeNull);
            }

            loginInput = loginInput.Trim().ToLower();
            password = password.Trim();

            if (!IsValidLoginInput(loginInput))
            {
                throw new InvalidLoginInputException(loginInputInvalid);
            }

            if (!IsValidPassword(password))
            {
                throw new InvalidLoginInputException(loginPasswordInvalid);
            }

            User? user = await _userRepository.GetUserByInputAsync(loginInput);

            if (user == null 
                || !VerifyPassword(password, user.PasswordHash, user.Salt))
            {
                return null;
            }

            if (user.IsBanned) {
                throw new UserBannedException(userAccountHasBeenBanned);
            }

            return user;
        }
        public async Task<bool> RegisterAsync(string username, string password, string firstname, string? lastname, string email)
        {
            if (string.IsNullOrWhiteSpace(username)
                || string.IsNullOrWhiteSpace(password)
                || string.IsNullOrWhiteSpace(firstname)
                || string.IsNullOrWhiteSpace(email))
            {
                throw new InvalidRegistrationInputException(registerUserAlreadyExists);
            }

            username = username.Trim().ToLower();
            email = email.Trim().ToLower();
            password = password.Trim();
            firstname = firstname.Trim();
            lastname = lastname?.Trim();

            if (!IsValidUsername(username))
            {
                throw new InvalidRegistrationInputException(registerUsernameInvalid);
            }

            if (!IsValidPassword(password))
            {
                throw new InvalidRegistrationInputException(registerPasswordInvalid);
            }

            if (!IsValidFirstname(firstname))
            {
                throw new InvalidRegistrationInputException(registerFirstnameInvalid);
            }

            if (!IsValidEmail(email))
            {
                throw new InvalidRegistrationInputException(registerEmailInvalid);
            }

            if (await _userRepository.UserExistsAsync(username, email))
            {
                throw new InvalidRegistrationInputException(registerUserAlreadyExists);
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
                RoleId = (int)Role.User
            };

            int rowsAffected = await _userRepository.RegisterUserAsync(newUser);
            return rowsAffected > 0;
        }
        public async Task<bool> UpdateUserAsync(UserEditDto dto)
        {
            var user = new User {
                Id = dto.Id,
                Username = dto.Username,
                Email = dto.Email,
                IsBanned = dto.Status.Equals("Banned", StringComparison.OrdinalIgnoreCase),
            };

            return await _userRepository.UpdateAsync(user, dto.Role);
        }
        public async Task UpdateUserBanStatusAsync(int userId, bool isBanned)
            => await _userRepository.UpdateUserBanStatusAsync(userId, isBanned);
        public async Task<List<AdminUserDto>> GetFilteredAdminUsersAsync(string search, string role, string status)
        {
            var users = await _userRepository.GetAllUsersAsync();

            var rolesMap = await _roleRepository.GetRoleIdToNameMapAsync();

            var result = users
                .Select(u => new AdminUserDto {
                    Id = u.Id,
                    Username = u.Username,
                    Email = u.Email,
                    RoleName = rolesMap[u.RoleId],
                    Status = u.IsBanned ? "Banned" : "Active"
                })
                .Where(u =>
                (string.IsNullOrWhiteSpace(search) || u.Username.Contains(search, StringComparison.OrdinalIgnoreCase)) &&
                (role == "All" || u.RoleName == role) &&
                (status == "All" || u.Status == status)
            )
            .ToList();

            return result;
        }
   }
}
