using Eventflow.Domain.Models.DTOs;
using Eventflow.Domain.Models.Entities;

namespace Eventflow.Application.Mapper
{
    public static class ViewModelMapper
    {
        public static class PersonalReminder
        {
            public static ReminderDto ToBoxViewModel(PersonalEventReminder r) => new ReminderDto()
            {
                Id = r.Id,
                EventId = r.PersonalEventId,
                Title = r.Title,
                Description = r.Description,
                Date = r.Date,
                Status = r.Status,
                EventTitle = r.PersonalEvent?.Title ?? "Unknown",
                IsLiked = r.IsLiked,
            };
            public static List<ReminderDto> ToReminderDtoList(IEnumerable<PersonalEventReminder> reminders)
                => reminders?.Select(ToBoxViewModel).ToList()
                    ?? new List<ReminderDto>();
        }
    }
}
