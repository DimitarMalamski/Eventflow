namespace Eventflow.Domain.Models.ViewModels
{
    public class CalendarViewModel
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public List<CalendarDay> Days { get; set; } = new List<CalendarDay>();
    }

    public class CalendarDay
    {
        public int? DayNumber { get; set; } = null!;

        public bool IsToday { get; set; } = false;
    }
}
