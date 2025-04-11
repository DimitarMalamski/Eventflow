using Eventflow.Application.Services.Interfaces;
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

            if (eventIds.Any())
            {
                return new List<PersonalEventReminder>();
            }

            var allReminders = await _personalEventReminderRepository.GetAllPersonalRemindersByEventIdAsync(eventIds);

            return allReminders;
        }
        public async Task<List<ReminderBoxViewModel>> GetRemindersWithEventTitlesByUserIdAsync(int userId, bool isRead)
        {
            var reminders = await _personalEventReminderRepository.GetRemindersWithEventAndTitleByUserIdAsync(userId);

            var filteredReminders = reminders
                .Where(r => r.IsRead == isRead)
                .Select(r => new ReminderBoxViewModel
                {
                    Id = r.Id,
                    ReminderId = r.Id,
                    Title = r.Title,
                    Description = r.Description,
                    Date = r.Date,
                    IsRead = r.IsRead,
                    EventTitle = r.PersonalEvent?.Title ?? "Unknown"
                })
                .ToList();

            return filteredReminders;
        }
        public async Task MarkPersonalEventReminderAsReadAsync(int reminderId)
            => await _personalEventReminderRepository.MarkPersonalReminderAsReadAsync(reminderId);
    }
}
