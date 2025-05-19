using Eventflow.Domain.Enums;
using Eventflow.Domain.Interfaces.Repositories;
using Eventflow.Domain.Models.Entities;
using Eventflow.Infrastructure.Data.Interfaces;
using System.Data;

namespace Eventflow.Infrastructure.Repositories
{
    public class PersonalEventReminderRepository : IPersonalEventReminderRepository
    {
        private readonly IDbHelper _dbHelper;
        public PersonalEventReminderRepository(IDbHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }
        public async Task CreatePersonalReminderAsync(PersonalEventReminder reminder)
        {
            string createPersonalReminderQuery = @"
                INSERT INTO PersonalEventReminder (Title, Description, Date, IsRead, IsLiked, PersonalEventId, UserId)
                VALUES (@Title, @Description, @Date, 0, 0, @PersonalEventId, @UserId);";

            var parameters = new Dictionary<string, object?>()
            {
                { "@Title", reminder.Title },
                { "@Description", (object?)reminder.Description ?? DBNull.Value },
                { "@Date", reminder.Date },
                { "@PersonalEventId", reminder.PersonalEventId },
                { "@UserId", reminder.UserId }
            };

            await _dbHelper.ExecuteNonQueryAsync(createPersonalReminderQuery, parameters!);
        }
        public async Task<PersonalEventReminder?> GetPersonalReminderByIdAsync(int reminderId)
        {
            string getPersonalReminderByIdQuery = @"
                                        SELECT 
                                            r.*,
                                            e.UserId AS EventUserId
                                        FROM PersonalEventReminder r
                                        INNER JOIN PersonalEvent e ON r.PersonalEventId = e.Id
                                        INNER JOIN [User] u ON e.UserId = u.Id
                                        WHERE r.Id = @Id
                                        AND u.IsDeleted = 0";

            var parameters = new Dictionary<string, object>()
            {
                { "@Id", reminderId }
            };

            var dt = await _dbHelper.ExecuteQueryAsync(getPersonalReminderByIdQuery, parameters);

            if (dt.Rows.Count == 0)
            {
                return null;
            }

            var row = dt.Rows[0];

            return new PersonalEventReminder
            {
                Id = reminderId,
                Title = row["Title"].ToString()!,
                Description = row["Description"]?.ToString(),
                Date = Convert.ToDateTime(row["Date"]),
                IsLiked = Convert.ToBoolean(row["IsLiked"]),
                Status = Convert.ToBoolean(row["IsRead"]) 
                    ? ReminderStatus.Read 
                    : ReminderStatus.Unread,
                PersonalEventId = Convert.ToInt32(row["PersonalEventId"]),
                UserId = Convert.ToInt32(row["UserId"]),
                ReadAt = row["ReadAt"] != DBNull.Value ? Convert.ToDateTime(row["ReadAt"]) : null,
                PersonalEvent = new PersonalEvent
                {
                    UserId = Convert.ToInt32(row["EventUserId"])
                }
            };
        }
        public async Task<List<PersonalEventReminder>> GetPersonalRemindersWithEventAndTitleByUserIdAsync(int userId)
        {
            string getAllPersonalRemindersWithEventTitleByUserIdQuery = @"
                        SELECT 
                            r.*, 
                            e.Id AS EventId,
                            e.Title AS EventTitle,
                            e.Description AS EventDescription,
                            e.Date AS EventDate,
                            e.UserId AS EventUserId,
                            e.CategoryId
                        FROM PersonalEventReminder r
                        INNER JOIN PersonalEvent e ON r.PersonalEventId = e.Id
                        INNER JOIn [User] u ON e.UserId = u.Id
                        WHERE r.UserId = @UserId
                        AND u.IsDeleted = 0";

            var parameters = new Dictionary<string, object>
            {
                { "@UserId", userId }
            };

            var dt = await _dbHelper.ExecuteQueryAsync(getAllPersonalRemindersWithEventTitleByUserIdQuery, parameters);
            var reminders = new List<PersonalEventReminder>();

            foreach (DataRow row in dt.Rows)
            {
                var reminder = new PersonalEventReminder
                {
                    Id = Convert.ToInt32(row["Id"]),
                    Title = row["Title"].ToString()!,
                    Description = row["Description"]?.ToString(),
                    Date = Convert.ToDateTime(row["Date"]),
                    Status = Convert.ToBoolean(row["IsRead"]) ? ReminderStatus.Read : ReminderStatus.Unread,
                    PersonalEventId = Convert.ToInt32(row["PersonalEventId"]),
                    PersonalEvent = new PersonalEvent
                    {
                        Id = Convert.ToInt32(row["EventId"]),
                        Title = row["EventTitle"].ToString()!,
                        Description = row["EventDescription"]?.ToString(),
                        Date = Convert.ToDateTime(row["EventDate"]),
                        UserId = Convert.ToInt32(row["EventUserId"]),
                        CategoryId = row["CategoryId"] != DBNull.Value ? Convert.ToInt32(row["CategoryId"]) : null
                    }
                };

                reminders.Add(reminder);
            }

            return reminders;
        }
        public async Task<List<PersonalEventReminder>> GetReadPersonalRemindersWithin3DaysAsync(int userId)
        {
            string getReadPersonalRemindersWithin3DaysQuery = @"
                        SELECT 
                            r.*, 
                            e.Id AS EventId,
                            e.Title AS EventTitle,
                            e.Description AS EventDescription,
                            e.Date AS EventDate,
                            e.UserId AS EventUserId,
                            e.CategoryId
                        FROM PersonalEventReminder r
                        INNER JOIN PersonalEvent e ON r.PersonalEventId = e.Id
                        INNER JOIN [User] u ON e.UserId = u.Id
                        WHERE r.UserId = @UserId
                        AND r.IsRead = 1
                        AND (
                            r.IsLiked = 1 OR 
                            DATEDIFF(DAY, r.ReadAt, GETDATE()) <= 3
                        )
                        AND u.IsDeleted = 0;";

            var parameters = new Dictionary<string, object>
            {
                { "@UserId", userId }
            };

            var dt = await _dbHelper.ExecuteQueryAsync(getReadPersonalRemindersWithin3DaysQuery, parameters);
            return MapPersonalReminderWithEvent(dt);
        }
        public async Task<List<PersonalEventReminder>> GetUnreadPersonalRemindersForTodayAsync(int userId)
        {
            string getUnreadPersonalRemindersForTodayQuery = @"
                        SELECT 
                            r.*, 
                            e.Id AS EventId,
                            e.Title AS EventTitle,
                            e.Description AS EventDescription,
                            e.Date AS EventDate,
                            e.UserId AS EventUserId,
                            e.CategoryId
                        FROM PersonalEventReminder r
                        INNER JOIN PersonalEvent e ON r.PersonalEventId = e.Id
                        INNER JOIN [User] u ON e.UserId = u.Id
                        WHERE r.UserId = @UserId
                        AND r.IsRead = 0
                        AND r.Date = CAST(GETDATE() AS DATE)
                        AND u.IsDeleted = 0;";

            var parameters = new Dictionary<string, object>
            {
                { "@UserId", userId }
            };

            var dt = await _dbHelper.ExecuteQueryAsync(getUnreadPersonalRemindersForTodayQuery, parameters);
            return MapPersonalReminderWithEvent(dt);
        }
        public async Task LikePersonalReminderAsync(int reminderId, int userId)
        {
            string likePersonalReminderQuery = @"UPDATE PersonalEventReminder 
                    SET IsLiked = 1 
                    WHERE Id = @Id
                    AND UserId = @UserId";

            var parameters = new Dictionary<string, object>
            {
                { "@Id", reminderId },
                { "@UserId", userId }
            };

            await _dbHelper.ExecuteNonQueryAsync(likePersonalReminderQuery, parameters);
        }
        public async Task MarkPersonalReminderAsReadAsync(int reminderId, int userId)
        {
            string markPersonalReminderAsReadQuery = @"UPDATE PersonalEventReminder SET IsRead = 1,
                        ReadAt = GETDATE()
                        WHERE Id = @Id 
                        AND UserId = @UserId
                        AND IsRead = 0";

            var parameters = new Dictionary<string, object>()
            {
                { "@Id", reminderId },
                { "@UserId", userId }
            };

            await _dbHelper.ExecuteNonQueryAsync(markPersonalReminderAsReadQuery, parameters);
        }
        public async Task UnlikePersonalReminderAsync(int reminderId, int userId)
        {
            string unlikePersonalReminderQuery = @"UPDATE PersonalEventReminder 
                    SET IsLiked = 0
                    WHERE Id = @Id
                    AND UserId = @UserId";

            var parameters = new Dictionary<string, object>()
            {
                { "@Id", reminderId },
                { "@UserId", userId }
            };

            await _dbHelper.ExecuteNonQueryAsync(unlikePersonalReminderQuery, parameters);
        }
        public async Task<bool> HasUnreadPersonalRemindersForTodayAsync(int userId)
        {
            string hasUnreadPersonalRemindersForTodayQuery = @"
                    SELECT COUNT(*) 
                    FROM PersonalEventReminder r
                    WHERE r.UserId = @UserId 
                    AND r.IsRead = 0 
                    AND CAST(r.Date AS DATE) = CAST(GETDATE() AS DATE)";

            var parameters = new Dictionary<string, object>()
            {
                { "@UserId", userId }
            };

            var result = await _dbHelper.ExecuteScalarAsync(hasUnreadPersonalRemindersForTodayQuery, parameters);

            int count = result != null ? Convert.ToInt32(result) : 0;
            return count > 0;
        }
        public async Task<List<PersonalEventReminder>> GetLikedRemindersByUserAsync(int userId)
        {
            string getAllLikedRemindersQuery = @"
                    SELECT r.*, 
                           e.Id AS EventId,
                           e.Title AS EventTitle,
                           e.Description AS EventDescription,
                           e.Date AS EventDate,
                           e.UserId AS EventUserId,
                           e.CategoryId
                    FROM PersonalEventReminder r
                    INNER JOIN PersonalEvent e ON r.PersonalEventId = e.Id
                    INNER JOIN [User] u ON e.UserId = u.Id
                    WHERE r.UserId = @UserId 
                    AND r.IsLiked = 1
                    AND u.IsDeleted = 0;";

            var parameters = new Dictionary<string, object>
            {
                { "@UserId", userId }
            };

            var dt = await _dbHelper.ExecuteQueryAsync(getAllLikedRemindersQuery, parameters);
            return MapPersonalReminderWithEvent(dt);
        }
        private List<PersonalEventReminder> MapPersonalReminderWithEvent(DataTable dt)
        {
            var personalReminders = new List<PersonalEventReminder>();

            if (dt.Rows.Count == 0)
            {
                return new List<PersonalEventReminder>();
            }

            foreach (DataRow row in dt.Rows)
            {
                personalReminders.Add(new PersonalEventReminder
                {
                    Id = Convert.ToInt32(row["Id"]),
                    Title = row["Title"].ToString()!,
                    Description = row["Description"]?.ToString(),
                    Date = Convert.ToDateTime(row["Date"]),
                    Status = Convert.ToBoolean(row["IsRead"]) ? ReminderStatus.Read : ReminderStatus.Unread,
                    IsLiked = Convert.ToBoolean(row["IsLiked"]),
                    ReadAt = row["ReadAt"] != DBNull.Value ? Convert.ToDateTime(row["ReadAt"]) : null,
                    PersonalEventId = Convert.ToInt32(row["PersonalEventId"]),
                    PersonalEvent = MapPersonalEvent(row)
                });
            }

            return personalReminders;
        }
        private PersonalEvent MapPersonalEvent(DataRow row)
        {
            return new PersonalEvent
            {
                Id = Convert.ToInt32(row["EventId"]),
                Title = row["EventTitle"].ToString()!,
                Description = row["EventDescription"]?.ToString(),
                Date = Convert.ToDateTime(row["EventDate"]),
                UserId = Convert.ToInt32(row["EventUserId"]),
                CategoryId = row["CategoryId"] != DBNull.Value ? Convert.ToInt32(row["CategoryId"]) : null
            };
        }
    }
}
