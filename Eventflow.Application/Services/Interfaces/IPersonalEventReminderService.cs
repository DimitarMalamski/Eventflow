using Eventflow.Domain.Enums;
using Eventflow.Domain.Models.Models;
using Eventflow.Domain.Models.ViewModels;

namespace Eventflow.Application.Services.Interfaces
{
    public interface IPersonalEventReminderService
    {
        public Task CreatePersonalEventReminderAsync(PersonalEventReminder reminder);
        public Task<bool> MarkPersonalEventReminderAsReadAsync(int reminderId, int userId);
        public Task<List<ReminderBoxViewModel>> GetRemindersWithEventTitlesByUserIdAsync(int userId, ReminderStatus status);
        public Task<List<ReminderBoxViewModel>> GetTodaysUnreadRemindersAsync(int userId);
        public Task<bool> LikePersonalReminderAsync(int reminderId, int userId);
        public Task<bool> UnlikePersonalReminderAsync(int reminderId, int userId);
        public Task<PersonalEventReminder?> GetPersonalReminderByIdAsync(int reminderId);
        public Task<PaginatedRemindersViewModel> GetPaginatedPersonalRemindersAsync(int userId, ReminderStatus status, int page, int pageSize);
        public Task<bool?> ToggleLikeAsync(int reminderId, int userId);
        public Task<PaginatedRemindersViewModel> GetPaginatedFilteredPersonalRemindersAsync(
            int userId,
            ReminderStatus status,
            string? search,
            string? sortBy,
            int page,
            int pageSize);
        public Task<bool> HasUnreadRemindersForTodayAsync(int userId);
        public Task<int> GetLikedReminderCountAsync(int userId);
        public Task<int> CountUnreadRemindersForTodayAsync(int userId);
    }
}
