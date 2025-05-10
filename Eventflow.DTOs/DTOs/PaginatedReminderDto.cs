namespace Eventflow.DTOs.DTOs
{
    public class PaginatedReminderDto
    {
        public List<ReminderDto> PersonalReminders { get; set; } = new();
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
    }
}
