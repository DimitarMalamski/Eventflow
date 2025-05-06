using Eventflow.Domain.Models.Entities;

namespace Eventflow.ViewModels
{
    public class CalendarPageViewModel
    {
        public List<Continent> Continents { get; set; } = new List<Continent>();
        public CalendarViewModel Calendar { get; set; } = new CalendarViewModel();
        public CalendarNavigationViewModel Navigation { get; set; } = new CalendarNavigationViewModel();
        public SidebarViewModel SidebarViewModel { get; set; }
        public int? SelectedCountryId { get; set; }
    }
}
