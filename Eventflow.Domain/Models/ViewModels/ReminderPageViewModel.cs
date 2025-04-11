namespace Eventflow.Domain.Models.ViewModels
{
    public class ReminderPageViewModel
    {
        public string CurrentFilter { get; set; } = "unread";
        public List<ReminderBoxViewModel> Reminders { get; set; } = new List<ReminderBoxViewModel>();
    }
}
