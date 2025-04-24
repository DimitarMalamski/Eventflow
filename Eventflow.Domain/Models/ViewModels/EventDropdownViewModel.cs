namespace Eventflow.Domain.Models.ViewModels
{
    public class EventDropdownViewModel
    {
        public int Index { get; set; }
        public DateTime Date { get; set; }
        public List<PersonalEventWithCategoryNameViewModel> PersonalEvents { get; set; } = new List<PersonalEventWithCategoryNameViewModel>();
        public List<NationalEventViewModel> NationalEvents { get; set; } = new List<NationalEventViewModel>();
    }
}
