namespace Eventflow.ViewModels.Calendar.Component
{
    public class CalendarComponentViewModel
    {
        public CalendarViewModel Calendar { get; set; } = new CalendarViewModel();
        public CalendarNavigationViewModel Navigation { get; set; } = new CalendarNavigationViewModel();
    }
}
