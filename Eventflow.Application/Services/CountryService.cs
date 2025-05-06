using Eventflow.Application.Services.Interfaces;
using Eventflow.Domain.Exceptions;
using Eventflow.Domain.Interfaces.Repositories;
using Eventflow.Domain.Models.Entities;
using static Eventflow.Domain.Common.CustomErrorMessages.CountryService;

namespace Eventflow.Application.Services
{
    public class CountryService : ICountryService
    {
        private readonly ICountryRepository _countryRepository;
        public CountryService(ICountryRepository countryRepository)
        {
            _countryRepository = countryRepository
                ?? throw new ArgumentNullException(nameof(countryRepository));
        }
        public async Task<List<Country>> GetCountriesByContinentIdAsync(int continentId)
        {
            if (continentId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(continentId), continentIdWasInvalid);
            }

            try
            {
                return (await _countryRepository.GetAllCountriesByContinentIdAsync(continentId))
                    .Where(c => !string.IsNullOrWhiteSpace(c.Name))
                    .OrderBy(c => c.Name)
                    .ToList();
            }
            catch (Exception)
            {
                throw new CountryRetrievalException(countryRetrievalFailed);
            }
        }
    }
}
