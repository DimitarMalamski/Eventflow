namespace Eventflow.ViewModels.Calendar
{
    public class CalendarViewModel
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public List<CalendarDayViewModel> Days { get; set; } = new List<CalendarDayViewModel>();
    }
}
