using Eventflow.Models.ApiModels;
using Eventflow.Repositories.Interfaces;
using Eventflow.Services.Interfaces;
using System.Text.Json;

namespace Eventflow.Services
{
    public class CountryPopulationService : ICountryPopulationService
    {
        private readonly IContinentRepository _continentRepository;
        private readonly ICountryRepository _countryRepository;
        public CountryPopulationService(IContinentRepository continentRepository, ICountryRepository countryRepository)
        {
            _continentRepository = continentRepository;
            _countryRepository = countryRepository;
        }
        public async Task PopulateCountriesAndContinents()
        {
            string apiUrl = "https://restcountries.com/v3.1/all";
            using (HttpClient client = new HttpClient())
            {
                string json = await client.GetStringAsync(apiUrl);

                var countries = JsonSerializer.Deserialize<List<CountryApiModel>>(json);

                foreach (var country in countries)
                {
                    if (string.IsNullOrEmpty(country.region) || string.IsNullOrEmpty(country.name?.common))
                        continue;

                    int continentId = _continentRepository.GetOrInsertContinent(country.region.Trim());

                    if (!_countryRepository.CountryExists(country.name.common))
                    {
                        _countryRepository.InsertCountry(country.name.common, continentId);
                    }
                }
            }
        }
    }
}
