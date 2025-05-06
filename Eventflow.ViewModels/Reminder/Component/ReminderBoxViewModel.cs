using Eventflow.ViewModels.Reminder.Enums;

namespace Eventflow.ViewModels.Reminder.Component
{
    public class ReminderBoxViewModel
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime Date { get; set; }
        public ReminderStatusEnum Status { get; set; }
        public string EventTitle { get; set; } = "Unknown";
        public bool IsLiked { get; set; }
    }
}
