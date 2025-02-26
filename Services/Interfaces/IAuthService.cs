using Eventflow.Models.Models;

namespace Eventflow.Services.Interfaces
{
    public interface IAuthService
    {
        bool RegisterUser(User user);
        User? AuthenticateUser(string username, string password);
    }
}
