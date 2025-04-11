namespace Eventflow.Domain.Models.ViewModels
{
    public class ReminderRequestModel
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime ReminderDate { get; set; }
        public int PersonalEventId { get; set; }
    }
}
