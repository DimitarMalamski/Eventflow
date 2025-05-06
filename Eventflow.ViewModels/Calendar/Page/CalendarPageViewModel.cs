using Eventflow.ViewModels.Continent;
using Eventflow.ViewModels.Shared.Component;

namespace Eventflow.ViewModels.Calendar.Page
{
    public class CalendarPageViewModel
    {
        public List<ContinentViewModel> Continents { get; set; } = new List<ContinentViewModel>();
        public CalendarViewModel Calendar { get; set; } = new CalendarViewModel();
        public CalendarNavigationViewModel Navigation { get; set; } = new CalendarNavigationViewModel();
        public SidebarViewModel SidebarViewModel { get; set; } = null!;
        public int? SelectedCountryId { get; set; }
    }
}
