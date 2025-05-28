using System.Text.Json;
using Eventflow.Application.Services.Interfaces;
using Eventflow.Domain.Interfaces.Repositories;
using Eventflow.Domain.Models.ApiModels;
using Eventflow.DTOs.DTOs;

namespace Eventflow.Application.Services
{
    public class NationalEventsService : INationalEventService
    {
        private readonly INationalEventRepository _nationalEventRepository;
        private readonly ICountryRepository _countryRepository;
        private readonly IContinentRepository _continentRepository;
        private readonly HttpClient _httpClient;
        public NationalEventsService(INationalEventRepository nationalEventRepository,
            ICountryRepository countryRepository,
            IContinentRepository continentRepository,
            IHttpClientFactory httpClientFactory)
        {
            _nationalEventRepository = nationalEventRepository;
            _countryRepository = countryRepository;
            _continentRepository = continentRepository;

            _httpClient = httpClientFactory.CreateClient();
            _httpClient.Timeout = TimeSpan.FromSeconds(30);
        }
        public async Task<List<NationalEventDto>> GetNationalHolidaysForCountryAsync(int countryId, int year, int month)
        {
            var nationalEvents = await _nationalEventRepository.GetNationalHolidaysForCountryAsync(countryId, year, month);

            var country = await _countryRepository.GetCountryByIdAsync(countryId);

            return nationalEvents.Select(ne => new NationalEventDto
            {
                Title = ne.Title,
                Description = ne.Description ?? "No description.",
                Date = ne.Date,
                CountryId = ne.CountryId,
                CountryName = country?.Name ?? "Unknown"
            })
            .ToList();
        }

        // public async Task PopulateNationalHolidaysAsync()
        // {
        //    try
        //    {
        //        // Get all continents
        //        var continents = await _continentRepository.GetAllContinentsAsync();

        //        // Loop through each continent to get countries
        //        foreach (var continent in continents)
        //        {
        //            var countries = await _countryRepository.GetAllCountriesByContinentIdAsync(continent.Id);

        //            // Loop through countries to fetch and populate holidays
        //            foreach (var country in countries)
        //            {
        //                string countryCode = country.ISOCode; // ISOCode for the country

        //                // Fetch holidays from the Calendarific API for each country
        //                string apiUrl = $"https://calendarific.com/api/v2/holidays?api_key=dhROTVHBSiCVuJFTDc7sFyn9x4fQ9QNy&country={countryCode}&year=2025";
        //                var response = await _httpClient.GetStringAsync(apiUrl);

        //                Console.WriteLine($"Response for {countryCode}: {response}");

        //                if (string.IsNullOrWhiteSpace(response) || response == "{\"meta\":{\"code\":200},\"response\":[]}")
        //                {
        //                    Console.WriteLine($"No holidays found for {country.Name} ({country.ISOCode})");
        //                    continue;
        //                }

        //                // Deserialize the response
        //                var holidays = JsonSerializer.Deserialize<HolidayApiResponse>(response);

        //                // Check if the response or holidays list is null
        //                if (holidays?.Response?.Holidays == null || !holidays.Response.Holidays.Any())
        //                {
        //                    Console.WriteLine($"No holidays found for {country.Name} ({country.ISOCode})");
        //                    continue; // Skip this country if no holidays found
        //                }

        //                // Process each holiday for the country
        //                foreach (var holiday in holidays.Response.Holidays)
        //                {
        //                    if (holiday != null)
        //                    {
        //                        // Ensure the holiday has a valid date
        //                        if (holiday.Date?.DateTimeDetails != null)
        //                        {                                   
        //                            // Construct the holiday date from the DateTimeDetails object
        //                            DateTime holidayDate = new DateTime(
        //                                holiday.Date.DateTimeDetails.Year,
        //                                holiday.Date.DateTimeDetails.Month,
        //                                holiday.Date.DateTimeDetails.Day
        //                            );

        //                            if (string.IsNullOrEmpty(holiday.States))
        //                            {
        //                                holiday.States = "Unknown";  // Default value if States is empty
        //                            }

        //                            // Insert holiday into the database
        //                            await _countryRepository.InsertHolidayAsync(country.Id, holiday.Name, holidayDate.ToString("yyyy-MM-dd"), holiday.Description);
        //                            Console.WriteLine($"Holiday {holiday.Name} added for {country.Name} on {holidayDate:yyyy-MM-dd}");
        //                        }
        //                        else
        //                        {
        //                            Console.WriteLine($"Holiday {holiday.Name} has invalid date details.");
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.Error.WriteLine($"Error occurred while populating national holidays: {ex.Message}");
        //    }
        // }
        //public async Task PopulateNationalHolidaysForSingleCountryAsync()
        //{
        //    try
        //    {
        //        string countryCode = "BG";

        //        var country = await _countryRepository.GetCountryByISOCodeAsync(countryCode);

        //        if (country == null)
        //        {
        //            Console.WriteLine($"Country with ISO code {countryCode} not found.");
        //            return;
        //        }

        //        string apiUrl = $"https://calendarific.com/api/v2/holidays?api_key=pt9dTYatNelHbvz9SYgdJpmnTwMKy9MW&country={countryCode}&year=2023";
        //        var response = await _httpClient.GetStringAsync(apiUrl);

        //        Console.WriteLine($"Response for {countryCode}: {response}");

        //        var holidays = JsonSerializer.Deserialize<HolidayApiResponse>(response);

        //        if (holidays?.Response?.Holidays == null || !holidays.Response.Holidays.Any())
        //        {
        //            Console.WriteLine($"No holidays found for country code {countryCode}");
        //            return;
        //        }

        //        Console.WriteLine($"Found {holidays.Response.Holidays.Count} holidays for country code {countryCode}");
        //        Console.WriteLine($"First holiday details: {holidays.Response.Holidays[0].Name}, Date: {holidays.Response.Holidays[0].Date.Iso}");

        //        foreach (var holiday in holidays.Response.Holidays)
        //        {
        //            if (holiday != null)
        //            {
        //                DateTime holidayDate = new DateTime(holiday.Date.DateTimeDetails.Year,
        //                                                     holiday.Date.DateTimeDetails.Month,
        //                                                     holiday.Date.DateTimeDetails.Day);


        //                await _countryRepository.InsertHolidayAsync(country.Id, holiday.Name, holidayDate.ToString("yyyy-MM-dd"), holiday.Description);
        //                Console.WriteLine($"Holiday {holiday.Name} added for {country.Name} on {holidayDate.ToString("yyyy-MM-dd")}");
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.Error.WriteLine($"Error occurred while populating national holidays: {ex.Message}");
        //    }
        //}
    }
}
