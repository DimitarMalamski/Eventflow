using Eventflow.Models.Models;

namespace Eventflow.Repositories.Interfaces
{
    public interface IUserRepository
    {
        public Task<User?> GetUserByInputAsync(string username);
        public Task<bool> UserExistsAsync(string username, string email);
        public Task<int> RegisterUserAsync(User user);
    }
}
