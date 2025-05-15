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
    }
}
