using Eventflow.Domain.Models.Entities;

namespace Eventflow.Application.Services.Interfaces
{
    public interface IUserService
    {
        public Task<User?> GetUserByUsernameAsync(string username);
        public Task<User?> GetUserByIdAsync(int userId);
    }
}
