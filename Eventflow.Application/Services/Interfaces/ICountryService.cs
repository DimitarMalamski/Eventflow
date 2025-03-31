using Eventflow.Domain.Models.Models;

namespace Eventflow.Services.Interfaces
{
    public interface ICountryService
    {
        public Task<List<Country>> GetCountriesByContinentIdAsync(int continentId);
    }
}
