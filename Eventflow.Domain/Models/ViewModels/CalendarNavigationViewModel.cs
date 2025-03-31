namespace Eventflow.Domain.Models.ViewModels
{
    public class CalendarNavigationViewModel
    {
        public int CurrentMonth { get; set; }
        public int CurrentYear { get; set; }

        public int PrevMonth => CurrentMonth == 1 ? 12 : CurrentMonth - 1;
        public int PrevYear => CurrentMonth == 1 ? CurrentYear - 1 : CurrentYear;

        public int NextMonth => CurrentMonth == 12 ? 1 : CurrentMonth + 1;
        public int NextYear => CurrentMonth == 12 ? CurrentYear + 1 : CurrentYear;
    }
}
