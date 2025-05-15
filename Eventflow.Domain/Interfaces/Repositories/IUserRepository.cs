using Eventflow.Domain.Models.Entities;

namespace Eventflow.Domain.Interfaces.Repositories
{
    public interface IUserRepository
    {
        public Task<User?> GetUserByInputAsync(string username);
        public Task<bool> UserExistsAsync(string username, string email);
        public Task<int> RegisterUserAsync(User user);
        public Task<User?> GetByUsernameAsync(string username);
        public Task<User?> GetUserByIdAsync(int userId);
        public Task<List<string>> GetUsernamesByEventIdAsync(int eventId);
        public Task<int> GetUserCountAsync();
        public Task<List<User>> GetRecentUsersAsync(int count);
        public Task<Dictionary<int, string>> GetUsernamesByIdsAsync(List<int> userIds);
    }
}
