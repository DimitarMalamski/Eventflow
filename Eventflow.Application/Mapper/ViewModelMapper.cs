using Eventflow.Domain.Models.Models;
using Eventflow.Domain.Models.ViewModels;

namespace Eventflow.Application.Mapper
{
    public static class ViewModelMapper
    {
        public static class PersonalReminder
        {
            public static ReminderBoxViewModel ToBoxViewModel(PersonalEventReminder r) => new ReminderBoxViewModel()
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
            public static List<ReminderBoxViewModel> ToBoxViewModelList(IEnumerable<PersonalEventReminder> reminders)
                => reminders?.Select(ToBoxViewModel).ToList()
                    ?? new List<ReminderBoxViewModel>();
        }
    }
}
