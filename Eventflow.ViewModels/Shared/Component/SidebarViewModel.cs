using Eventflow.ViewModels.Continent;

namespace Eventflow.ViewModels.Shared.Component
{
    public class SidebarViewModel
    {
        public string Context { get; set; } = "";
        public List<ContinentViewModel> Continents { get; set; } = new List<ContinentViewModel>();
        public string Username { get; set; } = "";
        public bool IsLoggedin { get; set; }
        public int? SelectedCountryId { get; set; }
        public List<SidebarButtonViewModel> Buttons { get; set; } = new List<SidebarButtonViewModel>();
    }
}
