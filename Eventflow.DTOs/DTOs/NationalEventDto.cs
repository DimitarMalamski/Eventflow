namespace Eventflow.DTOs.DTOs
{
    public class NationalEventDto
    {
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public DateTime Date { get; set; }
        public int CountryId { get; set; }
        public string CountryName { get; set; } = null!;
    }
}
