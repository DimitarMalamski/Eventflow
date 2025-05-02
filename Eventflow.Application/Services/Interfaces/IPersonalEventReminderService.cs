using Eventflow.Domain.Enums;
using Eventflow.Domain.Models.Models;
using Eventflow.Domain.Models.ViewModels;

namespace Eventflow.Application.Services.Interfaces
{
    public interface IPersonalEventReminderService
    {
        public Task CreatePersonalEventReminderAsync(PersonalEventReminder reminder, int userId);
        public Task MarkPersonalEventReminderAsReadAsync(int reminderId, int userId);
        public Task<bool> ToggleLikeAsync(int reminderId, int userId);
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
        public Task<PaginatedRemindersViewModel> GetPaginatedLikedRemindersAsync(
            int userId,
            string? search,
            string? sortBy,
            int page,
            int pageSize);
    }
}
