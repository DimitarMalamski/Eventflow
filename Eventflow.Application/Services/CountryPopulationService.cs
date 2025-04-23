using Eventflow.Domain.Interfaces.Repositories;
using Eventflow.Domain.Models.ApiModels;
using System.Text.Json;

namespace Eventflow.Application.Services
{
    public class CountryPopulationService
    {
        private readonly IContinentRepository _continentRepository;
        private readonly ICountryRepository _countryRepository;
        private readonly HttpClient _httpClient;

        public CountryPopulationService(IContinentRepository continentRepository, ICountryRepository countryRepository, IHttpClientFactory httpClientFactory)
        {
            _continentRepository = continentRepository;
            _countryRepository = countryRepository;
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.Timeout = TimeSpan.FromSeconds(30);
        }

        public async Task PopulateCountriesAsync()
        {
            try
            {
                string apiUrl = "https://restcountries.com/v3.1/all";

                string json = await _httpClient.GetStringAsync(apiUrl);

                Console.WriteLine(json);

                if (string.IsNullOrEmpty(json))
                {
                    Console.Error.WriteLine("API returned an empty response.");
                    return;
                }

                var countries = JsonSerializer.Deserialize<List<CountryApiModel>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (countries == null || !countries.Any())
                {
                    return;
                }

                foreach (var country in countries)
                {
                    if (string.IsNullOrEmpty(country.Region) || string.IsNullOrEmpty(country.Name?.Common))
                        continue;

                    string continentName = MapRegionToContinent(country.Region);

                    int continentId = await _continentRepository.GetOrInsertContinentAsync(continentName);

                    if (!await _countryRepository.CountryExistsAsync(country.Name.Common))
                    {
                        string? flagUrl = string.IsNullOrEmpty(country.Flags?.Png) ? null : country.Flags?.Png; ;

                        await _countryRepository.InsertCountryAsync(country.Name.Common, continentId, flagUrl, country.Cca2);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error occurred while populating countries: {ex.Message}");
            }
        }
        private string MapRegionToContinent(string region)
        {
            return region switch
            {
                "Africa" => "Africa",
                "Antarctic" => "Antarctica",
                "Asia" => "Asia",
                "Europe" => "Europe",
                "Americas" => "North America",
                "Oceania" => "Oceania",
                "South America" => "South America",
                _ => "Unknown"
            };
        }
    }
}

