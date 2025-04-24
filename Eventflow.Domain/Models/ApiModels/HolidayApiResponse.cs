using Eventflow.Domain.Common;
using System.Text.Json.Serialization;

namespace Eventflow.Domain.Models.ApiModels
{
    public class HolidayApiResponse
    {
        public Meta Meta { get; set; }

        [JsonPropertyName("response")]
        public ResponseWrapper Response { get; set; }
    }

    public class Meta
    {
        public int Code { get; set; }
    }

    public class ResponseWrapper
    {
        [JsonPropertyName("holidays")]
        public List<Holiday> Holidays { get; set; }
    }

    public class Holiday
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("country")]
        public CountryInfo Country { get; set; }

        [JsonPropertyName("date")]
        public HolidayDate Date { get; set; }

        [JsonPropertyName("type")]
        public List<string> Type { get; set; }

        [JsonPropertyName("primary_type")]
        public string PrimaryType { get; set; }

        [JsonPropertyName("canonical_url")]
        public string CanonicalUrl { get; set; }

        [JsonPropertyName("urlid")]
        public string UrlId { get; set; }

        [JsonPropertyName("locations")]
        public string Locations { get; set; }

        [JsonPropertyName("states")]
        [JsonConverter(typeof(StatesConverter))]
        public string? States { get; set; }
    }

    public class CountryInfo
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }
    }

    public class HolidayDate
    {
        [JsonPropertyName("iso")]
        public string Iso { get; set; }

        [JsonPropertyName("datetime")]
        public DateTimeDetails DateTimeDetails { get; set; }
        public DateTime GetHolidayDate()
        {
            return new DateTime(DateTimeDetails.Year, DateTimeDetails.Month, DateTimeDetails.Day);
        }
    }
    public class DateTimeDetails
    {
        [JsonPropertyName("year")]
        public int Year { get; set; }

        [JsonPropertyName("month")]
        public int Month { get; set; }

        [JsonPropertyName("day")]
        public int Day { get; set; }
    }

}
