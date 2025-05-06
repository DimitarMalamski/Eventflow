using Eventflow.ViewModels.Reminder.Component;
using Eventflow.ViewModels.Reminder.Enums;

namespace Eventflow.ViewModels.Reminder.Page
{
    public class ReminderPageViewModel
    {
        public ReminderStatusEnum CurrentStatus { get; set; } = ReminderStatusEnum.Unread;
        public List<ReminderBoxViewModel> Reminders { get; set; } = new List<ReminderBoxViewModel>();
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public string? SearchTerm { get; set; }
        public string? SortBy { get; set; }
    }
}
