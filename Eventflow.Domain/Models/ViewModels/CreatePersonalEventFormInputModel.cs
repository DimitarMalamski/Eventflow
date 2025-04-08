namespace Eventflow.Domain.Models.ViewModels
{
    public class CreatePersonalEventFormInputModel
    {
        public string Label { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = "text";
        public string Placeholder { get; set; } = "";
    }
}
