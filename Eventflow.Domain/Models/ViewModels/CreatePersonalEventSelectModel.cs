using Eventflow.Domain.Models.Models;

namespace Eventflow.Domain.Models.ViewModels
{
    public class CreatePersonalEventSelectModel
    {
        public string Label { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public List<Category> CategoryOptions { get; set; } = new List<Category>();
        public string CategoryOptionLabel { get; set; } = "Select";
        public int? SelectedValue { get; set; }
    }
}
