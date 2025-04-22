using Eventflow.Domain.Interfaces.Repositories;
using Eventflow.Domain.Models.Models;
using Eventflow.Infrastructure.Data.Interfaces;
using System.Data;

namespace Eventflow.infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IDbHelper _dbHelper;
        public UserRepository(IDbHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }
        public async Task<User?> GetUserByInputAsync(string input)
        {
            string getuserQuery = @"SELECT * FROM [User] WHERE [Username] = @Input OR [Email] = @Input";

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@Input", input }
            };

            DataTable dt = await _dbHelper.ExecuteQueryAsync(getuserQuery, parameters);

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
                Salt = row["Salt"].ToString()!,
                Firstname = row["Firstname"].ToString()!,
                Lastname = row["Lastname"].ToString() ?? "",
                Email = row["Email"].ToString()!,
                RoleId = Convert.ToInt32(row["RoleId"])
            };
        }
        public async Task<bool> UserExistsAsync(string username, string email)
        {
            string userExistsQuery = "SELECT COUNT(*) FROM [User] WHERE Username = @Username OR Email = @Email";
            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@Username", username },
                { "@Email", email }
            };

            int count = Convert.ToInt32(await _dbHelper.ExecuteScalarAsync(userExistsQuery, parameters));
            return count > 0;
        }
        public async Task<int> RegisterUserAsync(User user)
        {
            string registerUserQuery = @"
            INSERT INTO [User] (Username, PasswordHash, Salt, Firstname, Lastname, Email, RoleId) 
            VALUES (@Username, @PasswordHash, @Salt, @Firstname, @Lastname, @Email, @RoleId)";

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@Username", user.Username },
                { "@PasswordHash", user.PasswordHash },
                { "@Salt", user.Salt },
                { "@Firstname", user.Firstname },
                { "@Lastname", user.Lastname! },
                { "@Email", user.Email },
                { "@RoleId", user.RoleId }
            };

            return await _dbHelper.ExecuteNonQueryAsync(registerUserQuery, parameters);
        }
        public async Task<User?> GetByUsernameAsync(string username)
        {
            string getUserByUsernameQuery = "SELECT * FROM [User] WHERE Username = @Username";

            var parameters = new Dictionary<string, object>()
            {
                { "@Username", username }
            };

            var dt = await _dbHelper.ExecuteQueryAsync(getUserByUsernameQuery, parameters);

            if (dt.Rows.Count == 0)
            {
                return null;
            }

            var row = dt.Rows[0];

            return new User
            {
                Id = Convert.ToInt32(row["Id"]),
                Username = row["Username"].ToString()!,
                PasswordHash = row["PasswordHash"].ToString()!,
                Salt = row["Salt"].ToString()!,
                Firstname = row["Firstname"].ToString()!,
                Lastname = row["Lastname"].ToString() ?? "",
                Email = row["Email"].ToString()!,
                RoleId = Convert.ToInt32(row["RoleId"])
            };
        }
        public async Task<User?> GetUserByIdAsync(int userId)
        {
            string getUserByIdQuery = "SELECT * FROM [User] WHERE Id = @Id";

            var parameters = new Dictionary<string, object>()
            {
                { "@Id", userId }
            };

            var table = await _dbHelper.ExecuteQueryAsync(getUserByIdQuery, parameters);

            if (table.Rows.Count == 0)
            {
                return null;
            }

            var row = table.Rows[0];

            return new User
            {
                Id = Convert.ToInt32(row["Id"]),
                Username = row["Username"].ToString()!,
                PasswordHash = row["PasswordHash"].ToString()!,
                Salt = row["Salt"].ToString()!,
                Firstname = row["Firstname"].ToString()!,
                Email = row["Email"].ToString()!,
                RoleId = Convert.ToInt32(row["RoleId"])
            };
        }
    }
}
