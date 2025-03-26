using Eventflow.Models.DTOs;

namespace Eventflow.Repositories.Interfaces
{
    public interface ICountryRepository
    {
        bool CountryExists(string countryName);
        void InsertCountry(string countryName, int continentId);
        public List<CountryDto> GetAllCountriesByContinentId(int continentId);
    }
}
