using Eventflow.Domain.Models.Models;

namespace Eventflow.Domain.Interfaces.Repositories
{
    public interface IPersonalEventReminderRepository
    {
        public Task CreatePersonalReminderAsync(PersonalEventReminder reminder);
        public Task<List<PersonalEventReminder>> GetAllPersonalRemindersByEventIdAsync(List<int> eventIds);
        public Task MarkPersonalReminderAsReadAsync(int reminderId);
        public Task<List<PersonalEventReminder>> GetRemindersWithEventAndTitleByUserIdAsync(int userId);

    }
}
