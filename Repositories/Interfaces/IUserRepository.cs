using Eventflow.Models.Models;

namespace Eventflow.Repositories.Interfaces
{
    public interface IUserRepository
    {
        User? GetUserByInput(string username);
        bool UserExists(string username, string email);
        int RegisterUser(User user);
    }
}
