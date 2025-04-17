namespace Eventflow.Domain.Models.ViewModels
{
    public class CalendarComponentViewModel
    {
        public CalendarViewModel Calendar { get; set; } = new CalendarViewModel();
        public CalendarNavigationViewModel Navigation { get; set; } = new CalendarNavigationViewModel();
    }
}
