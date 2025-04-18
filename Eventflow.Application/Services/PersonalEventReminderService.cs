using Eventflow.Application.Services.Interfaces;
using Eventflow.Domain.Enums;
using Eventflow.Domain.Interfaces.Repositories;
using Eventflow.Domain.Models.Models;
using Eventflow.Domain.Models.ViewModels;

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
        public async Task<List<PersonalEventReminder>> GetAllPersonalEventRemindersByUserIdAsync(int userId)
        {
            var allUserEvents = await _personalEventRepository.GetAllPersonalEventsByUserIdAsync(userId);
            var eventIds = allUserEvents.Select(e => e.Id).ToList();

            if (!eventIds.Any())
            {
                return new List<PersonalEventReminder>();
            }

            var allReminders = await _personalEventReminderRepository.GetAllPersonalRemindersByEventIdAsync(eventIds);

            return allReminders;
        }
        public async Task<PaginatedRemindersViewModel> GetPaginatedPersonalRemindersAsync(int userId, ReminderStatus status, int page, int pageSize)
        {
            page = Math.Max(1, page);

            var personalReminders = status == ReminderStatus.Read
                ? await _personalEventReminderRepository.GetReadPersonalRemindersWithin3DaysAsync(userId)
                : await _personalEventReminderRepository.GetUnreadPersonalRemindersForTodayAsync(userId);

            var totalCount = personalReminders.Count;
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var paginatedPersonalReminders = personalReminders
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(r => new ReminderBoxViewModel
                {
                    Id = r.Id,
                    EventId = r.PersonalEventId,
                    Title = r.Title,
                    Description = r.Description,
                    Date = r.Date,
                    Status = r.Status,
                    EventTitle = r.PersonalEvent?.Title ?? "Unknown",
                    IsLiked = r.IsLiked
                })
                .ToList();

            return new PaginatedRemindersViewModel
            {
                PersonalReminders = paginatedPersonalReminders,
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

                return personalReminders
                    .Select(r => new ReminderBoxViewModel
                    {
                        Id = r.Id,
                        EventId = r.PersonalEventId,
                        Title = r.Title,
                        Description = r.Description,
                        Date = r.Date,
                        Status = r.Status,
                        EventTitle = r.PersonalEvent?.Title ?? "Unknown",
                        IsLiked = r.IsLiked
                    })
                    .ToList();
            }
            else
            {
                var personalReminders = await _personalEventReminderRepository.GetReadPersonalRemindersWithin3DaysAsync(userId);

                return personalReminders
                    .Select(r => new ReminderBoxViewModel
                    {
                        Id = r.Id,
                        EventId = r.PersonalEventId,
                        Title = r.Title,
                        Description = r.Description,
                        Date = r.Date,
                        Status = r.Status,
                        EventTitle = r.PersonalEvent?.Title ?? "Unknown"
                    })
                    .ToList();
            }
        }
        public async Task<List<ReminderBoxViewModel>> GetTodaysUnreadRemindersAsync(int userId)
        {
            var personalReminders = await _personalEventReminderRepository.GetUnreadPersonalRemindersForTodayAsync(userId);

            return personalReminders
                .Select(r => new ReminderBoxViewModel
                {
                    Id = r.Id,
                    EventId = r.PersonalEventId,
                    Title = r.Title,
                    Description = r.Description,
                    Date = r.Date,
                    Status = r.Status,
                    EventTitle = r.PersonalEvent?.Title ?? "Unknown",
                    IsLiked = r.IsLiked,
                })
                .ToList();
        }
        public async Task<bool> LikePersonalReminderAsync(int reminderId, int userId)
        {
            var personalReminder = await _personalEventReminderRepository.GetPersonalReminderByIdAsync(reminderId);

            if (!UserOwnsPersonalReminder(personalReminder, userId))
            {
                return false;
            }

            await _personalEventReminderRepository.LikePersonalReminderAsync(reminderId);
            return true;
        }
        public async Task<bool> MarkPersonalEventReminderAsReadAsync(int reminderId, int userId)
        {
            var personalReminder = await _personalEventReminderRepository.GetPersonalReminderByIdAsync(reminderId);

            if (!UserOwnsPersonalReminder(personalReminder, userId))
            {
                return false;
            }

            await _personalEventReminderRepository.MarkPersonalReminderAsReadAsync(reminderId);
            return true;
        }
        public async Task<bool?> ToggleLikeAsync(int reminderId, int userId)
        {
            var personalReminder = await _personalEventReminderRepository
                .GetPersonalReminderByIdAsync(reminderId);

            if (personalReminder?.PersonalEvent?.UserId != userId)
            {
                return null;
            }

            if (personalReminder.IsLiked)
            {
                await _personalEventReminderRepository.UnlikePersonalReminderAsync(reminderId);
                return false;
            }
            else
            {
                await _personalEventReminderRepository.LikePersonalReminderAsync(reminderId);
                return true;
            }

            return true;
        }
        public async Task<bool> UnlikePersonalReminderAsync(int reminderId, int userId)
        {
            var personalReminder = await _personalEventReminderRepository.GetPersonalReminderByIdAsync(reminderId);

            if (!UserOwnsPersonalReminder(personalReminder, userId))
            {
                return false;
            }

            await _personalEventReminderRepository.UnlikePersonalReminderAsync(reminderId);
            return true;
        }
        private bool UserOwnsPersonalReminder(PersonalEventReminder? reminder, int userId)
        {
            return reminder != null &&
                reminder.PersonalEvent != null &&
                reminder.PersonalEvent.UserId == userId;
        }
    }
}
