using Eventflow.Application.Services.Interfaces;
using Eventflow.Domain.Interfaces.Repositories;
using Eventflow.Domain.Models.ApiModels;
using System.Text.Json;

namespace Eventflow.Application.Services
{
    public class CountryPopulationService : ICountryPopulationService
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
        public async Task PopulateCountriesAndContinentsAsync()
        {
            string apiUrl = "https://restcountries.com/v3.1/all";

            string json = await _httpClient.GetStringAsync(apiUrl);

            var countries = JsonSerializer.Deserialize<List<CountryApiModel>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (countries == null)
            {
                return;
            }

            foreach (var country in countries)
            {
                if (string.IsNullOrEmpty(country.region) || string.IsNullOrEmpty(country.name?.common))
                    continue;

                int continentId = await _continentRepository.GetOrInsertContinentAsync(country.region.Trim());

                if (!await _countryRepository.CountryExistsAsync(country.name.common))
                {
                    await _countryRepository.InsertCountryAsync(country.name.common, continentId);
                }
            }
        }
    }
}
