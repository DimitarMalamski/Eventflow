using Eventflow.Domain.Models.Entities;

namespace Eventflow.Domain.Interfaces.Repositories
{
    public interface IPersonalEventReminderRepository
    {
        public Task CreatePersonalReminderAsync(PersonalEventReminder reminder);
        public Task MarkPersonalReminderAsReadAsync(int reminderId, int userId);
        public Task<List<PersonalEventReminder>> GetPersonalRemindersWithEventAndTitleByUserIdAsync(int userId);
        public Task LikePersonalReminderAsync(int reminderId, int userId);
        public Task UnlikePersonalReminderAsync(int reminderId, int userId);
        public Task<List<PersonalEventReminder>> GetUnreadPersonalRemindersForTodayAsync(int userId);
        public Task<List<PersonalEventReminder>> GetReadPersonalRemindersWithin3DaysAsync(int userId);
        public Task<PersonalEventReminder?> GetPersonalReminderByIdAsync(int reminderId);
        public Task<bool> HasUnreadPersonalRemindersForTodayAsync(int userId);
        public Task<List<PersonalEventReminder>> GetLikedRemindersByUserAsync(int userId);

    }
}
