namespace Eventflow.Domain.Models.ViewModels
{
    public class CalendarRowViewModel
    {
        public List<CalendarDay> Days { get; set; } = new List<CalendarDay>();
        public int StartIndex { get; set; }
    }
}
