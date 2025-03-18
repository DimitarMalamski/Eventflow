using Eventflow.Models.Models;

namespace Eventflow.Repositories.Interfaces
{
    public interface IUserRepository
    {
        User? GetUserByUsername(string username);
        bool UserExists(string username, string email);
        int RegisterUser(User user);
    }
}
