using Eventflow.Domain.Models.Models;

namespace Eventflow.Application.Services.Interfaces
{
    public interface IAuthService
    {
        public Task<User?> LoginAsync(string loginInput, string password);
        public Task<bool> RegisterAsync(string username, string password, string firstname, string? lastname, string email);
    }
}
