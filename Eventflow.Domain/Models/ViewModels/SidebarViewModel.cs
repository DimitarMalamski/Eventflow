using Eventflow.Domain.Models.Models;

namespace Eventflow.Domain.Models.ViewModels
{
    public class SidebarViewModel
    {
        public string Context { get; set; } = "";
        public List<Continent> Continents { get; set; } = new List<Continent>();
        public string Username { get; set; } = "";
        public bool IsLoggedin { get; set; }
        public List<SidebarButtonViewModel> Buttons { get; set; } = new List<SidebarButtonViewModel>();
    }
}
