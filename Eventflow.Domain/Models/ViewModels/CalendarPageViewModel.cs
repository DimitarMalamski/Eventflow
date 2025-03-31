using Eventflow.Models.Models;

namespace Eventflow.Domain.Models.ViewModels
{
    public class CalendarPageViewModel
    {
        public List<Continent> Continents { get; set; } = new List<Continent>();
        public CalendarViewModel Calendar { get; set; } = new CalendarViewModel();
    }
}
