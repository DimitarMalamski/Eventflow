namespace Eventflow.ViewModels
{
    public class PaginatedRemindersViewModel
    {
        public List<ReminderBoxViewModel> PersonalReminders { get; set; } = new List<ReminderBoxViewModel>();
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
    }
}
