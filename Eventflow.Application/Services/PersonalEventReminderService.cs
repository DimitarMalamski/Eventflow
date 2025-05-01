using Eventflow.Application.Services.Interfaces;
using Eventflow.Domain.Enums;
using Eventflow.Domain.Exceptions;
using Eventflow.Domain.Interfaces.Repositories;
using Eventflow.Domain.Models.Models;
using Eventflow.Domain.Models.ViewModels;
using static Eventflow.Application.Helper.StringMatchHelper;
using static Eventflow.Application.Mapper.ViewModelMapper.PersonalReminder;
using static Eventflow.Domain.Common.CustomErrorMessages.PersonalEventReminderService;

namespace Eventflow.Application.Services
{
    public class PersonalEventReminderService : IPersonalEventReminderService
    {
        private readonly IPersonalEventReminderRepository _personalEventReminderRepository;
        private readonly IPersonalEventRepository _personalEventRepository;
        public PersonalEventReminderService(IPersonalEventReminderRepository personalEventReminderRepository,
            IPersonalEventRepository personalEventRepository)
        {
            _personalEventReminderRepository = personalEventReminderRepository
                ?? throw new ArgumentNullException(nameof(personalEventReminderRepository));
            _personalEventRepository = personalEventRepository
                ?? throw new ArgumentNullException(nameof(personalEventRepository));
        }

        public async Task CreatePersonalEventReminderAsync(PersonalEventReminder reminder, int userId)
        {
            if (reminder == null)
            {
                throw new ArgumentNullException(nameof(reminder), personalEventReminderCannotBeNull);
            }

            if (string.IsNullOrWhiteSpace(reminder.Title))
            {
                throw new InvalidReminderInputException(personalEventRemindedrTitleCannotBeNull);
            }

            if (reminder.Date < DateTime.Today)
            {
                throw new InvalidReminderInputException(personalEventReminderDataCannotBeInThePast);
            }

            bool hasAccess = await _personalEventRepository.UserHasAccessToEventAsync(reminder.PersonalEventId, userId);

            if (!hasAccess)
            {
                throw new UnauthorizedReminderAccessException(personalEventReminderHasNoAccessToCreate);
            }

            await _personalEventReminderRepository.CreatePersonalReminderAsync(reminder);
        }
        public async Task<PaginatedRemindersViewModel> GetPaginatedFilteredPersonalRemindersAsync(int userId,
            ReminderStatus status,
            string? search,
            string? sortBy,
            int page,
            int pageSize)
        {
            var personalReminders = await GetPersonalRemindersByStatusAsync(userId, status);
            return PaginateAndWrap(personalReminders, search, sortBy, page, pageSize);
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
            var likedReminders = await _personalEventReminderRepository.GetLikedRemindersByUserAsync(userId);
            return PaginateAndWrap(likedReminders, search, sortBy, page, pageSize);
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
                    Match(r.Title, search) ||
                    Match(r.Description, search) ||
                    Match(r.PersonalEvent?.Title, search))
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
        private PaginatedRemindersViewModel PaginateAndWrap(IEnumerable<PersonalEventReminder> personalReminders,
            string? search, 
            string? sortBy, 
            int page, 
            int pageSize)
        {
            if (!string.IsNullOrWhiteSpace(search))
            {
                personalReminders = FilterPersonalReminders(personalReminders.ToList(), search);
            }

            var sorted = SortPersonalReminders(personalReminders.ToList(), sortBy);

            var totalCount = sorted.Count;
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var paginated = sorted
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
