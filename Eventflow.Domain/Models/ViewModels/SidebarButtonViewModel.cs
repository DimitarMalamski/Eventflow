namespace Eventflow.Domain.Models.ViewModels
{
    public class SidebarButtonViewModel
    {
        public string Label { get; set; } = "";
        public string Url { get; set; } = "";
        public string? CssClass { get; set; }
        public bool ShowNotification { get; set; } = false;
    }
}
