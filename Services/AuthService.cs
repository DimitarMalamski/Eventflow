using Eventflow.Data;
using Eventflow.Models.Models;
using Eventflow.Services.Interfaces;

namespace Eventflow.Services
{
    public class AuthService : IAuthService
    {
        private readonly DbHelper _dbHelper;
        public AuthService(DbHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        public User? AuthenticateUser(string username, string password)
        {
            throw new NotImplementedException();
        }

        public bool RegisterUser(User user)
        {
            string registerUserQuery = "INSERT INTO [User] (Username, Email, PasswordHash, RoleId, Firstname, Lastname) " +
                           "VALUES (@Username, @Email, @PasswordHash, @RoleId, @Firstname, @Lastname)";
        }
    }
}
