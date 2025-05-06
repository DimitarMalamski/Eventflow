namespace Eventflow.ViewModels
{
    public class FormInputModel
    {
        public string Label { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = "text";
        public string Placeholder { get; set; } = "";
        public string? Value { get; set; }
    }
}
