using Eventflow.Domain.Enums;
using Eventflow.Domain.Interfaces.Repositories;
using Eventflow.Domain.Models.Models;
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
                INSERT INTO PersonalEventReminder (Title, Description, Date, IsRead, PersonalEventId)
                VALUES (@Title, @Description, @Date, @IsRead, @PersonalEventId);";

            var parameters = new Dictionary<string, object?>()
            {
                { "@Title", reminder.Title },
                { "@Description", (object?)reminder.Description ?? DBNull.Value },
                { "@Date", reminder.Date },
                { "@IsRead", reminder.Status },
                { "@PersonalEventId", reminder.PersonalEventId }
            };

            await _dbHelper.ExecuteNonQueryAsync(createPersonalReminderQuery, parameters);
        }
        public async Task<List<PersonalEventReminder>> GetAllPersonalRemindersByEventIdAsync(List<int> eventIds)
        {
            if (eventIds == null || !eventIds.Any())
            {
                return new List<PersonalEventReminder>();
            }

            string[] paramNames = eventIds.Select((_, i) => $"@Id{i}").ToArray();

            string getAllPersonalRemindersByEventIdQuery = $"SELECT * FROM PersonalEventReminder WHERE PersonalEventId IN ({string.Join(",", paramNames)})";

            var parameters = new Dictionary<string, object>();

            for (int i = 0; i < eventIds.Count; i++)
            {
                parameters.Add(paramNames[i], eventIds[i]);
            }

            var dt = await _dbHelper.ExecuteQueryAsync(getAllPersonalRemindersByEventIdQuery, parameters);

            var reminders = new List<PersonalEventReminder>();

            foreach (DataRow row in dt.Rows)
            {
                reminders.Add(new PersonalEventReminder
                {
                    Id = Convert.ToInt32(row["Id"]),
                    Title = row["Title"].ToString()!,
                    Description = row["Description"]?.ToString(),
                    Date = Convert.ToDateTime(row["Date"]),
                    Status = Convert.ToBoolean(row["IsRead"]) ? ReminderStatus.Read : ReminderStatus.Unread,
                    PersonalEventId = Convert.ToInt32(row["PersonalEventId"])
                });
            }

            return reminders;
        }
        public async Task<List<PersonalEventReminder>> GetRemindersWithEventAndTitleByUserIdAsync(int userId)
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
                        WHERE e.UserId = @UserId";

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
        public async Task MarkPersonalReminderAsReadAsync(int reminderId)
        {
            string markPersonalReminderAsReadQuery = "UPDATE PersonalEventReminder SET isRead = 1 WHERE Id = @Id";

            var parameters = new Dictionary<string, object>()
            {
                { "@Id", reminderId }
            };

            await _dbHelper.ExecuteNonQueryAsync(markPersonalReminderAsReadQuery, parameters);
        }
    }
}
