using Eventflow.Domain.Models.Models;

namespace Eventflow.Domain.Interfaces.Repositories
{
    public interface IPersonalEventReminderRepository
    {
        public Task CreatePersonalReminderAsync(PersonalEventReminder reminder);
        public Task<List<PersonalEventReminder>> GetAllPersonalRemindersByEventIdAsync(List<int> eventIds);
        public Task MarkPersonalReminderAsReadAsync(int reminderId);
        public Task<List<PersonalEventReminder>> GetPersonalRemindersWithEventAndTitleByUserIdAsync(int userId);
        public Task LikePersonalReminderAsync(int reminderId);
        public Task UnlikePersonalReminderAsync(int reminderId);
        public Task<List<PersonalEventReminder>> GetUnreadPersonalRemindersForTodayAsync(int userId);
        public Task<List<PersonalEventReminder>> GetReadPersonalRemindersWithin3DaysAsync(int userId);
        public Task<PersonalEventReminder?> GetPersonalReminderByIdAsync(int reminderId);
        public Task<bool> HasUnreadPersonalRemindersForTodayAsync(int userId);

    }
}
