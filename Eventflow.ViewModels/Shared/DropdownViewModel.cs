namespace Eventflow.ViewModels.Shared
{
    public class DropdownViewModel
    {
        public string? Id { get; set; }
        public string Label { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? SelectedValue { get; set; }
        public string? DefaultOptionLabel { get; set; }
        public List<DropdownOptionViewModel> Options { get; set; } = new List<DropdownOptionViewModel>();
    }
}
