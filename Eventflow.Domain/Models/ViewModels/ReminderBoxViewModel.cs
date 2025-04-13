namespace Eventflow.Domain.Models.ViewModels
{
    public class ReminderBoxViewModel
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime Date { get; set; }
        public bool IsRead { get; set; }
        public string EventTitle { get; set; } = "Unknown";
    }
}
