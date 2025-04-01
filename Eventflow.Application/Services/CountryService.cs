using Eventflow.Application.Services.Interfaces;
using Eventflow.Domain.Interfaces.Repositories;
using Eventflow.Domain.Models.Models;

namespace Eventflow.Application.Services
{
    public class CountryService : ICountryService
    {
        private readonly ICountryRepository _countryRepository;
        public CountryService(ICountryRepository countryRepository)
        {
            _countryRepository = countryRepository;
        }

        public async Task<List<Country>> GetCountriesByContinentIdAsync(int continentId)
            => (await _countryRepository.GetAllCountriesByContinentIdAsync(continentId))
                .OrderBy(c => c.Name).ToList();
    }
}
