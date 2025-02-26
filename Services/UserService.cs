using Eventflow.Data;
using Eventflow.Models.Models;

namespace Eventflow.Services
{
    public class UserService
    {
        private readonly DbHelper _dbHelper;
        public UserService(DbHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }
        public List<User> GetAllUsers()
        {
            string getAllUsersQuery = "Select "
        }
    }
}
