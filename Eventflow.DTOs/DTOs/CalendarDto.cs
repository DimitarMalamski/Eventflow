namespace Eventflow.DTOs.DTOs
{
    public class CalendarDto
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public List<CalendarDayDto> Days { get; set; } = new List<CalendarDayDto>();
    }

    public class CalendarDayDto
    {
        public int? DayNumber { get; set; }
        public bool IsToday { get; set; }
        public DateTime? Date { get; set; }
        public List<PersonalEventWithCategoryNameDto> PersonalEvents { get; set; } = new List<PersonalEventWithCategoryNameDto>();
        public List<NationalEventDto> NationalEvents { get; set; } = new List<NationalEventDto>();
    }
}
