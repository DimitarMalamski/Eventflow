using Eventflow.Domain.Interfaces.Repositories;
using Eventflow.Domain.Models.Entities;
using Eventflow.Infrastructure.Data.Interfaces;
using System.Data;

namespace Eventflow.Infrastructure.Repositories
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
            string getuserQuery = @"SELECT * FROM [User] 
                                WHERE LOWER([Username]) = LOWER(@Input)
                                OR LOWER([Email]) = LOWER(@Input)";

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@Input", input }
            };

            DataTable dt = await _dbHelper.ExecuteQueryAsync(getuserQuery, parameters);

            if (dt.Rows.Count == 0)
            {
                return null;
            }

            return MapUser(dt.Rows[0]);
        }
        public async Task<bool> UserExistsAsync(string username, string email)
        {
            string userExistsQuery = @"SELECT COUNT(*) FROM [User]
                            WHERE LOWER(Username) = LOWER(@Username)
                            OR LOWER(Email) = LOWER(@Email)";
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

            return MapUser(dt.Rows[0]);
        }
        public async Task<User?> GetUserByIdAsync(int userId)
        {
            string getUserByIdQuery = "SELECT * FROM [User] WHERE Id = @Id";

            var parameters = new Dictionary<string, object>()
            {
                { "@Id", userId }
            };

            var dt = await _dbHelper.ExecuteQueryAsync(getUserByIdQuery, parameters);

            if (dt.Rows.Count == 0)
            {
                return null;
            }

            return MapUser(dt.Rows[0]);
        }
        public async Task<List<string>> GetUsernamesByEventIdAsync(int eventId)
        {
            string getUsernamesByEventIdQuery = @"
                    SELECT u.Username
                    FROM [User] u
                    WHERE u.Id IN (

                        SELECT pe.UserId
                        FROM PersonalEvent pe
                        WHERE pe.Id = @eventId

                        UNION

                        SELECT i.InvitedUserId
                        FROM Invite i
                        WHERE i.PersonalEventId = @eventId
                          AND i.StatusId = (
                              SELECT Id FROM Status WHERE Name = 'Accepted'
                          )
                    );";

            var parameters = new Dictionary<string, object>()
            {
                { "@EventId", eventId }
            };

            var dt = await _dbHelper.ExecuteQueryAsync(getUsernamesByEventIdQuery, parameters);

            var usernames = new List<string>();

            foreach (DataRow row in dt.Rows)
            {
                usernames.Add(row["Username"].ToString() ?? "Unknown");
            }

            return usernames;
        }
        private User MapUser(DataRow row) => new User
        {
            Id = Convert.ToInt32(row["Id"]),
            Username = row["Username"].ToString()!,
            PasswordHash = row["PasswordHash"].ToString()!,
            Salt = row["Salt"].ToString()!,
            Firstname = row["Firstname"].ToString()!,
            Lastname = row["Lastname"].ToString() ?? "",
            Email = row["Email"].ToString()!,
            RoleId = Convert.ToInt32(row["RoleId"]),
            IsBanned = Convert.ToBoolean(row["IsBanned"])
        };
        public async Task<int> GetUserCountAsync()
        {
            string getUserCountQuery = @"SELECT COUNT(*) FROM [User]";

            var count = await _dbHelper.ExecuteScalarAsync(getUserCountQuery);

            return Convert.ToInt32(count);
        }
        public async Task<List<User>> GetRecentUsersAsync(int count)
        {
            string getRecentUsersQuery = @"
                        SELECT TOP (@Count) *
                        FROM [User]
                        ORDER BY [Id] DESC";
            
            var parameters = new Dictionary<string, object>() {
                { "@Count", count }
            };

            var dt = await _dbHelper.ExecuteQueryAsync(getRecentUsersQuery, parameters);

            List<User> users = new List<User>();

            foreach (DataRow row in dt.Rows) {
                users.Add(MapUser(row));
            }

            return users;
        }
        public async Task<Dictionary<int, string>> GetUsernamesByIdsAsync(List<int> userIds)
        { 
            if (userIds == null || 
                userIds.Count == 0) {
                return new Dictionary<int, string>();
            }

            var paramList = userIds
                .Select((id, index) => $"@Id{index}")
                .ToList();

            string getUserUsernamesByIdsQuery = $"SELECT Id, Username FROM [User] WHERE Id IN ({string.Join(",", paramList)})";

            var parameters = userIds
                .Select((id, index) => new KeyValuePair<string, object>($"@Id{index}", id))
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            var dt = await _dbHelper.ExecuteQueryAsync(getUserUsernamesByIdsQuery, parameters);

            var map = new Dictionary<int, string>();

            foreach (DataRow row in dt.Rows) {
                map[Convert.ToInt32(row["Id"])] = row["Username"].ToString()!;
            }

            return map;
        }
        public async Task<List<User>> GetAllUsersAsync()
        {
            string getAllUsersQuery = "SELECT * FROM [User]";

            var dt = await _dbHelper.ExecuteQueryAsync(getAllUsersQuery);

            var users = new List<User>();

            foreach (DataRow row in dt.Rows) {
                users.Add(MapUser(row));
            }

            return users;
        }
        public async Task<bool> UpdateAsync(User user, string roleName)
        {
            string updateUserQuery = @"
                    UPDATE [User]
                    SET 
                        Username = @Username,
                        Email = @Email,
                        RoleId = (SELECT Id FROM [Role] WHERE Name = @RoleName),
                        IsBanned = @IsBanned
                    WHERE Id = @Id";

            var parameters = new Dictionary<string, object>() {
                { "@Id", user.Id },
                { "@Username", user.Username },
                { "@Email", user.Email },
                { "@RoleName", roleName },
                { "@IsBanned", user.IsBanned }
            };

            int rowsAffected = await _dbHelper.ExecuteNonQueryAsync(updateUserQuery, parameters);
            return rowsAffected > 0;
        }
   }
}
