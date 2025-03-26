using Eventflow.Models.Models;

namespace Eventflow.Repositories.Interfaces
{
    public interface ICountryRepository
    {
        public Task<bool> CountryExistsAsync(string countryName);
        public Task InsertCountryAsync(string countryName, int continentId);
        public Task<List<Country>> GetAllCountriesByContinentIdAsync(int continentId);
    }
}
