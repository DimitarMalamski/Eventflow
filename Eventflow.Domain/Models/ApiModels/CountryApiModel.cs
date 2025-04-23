namespace Eventflow.Domain.Models.ApiModels
{
    public class CountryApiModel
    {
        public NameModel Name { get; set; }
        public string? Region { get; set; }
        public FlagsModel Flags { get; set; }
        public string? Cca2 { get; set; }
        public string? Cca3 { get; set; }
    }
    public class NameModel
    {
        public string? Common { get; set; }
        public string? Official { get; set; }
    }
    public class FlagsModel
    {
        public string? Png { get; set; }
        public string? Svg { get; set; }
    }
}
