using Eventflow.ViewModels.Calendar.Component;

namespace Eventflow.ViewModels.Calendar
{
    public class CalendarDayViewModel
    {
        public int? DayNumber { get; set; } = null!;
        public bool IsToday { get; set; } = false;
        public DateTime? Date { get; set; }
        public List<PersonalEventWithCategoryNameViewModel> PersonalEvents { get; set; } = new List<PersonalEventWithCategoryNameViewModel>();
        public List<NationalEventViewModel> NationalEvents { get; set; } = new List<NationalEventViewModel>();
    }
}
