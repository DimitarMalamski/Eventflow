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
        public async Task LikePersonalReminderAsync(int reminderId)
            => await _personalEventReminderRepository.LikePersonalReminderAsync(reminderId);
        public async Task MarkPersonalEventReminderAsReadAsync(int reminderId)
            => await _personalEventReminderRepository.MarkPersonalReminderAsReadAsync(reminderId);
        public async Task UnlikePersonalReminderAsync(int reminderId)
            => await _personalEventReminderRepository.UnlikePersonalReminderAsync(reminderId);
    }
}
