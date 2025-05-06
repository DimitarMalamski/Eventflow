namespace Eventflow.ViewModels.Calendar.Component
{
    public class NationalEventViewModel
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public int CountryId { get; set; }
        public string CountryName { get; set; } = string.Empty;
        public string Label => "National Holiday";
    }
}
