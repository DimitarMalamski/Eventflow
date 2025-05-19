using System.ComponentModel;
using Eventflow.Domain.Models.Entities;
using Eventflow.DTOs.DTOs;

namespace Eventflow.Application.Services.Interfaces
{
    public interface IUserService
    {
        public Task<User?> GetUserByUsernameAsync(string username);
        public Task<User?> GetUserByIdAsync(int userId);
        public Task<int> GetUserCountAsync();
        public Task<List<RecentUserDto>> GetRecentUsersAsync(int count);
        public Task<List<AdminUserDto>> GetAllUsersAsync();
        public Task<bool> UpdateUserAsync(UserEditDto dto);
        public Task UpdateUserBanStatusAsync(int userId, bool isBanned);
        public Task<bool> SoftDeleteUserAsync(int id);
        public Task<List<AdminUserDto>> GetFilteredAdminUsersAsync(string search, string role, string status);
    }
}
