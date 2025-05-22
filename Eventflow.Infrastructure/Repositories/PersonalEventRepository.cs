using Eventflow.Domain.Enums;
using Eventflow.Domain.Helper;
using Eventflow.Domain.Interfaces.Repositories;
using Eventflow.Domain.Models.Entities;
using Eventflow.Infrastructure.Data.Interfaces;
using Microsoft.Identity.Client;
using System.Data;
using System.Runtime.CompilerServices;

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
                        AND i.StatusId = @AcceptedStatus
                        AND CONVERT(date, e.Date) >= CONVERT(date, GETDATE())
                        AND e.IsDeleted = 0;";

            var parameters = new Dictionary<string, object>
            {
                { "@UserId", userId },
                { "@AcceptedStatus", InviteStatusHelper.Accepted }
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
            string getPersonalEventQuery = @"SELECT * FROM PersonalEvent
                                         WHERE Id = @Id
                                         AND IsDeleted = 0";

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
                    AND IsDeleted = 0;";

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
                var ev = new PersonalEvent
                {
                    Id = Convert.ToInt32(row["Id"]),
                    Title = row["Title"].ToString()!,
                    Description = row["Description"] == DBNull.Value ? "None" : row["Description"].ToString()!,
                    Date = Convert.ToDateTime(row["Date"]),
                    IsCompleted = Convert.ToBoolean(row["IsCompleted"]),
                    UserId = Convert.ToInt32(row["UserId"]),
                    CategoryId = row["CategoryId"] != DBNull.Value ? Convert.ToInt32(row["CategoryId"]) : null
                };

                if (ev.Date.Date < DateTime.Today && !ev.IsCompleted)
                {
                    ev.IsCompleted = true;
                    await UpdateIsCompleteAsync(ev.Id, true);
                }

                events.Add(ev);
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
        private async Task UpdateIsCompleteAsync(int eventId, bool isComplete)
        {
            string updateIsCompleteStatusQuery = @"
                    UPDATE PersonalEvent
                    SET IsCompleted = @IsCompleted
                    WHERE Id = @Id";

            var parameters = new Dictionary<string, object>
            {
                { "@IsCompleted", isComplete },
                { "@Id", eventId }
            };

            await _dbHelper.ExecuteNonQueryAsync(updateIsCompleteStatusQuery, parameters);
        }
        public async Task<bool> UserHasAccessToEventAsync(int eventId, int userId)
        {
            string hasAccesstoEventQuery = @"
                    SELECT 1
                    WHERE EXISTS (
                        SELECT 1
                        FROM PersonalEvent
                        WHERE Id = @EventId 
                        AND UserId = @UserId
                        AND IsDeleted = 0
                    )
                    OR EXISTS (
                        SELECT 1
                        FROM Invite i
                        INNER JOIN PersonalEvent e ON i.PersonalEventId = e.Id
                        WHERE i.PersonalEventId = @EventId 
                        AND i.InvitedUserId = @UserId 
                        AND i.StatusId = @AcceptedStatusId
                        AND e.IsDeleted = 0
                    );";

            var parameters = new Dictionary<string, object>
            {
                { "@EventId", eventId },
                { "@UserId", userId },
                { "@AcceptedStatusId", (int)InviteStatus.Accepted }
            };

            var result = await _dbHelper.ExecuteScalarAsync(hasAccesstoEventQuery, parameters);
            return result != null;
        }
        public async Task<int> GetPersonalEventsCountAsync()
        {
            string getPersonalEventCountQuery = @"SELECT COUNT(*) 
                                        FROM [PersonalEvent]
                                        WHERE IsDeleted = 0";

            var count = await _dbHelper.ExecuteScalarAsync(getPersonalEventCountQuery);

            return Convert.ToInt32(count);
        }
        public async Task<List<PersonalEvent>> GetRecentPersonalEventsAsync(int count)
        {
            string recentPersonalEventQuery = @"
                        SELECT TOP(@Count) *
                        FROM PersonalEvent
                        WHERE IsDeleted = 0
                        ORDER BY Id DESC";
            
            var parameters = new Dictionary<string, object>() {
                { "@Count", count }
            };

            var dt = await _dbHelper.ExecuteQueryAsync(recentPersonalEventQuery, parameters);

            var result = new List<PersonalEvent>();

            foreach (DataRow row in dt.Rows) {
                result.Add(new PersonalEvent {
                    Id = Convert.ToInt32(row["Id"]),
                    Title = row["Title"].ToString()!,
                    Description = row["Description"] == DBNull.Value ? null : row["Description"].ToString(),
                    Date = Convert.ToDateTime(row["Date"]),
                    UserId = Convert.ToInt32(row["UserId"]),
                    CategoryId = row["CategoryId"] == DBNull.Value ? null : Convert.ToInt32(row["CategoryId"]),
                    IsCompleted = Convert.ToBoolean(row["IsCompleted"])
                });
            }

            return result;
        }
        public async Task<List<PersonalEvent>> GetAllPersonalEventsAsync()
        {
            string getAllPersonalEventsQuery = @"SELECT * FROM [PersonalEvent] 
                                    WHERE IsDeleted = 0
                                    ORDER BY Id DESC";

            var dt = await _dbHelper.ExecuteQueryAsync(getAllPersonalEventsQuery);

            var result = new List<PersonalEvent>();

            foreach (DataRow row in dt.Rows) {
                result.Add(new PersonalEvent {
                    Id = Convert.ToInt32(row["Id"]),
                    Title = row["Title"].ToString()!,
                    Description = row["Description"] == DBNull.Value ? "None" : row["Description"].ToString()!,
                    Date = Convert.ToDateTime(row["Date"]),
                    IsCompleted = Convert.ToBoolean(row["IsCompleted"]),
                    UserId = Convert.ToInt32(row["UserId"]),
                    CategoryId = row["CategoryId"] != DBNull.Value ? Convert.ToInt32(row["CategoryId"]) : null
                });
            }

            return result;
        }
        public async Task SoftDeleteEventAsync(int eventId)
        {
            string softDeleteEventQuery = @"UPDATE [PersonalEvent] 
                                        SET IsDeleted = 1
                                        WHERE Id = @Id";
                                    
            var parameters = new Dictionary<string, object>() {
                { "@Id", eventId }
            };

            await _dbHelper.ExecuteNonQueryAsync(softDeleteEventQuery, parameters);
        }
   }
}
