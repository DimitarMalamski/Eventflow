using Eventflow.Models.Models;

namespace Eventflow.Services.Interfaces
{
    public interface IAuthService
    {
        User? Login(string username, string password);
        bool Register(string username, string password, string firstname, string? lastname, string email);
    }
}
