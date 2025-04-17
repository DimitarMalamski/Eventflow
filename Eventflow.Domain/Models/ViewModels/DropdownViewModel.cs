using Eventflow.Domain.Models.Models;

namespace Eventflow.Domain.Models.ViewModels
{
    public class DropdownViewModel
    {
        public string? Id { get; set; }
        public string Label { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? SelectedValue { get; set; }
        public string? DefaultOptionLabel { get; set; }
        public List<DropdownOption> Options { get; set; } = new List<DropdownOption>();
    }
    public class DropdownOption
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }
}
