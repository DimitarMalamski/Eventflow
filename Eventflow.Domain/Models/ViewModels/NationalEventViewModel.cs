namespace Eventflow.Domain.Models.ViewModels
{
    public class NationalEventViewModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public int CountryId { get; set; }
        public string CountryName { get; set; }
        public string Label => "National Holiday";
    }
}
