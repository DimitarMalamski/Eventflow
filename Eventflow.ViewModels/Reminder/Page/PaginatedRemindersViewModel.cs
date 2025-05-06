using Eventflow.ViewModels.Reminder.Component;

namespace Eventflow.ViewModels.Reminder.Page
{
    public class PaginatedRemindersViewModel
    {
        public List<ReminderBoxViewModel> PersonalReminders { get; set; } = new List<ReminderBoxViewModel>();
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
    }
}
