using Eventflow.Domain.Interfaces.Repositories;
using Eventflow.Domain.Models.Models;
using Eventflow.Infrastructure.Data.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Eventflow.Infrastructure.Repositories
{
    public class PersonalEventRepository : IPersonalEventRepository
    {
        private readonly IDbHelper _dbHelper;
        public PersonalEventRepository(IDbHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }
        public async Task CreateEventAsync(PersonalEvent personalEvent)
        {
            string insertPersonalEventQuery = @"
                        INSERT INTO PersonalEvent (Title, Description, Date, UserId, IsCompleted, CategoryId)
                        VALUES (@Title, @Description, @Date, @UserId, @IsCompleted, @CategoryId);";

            var parameters = new Dictionary<string, object?>
            {
                { "@Title", personalEvent.Title },
                { "@Description", (object?)personalEvent.Description ?? DBNull.Value },
                { "@Date", personalEvent.Date },
                { "@UserId", personalEvent.UserId },
                { "@IsCompleted", personalEvent.IsCompleted },
                { "@CategoryId", (object?)personalEvent.CategoryId ?? DBNull.Value }
            };

            await _dbHelper.ExecuteNonQueryAsync(insertPersonalEventQuery, parameters!);
        }
        public async Task<List<PersonalEvent>> GetAcceptedInvitedEventsAsync(int userId)
        {
            string getAllAcceptedInvitedEventsQuery = @"
                    SELECT e.*
                    FROM Invite i
                    INNER JOIN PersonalEvent e ON i.PersonalEventId = e.Id
                    WHERE i.InvitedUserId = @UserId
                    AND i.StatusId = 2
                    AND CONVERT(date, e.Date) >= CONVERT(date, GETDATE());";

            var parameters = new Dictionary<string, object>
            {
                { "@UserId", userId }
            };

            var dt = await _dbHelper.ExecuteQueryAsync(getAllAcceptedInvitedEventsQuery, parameters);

            var events = new List<PersonalEvent>();

            foreach (DataRow row in dt.Rows)
            {
                events.Add(new PersonalEvent
                {
                    Id = Convert.ToInt32(row["Id"]),
                    Title = row["Title"].ToString()!,
                    Description = row["Description"]?.ToString(),
                    Date = Convert.ToDateTime(row["Date"]),
                    IsCompleted = Convert.ToBoolean(row["IsCompleted"]),
                    CategoryId = row["CategoryId"] != DBNull.Value ? Convert.ToInt32(row["CategoryId"]) : null,
                    UserId = Convert.ToInt32(row["UserId"]),
                });
            }

            return events;
        }
        public async Task<PersonalEvent?> GetPersonalEventByIdAsync(int id)
        {
            string getPersonalEventQuery = "SELECT * FROM PersonalEvent WHERE Id = @Id";

            var parameters = new Dictionary<string, object>
            {
                { "@Id", id },
            };

            var dt = await _dbHelper.ExecuteQueryAsync(getPersonalEventQuery, parameters);

            if (dt.Rows.Count == 0)
            {
                return null;
            }

            var row = dt.Rows[0];

            return new PersonalEvent
            {
                Id = Convert.ToInt32(row["Id"]),
                Title = row["Title"].ToString()!,
                Description = row["Description"].ToString(),
                Date = Convert.ToDateTime(row["Date"]),
                CategoryId = row["CategoryId"] != DBNull.Value ? Convert.ToInt32(row["CategoryId"]) : (int?)null,
                UserId = Convert.ToInt32(row["UserId"]),
                IsCompleted = Convert.ToBoolean(row["IsCompleted"])
            };
        }
        public async Task<List<PersonalEvent>> GetByUserAndMonthAsync(int userId, int year, int month)
        {
            var monthStart = new DateTime(year, month, 1);
            var monthEnd = monthStart.AddMonths(1).AddDays(-1);

            var allEventForThisMonthQuery = @"
                    SELECT * 
                    FROM PersonalEvent 
                    WHERE UserId = @UserId 
                    AND Date >= @StartDate 
                    AND Date <= @EndDate
                    AND CONVERT(date, Date) >= CONVERT(date, GETDATE())";

            var parameters = new Dictionary<string, object?>()
            {
                { "@UserId", userId },
                { "@StartDate", monthStart },
                { "@EndDate", monthEnd }
            };

            var dt = await _dbHelper.ExecuteQueryAsync(allEventForThisMonthQuery, parameters!);

            List<PersonalEvent> events = new List<PersonalEvent>();

            foreach (DataRow row in dt.Rows)
            {
                events.Add(new PersonalEvent
                {
                    Id = Convert.ToInt32(row["Id"]),
                    Title = row["Title"].ToString()!,
                    Description = row["Description"] == DBNull.Value ? "None" : row["Description"].ToString()!,
                    Date = Convert.ToDateTime(row["Date"]),
                    IsCompleted = Convert.ToBoolean(row["IsCompleted"]),
                    UserId = Convert.ToInt32(row["UserId"]),
                    CategoryId = row["CategoryId"] != DBNull.Value ? Convert.ToInt32(row["CategoryId"]) : null
                });
            }

            return events;
        }
        public async Task UpdatePersonalEventAsync(PersonalEvent personalEvent)
        {
            string updatePersonalEventQuery = @"
                    UPDATE PersonalEvent
                    SET
                        Title = @Title,
                        Description = @Description,
                        Date = @Date,
                        CategoryId = @CategoryId
                    WHERE Id = @Id";

            var parameters = new Dictionary<string, object>
            {
                { "@Id", personalEvent.Id },
                { "@Title", personalEvent.Title },
                { "@Description", personalEvent.Description ?? string.Empty },
                { "@Date", personalEvent.Date },
                { "@CategoryId", personalEvent.CategoryId ?? (object)DBNull.Value }
            };

            await _dbHelper.ExecuteNonQueryAsync(updatePersonalEventQuery, parameters);
        }
        public async Task<List<PersonalEvent>> GetAllPersonalEventsByUserIdAsync(int userId)
        {
            string getAllPersonalEventsByUserIdQuery = @"
                                    SELECT * FROM PersonalEvent
                                    WHERE UserId = @UserId
                                    AND Date >= CAST(GETDATE() AS DATE)";

            var parameters = new Dictionary<string, object>
            {
                { "@UserId", userId }
            };

            var dt = await _dbHelper.ExecuteQueryAsync(getAllPersonalEventsByUserIdQuery, parameters);

            List<PersonalEvent> events = new List<PersonalEvent>();

            foreach (DataRow row in dt.Rows)
            {
                events.Add(new PersonalEvent
                {
                    Id = Convert.ToInt32(row["Id"]),
                    Title = row["Title"].ToString()!,
                    Description = row["Description"]?.ToString(),
                    Date = Convert.ToDateTime(row["Date"]),
                    IsCompleted = Convert.ToBoolean(row["IsCompleted"]),
                    CategoryId = row["CategoryId"] != DBNull.Value ? Convert.ToInt32(row["CategoryId"]) : null,
                    UserId = Convert.ToInt32(row["UserId"])
                });
            }

            return events;
        }
    }
}
