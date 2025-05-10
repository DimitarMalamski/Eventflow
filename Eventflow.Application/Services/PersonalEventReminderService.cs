using Eventflow.Application.Services.Interfaces;
using Eventflow.Domain.Enums;
using Eventflow.Domain.Exceptions;
using Eventflow.Domain.Interfaces.Repositories;
using Eventflow.Domain.Models.Entities;
using Eventflow.DTOs.DTOs;
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
        public async Task<PaginatedReminderDto> GetPaginatedFilteredPersonalRemindersAsync(int userId,
            ReminderStatus status,
            string? search,
            string? sortBy,
            int page,
            int pageSize)
        {
            var personalReminders = await GetPersonalRemindersByStatusAsync(userId, status);
            return PaginateAndWrap(personalReminders, search, sortBy, page, pageSize);
        }
        public async Task MarkPersonalEventReminderAsReadAsync(int reminderId, int userId)
        {
            var personalReminder = await _personalEventReminderRepository.GetPersonalReminderByIdAsync(reminderId);

            if (personalReminder == null)
            {
                throw new ReminderNotFoundException(ReminderNotFound(reminderId));
            }

            if (!UserOwnsPersonalReminder(personalReminder, userId))
            {
                throw new UnauthorizedReminderAccessException(personalReminderMarkAsReadNotAllowed);
            }

            await _personalEventReminderRepository.MarkPersonalReminderAsReadAsync(reminderId, userId);
        }
        public async Task<bool> ToggleLikeAsync(int reminderId, int userId)
        {
            var personalReminder = await _personalEventReminderRepository
                .GetPersonalReminderByIdAsync(reminderId);

            if (personalReminder == null)
            {
                throw new ReminderNotFoundException(ReminderNotFound(reminderId));
            }

            if (personalReminder.UserId != userId)
            {
                throw new UnauthorizedReminderAccessException(personalReminderToggleLikeNotAllowed);
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
        public async Task<PaginatedReminderDto> GetPaginatedLikedRemindersAsync(int userId,
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
            var result = status == ReminderStatus.Read
                ? await _personalEventReminderRepository.GetReadPersonalRemindersWithin3DaysAsync(userId)
                : await _personalEventReminderRepository.GetUnreadPersonalRemindersForTodayAsync(userId);

            return result ?? new List<PersonalEventReminder>();
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
        private PaginatedReminderDto PaginateAndWrap(IEnumerable<PersonalEventReminder> personalReminders,
            string? search, 
            string? sortBy, 
            int page, 
            int pageSize)
        {
            if (page < 1)
            {
                throw new ArgumentException(pageMustBeGreaterThanZero, nameof(page));
            }

            if (pageSize <= 0)
            {
                throw new ArgumentException(pageSizeMustBeGreaterThanZero, nameof(pageSize));
            }

            var personalReminderList = personalReminders.ToList();

            if (!string.IsNullOrWhiteSpace(search))
            {
                personalReminderList = FilterPersonalReminders(personalReminderList, search);
            }

            var sorted = SortPersonalReminders(personalReminderList, sortBy);

            var totalCount = sorted.Count;
            var totalPages = Math.Max(1, (int)Math.Ceiling(totalCount / (double)pageSize));

            var paginated = sorted
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PaginatedReminderDto
            {
                PersonalReminders = ToReminderDtoList(paginated),
                TotalPages = totalPages,
                CurrentPage = page
            };
        }
    }
}
