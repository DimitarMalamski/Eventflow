using Eventflow.Domain.Models.Models;

namespace Eventflow.Domain.Interfaces.Repositories
{
    public interface IUserRepository
    {
        public Task<User?> GetUserByInputAsync(string username);
        public Task<bool> UserExistsAsync(string username, string email);
        public Task<int> RegisterUserAsync(User user);
        public Task<User?> GetByUsernameAsync(string username);
    }
}
