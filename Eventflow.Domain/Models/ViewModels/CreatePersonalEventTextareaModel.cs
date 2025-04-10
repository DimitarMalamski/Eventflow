namespace Eventflow.Domain.Models.ViewModels
{
    public class CreatePersonalEventTextareaModel
    {
        public string Label { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Placeholder { get; set; } = string.Empty;
        public string? Value { get; set; }
    }
}
