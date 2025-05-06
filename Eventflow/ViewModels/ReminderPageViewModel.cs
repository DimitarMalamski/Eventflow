using Eventflow.Domain.Enums;

namespace Eventflow.ViewModels
{
    public class ReminderPageViewModel
    {
        public ReminderStatus CurrentStatus { get; set; } = ReminderStatus.Unread;
        public List<ReminderBoxViewModel> Reminders { get; set; } = new List<ReminderBoxViewModel>();
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public string? SearchTerm { get; set; }
        public string? SortBy { get; set; }
    }
}
