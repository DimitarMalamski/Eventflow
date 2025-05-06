namespace Eventflow.ViewModels.Calendar.Component
{
    public class CalendarRowViewModel
    {
        public List<CalendarDayViewModel> Days { get; set; } = new List<CalendarDayViewModel>();
        public int StartIndex { get; set; }
    }
}
