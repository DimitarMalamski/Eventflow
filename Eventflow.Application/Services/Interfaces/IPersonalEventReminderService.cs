using Eventflow.Domain.Enums;
using Eventflow.Domain.Models.Entities;
using Eventflow.DTOs.DTOs;

namespace Eventflow.Application.Services.Interfaces
{
    public interface IPersonalEventReminderService
    {
        public Task CreatePersonalEventReminderAsync(PersonalEventReminder reminder, int userId);
        public Task MarkPersonalEventReminderAsReadAsync(int reminderId, int userId);
        public Task<bool> ToggleLikeAsync(int reminderId, int userId);
        public Task<PaginatedReminderDto> GetPaginatedFilteredPersonalRemindersAsync(
            int userId,
            ReminderStatus status,
            string? search,
            string? sortBy,
            int page,
            int pageSize);
        public Task<bool> HasUnreadRemindersForTodayAsync(int userId);
        public Task<int> GetLikedReminderCountAsync(int userId);
        public Task<int> CountUnreadRemindersForTodayAsync(int userId);
        public Task<PaginatedReminderDto> GetPaginatedLikedRemindersAsync(
            int userId,
            string? search,
            string? sortBy,
            int page,
            int pageSize);
    }
}
