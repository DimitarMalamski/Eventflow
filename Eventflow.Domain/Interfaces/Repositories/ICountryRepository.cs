using Eventflow.Domain.Models.Models;

namespace Eventflow.Domain.Interfaces.Repositories
{
    public interface ICountryRepository
    {
        public Task<bool> CountryExistsAsync(string countryName);
        public Task InsertCountryAsync(string countryName, int continentId, string flagUrl, string isoCode);
        public Task<List<Country>> GetAllCountriesByContinentIdAsync(int continentId);
    }
}
