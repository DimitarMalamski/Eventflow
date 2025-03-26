using Eventflow.Models.DTOs;

namespace Eventflow.Services.Interfaces
{
    public interface ICountryService
    {
        List<CountryDto> GetCountriesByContinentId(int continentId);
    }
}
