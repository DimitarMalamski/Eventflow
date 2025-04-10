using Eventflow.Domain.Models.Models;

namespace Eventflow.Application.Services.Interfaces
{
    public interface IUserService
    {
        public Task<User?> GetUserByUsernameAsync(string username);
    }
}
