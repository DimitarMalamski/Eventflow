using Eventflow.Data;
using Eventflow.Models.Models;
using Eventflow.Repositories.Interfaces;
using System.Data;

namespace Eventflow.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DbHelper _dbHelper;
        public UserRepository(DbHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }
        public User? GetUserByInput(string input)
        {
            string getuserQuery = @"SELECT * FROM [User] WHERE [Username] = @Input OR [Email] = @Input";

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@Input", input }
            };

            DataTable dt = _dbHelper.ExecuteQuery(getuserQuery, parameters);

            if (dt.Rows.Count == 0)
            {
                return null;
            }

            DataRow row = dt.Rows[0];

            return new User
            {
                Id = Convert.ToInt32(row["Id"]),
                Username = row["Username"].ToString()!,
                PasswordHash = row["PasswordHash"].ToString()!,
                Firstname = row["Firstname"].ToString()!,
                Lastname = row["Lastname"].ToString() ?? "",
                Email = row["Email"].ToString()!,
                RoleId = Convert.ToInt32(row["RoleId"])
            };
        }
        public bool UserExists(string username, string email)
        {
            string userExistsQuery = "SELECT COUNT(*) FROM [User] WHERE Username = @Username OR Email = @Email";
            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@Username", username },
                { "@Email", email }
            };

            int count = Convert.ToInt32(_dbHelper.ExecuteScalar(userExistsQuery, parameters));
            return count > 0;
        }
        public int RegisterUser(User user)
        {
            string registerUserQuery = @"
            INSERT INTO [User] (Username, PasswordHash, Firstname, Lastname, Email, RoleId) 
            VALUES (@Username, @PasswordHash, @Firstname, @Lastname, @Email, @RoleId)";

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@Username", user.Username },
                { "@PasswordHash", user.PasswordHash },
                { "@Firstname", user.Firstname },
                { "@Lastname", user.Lastname! },
                { "@Email", user.Email },
                { "@RoleId", user.RoleId }
            };

            return _dbHelper.ExecuteNonQuery(registerUserQuery, parameters);
        }
    }
}
