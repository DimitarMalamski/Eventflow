using Eventflow.Domain.Models.Entities;

namespace Eventflow.Domain.Interfaces.Repositories
{
    public interface ICountryRepository
    {
        public Task<bool> CountryExistsAsync(string countryName);
        public Task InsertCountryAsync(string countryName, int continentId, string flagUrl, string isoCode);
        public Task<List<Country>> GetAllCountriesByContinentIdAsync(int continentId);
        public Task InsertHolidayAsync(int countryId, string name, string date, string description);
        public Task<Country> GetCountryByISOCodeAsync(string isoCode);
        public Task<Country?> GetCountryByIdAsync(int countryId);
    }
}
