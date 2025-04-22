using Eventflow.Application.Services.Interfaces;
using Eventflow.Domain.Enums;
using Eventflow.Domain.Interfaces.Repositories;
using Eventflow.Domain.Models.Models;
using Eventflow.Domain.Models.ViewModels;
using static Eventflow.Application.Mapper.ViewModelMapper.PersonalReminder;

namespace Eventflow.Application.Services
{
    public class PersonalEventReminderService : IPersonalEventReminderService
    {
        private readonly IPersonalEventReminderRepository _personalEventReminderRepository;
        private readonly IPersonalEventRepository _personalEventRepository;
        public PersonalEventReminderService(IPersonalEventReminderRepository personalEventReminderRepository,
            IPersonalEventRepository personalEventRepository)
        {
            _personalEventReminderRepository = personalEventReminderRepository;
            _personalEventRepository = personalEventRepository;
        }

        public async Task CreatePersonalEventReminderAsync(PersonalEventReminder reminder)
            => await _personalEventReminderRepository.CreatePersonalReminderAsync(reminder);
        public async Task<PaginatedRemindersViewModel> GetPaginatedFilteredPersonalRemindersAsync(int userId,
            ReminderStatus status,
            string? search,
            string? sortBy,
            int page,
            int pageSize)
        {
            page = Math.Max(1, page);

            var personalReminders = await GetPersonalRemindersByStatusAsync(userId, status);

            if (!string.IsNullOrWhiteSpace(search))
            {
                personalReminders = FilterPersonalReminders(personalReminders, search);
            }

            personalReminders = SortPersonalReminders(personalReminders, sortBy);

            var totalCount = personalReminders.Count;
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var peginatedPersonalReminders = personalReminders
                .Skip((page - 1) * pageSize)
                .Take(pageSize);

            return new PaginatedRemindersViewModel
            {
                PersonalReminders = ToBoxViewModelList(peginatedPersonalReminders),
                TotalPages = totalPages,
                CurrentPage = page
            };
        }
        public async Task<PaginatedRemindersViewModel> GetPaginatedPersonalRemindersAsync(int userId, ReminderStatus status, int page, int pageSize)
        {
            page = Math.Max(1, page);

            var personalReminders = await GetPersonalRemindersByStatusAsync(userId, status);

            var totalCount = personalReminders.Count;
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var paginatedPersonalReminders = personalReminders
                .Skip((page - 1) * pageSize)
                .Take(pageSize);

            return new PaginatedRemindersViewModel
            {
                PersonalReminders = ToBoxViewModelList(paginatedPersonalReminders),
                TotalPages = totalPages,
                CurrentPage = page,
            };
        }
        public async Task<PersonalEventReminder?> GetPersonalReminderByIdAsync(int reminderId)
            => await _personalEventReminderRepository.GetPersonalReminderByIdAsync(reminderId);
        public async Task<List<ReminderBoxViewModel>> GetRemindersWithEventTitlesByUserIdAsync(int userId, ReminderStatus status)
        {
            if (status == ReminderStatus.Unread)
            {
                var personalReminders = await _personalEventReminderRepository.GetUnreadPersonalRemindersForTodayAsync(userId);

                return ToBoxViewModelList(personalReminders);
            }
            else
            {
                var personalReminders = await _personalEventReminderRepository.GetReadPersonalRemindersWithin3DaysAsync(userId);

                return ToBoxViewModelList(personalReminders);
            }
        }
        public async Task<List<ReminderBoxViewModel>> GetTodaysUnreadRemindersAsync(int userId)
        {
            var personalReminders = await _personalEventReminderRepository.GetUnreadPersonalRemindersForTodayAsync(userId);

            return ToBoxViewModelList(personalReminders);
        }
        public async Task<bool> LikePersonalReminderAsync(int reminderId, int userId)
        {
            var personalReminder = await _personalEventReminderRepository.GetPersonalReminderByIdAsync(reminderId);

            if (!UserOwnsPersonalReminder(personalReminder, userId))
            {
                return false;
            }

            await _personalEventReminderRepository.LikePersonalReminderAsync(reminderId, userId);
            return true;
        }
        public async Task<bool> MarkPersonalEventReminderAsReadAsync(int reminderId, int userId)
        {
            var personalReminder = await _personalEventReminderRepository.GetPersonalReminderByIdAsync(reminderId);

            if (!UserOwnsPersonalReminder(personalReminder, userId))
            {
                return false;
            }

            await _personalEventReminderRepository.MarkPersonalReminderAsReadAsync(reminderId, userId);
            return true;
        }
        public async Task<bool?> ToggleLikeAsync(int reminderId, int userId)
        {
            var personalReminder = await _personalEventReminderRepository
                .GetPersonalReminderByIdAsync(reminderId);

            if (personalReminder == null || personalReminder.UserId != userId)
            {
                return null;
            }

            if (personalReminder.IsLiked)
            {
                await _personalEventReminderRepository.UnlikePersonalReminderAsync(reminderId, userId);
                return false;
            }

            await _personalEventReminderRepository.LikePersonalReminderAsync(reminderId, userId);
            return true;
        }
        public async Task<bool> UnlikePersonalReminderAsync(int reminderId, int userId)
        {
            var personalReminder = await _personalEventReminderRepository.GetPersonalReminderByIdAsync(reminderId);

            if (!UserOwnsPersonalReminder(personalReminder, userId))
            {
                return false;
            }

            await _personalEventReminderRepository.UnlikePersonalReminderAsync(reminderId, userId);
            return true;
        }
        private bool UserOwnsPersonalReminder(PersonalEventReminder? reminder, int userId)
        {
            return reminder != null && reminder.UserId == userId;
        }
        private async Task<List<PersonalEventReminder>> GetPersonalRemindersByStatusAsync(int userId, ReminderStatus status)
        {
            return status == ReminderStatus.Read
                ? await _personalEventReminderRepository.GetReadPersonalRemindersWithin3DaysAsync(userId)
                : await _personalEventReminderRepository.GetUnreadPersonalRemindersForTodayAsync(userId);
        }
        private List<PersonalEventReminder> FilterPersonalReminders(List<PersonalEventReminder> personalReminders, string? search)
        {
            if (string.IsNullOrWhiteSpace(search))
            {
                return personalReminders;
            }

            return personalReminders
                .Where(r =>
                    (r.Title?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (r.Description?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (r.PersonalEvent?.Title?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false))
                .ToList();
        }
        private List<PersonalEventReminder> SortPersonalReminders(List<PersonalEventReminder> personalReminders, string? sortBy)
        {
            return sortBy?.ToLower() switch
            {
                "event" => personalReminders.OrderBy(r => r.PersonalEvent?.Title).ToList(),
                "date" => personalReminders.OrderBy(r => r.Date).ToList(),
                "liked" => personalReminders
                            .Where(r => r.IsLiked)
                            .OrderByDescending(r => r.Date)
                            .ToList(),
                _ => personalReminders.OrderBy(r => r.Id).ToList()
            };
        }
        public async Task<bool> HasUnreadRemindersForTodayAsync(int userId)
            => await _personalEventReminderRepository.HasUnreadPersonalRemindersForTodayAsync(userId);
        public async Task<int> GetLikedReminderCountAsync(int userId)
            => (await _personalEventReminderRepository.GetLikedRemindersByUserAsync(userId)).Count;
        public async Task<int> CountUnreadRemindersForTodayAsync(int userId)
            => (await _personalEventReminderRepository.GetUnreadPersonalRemindersForTodayAsync(userId)).Count;
        public async Task<PaginatedRemindersViewModel> GetPaginatedLikedRemindersAsync(int userId,
            string? search, 
            string? sortBy,
            int page, 
            int pageSize)
        {
            var allLikedReminders = await _personalEventReminderRepository.GetLikedRemindersByUserAsync(userId);

            allLikedReminders = FilterPersonalReminders(allLikedReminders, search);
            allLikedReminders = SortPersonalReminders(allLikedReminders, sortBy);

            int totalCount = allLikedReminders.Count;
            int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var paginated = allLikedReminders
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PaginatedRemindersViewModel
            {
                PersonalReminders = ToBoxViewModelList(paginated),
                TotalPages = totalPages,
                CurrentPage = page
            };
        }
    }
}
