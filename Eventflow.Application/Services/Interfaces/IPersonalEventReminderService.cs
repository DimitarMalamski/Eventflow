using Eventflow.Domain.Models.Models;
using Eventflow.Domain.Models.ViewModels;

namespace Eventflow.Application.Services.Interfaces
{
    public interface IPersonalEventReminderService
    {
        public Task CreatePersonalEventReminderAsync(PersonalEventReminder reminder);
        public Task<List<PersonalEventReminder>> GetAllPersonalEventRemindersByUserIdAsync(int userId);
        public Task MarkPersonalEventReminderAsReadAsync(int reminderId);
        public Task<List<ReminderBoxViewModel>> GetRemindersWithEventTitlesByUserIdAsync(int userId, bool isRead);
        public Task<List<ReminderBoxViewModel>> GetTodaysUnreadRemindersAsync(int userId);
    }
}
