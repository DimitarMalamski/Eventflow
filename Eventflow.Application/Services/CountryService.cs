using Eventflow.Domain.Interfaces.Repositories;
using Eventflow.Models.Models;
using Eventflow.Services.Interfaces;

namespace Eventflow.Services
{
    public class CountryService : ICountryService
    {
        private readonly ICountryRepository _countryRepository;
        public CountryService(ICountryRepository countryRepository)
        {
            _countryRepository = countryRepository;
        }

        public async Task<List<Country>> GetCountriesByContinentIdAsync(int continentId)
            => await _countryRepository.GetAllCountriesByContinentIdAsync(continentId);
    }
}
