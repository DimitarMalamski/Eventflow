using Eventflow.Domain.Models.Common;

namespace Eventflow.ViewModels
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
}
