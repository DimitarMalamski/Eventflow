using Eventflow.Models.DTOs;
using Eventflow.Repositories.Interfaces;
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

        public List<CountryDto> GetCountriesByContinentId(int continentId)
            => _countryRepository.GetAllCountriesByContinentId(continentId);
    }
}
